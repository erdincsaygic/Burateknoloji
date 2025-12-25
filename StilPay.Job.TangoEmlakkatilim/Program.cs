using StilPay.Utility.Models;
using StilPay.Utility.Worker;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static StilPay.Utility.Helper.Enums;

namespace StilPay.Job.Emlakkatilim
{
    internal class Program
    {
        private const string SoapAction =
          "http://boa.net/BOA.Integration.CoreBanking.Teller/Service/IAccountStatementService/GetAccountStatement";

        private const string NS_SERVICE =
          "http://boa.net/BOA.Integration.CoreBanking.Teller/Service";

        private const string NS_BASE =
          "http://schemas.datacontract.org/2004/07/BOA.Integration.Base";

        private const string NS_MODEL =
          "http://schemas.datacontract.org/2004/07/BOA.Integration.Model.CoreBanking.Teller";

        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var startUp = new Startup();

            Dictionary<string, string> header = new Dictionary<string, string>();
            Dictionary<string, object> body = new Dictionary<string, object>();


            var bankId = startUp.EmlakkatilimApi.bank_id;
            var transaction_url = startUp.EmlakkatilimApi.transaction_url;
            var startDate = startUp.EmlakkatilimApi.startDate; 
            var endDate = startUp.EmlakkatilimApi.endDate;
            var accountNumber = startUp.EmlakkatilimApi.accountNumber;
            var accountSuffix = startUp.EmlakkatilimApi.accountSuffix;
            var username = startUp.EmlakkatilimApi.username;
            var password = startUp.EmlakkatilimApi.password;
            var queryperiodintervalsecond = startUp.EmlakkatilimApi.query_period_interval_second;
            var notificationRangeMinute = startUp.EmlakkatilimApi.notification_range_minute;
            var serviceID = startUp.EmlakkatilimApi.serviceID;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            int accountTransactionCount = 0;
            int tableInsertionErrorCount = 0;
            int tableInsertionSuccessCount = 0;
            int timeoutNotificationsCount = 0;


            var reqEndDate = DateTime.Now;
            var today = DateTime.Today.ToString("yyyy-MM-dd");
            string previousDay = null;
            var companyBankAccountID = "A7901098-DA21-4869-96B6-7C3DDCE1AED5"; /*startUp.EmlakkatilimApi.companyBankAccountID; */
            Console.WriteLine(
                    string.Concat("-------------------------------------------------",
                                   Environment.NewLine, Environment.NewLine,
                                  "            STILPAY TANGO-EMLAKKATILIM-BANK WORKING..            ",
                                   Environment.NewLine, Environment.NewLine,
                                  "-------------------------------------------------")
            );

