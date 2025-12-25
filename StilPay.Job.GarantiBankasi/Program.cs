using Irony.Parsing.Construction;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using StilPay.Job.GarantiBankasi.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.Models;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using static StilPay.Utility.Helper.Enums;

namespace StilPay.Job.GarantiBankasi
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            int accountTransactionCount = 0;
            int tableInsertionErrorCount = 0;
            int tableInsertionSuccessCount = 0;
            int timeoutNotificationsCount = 0;

            var startUp = new Startup();

            Console.WriteLine(
                string.Concat("-------------------------------------------------",
                               Environment.NewLine, Environment.NewLine,
                              "       OVG GARANTİ-BBVA BANK WORKING..       ",
                               Environment.NewLine, Environment.NewLine,
                              "-------------------------------------------------")
            );

            Dictionary<string, string> header = new Dictionary<string, string>();
            Dictionary<string, object> body = new Dictionary<string, object>();

            var bankIdGaranti = startUp.GarantiApi.bank_id;
            var baseUrlGaranti = startUp.GarantiApi.base_url;
            var tokenUrlGaranti = startUp.GarantiApi.token_url;
            var transactionUrlGaranti = startUp.GarantiApi.transaction_url;
            var queryperiodintervalsecond = startUp.GarantiApi.query_period_interval_second;
            var transactionRangeClock = startUp.GarantiApi.transaction_range_hour;
            var notificationRangeMinute = startUp.GarantiApi.notification_range_minute;
            var endDateGaranti = DateTime.Now;
            var startDateGaranti = endDateGaranti.AddHours(transactionRangeClock * -1);
            var companyBankAccountID = startUp.GarantiApi.companyBankAccountID;

            while (true)
            {
                #region Garanti Bankası Api
                int responseStatus = 0;
                header.Clear(); body.Clear();

                try
                {
                    foreach (var auth in startUp.GarantiAuth)
                        body.Add(auth.Key, auth.Value);

                    var tokenModel = tHttpClientManager<GarantiTokenModel>.PostFormDataGetJson(string.Concat(baseUrlGaranti, tokenUrlGaranti), header, body);
                    if (tokenModel != null && !string.IsNullOrEmpty(tokenModel.AccessToken))
                    {
                        header.Clear(); body.Clear();

                        header.Add("Authorization", string.Concat(tokenModel.TokenType, " ", tokenModel.AccessToken));

                        foreach (var account in startUp.GarantiAccount)
                            body.Add(account.Key, account.Value);

                        body.Add("startDate", startDateGaranti.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                        body.Add("endDate", endDateGaranti.ToString("yyyy-MM-ddTHH:mm:ss.fff"));

                        var transactionModel = tHttpClientManager<GarantiTransactionModel>.PostJsonDataGetJson(string.Concat(baseUrlGaranti, transactionUrlGaranti), header, body);

                        if (transactionModel != null && transactionModel.Result != null && transactionModel.Result.ReturnCode == 200)
                        {
                            if (transactionModel.Transactions != null && transactionModel.Transactions.Count > 0)
                            {
                                accountTransactionCount = transactionModel.Transactions.Count();

                                foreach (var hareketDetay in transactionModel.Transactions.OrderByDescending(o => Convert.ToDateTime(o.TransactionInstanceId)))
                                {
                                    if (!tSQLBankManager.HasPaymentTransferPool(hareketDetay.TransactionInstanceId) && !tSQLBankManager.HasNotificationTransaction(hareketDetay.TransactionInstanceId) && (hareketDetay.ClasificationCode.Equals("NTRF") || hareketDetay.ClasificationCode.Equals("NEFT") || hareketDetay.ClasificationCode.Equals("NMSC")) && (hareketDetay.SenderTCKN != null || hareketDetay.SenderVKN != null))
                                    {
                                        var senderName = hareketDetay.EnrichmentInformation.Any(f => !string.IsNullOrEmpty(f.EnrichmentCode) && f.EnrichmentCode == "MUS") ? hareketDetay.EnrichmentInformation.FirstOrDefault(f => !string.IsNullOrEmpty(f.EnrichmentCode) && f.EnrichmentCode == "MUS").EnrichmentValue.SenderName : hareketDetay.Explanation;

                                        var senderIban = hareketDetay.EnrichmentInformation.Any(f => !string.IsNullOrEmpty(f.EnrichmentCode) && f.EnrichmentCode == "CORRACNT") ? hareketDetay.EnrichmentInformation.FirstOrDefault(f => !string.IsNullOrEmpty(f.EnrichmentCode) && f.EnrichmentCode == "CORRACNT").EnrichmentValue.SenderIban : " ";

                                        senderName ??= hareketDetay.Explanation;
                                        var isCaughtInFraudControl = false;
                                        var fraudDescription = "Fraud kontrolleri başarıyla tamamlandı.";
                                        var isTrusted = false;
                                        var (Result, ReferenceNr, ServiceId, CallbackUrl, AutoTransferLimit) = tSQLBankManager.CheckReferenceNr(hareketDetay.Explanation);

                                        var paymentTransferPoolDescriptionControlList = tSQLBankManager.GetPaymentTransferPoolDescriptionControls();
                                        var isHaveBlockedWord = paymentTransferPoolDescriptionControlList.Any(x =>
                                           hareketDetay.Explanation.ToLower().Contains(x.Name)
                                        );

                                        if (!string.IsNullOrEmpty(Result) && !string.IsNullOrEmpty(ReferenceNr) && Result != "" && ReferenceNr != "" && ServiceId != "")
                                        {
                                            var (IsTrusted, FraudResult, FraudDescription) = tSQLBankManager.TransferCheckFraudControl(null, ReferenceNr, senderName, ServiceId, hareketDetay.Amount);
                                            isCaughtInFraudControl = !FraudResult;
                                            fraudDescription = IsTrusted ? "Aynı kişi 24 saat veya öncesinde ödeme yaptı, kişi güvenilir olduğu için fraud kontrolü yapılmadı" : FraudDescription;
                                            isTrusted = IsTrusted;
                                        }

                                        if (!string.IsNullOrEmpty(Result) && !string.IsNullOrWhiteSpace(Result) && !string.IsNullOrEmpty(ReferenceNr) && !string.IsNullOrWhiteSpace(ReferenceNr) && Result == "OK" && ServiceId != "" && CallbackUrl != "" && !tSQLBankManager.HasNotificationTransaction(hareketDetay.TransactionInstanceId) && !tSQLBankManager.HasPaymentTransferPool(hareketDetay.TransactionInstanceId) && (isTrusted || hareketDetay.Amount <= AutoTransferLimit) && !isHaveBlockedWord && !isCaughtInFraudControl)
                                        {
                                            string transactionId = DateTime.Now.Ticks.ToString("D16");

                                            var transactionNr = tSQLBankManager.AddNotificationTransaction(DateTime.Now, Convert.ToDateTime(hareketDetay.TransactionInstanceId), Convert.ToDateTime(hareketDetay.TransactionInstanceId), bankIdGaranti, ServiceId, transactionId, hareketDetay.TransactionInstanceId, hareketDetay.Amount, hareketDetay.Explanation, "00000000-0000-0000-0000-000000000000", senderName ?? "", "11111111111", false, true);

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
                                                        transfer_date = Convert.ToDateTime(hareketDetay.TransactionInstanceId),
                                                        amount = hareketDetay.Amount
                                                    },
                                                    user_entered_data = new
                                                    {
                                                        sender_name = senderName,
                                                        bank_description = hareketDetay.Explanation
                                                    }
                                                };

                                        
                                                var pyID = tSQLBankManager.AddAutoPaymentNotification(Convert.ToDateTime(hareketDetay.TransactionInstanceId), bankIdGaranti, senderName ?? hareketDetay.Explanation, ServiceId, transactionId, hareketDetay.TransactionInstanceId, hareketDetay.Amount, "Otomatik Bakiye Yükleme İşlemi Bildirimi", senderIban, companyBankAccountID, isCaughtInFraudControl, fraudDescription);

                                                var pyTransactionNr = tSQLBankManager.GetPaymentNotificationTransactionNr(pyID);

                                                var IDOutAuto = tSQLBankManager.AddPaymentTransferPoolWithReference(Convert.ToDateTime(hareketDetay.TransactionInstanceId), bankIdGaranti, senderName ?? hareketDetay.Explanation, senderIban ?? " ", hareketDetay.Amount, hareketDetay.TransactionInstanceId, hareketDetay.Explanation, true, companyBankAccountID, pyTransactionNr, transactionId, isCaughtInFraudControl, fraudDescription);

                                                if (pyID != null && IDOutAuto != null)
                                                {
                                                    tSQLBankManager.SetPaymentTransactionStatus(pyID, (int)StatusType.Confirmed, "Otomatik Bakiye Yükleme İşlemi Bildirimi");

                                                    var response = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                                                    if (response != null && response.Result != null && !string.IsNullOrEmpty(response.Result.Status))
                                                    {
                                                        tSQLBankManager.AcceptNotificationTransaction(transactionNr);

                                                        responseStatus = response.Result.Status switch
                                                        {
                                                            "OK" => 1,
                                                            "RED" => 2,
                                                            "ERROR" => 3,
                                                            _ => 0,
                                                        };
                                                    }

                                                    var opt = new JsonSerializerOptions() { WriteIndented = true };
                                                    tSQLBankManager.AddCallbackResponseLog(transactionId, "STILPAY", System.Text.Json.JsonSerializer.Serialize(dataCallback, opt), companyIntegration.ID, "Ödeme Bildirimi", responseStatus);

                                                    tableInsertionErrorCount = string.IsNullOrEmpty(IDOutAuto) ? tableInsertionErrorCount + 1 : tableInsertionErrorCount;
                                                    tableInsertionSuccessCount = string.IsNullOrEmpty(IDOutAuto) ? tableInsertionSuccessCount : tableInsertionSuccessCount + 1;
                                                }                                              
                                            }
                                        }
                                        else
                                        {
                                            if (!tSQLBankManager.HasPaymentTransferPool(hareketDetay.TransactionInstanceId) && !tSQLBankManager.HasNotificationTransaction(hareketDetay.TransactionInstanceId))
                                            {
                                                var IDOut = tSQLBankManager.AddPaymentTransferPool(Convert.ToDateTime(hareketDetay.TransactionInstanceId), bankIdGaranti, senderName ?? " ", senderIban ?? " ", hareketDetay.Amount, hareketDetay.TransactionInstanceId, hareketDetay.Explanation, companyBankAccountID, isHaveBlockedWord ? (byte)Enums.StatusType.Risk : 1, isCaughtInFraudControl, fraudDescription);

                                                tableInsertionErrorCount = string.IsNullOrEmpty(IDOut) ? tableInsertionErrorCount + 1 : tableInsertionErrorCount;
                                                tableInsertionSuccessCount = string.IsNullOrEmpty(IDOut) ? tableInsertionSuccessCount : tableInsertionSuccessCount + 1;
                                            }
                                        }
                                    }
                                }
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


                endDateGaranti = DateTime.Now;
                startDateGaranti = endDateGaranti.AddHours(transactionRangeClock * -1);
                responseStatus = 0;

                Console.WriteLine(
                string.Concat(Environment.NewLine, Environment.NewLine,
                              $"Bankaya Atılan Sorgu Başlangıç Tarihi: {startDateGaranti}\n",
                              $"Bankaya Atılan Sorgu Bitiş Tarihi: {endDateGaranti}\n",
                              $"Hesap Hareketleri Sayısı : {accountTransactionCount}\n",
                              $"Tabloya Kayıt Edilen Başarılı İşlem Sayısı : {tableInsertionSuccessCount}\n",
                              $"Tabloya Kayıt Edilen Hatalı İşlem Sayısı Sayısı: {tableInsertionErrorCount}\n",
                              $"Zaman Aşımına Uğrayan Bildirim Sayısı: {timeoutNotificationsCount}\n",
                              "-------------------------------------------------"));
                #endregion

                Thread.Sleep(queryperiodintervalsecond * 1000);
            }
        }
    }
}
