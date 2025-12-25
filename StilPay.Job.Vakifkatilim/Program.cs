using Irony.Parsing;
using StilPay.Utility.Models;
using StilPay.Utility.Worker;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static StilPay.Utility.Helper.Enums;

namespace StilPay.Job.Vakifkatilim
{
    internal class Program
    {
        private const string SOAP_ACTION =
          "http://boa.net/BOA.Integration.CustomerTransaction/CustomerTransactionService/ICustomerTransactionService/GetCustomerTransactions";

        private const string NS_SERVICE =
          "http://boa.net/BOA.Integration.CustomerTransaction/CustomerTransactionService";

        private const string NS_BASE =
          "http://schemas.datacontract.org/2004/07/BOA.Integration.Base";

        private const string NS_MODEL =
          "http://schemas.datacontract.org/2004/07/BOA.Integration.Model.CustomerTransaction";

        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var startUp = new Startup();

            var bankId = startUp.VakifkatilimApi.bank_id;
            var transaction_url = startUp.VakifkatilimApi.transaction_url;
            var startDate = startUp.VakifkatilimApi.startDate;
            var endDate = startUp.VakifkatilimApi.endDate;
            var accountNumber = startUp.VakifkatilimApi.accountNumber; 
            var accountSuffix = startUp.VakifkatilimApi.accountSuffix; 
            var username = startUp.VakifkatilimApi.username;
            var password = startUp.VakifkatilimApi.password;
            var queryperiodintervalsecond = startUp.VakifkatilimApi.query_period_interval_second;
            var notificationRangeMinute = startUp.VakifkatilimApi.notification_range_minute;
            var companyBankAccountID = startUp.VakifkatilimApi.companyBankAccountID;

            int accountTransactionCount = 0;
            int tableInsertionErrorCount = 0;
            int tableInsertionSuccessCount = 0;
            int timeoutNotificationsCount = 0;

            Console.WriteLine(
                string.Concat("-------------------------------------------------",
                               Environment.NewLine, Environment.NewLine,
                              "        STILPAY VAKIF KATILIM - BANK WORKER        ",
                               Environment.NewLine, Environment.NewLine,
                              "-------------------------------------------------")
            );

            while (true)
            {
                int responseStatus = 0;

                // Gün 10:00'dan önce ise bir önceki günü sor (senin mantığını korudum)
                var now = DateTime.Now;
                bool before10 = now.TimeOfDay < new TimeSpan(10, 0, 0);

                string today = DateTime.Today.ToString("yyyy-MM-dd");
                string previousDay = before10 ? DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd") : null;

                // 🔸 Son 3 saati hesapla
                var threeHoursAgo = now.AddHours(-3);

                // Dışarıdan start/end yoksa, bugünü (veya önceki günü) kullan
                string beginDateF = !string.IsNullOrEmpty(startDate) ? startDate : (previousDay ?? threeHoursAgo.ToString("yyyy-MM-ddTHH:mm:ss"));
                string endDateF = !string.IsNullOrEmpty(endDate) ? endDate : now.ToString("yyyy-MM-ddTHH:mm:ss");
                try
                {

                    using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };

                    var soap = BuildSoapEnvelope(username, password, accountNumber, accountSuffix, beginDateF, endDateF);

                    using var req = new HttpRequestMessage(HttpMethod.Post, transaction_url);
                    req.Content = new StringContent(soap, Encoding.UTF8, "text/xml");
                    req.Headers.Add("SOAPAction", SOAP_ACTION);

                    var res = http.Send(req);
                    var xmlText = res.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (!res.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"VK SOAP HTTP {(int)res.StatusCode} - {res.ReasonPhrase}");
                        continue;
                    }

                    var parsed = VkParse(xmlText);

                    if (!parsed.Success)
                    {
                        Console.WriteLine($"VK HATA: Success=false, Code={parsed.Error?.Code} Msg={parsed.Error?.Message}");
                        continue;
                    }