            while (true)
            {
                #region Emlakkatilim Api
                try
                {
                    int responseStatus = 0;

                    DateTime dateNow = DateTime.Now;
                    TimeSpan timeLimit = new TimeSpan(10, 0, 0);

                    if (dateNow.TimeOfDay < timeLimit)
                        previousDay = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
                    else
                        previousDay = null;

                    using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };

                    string beginDateF = !string.IsNullOrEmpty(startDate) ? startDate : (previousDay ?? today);
                    string endDateF = !string.IsNullOrEmpty(endDate) ? endDate : today;

                    var soap = BuildSoapEnvelope(username, password, accountNumber, accountSuffix, beginDateF, endDateF);

                    using var req = new HttpRequestMessage(HttpMethod.Post, transaction_url);
                    req.Content = new StringContent(soap, Encoding.UTF8, "text/xml");
                    req.Headers.Add("SOAPAction", SoapAction);

                    var res = http.Send(req);
                    var xmlText = res.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (!res.IsSuccessStatusCode)
                        throw new Exception($"EKB SOAP HTTP {(int)res.StatusCode} - {res.ReasonPhrase}");

                    var parsed = EkbParse(xmlText);
                    if (parsed.Error != null)
                    {
                        Console.WriteLine($"EKB HATA: Code={parsed.Error.Code} Msg={parsed.Error.Message}");
                        continue;
                    }

                    var movements = parsed.Details; 
                    accountTransactionCount = movements.Count;

                    foreach (var hareket in movements
                             .OrderByDescending(o => o.SystemDate))
                    {
                        var amountDec = Convert.ToDecimal(hareket.Amount, CultureInfo.InvariantCulture);
                        var businessKey = hareket.BusinessKey;

                        if (tSQLBankManager.HasPaymentTransferPool(businessKey) ||
                            tSQLBankManager.HasNotificationTransaction(businessKey) || amountDec <= 0)
                        {
                            continue;
                        }

                        var (parsedSender, parsedRef) = ParseRefAndSender(hareket.Description);

                        string senderName = parsedSender;


                        var tranDate = hareket.SystemDate ?? DateTime.Now;

                        string transactionId = DateTime.Now.Ticks.ToString("D16");

                        var transactionNr = tSQLBankManager.AddNotificationTransaction(
                            DateTime.Now,
                            tranDate, tranDate,
                            bankId, serviceID, transactionId,
                            businessKey, amountDec,
                            hareket.Description, Guid.Empty.ToString(),
                            senderName, "11111111111",
                            false, true);

                        if (!string.IsNullOrEmpty(transactionNr))
                        {
                            var companyIntegration = tSQLBankManager.GetCompanyIntegration(serviceID);

                            var pyID = tSQLBankManager.AddAutoPaymentNotification(
                                tranDate, bankId, senderName, serviceID, transactionId,
                                businessKey, amountDec, "Otomatik Bakiye Yükleme İşlemi Bildirimi", "",
                                companyBankAccountID, false);

                            var pyTransactionNr = tSQLBankManager.GetPaymentNotificationTransactionNr(pyID);

                            var IDOutAuto = tSQLBankManager.AddPaymentTransferPoolWithReference(
                                tranDate, bankId, senderName, "",
                                amountDec, businessKey, hareket.Description,
                                true, companyBankAccountID, pyTransactionNr,
                                transactionId, false);

                            if (pyID != null && IDOutAuto != null)
                            {
                                tSQLBankManager.SetPaymentTransactionStatus(pyID, (int)StatusType.Confirmed,
                                    "Otomatik Bakiye Yükleme İşlemi Bildirimi");

                                responseStatus = 1;

                                var phones = new List<string>
                                {
                                    "05468130828",
                                    "05425575091",
                                    "05465508493",
                                    "05527364171"
                                };

                                string formattedDate = tranDate.ToString("dd.MM.yyyy HH:mm");
                                string formattedAmount = amountDec.ToString("N2", new CultureInfo("tr-TR"));

                                foreach (var item in phones)
                                {
                                    tSmsSender sender = new tSmsSender();
                                    string msg = $"TR500021100000082947800002 {formattedDate} {senderName} {formattedAmount}"; 
                                    sender.SendSms(item, msg);
                                }

                                tableInsertionErrorCount = string.IsNullOrEmpty(IDOutAuto) ? tableInsertionErrorCount + 1 : tableInsertionErrorCount;
                                tableInsertionSuccessCount = string.IsNullOrEmpty(IDOutAuto) ? tableInsertionSuccessCount : tableInsertionSuccessCount + 1;
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

                #endregion

                reqEndDate = DateTime.Now;
                today = DateTime.Today.ToString("yyyy-MM-dd");
                previousDay = null;
                
                Console.WriteLine(
                string.Concat(Environment.NewLine, Environment.NewLine,
                              $"Bankaya Atılan Sorgu Başlangıç Tarihi: {previousDay??today}\n",
                              $"Bankaya Atılan Sorgu Bitiş Tarihi: {reqEndDate}\n",
                              $"Hesap Hareketleri Sayısı : {accountTransactionCount}\n",
                              $"Tabloya Kayıt Edilen Başarılı İşlem Sayısı : {tableInsertionSuccessCount}\n",
                              $"Tabloya Kayıt Edilen Hatalı İşlem Sayısı Sayısı: {tableInsertionErrorCount}\n",
                              $"Zaman Aşımına Uğrayan Bildirim Sayısı: {timeoutNotificationsCount}\n",
                              "-------------------------------------------------"));

                Thread.Sleep(queryperiodintervalsecond * 1000);
            }
        }

        private static (string SenderName, string LeadingRef) ParseRefAndSender(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return ("", null);

            string sender = "";
            string refCode = null;

            var matchLabelRef = Regex.Match(description,
                @"(?i)(?:aciklama|açıklama|ref)\s*[:=]?\s*(\d{6,30})",
                RegexOptions.CultureInvariant);

            var matchPrefixRef = Regex.Match(description,
                @"(\d{6,30})(?=\s*(?:g[öo]nderen|amir))",
                RegexOptions.IgnoreCase);

            var matchStartRef = Regex.Match(description, @"^\s*(\d{6,30})");

            if (matchLabelRef.Success)
            {
                refCode = matchLabelRef.Groups[1].Value;
            }
            else if (matchPrefixRef.Success)
            {
                refCode = matchPrefixRef.Value;
            }
            else if (matchStartRef.Success)
            {
                refCode = matchStartRef.Groups[1].Value;
            }

            var matchSender = Regex.Match(description,
                @"(?i)(?:amir|g[öo]nderen)\s*[:=]?\s*(?<name>.+?)(?=\s*(?:al[ıi]c[ıi]|sorgu|aciklama|açıklama|g[öo]nd|bank|hesap|$))",
                RegexOptions.CultureInvariant);

            if (matchSender.Success)
            {
                sender = matchSender.Groups["name"].Value.Trim();
            }

            if (!string.IsNullOrEmpty(sender))
            {
                sender = Regex.Replace(sender, @"\s{2,}", " ").Trim();
            }

            return (sender, refCode);
        }

        private static string BuildSoapEnvelope(
            string user, string pass, string accountNumber, string accountSuffix, string beginDate, string endDate)
        {
            var soapEnv = "http://schemas.xmlsoap.org/soap/envelope/";

            return $@"
<soapenv:Envelope xmlns:soapenv=""{soapEnv}""
                  xmlns:ser=""{NS_SERVICE}""
                  xmlns:boa=""{NS_BASE}""
                  xmlns:boa1=""{NS_MODEL}"">
  <soapenv:Header/>
  <soapenv:Body>
    <ser:GetAccountStatement>
      <ser:request>
        <boa:ExtUName>{SecurityHelper.XmlEncode(user)}</boa:ExtUName>
        <boa:ExtUPassword>{SecurityHelper.XmlEncode(pass)}</boa:ExtUPassword>
        <boa1:AccountNumber>{accountNumber}</boa1:AccountNumber>
        <boa1:AccountSuffix>{accountSuffix}</boa1:AccountSuffix>
        <boa1:BeginDate>{beginDate}</boa1:BeginDate>
        <boa1:EndDate>{endDate}</boa1:EndDate>
      </ser:request>
    </ser:GetAccountStatement>
  </soapenv:Body>
</soapenv:Envelope>".Trim();
        }

        // ---- XML Model ----
        private class EkbParsed
        {
            public EkbError Error { get; set; }
            public bool Success { get; set; }
            public EkbAccount Account { get; set; } = new();
            public List<EkbTransactionDetail> Details { get; set; } = new();
        }

        private class EkbError
        {
            public string Code { get; set; }
            public string Message { get; set; }
            public string Exception { get; set; }
        }
        private class EkbAccount
        {
            public string AccountAlias { get; set; }
            public int AccountNumber { get; set; }
            public short AccountSuffix { get; set; }
            public string IBAN { get; set; }
            public string BranchName { get; set; }
            public short BranchId { get; set; }
            public decimal Balance { get; set; }
            public decimal AvailableBalance { get; set; }
            public string FECName { get; set; }            // TL vs.
            public string FECLongName { get; set; }        // Türk Lirası vs.
            public DateTime? OpenDate { get; set; }
            public DateTime? LastTranDate { get; set; }
        }

        private class EkbTransactionDetail
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

        // ---- XML Parse ----
        private static EkbParsed EkbParse(string xml)
        {
            var doc = XDocument.Parse(xml);

            XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
            XNamespace svc = "http://boa.net/BOA.Integration.CoreBanking.Teller/Service";
            XNamespace boa = "http://schemas.datacontract.org/2004/07/BOA.Integration.Base";
            XNamespace boa1 = "http://schemas.datacontract.org/2004/07/BOA.Integration.Model.CoreBanking.Teller";

            var parsed = new EkbParsed();

            // ---- Success / Error ----
            var resultNode = doc
                .Descendants(svc + "GetAccountStatementResult")
                .FirstOrDefault();

            if (resultNode != null)
            {
                parsed.Success = (string)resultNode.Element(boa + "Success") == "true";

                // Results -> Result -> ErrorCode/ErrorMessage/Exception
                var r = resultNode.Element(boa + "Results")?
                                   .Element(boa + "Result");
                if (r != null)
                {
                    parsed.Error = new EkbError
                    {
                        Code = (string)r.Element(boa + "ErrorCode"),
                        Message = (string)r.Element(boa + "ErrorMessage"),
                        Exception = (string)r.Element(boa + "Exception")
                    };
                }
                else
                {
                    // Ayrıca ErrorCode/ErrorMessage üst seviyede de var (nil değilse al)
                    var code = (string)resultNode.Element(boa + "ErrorCode");
                    var msg = (string)resultNode.Element(boa + "ErrorMessage");
                    if (!string.IsNullOrEmpty(code) || !string.IsNullOrEmpty(msg))
                    {
                        parsed.Error = new EkbError { Code = code, Message = msg };
                    }
                }
            }

            // ---- Account (ilk hesap) ----
            var accountNode = doc
                .Descendants(boa1 + "AccountContract")
                .FirstOrDefault();

            DateTime? TryDate(string s)
            {
                if (DateTime.TryParse(s, CultureInfo.InvariantCulture,
                                      DateTimeStyles.AssumeLocal, out var dt)) return dt;
                return null;
            }

            decimal TryDec(string s)
            {
                return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0m;
            }

            short TryShort(string s)
            {
                return short.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : (short)0;
            }

            int TryInt(string s)
            {
                return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : 0;
            }

            if (accountNode != null)
            {
                parsed.Account = new EkbAccount
                {
                    AccountAlias = (string)accountNode.Element(boa1 + "AccountAlias"),
                    AccountNumber = TryInt((string)accountNode.Element(boa1 + "AccountNumber")),
                    AccountSuffix = TryShort((string)accountNode.Element(boa1 + "AccountSuffix")),
                    IBAN = (string)accountNode.Element(boa1 + "IBAN"),
                    BranchName = (string)accountNode.Element(boa1 + "BranchName"),
                    BranchId = TryShort((string)accountNode.Element(boa1 + "BranchId")),
                    Balance = TryDec((string)accountNode.Element(boa1 + "Balance")),
                    AvailableBalance = TryDec((string)accountNode.Element(boa1 + "AvailableBalance")),
                    FECName = (string)accountNode.Element(boa1 + "FECName"),
                    FECLongName = (string)accountNode.Element(boa1 + "FECLongName"),
                    OpenDate = TryDate((string)accountNode.Element(boa1 + "OpenDate")),
                    LastTranDate = TryDate((string)accountNode.Element(boa1 + "LastTranDate")),
                };
            }

            // ---- Movements ----
            var details = doc.Descendants(boa1 + "TransactionDetailContract");
            foreach (var d in details)
            {
                parsed.Details.Add(new EkbTransactionDetail
                {
                    Amount = (string)d.Element(boa1 + "Amount"),
                    BranchId = (string)d.Element(boa1 + "BranchId"),
                    BranchName = (string)d.Element(boa1 + "BranchName"),
                    BusinessKey = (string)d.Element(boa1 + "BusinessKey"),
                    ChannelId = (string)d.Element(boa1 + "ChannelId"),
                    Credit = (string)d.Element(boa1 + "Credit"),
                    CurrentBalance = (string)d.Element(boa1 + "CurrentBalance"),
                    Debit = (string)d.Element(boa1 + "Debit"),
                    Description = (string)d.Element(boa1 + "Description"),
                    FEC = (string)d.Element(boa1 + "FEC"),
                    FECName = (string)d.Element(boa1 + "FECName"),
                    ReceiverIBAN = (string)d.Element(boa1 + "ReceiverIBAN"),
                    ResourceCode = (string)d.Element(boa1 + "ResourceCode"),
                    SenderIBAN = (string)d.Element(boa1 + "SenderIBAN"),
                    SenderIdentityNumber = (string)d.Element(boa1 + "SenderIdentityNumber"),
                    SystemDate = TryDate((string)d.Element(boa1 + "SystemDate")),
                    TranDate = TryDate((string)d.Element(boa1 + "TranDate")),
                    TranRef = (string)d.Element(boa1 + "TranRef"),
                    TranType = (string)d.Element(boa1 + "TranType"),
                    ValueDate = TryDate((string)d.Element(boa1 + "ValueDate")),
                    VirtualIBAN = (string)d.Element(boa1 + "VirtualIBAN"),
                });
            }

            return parsed;
        }

        // Basit XML encode helper (istiyorsan kendi util’ını kullan)
        private static class SecurityHelper
        {
            public static string XmlEncode(string s)
                => string.IsNullOrEmpty(s) ? s : System.Security.SecurityElement.Escape(s);
        }
    }
}
