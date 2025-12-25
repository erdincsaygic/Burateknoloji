using DocumentFormat.OpenXml.Drawing.Charts;
using Newtonsoft.Json.Linq;
using StilPay.Utility.Models;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using ZiraatBankReferance;
using static StilPay.Utility.Helper.Enums;

namespace StilPay.Job.GeneralBank
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            int requestCount = 0;
            int bankTransactionsCount = 0;
            int validNotificationsCount = 0;
            int invalidNotificationsCount = 0;
            int timeoutNotificationsCount = 0;
            var startup = new Startup();

            var ziraatBankId = startup.GeneralApi.bank_id;
            var queryperiodintervalsecond = startup.GeneralApi.query_period_interval_second;
            var transactionRangeHour = startup.GeneralApi.transaction_range_hour;
            var notificationRangeMinute = startup.GeneralApi.notification_range_minute;

            var EndDate = DateTime.Now;
            var StartDate = EndDate.AddHours(transactionRangeHour * -1);
            int hareketSayisi = 0;

            Console.WriteLine(
                    string.Concat("-------------------------------------------------",
                                   Environment.NewLine, Environment.NewLine,
                                  "            STILPAY GENERAL-BANK WORKING..            ",
                                   Environment.NewLine, Environment.NewLine,
                                  "-------------------------------------------------"));
            while (true)
            {
                #region General Bank Api
                int responseStatus = 0;
                var paymentNotifications = tSQLBankManager.GetPaymentNotifications();
                var banks = tSQLBankManager.GetBankList();
                paymentNotifications.ForEach(py =>
                {
                    try
                    {
                        var companyIntegration = tSQLBankManager.GetCompanyIntegration(py.ServiceID);

                        if (companyIntegration != null && !string.IsNullOrEmpty(companyIntegration.SecretKey) && !string.IsNullOrEmpty(companyIntegration.CallbackUrl))
                        {
                            var paymentTransfer = tSQLBankManager.GetPaymentTransferPool();

                            hareketSayisi = paymentTransfer.Count();

                            requestCount++;

                            if (paymentTransfer != null)
                            {
                                var bankTransactions = paymentTransfer.OrderByDescending(o => Convert.ToDateTime(o.TransactionDate)).Where(w =>
                                    ((!string.IsNullOrEmpty(w.SenderName) && ReplaceTurkish(w.SenderName,py.SenderName))
                                    || (!string.IsNullOrEmpty(w.SenderName) && w.SenderName.Replace(" ", "").ToLower().Equals(py.SenderName.Replace(" ", "").ToLower()))
                                    || (!string.IsNullOrEmpty(w.SenderName) && CheckMaskedName(w.SenderName, py.SenderName))
                                    || (!string.IsNullOrEmpty(w.Description) && w.Description.Replace(" ", "").ToLower().Contains(py.SenderName.Replace(" ", "").ToLower()))
                                    || (!string.IsNullOrEmpty(w.Description) && ReplaceTurkish(w.SenderName, py.SenderName)))
                                    && w.Amount == py.Amount
                                    && Convert.ToDateTime(w.TransactionDate) >= py.ActionDateTime.AddDays(-1)
                                ).ToList();

                                bankTransactionsCount++;

                                var IsCorrectTransaction = false;

                                foreach (var bankTransaction in bankTransactions)
                                {
                                    var transactionKey = bankTransaction.TransactionKey;
                                    string bankCode;
                                    string IDBank;

                                    if (bankTransaction.Description.Contains("ATM"))
                                    {
                                        IDBank = "98";
                                    }
                                    else
                                    {
                                        if (bankTransaction.IDBank == "03")
                                        {
                                            bankCode = "";
                                            int index;
                                            string IDBankString = "";
                                            index = bankTransaction.SenderIban.IndexOf("*");
                                            if (index > 0 && bankTransaction.SenderIban.Count() > index + 4)
                                            {                                              
                                                IDBankString = bankTransaction.SenderIban.Substring(index + 1, 4);
                                                bankCode = IDBankString;
                                            }
                                        }
                                        else
                                        {
                                            bankCode = bankTransaction.SenderIban != null && bankTransaction.SenderIban.Contains("TR") ? bankTransaction.SenderIban.Replace(" ", "")[5..9] : "99";
                                        }

                                        IDBank = banks.FirstOrDefault(x => x.Branch.Equals(bankCode))?.ID;

                                        if (bankTransaction.IDBank == "03" && IDBank == null)
                                        {
                                            IDBank = "03";
                                        }
                                    }

                                    IDBank ??= "99";
                                    tSQLBankManager.SetPaymentNotificationIbanAndBank(py.ID, bankTransaction.SenderIban, IDBank, transactionKey);

                                    var isCaughtInFraudControl = false;
                                    var fraudDescription = "Fraud kontrolleri başarıyla tamamlandı.";
                                    var isTrusted = false;

                                    if (bankTransaction.IsCaughtInFraudControl)
                                    {
                                        tSQLBankManager.SetManuelNotification(py.ID, bankTransaction.FraudControlDescription);
                                        IsCorrectTransaction = true;
                                        break;
                                    }
                                    else
                                    {
                                        if(!string.IsNullOrEmpty(py.CustomerName) && py.CustomerName != "")
                                        {
                                            var (IsTrusted, FraudResult, FraudDescription) = tSQLBankManager.TransferCheckFraudControl(py.CustomerName, null, py.SenderName, py.ServiceID, py.Amount);
                                            isCaughtInFraudControl = !FraudResult;
                                            fraudDescription = IsTrusted ? "Aynı kişi 24 saat veya öncesinde ödeme yaptı, kişi güvenilir olduğu için fraud kontrolü yapılmadı" : FraudDescription;
                                            isTrusted = IsTrusted;

                                            if(isCaughtInFraudControl && !isTrusted)
                                            {
                                                tSQLBankManager.SetManuelNotification(py.ID, fraudDescription);
                                                tSQLBankManager.SetPaymentTransferPoolFraudControl(bankTransaction.ID,fraudDescription);
                                                IsCorrectTransaction = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (!isCaughtInFraudControl && !tSQLBankManager.HasNotificationTransaction(transactionKey))
                                    {
                                        validNotificationsCount++;

                                        var transactionNr = tSQLBankManager.AddNotificationTransaction(py.ActionDateTime, DateTime.Now, Convert.ToDateTime(bankTransaction.TransactionDate), ziraatBankId, py.ServiceID, py.TransactionID, transactionKey, Convert.ToDecimal(bankTransaction.Amount, CultureInfo.InvariantCulture), bankTransaction.Description, py.IDMember, py.SenderName, py.SenderIdentityNr, false, true);

                                        if (!string.IsNullOrEmpty(transactionNr))
                                        {
                                            var dataCallback = new { status_code = "OK", status_type = 0, service_id = py.ServiceID, ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey), data = new { transaction_id = py.TransactionID, sp_transactionNr = transactionNr, amount = Convert.ToDecimal(bankTransaction.Amount, CultureInfo.InvariantCulture), sp_id = py.ID }, user_entered_data = new { member = py.Member, sender_name = py.SenderName, action_date = py.ActionDate, action_time = py.ActionTime, amount = py.Amount, user_ip = py.MemberIPAddress, user_port = py.MemberPort, bank_description = bankTransaction.Description }
                                            };

                                            // status_type alanı havale - ödeme - eft için 0 kredi kartı 1 

                                            var response = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

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

                                            tSQLBankManager.SetPaymentTransactionStatus(py.ID, (int)StatusType.Confirmed, "İşlem Başarılı", fraudDescription);

                                            tSQLBankManager.SetPaymentTransferPool(bankTransaction.ID, (int)StatusType.Confirmed, py.TransactionNr, py.TransactionID, "İşlem Başarılı", fraudDescription);

                                            var opt = new JsonSerializerOptions() { WriteIndented = true };
                                            tSQLBankManager.AddCallbackResponseLog(py.TransactionID, "STILPAY", JsonSerializer.Serialize(dataCallback, opt), companyIntegration.ID, "Ödeme Bildirimi", responseStatus);

                                            IsCorrectTransaction = true;

                                            break;
                                        }
                                    }
                                    else
                                    {
                                        invalidNotificationsCount++;

                                        var dataCallback = new { status_code = "ERROR", status_type = 0, service_id = py.ServiceID, ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey), data = new { transaction_id = py.TransactionID, sp_transactionNr = py.TransactionNr, amount = py.Amount, sp_id = py.ID, message = "Hata! Daha Önce Onaylanmış Bildirim." }, user_entered_data = new { member = py.Member, sender_name = py.SenderName, action_date = py.ActionDate, action_time = py.ActionTime, amount = py.Amount, user_ip = py.MemberIPAddress, user_port = py.MemberPort } };

                                        var response = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                                        var idNotificationTransaction = tSQLBankManager.AddNotificationTransaction(py.ActionDateTime, DateTime.Now, null, ziraatBankId, py.ServiceID, py.TransactionID, "X~X", py.Amount, "Daha önce onaylanmış.", py.IDMember, py.SenderName, py.SenderIdentityNr, false, false);

                                        tSQLBankManager.SetPaymentTransactionStatus(py.ID, (int)StatusType.Canceled, "Hata! Daha Önce Onaylanmış Bildirim");

                                        if (response != null && response.Result != null && !string.IsNullOrEmpty(response.Result.Status))
                                        {
                                            responseStatus = response.Result.Status switch
                                            {
                                                "OK" => 1,
                                                "RED" => 2,
                                                "ERROR" => 3,
                                                _ => 0,
                                            };
                                        }

                                        var opt = new JsonSerializerOptions() { WriteIndented = true };
                                        tSQLBankManager.AddCallbackResponseLog(py.TransactionID, "STILPAY", System.Text.Json.JsonSerializer.Serialize(dataCallback, opt), companyIntegration.ID, "Ödeme Bildirimi", responseStatus);
                                    }
                                }

                                if (py.CDate < DateTime.Now.AddMinutes(notificationRangeMinute * -1) && !IsCorrectTransaction)
                                {

                                    timeoutNotificationsCount++;

                                    var dataCallback = new { status_code = "ERROR", status_type = 0, service_id = py.ServiceID, ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey), data = new { transaction_id = py.TransactionID, sp_transactionNr = py.TransactionNr, amount = py.Amount, sp_id = py.ID, message = "Bildirim zaman aşımına uğradı." }, user_entered_data = new { member = py.Member, sender_name = py.SenderName, action_date = py.ActionDate, action_time = py.ActionTime, amount = py.Amount, user_ip = py.MemberIPAddress, user_port = py.MemberPort } };

                                    var response = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                                    var idNotificationTransaction = tSQLBankManager.AddNotificationTransaction(py.ActionDateTime, DateTime.Now, null, ziraatBankId, py.ServiceID, py.TransactionID, "X~X", py.Amount, "Ödeme bulunamadı.", py.IDMember, py.SenderName, py.SenderIdentityNr, false, false);

                                    tSQLBankManager.SetPaymentTransactionStatus(py.ID, (int)StatusType.Canceled, "Bildirim zaman aşımına uğradı.");

                                    if (response != null && response.Result != null && !string.IsNullOrEmpty(response.Result.Status))
                                    {
                                        responseStatus = response.Result.Status switch
                                        {
                                            "OK" => 1,
                                            "RED" => 2,
                                            "ERROR" => 3,
                                            _ => 0,
                                        };
                                    }

                                    var opt = new JsonSerializerOptions() { WriteIndented = true };
                                    tSQLBankManager.AddCallbackResponseLog(py.TransactionID, "STILPAY", System.Text.Json.JsonSerializer.Serialize(dataCallback, opt), companyIntegration.ID, "Ödeme Bildirimi", responseStatus);
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
                });

                #endregion
                EndDate = DateTime.Now;
                StartDate = EndDate.AddHours(transactionRangeHour * -1);
                responseStatus = 0;
                Console.WriteLine(
                string.Concat(Environment.NewLine, Environment.NewLine,
                              $"Havuza Atılan Sorgu Başlangıç Tarihi: {EndDate}\n",
                              $"Havuza Atılan Sorgu Bitiş Tarihi: {StartDate}\n",
                              $"Havuza Atılan Sorgu Sayısı : {requestCount}\n",
                              $"Hesap Hareketleri Sayısı : {hareketSayisi}\n",
                              $"Hesap Hareketleri İle Eşleşen Ödeme Bildirimleri Sayısı : {bankTransactionsCount}\n",
                              $"İşleme Alınan Geçerli Ödeme Bildirimleri Sayısı : {validNotificationsCount}\n",
                              $"Geçersiz Ödeme Bildirimleri Sayısı: {invalidNotificationsCount}\n",
                              $"Zaman Aşımına Uğrayan Bildirim Sayısı: {timeoutNotificationsCount}\n",
                              "-------------------------------------------------"));
                Thread.Sleep(queryperiodintervalsecond * 1000);
            }
        }

        static bool ReplaceTurkish ( string bankSenderName, string pySenderName)
        {
            var newBankSenderName = bankSenderName.Replace(" ", "").ToLower().Replace("ö", "o").Replace("ü", "u").Replace("ç", "c").Replace("ş", "s").Replace("ğ", "g").Replace("i", "ı");
            var newPySenderName = pySenderName.Replace(" ", "").ToLower().Replace("ö", "o").Replace("ü", "u").Replace("ç", "c").Replace("ş", "s").Replace("ğ", "g").Replace("i", "ı");

            return newBankSenderName.Equals(newPySenderName);
        }

        static bool CheckMaskedName(string bankSenderName, string pySenderName)
        {
            var maskedParts = new List<string>();
            string[] splitSenderName = pySenderName.Split(' ');

            foreach (var part in splitSenderName)
            {
                if (!string.IsNullOrEmpty(part))
                {
                    // İlk harf al ve sonuna 3 yıldız ekle
                    maskedParts.Add(part.Substring(0, 1) + "***");
                }
            }

            // Tüm maskelemeleri birleştirme
            var fullMaskedName = string.Join(" ", maskedParts);

            return bankSenderName.Equals(fullMaskedName);
        }
    }
}