                    if (parsed.Error != null)
                    {
                        Console.WriteLine($"VK HATA: Code={parsed.Error.Code} Msg={parsed.Error.Message}");
                        continue;
                    }

                    // Hareketler
                    var movements = parsed.Details; // List<VkTransactionDetail>
                    accountTransactionCount = movements.Count;

                    foreach (var hareket in movements.OrderByDescending(o => o.SystemDate ?? o.TranDate ?? DateTime.MinValue))
                    {
                        var businessKey = hareket.BusinessKey;

                        if (tSQLBankManager.HasPaymentTransferPool(businessKey) ||
                            tSQLBankManager.HasNotificationTransaction(businessKey))
                        {
                            continue;
                        }

                        // Açıklamadan referans ve gönderici adı çıkar
                        var (parsedSender, parsedRef) = ParseRefAndSender(hareket.Description);
                        string senderName = parsedSender;

                        bool isCaughtInFraudControl = false;
                        bool isTrusted = false;
                        string fraudDescription = "Fraud kontrolleri başarıyla tamamlandı.";

                        var (Result, ReferenceNr, ServiceId, CallbackUrl, AutoTransferLimit) =
                            tSQLBankManager.CheckReferenceNr(parsedRef ?? "");

                        var blockedList = tSQLBankManager.GetPaymentTransferPoolDescriptionControls();
                        bool isHaveBlockedWord = blockedList.Any(x =>
                            (hareket.Description ?? "").ToLowerInvariant().Contains(x.Name));

                        if (!string.IsNullOrEmpty(Result) && !string.IsNullOrEmpty(ReferenceNr) && !string.IsNullOrEmpty(ServiceId))
                        {
                            var (IsTrusted, FraudResult, FraudDescription) =
                                tSQLBankManager.TransferCheckFraudControl(null, ReferenceNr, senderName, ServiceId,
                                    Convert.ToDecimal(hareket.Amount, CultureInfo.InvariantCulture));

                            isCaughtInFraudControl = !FraudResult;
                            fraudDescription = IsTrusted
                                ? "Aynı kişi 24 saat veya öncesinde ödeme yaptı, kişi güvenilir olduğu için fraud kontrolü yapılmadı"
                                : FraudDescription;
                            isTrusted = IsTrusted;
                        }

                        var amountDec = Convert.ToDecimal(hareket.Amount, CultureInfo.InvariantCulture);
                        var tranDate = hareket.SystemDate ?? hareket.TranDate ?? DateTime.Now;

                        if (!string.IsNullOrEmpty(Result)
                            && !string.IsNullOrWhiteSpace(ReferenceNr)
                            && Result == "OK"
                            && !string.IsNullOrEmpty(ServiceId)
                            && !string.IsNullOrEmpty(CallbackUrl)
                            && !tSQLBankManager.HasNotificationTransaction(businessKey)
                            && !tSQLBankManager.HasPaymentTransferPool(businessKey)
                            && (isTrusted || amountDec <= AutoTransferLimit)
                            && !isHaveBlockedWord
                            && !isCaughtInFraudControl)
                        {
                            string transactionId = DateTime.Now.Ticks.ToString("D16");

                            var transactionNr = tSQLBankManager.AddNotificationTransaction(
                                DateTime.Now,
                                tranDate, tranDate,
                                bankId, ServiceId, transactionId,
                                businessKey, amountDec,
                                hareket.Description, Guid.Empty.ToString(),
                                senderName, "11111111111",
                                false, true);

                            if (!string.IsNullOrEmpty(transactionNr))
                            {
                                var companyIntegration = tSQLBankManager.GetCompanyIntegration(ServiceId);

                                var dataCallback = new
                                {
                                    status_code = "OK",
                                    status_type = 0,
                                    service_id = ServiceId,
                                    ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                    data = new
                                    {
                                        transaction_id = transactionId,
                                        reference_nr = ReferenceNr,
                                        transfer_date = tranDate,
                                        amount = amountDec
                                    },
                                    user_entered_data = new
                                    {
                                        sender_name = senderName,
                                        bank_description = hareket.Description
                                    }
                                };

                                var pyID = tSQLBankManager.AddAutoPaymentNotification(
                                    tranDate, bankId, senderName, ServiceId, transactionId,
                                    businessKey, amountDec, "Otomatik Bakiye Yükleme İşlemi Bildirimi", "",
                                    companyBankAccountID, isCaughtInFraudControl, fraudDescription);

                                var pyTransactionNr = tSQLBankManager.GetPaymentNotificationTransactionNr(pyID);

                                var IDOutAuto = tSQLBankManager.AddPaymentTransferPoolWithReference(
                                    tranDate, bankId, senderName, "",
                                    amountDec, businessKey, hareket.Description,
                                    true, companyBankAccountID, pyTransactionNr,
                                    transactionId, isCaughtInFraudControl, fraudDescription);

                                if (pyID != null && IDOutAuto != null)
                                {
                                    tSQLBankManager.SetPaymentTransactionStatus(pyID, (int)StatusType.Confirmed,
                                        "Otomatik Bakiye Yükleme İşlemi Bildirimi");

                                    var response = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(
                                        CallbackUrl,
                                        new Dictionary<string, string>(),
                                        new Dictionary<string, object> { { "transaction", dataCallback } });

                                    responseStatus = 0;
                                    if (response?.Result != null && !string.IsNullOrEmpty(response.Result.Status))
                                    {
                                        tSQLBankManager.AcceptNotificationTransaction(transactionNr);
                                        responseStatus = response.Result.Status switch
                                        {
                                            "OK" => 1,
                                            "RED" => 2,
                                            "ERROR" => 3,
                                            _ => 0
                                        };
                                    }

                                    tSQLBankManager.AddCallbackResponseLog(
                                        transactionId, "STILPAY",
                                        System.Text.Json.JsonSerializer.Serialize(dataCallback, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }),
                                        companyIntegration.ID, "Ödeme Bildirimi", responseStatus);

                                    tableInsertionErrorCount = string.IsNullOrEmpty(IDOutAuto) ? tableInsertionErrorCount + 1 : tableInsertionErrorCount;
                                    tableInsertionSuccessCount = string.IsNullOrEmpty(IDOutAuto) ? tableInsertionSuccessCount : tableInsertionSuccessCount + 1;
                                }
                            }
                        }
                        else
                        {
                            if (!tSQLBankManager.HasPaymentTransferPool(businessKey) &&
                                !tSQLBankManager.HasNotificationTransaction(businessKey))
                            {
                                var IDOut = tSQLBankManager.AddPaymentTransferPool(
                                    tranDate, bankId, senderName, hareket.SenderIBAN ?? "",
                                    amountDec, businessKey, hareket.Description,
                                    companyBankAccountID, isHaveBlockedWord ? (byte)StatusType.Risk : (byte)1,
                                    isCaughtInFraudControl, fraudDescription);

                                tableInsertionErrorCount = string.IsNullOrEmpty(IDOut) ? tableInsertionErrorCount + 1 : tableInsertionErrorCount;
                                tableInsertionSuccessCount = string.IsNullOrEmpty(IDOut) ? tableInsertionSuccessCount : tableInsertionSuccessCount + 1;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        string.Concat(Environment.NewLine, Environment.NewLine,
                         $"Hata: {ex.Message}",
                          Environment.NewLine, Environment.NewLine,
                         "-------------------------------------------------"));
                }

