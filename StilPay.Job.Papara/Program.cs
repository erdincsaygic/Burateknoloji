using DocumentFormat.OpenXml.Bibliography;
using Org.BouncyCastle.Asn1.Ocsp;
using RestSharp;
using StilPay.Job.Papara.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.Models;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using static StilPay.Utility.Helper.Enums;

namespace StilPay.Job.Papara
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var startUp = new Startup();

            Dictionary<string, string> header = new Dictionary<string, string>();
            Dictionary<string, object> body = new Dictionary<string, object>();


            var bankId = startUp.PaparaApi.bank_id;
            var companyBankAccountID = startUp.PaparaApi.companyBankAccountID;
            var transaction_url = startUp.PaparaApi.transaction_url;
            var startDate = startUp.PaparaApi.startDate;
            var endDate = startUp.PaparaApi.endDate;
            var queryperiodintervalsecond = startUp.PaparaApi.query_period_interval_second;
            var notificationRangeMinute = startUp.PaparaApi.notification_range_minute;
            var transactionRangeClock = startUp.PaparaApi.transaction_range_hour;
            var page = startUp.PaparaApi.page;
            var pageSize = startUp.PaparaApi.pageSize;
            var dateTimeEndDate = DateTime.Now;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            int accountTransactionCount = 0;
            int tableInsertionErrorCount = 0;
            int tableInsertionSuccessCount = 0;
            int timeoutNotificationsCount = 0;

            if (string.IsNullOrEmpty(endDate))
            {       
                endDate = dateTimeEndDate.ToString("MM/dd/yyyy HH:mm:ss");
            }

            if (string.IsNullOrEmpty(startDate))
            {
                var startTimeEndDate = dateTimeEndDate.AddHours(transactionRangeClock * -1);
                startDate = startTimeEndDate.ToString("MM/dd/yyyy HH:mm:ss");
            }

            string previousDay = null;

            Console.WriteLine(
                    string.Concat("-------------------------------------------------",
                                   Environment.NewLine, Environment.NewLine,
                                  "            VTN PAPARA-BANK WORKING..            ",
                                   Environment.NewLine, Environment.NewLine,
                                  "-------------------------------------------------")
            );

            while (true)
            {
                #region Papara Api
                int responseStatus = 0;

                try
                {


                    DateTime dateNow = DateTime.Now;
                    TimeSpan timeLimit = new TimeSpan(10, 0, 0);

                    if (dateNow.TimeOfDay < timeLimit)
                        previousDay = DateTime.Today.AddDays(-1).ToString("MM/dd/yyyy HH:mm:ss");
                    else
                        previousDay = null;

                    header.Clear(); body.Clear();

                    header.Add("ApiKey", tSQLBankManager.GetSystemSettingValues("Papara").FirstOrDefault(x => x.ParamDef == "api_key").ParamVal);
    
                    body.Add("startDate", startDate);
                    body.Add("endDate", endDate);
                    body.Add("page", 1);
                    body.Add("pageSize", 50);

                    var transactionModel = tHttpClientManager<PaparaTransactionModel.Root>.PostJsonDataGetJson(transaction_url, header, body);

                    if (transactionModel != null && transactionModel.Data != null && transactionModel.Succeeded)
                    {
                        foreach (var hareketDetay in transactionModel.Data.Items.OrderByDescending(o => Convert.ToDateTime(o.CreatedAt)))
                        {
                            if (!tSQLBankManager.HasPaymentTransferPool(hareketDetay.Id.ToString()) && !tSQLBankManager.HasNotificationTransaction(hareketDetay.Id.ToString()))
                            {
                                var isCaughtInFraudControl = false;
                                var fraudDescription = "Fraud kontrolleri başarıyla tamamlandı.";
                                var isTrusted = false;
                                var (Result, ReferenceNr, ServiceId, CallbackUrl, AutoTransferLimit) = tSQLBankManager.CheckReferenceNr(hareketDetay.Description);

                                var paymentTransferPoolDescriptionControlList = tSQLBankManager.GetPaymentTransferPoolDescriptionControls();
                                var isHaveBlockedWord = paymentTransferPoolDescriptionControlList.Any(x =>
                                   hareketDetay.Description.ToLower().Contains(x.Name)
                                );

                                if (!string.IsNullOrEmpty(Result) && !string.IsNullOrEmpty(ReferenceNr) && Result != "" && ReferenceNr != "" && ServiceId != "")
                                {
                                    var (IsTrusted, FraudResult, FraudDescription) = tSQLBankManager.TransferCheckFraudControl(null, ReferenceNr, hareketDetay.DescriptionTitle, ServiceId, hareketDetay.Amount);
                                    isCaughtInFraudControl = !FraudResult;
                                    fraudDescription = IsTrusted ? "Aynı kişi 24 saat veya öncesinde ödeme yaptı, kişi güvenilir olduğu için fraud kontrolü yapılmadı" : FraudDescription;
                                    isTrusted = IsTrusted;
                                }

                                if (!string.IsNullOrEmpty(Result) && !string.IsNullOrWhiteSpace(Result) && !string.IsNullOrEmpty(ReferenceNr) && !string.IsNullOrWhiteSpace(ReferenceNr) && Result == "OK" && ServiceId != "" && CallbackUrl != "" && !tSQLBankManager.HasNotificationTransaction(hareketDetay.Id.ToString()) && !tSQLBankManager.HasPaymentTransferPool(hareketDetay.Id.ToString()) && (isTrusted || hareketDetay.Amount <= AutoTransferLimit) && !isHaveBlockedWord && !isCaughtInFraudControl)
                                {
                                    string transactionId = DateTime.Now.Ticks.ToString("D16");

                                    var transactionNr = tSQLBankManager.AddNotificationTransaction(DateTime.Now, hareketDetay.CreatedAt, hareketDetay.CreatedAt, bankId, ServiceId, transactionId, hareketDetay.Id.ToString(),hareketDetay.Amount,hareketDetay.Description, "00000000-0000-0000-0000-000000000000", hareketDetay.DescriptionTitle, "11111111111", false, true);

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
                                                transfer_date = hareketDetay.CreatedAt,
                                                amount = hareketDetay.Amount
                                            },
                                            user_entered_data = new
                                            {
                                                sender_name = hareketDetay.DescriptionTitle,
                                                bank_description = hareketDetay.Description
                                            }
                                        };

                                        var pyID = tSQLBankManager.AddAutoPaymentNotification(hareketDetay.CreatedAt, bankId, hareketDetay.DescriptionTitle, ServiceId, transactionId, hareketDetay.Id.ToString(), hareketDetay.Amount, "Otomatik Bakiye Yükleme İşlemi Bildirimi", "", companyBankAccountID, isCaughtInFraudControl, fraudDescription);

                                        var pyTransactionNr = tSQLBankManager.GetPaymentNotificationTransactionNr(pyID);

                                        var IDOutAuto = tSQLBankManager.AddPaymentTransferPoolWithReference(hareketDetay.CreatedAt, bankId, hareketDetay.DescriptionTitle, GetIbanFromDescription(hareketDetay.Description), hareketDetay.Amount, hareketDetay.Id.ToString(), hareketDetay.Description, true, companyBankAccountID, pyTransactionNr, transactionId, isCaughtInFraudControl, fraudDescription);

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
                                    if (!tSQLBankManager.HasPaymentTransferPool(hareketDetay.Id.ToString()) && !tSQLBankManager.HasNotificationTransaction(hareketDetay.Id.ToString()))
                                    {
                                        var IDOut = tSQLBankManager.AddPaymentTransferPool(hareketDetay.CreatedAt, bankId, hareketDetay.DescriptionTitle, GetIbanFromDescription(hareketDetay.Description) ?? "",hareketDetay.Amount, hareketDetay.Id.ToString(), hareketDetay.Description, companyBankAccountID, isHaveBlockedWord ? (byte)Enums.StatusType.Risk : 1, isCaughtInFraudControl, fraudDescription);

                                        tableInsertionErrorCount = string.IsNullOrEmpty(IDOut) ? tableInsertionErrorCount + 1 : tableInsertionErrorCount;
                                        tableInsertionSuccessCount = string.IsNullOrEmpty(IDOut) ? tableInsertionSuccessCount : tableInsertionSuccessCount + 1;
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



                #endregion


                dateTimeEndDate = DateTime.Now;
                endDate = dateTimeEndDate.ToString("MM/dd/yyyy HH:mm:ss");
                startDate = dateTimeEndDate.AddHours(transactionRangeClock * -1).ToString("MM/dd/yyyy HH:mm:ss");
                responseStatus = 0;
                previousDay = null;

                Console.WriteLine(
                string.Concat(Environment.NewLine, Environment.NewLine,
                              $"Bankaya Atılan Sorgu Başlangıç Tarihi: {previousDay ?? startDate}\n",
                              $"Bankaya Atılan Sorgu Bitiş Tarihi: {endDate}\n",
                              $"Hesap Hareketleri Sayısı : {accountTransactionCount}\n",
                              $"Tabloya Kayıt Edilen Başarılı İşlem Sayısı : {tableInsertionSuccessCount}\n",
                              $"Tabloya Kayıt Edilen Hatalı İşlem Sayısı Sayısı: {tableInsertionErrorCount}\n",
                              $"Zaman Aşımına Uğrayan Bildirim Sayısı: {timeoutNotificationsCount}\n",
                              "-------------------------------------------------"));

                Thread.Sleep(queryperiodintervalsecond * 1000);

                //#endregion

            }
        }


        static string GetIbanFromDescription(string description)
        {
            // IBAN'ları bulmak için esnek bir regex deseni oluştur
            Regex regex = new Regex(@"\bTR\d{2}( ?\d{4}){5}\b");

            // Eşleşen değeri bul ve döndür
            Match match = regex.Match(description);
            return match.Success ? match.Value.Replace(" ", "") : string.Empty;
        }
    }
}