                Console.WriteLine(
                    string.Concat(Environment.NewLine, Environment.NewLine,
                                  $"Bankaya Atılan Sorgu Başlangıç Tarihi: {beginDateF}\n",
                                  $"Bankaya Atılan Sorgu Bitiş Tarihi: {DateTime.Now}\n",
                                  $"Hesap Hareketleri Sayısı : {accountTransactionCount}\n",
                                  $"Tabloya Kayıt Edilen Başarılı İşlem Sayısı : {tableInsertionSuccessCount}\n",
                                  $"Tabloya Kayıt Edilen Hatalı İşlem Sayısı : {tableInsertionErrorCount}\n",
                                  $"Zaman Aşımına Uğrayan Bildirim Sayısı: {timeoutNotificationsCount}\n",
                                  "-------------------------------------------------"));

                Thread.Sleep(queryperiodintervalsecond * 1000);
            }
        }

        private static string BuildSoapEnvelope(
            string user, string pass, string accountNumber, string accountSuffix,
            string beginDate, string endDate)
        {
            const string SOAPENV = "http://schemas.xmlsoap.org/soap/envelope/";

            return $@"
<soapenv:Envelope xmlns:soapenv=""{SOAPENV}""
                  xmlns:ser=""{NS_SERVICE}""
                  xmlns:boa=""{NS_BASE}""
                  xmlns:mod=""{NS_MODEL}"">
  <soapenv:Header/>
  <soapenv:Body>
    <ser:GetCustomerTransactions>
      <ser:request>
        <boa:ExtUName>{SecurityHelper.XmlEncode(user)}</boa:ExtUName>
        <boa:ExtUPassword>{SecurityHelper.XmlEncode(pass)}</boa:ExtUPassword>

        <mod:AccountNumber>{SecurityHelper.XmlEncode(accountNumber)}</mod:AccountNumber>
        <mod:AccountSuffix>{SecurityHelper.XmlEncode(accountSuffix)}</mod:AccountSuffix>
        <mod:BeginDate>{beginDate}</mod:BeginDate>
        <mod:EndDate>{endDate}</mod:EndDate>
      </ser:request>
    </ser:GetCustomerTransactions>
  </soapenv:Body>
</soapenv:Envelope>".Trim();
        }

        private static (string SenderName, string LeadingRef) ParseRefAndSender(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return ("", null);

            string sender = "";
            string refCode = null;

            var matchSender = Regex.Match(description,
                @"(?i)(?:amir\s*[=:]\s*|g[öo]nderen\s*[=:]\s*)([A-Za-zÇĞİÖŞÜçğıöşü\s\.]+?)(?=\s*(?:aciklama|açıklama|sorgu|$))",
                RegexOptions.CultureInvariant);

            if (matchSender.Success)
            {
                sender = matchSender.Groups[1].Value.Trim();
            }

            var matchRef = Regex.Match(description, @"\b\d{6,30}\b");
            if (matchRef.Success)
            {
                refCode = matchRef.Value;
            }

            // 3️⃣ Gereksiz boşlukları temizle
            if (!string.IsNullOrEmpty(sender))
            {
                sender = Regex.Replace(sender, @"\s{2,}", " ").Trim();
            }

            return (sender, refCode);
        }

        private static class SecurityHelper
        {
            public static string XmlEncode(string s)
                => string.IsNullOrEmpty(s) ? s : System.Security.SecurityElement.Escape(s);
        }

        private class VkParsed
        {
            public VkError Error { get; set; }
            public bool Success { get; set; }
            public List<VkTransactionDetail> Details { get; set; } = new();
        }

        private class VkError
        {
            public string Code { get; set; }
            public string Message { get; set; }
            public string Exception { get; set; }
        }

        private class VkTransactionDetail
        {
            public string Amount { get; set; }
            public string BranchId { get; set; }
            public string BranchName { get; set; }
            public string BusinessKey { get; set; }
            public string ChannelId { get; set; }
            public string Credit { get; set; }
            public string CurrentBalance { get; set; }
            public string Debit { get; set; }
            public string Description { get; set; }
            public string FEC { get; set; }
            public string FECName { get; set; }
            public string ReceiverIBAN { get; set; }
            public string ResourceCode { get; set; }
            public string SenderIBAN { get; set; }
            public string SenderIdentityNumber { get; set; }
            public DateTime? SystemDate { get; set; }
            public DateTime? TranDate { get; set; }
            public string TranRef { get; set; }
            public string TranType { get; set; }
            public DateTime? ValueDate { get; set; }
            public string VirtualIBAN { get; set; }
        }

        private static VkParsed VkParse(string xml)
        {
            var parsed = new VkParsed();
            var doc = XDocument.Parse(xml);

            XNamespace boa = NS_BASE;
            XNamespace model = NS_MODEL;

            // GetCustomerTransactionsResult düğümü
            var result = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "GetCustomerTransactionsResult");
            if (result == null)
            {
                parsed.Success = false;
                parsed.Error = new VkError { Code = "Parse", Message = "GetCustomerTransactionsResult bulunamadı." };
                return parsed;
            }

            // Success
            var successText = result.Elements().FirstOrDefault(x => x.Name.LocalName == "Success")?.Value;
            parsed.Success = string.Equals(successText, "true", StringComparison.OrdinalIgnoreCase);

            // Error (Results->Result) veya üst seviye ErrorCode/Message
            var resNode = result.Element(boa + "Results")?.Element(boa + "Result");
            string errCode = resNode?.Element(boa + "ErrorCode")?.Value;
            string errMsg = resNode?.Element(boa + "ErrorMessage")?.Value;
            string errEx = resNode?.Element(boa + "Exception")?.Value;

            var topErrCode = result.Element(boa + "ErrorCode")?.Value;
            var topErrMsg = result.Element(boa + "ErrorMessage")?.Value;

            if (!string.IsNullOrEmpty(errCode) || !string.IsNullOrEmpty(errMsg) ||
                !string.IsNullOrEmpty(topErrCode) || !string.IsNullOrEmpty(topErrMsg))
            {
                parsed.Error = new VkError
                {
                    Code = !string.IsNullOrEmpty(errCode) ? errCode : topErrCode,
                    Message = !string.IsNullOrEmpty(errMsg) ? errMsg : topErrMsg,
                    Exception = errEx
                };
            }

            var valueNode = result.Elements().FirstOrDefault(x => x.Name.LocalName == "Value");
            if (valueNode != null)
            {
                var txNodes = valueNode.Descendants()
                    .Where(x => x.Name.LocalName == "CustomerTransactionResponseModel");

                foreach (var d in txNodes)
                {
                    DateTime? TryDate(string s)
                    {
                        if (DateTime.TryParse(s, CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeLocal, out var dt)) return dt;
                        return null;
                    }

                    string Val(string local) =>
                        d.Element(model + local)?.Value ??
                        d.Elements().FirstOrDefault(e => e.Name.LocalName == local)?.Value;

                    parsed.Details.Add(new VkTransactionDetail
                    {
                        Amount = Val("Amount"),
                        BranchId = Val("BranchId"),
                        BranchName = Val("BranchName"),
                        BusinessKey = Val("BusinessKey"),
                        ChannelId = Val("ChannelId"),
                        Credit = Val("Credit"),
                        CurrentBalance = Val("CurrentBalance"),
                        Debit = Val("Debit"),
                        Description = Val("Comment"),
                        FEC = Val("FEC"),
                        FECName = Val("FECName"),
                        ReceiverIBAN = Val("ReceiverIBAN"),
                        ResourceCode = Val("ResourceCode"),
                        SenderIBAN = Val("SenderIBAN"),
                        SenderIdentityNumber = Val("SenderIdentityNumber"),
                        SystemDate = TryDate(Val("SystemDate")),
                        TranDate = TryDate(Val("TranDate")),
                        TranRef = Val("TranRef"),
                        TranType = Val("TranType"),
                        ValueDate = TryDate(Val("ValueDate")),
                        VirtualIBAN = Val("VirtualIBAN"),
                    });
                }
            }

            return parsed;
        }
    }
}
