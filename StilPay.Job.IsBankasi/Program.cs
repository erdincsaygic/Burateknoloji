
// İŞ BANKASI HAVUZ

using DocumentFormat.OpenXml.VariantTypes;
using Newtonsoft.Json.Linq;
using RestSharp;
using StilPay.Job.IsBankasi.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.Models;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using static StilPay.Utility.Helper.Enums;

namespace StilPay.Job.IsBankasi
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var startUp = new Startup();

            Dictionary<string, string> header = new Dictionary<string, string>();
            Dictionary<string, object> body = new Dictionary<string, object>();


            var bankIdIs = startUp.IsApi.bank_id;
            var bankAccountNr = startUp.IsApi.bank_account_nr;
            var baseUrlIs = startUp.IsApi.base_url;
            var transactionUrlIs = startUp.IsApi.transaction_url;
            var queryperiodintervalsecond = startUp.IsApi.query_period_interval_second;
            var transactionRangHour = startUp.IsApi.transaction_range_hour;
            var notificationRangeMinute = startUp.IsApi.notification_range_minute;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            int accountTransactionCount = 0;
            int tableInsertionErrorCount = 0;
            int tableInsertionSuccessCount = 0;
            int timeoutNotificationsCount = 0;

            var endDate = DateTime.Now;
            var startDate = endDate.AddHours(transactionRangHour * -1);
            int responseStatus = 0;
            var companyBankAccountID = startUp.IsApi.companyBankAccountID;
            Console.WriteLine(
                    string.Concat("-------------------------------------------------",
                                   Environment.NewLine, Environment.NewLine,
                                  "            STILPAY İŞ-BANK WORKING..            ",
                                   Environment.NewLine, Environment.NewLine,
                                  "-------------------------------------------------")
            );

            while (true)
            {
                #region Is Bankası Api
                try
                {

                    header.Clear(); body.Clear();

                    foreach (var account in startUp.IsAccount)
                        body.Add(account.Key, account.Value);

                    body.Add("BeginDate ", startDate);
                    body.Add("EndDate ", endDate);

                    var transactionModel = tHttpClientManager<IsTransactionModel>.PostFormDataGetXML(string.Concat(baseUrlIs, transactionUrlIs), header, body);

                    if (transactionModel != null && transactionModel.Hesaplar != null && transactionModel.Hesaplar.Hesap != null)
                    {
                        var hesap = transactionModel.Hesaplar.Hesap.FirstOrDefault(f => f.Tanimlamalar != null && !string.IsNullOrEmpty(f.Tanimlamalar.HesapNo) && !string.IsNullOrEmpty(bankAccountNr) && f.Tanimlamalar.HesapNo.Equals(bankAccountNr));
                        accountTransactionCount = hesap.Hareketler.Hareket.Count();
                        if (hesap != null)
                        {
                            if (hesap.Hareketler != null && hesap.Hareketler.Hareket != null)
                            {
                                foreach (var hareketDetay in hesap.Hareketler.Hareket.OrderByDescending(o => Convert.ToDateTime(o.Tarih)))
                                {
                                    if (!tSQLBankManager.HasPaymentTransferPool(hareketDetay.HareketSirano) && !tSQLBankManager.HasNotificationTransaction(hareketDetay.HareketSirano) && hareketDetay.Miktar > 0)
                                    {
                                        DateTime dt3 = new DateTime(int.Parse(hareketDetay.TimeStamp.Substring(0, 4)), int.Parse(hareketDetay.TimeStamp.Substring(05, 2)), int.Parse(hareketDetay.TimeStamp.Substring(8, 2)), int.Parse(hareketDetay.TimeStamp.Substring(11, 2)), int.Parse(hareketDetay.TimeStamp.Substring(14, 2)), int.Parse(hareketDetay.TimeStamp.Substring(17, 2)), int.Parse(hareketDetay.TimeStamp.Substring(20, 2)));

                                        var isCaughtInFraudControl = false;
                                        var fraudDescription = "Fraud kontrolleri başarıyla tamamlandı.";
                                        var isTrusted = false;
                                        var (Result, ReferenceNr, ServiceId, CallbackUrl, AutoTransferLimit) = tSQLBankManager.CheckReferenceNr(hareketDetay.Aciklama);

                                        var paymentTransferPoolDescriptionControlList = tSQLBankManager.GetPaymentTransferPoolDescriptionControls();
                                        var isHaveBlockedWord = paymentTransferPoolDescriptionControlList.Any(x =>
                                           hareketDetay.Aciklama.ToLower().Contains(x.Name)
                                        );

                                        if (!string.IsNullOrEmpty(Result) && !string.IsNullOrEmpty(ReferenceNr) && Result != "" && ReferenceNr != "" && ServiceId != "")
                                        {
                                            var (IsTrusted, FraudResult, FraudDescription) = tSQLBankManager.TransferCheckFraudControl(null, ReferenceNr, hareketDetay.KarsiHesSahipAdUnvan.Trim(), ServiceId, hareketDetay.Miktar);
                                            isCaughtInFraudControl = !FraudResult;
                                            fraudDescription = IsTrusted ? "Aynı kişi 24 saat veya öncesinde ödeme yaptı, kişi güvenilir olduğu için fraud kontrolü yapılmadı" : FraudDescription;
                                            isTrusted = IsTrusted;
                                        }

                                        if (!string.IsNullOrEmpty(Result) && !string.IsNullOrWhiteSpace(Result) && !string.IsNullOrEmpty(ReferenceNr) && !string.IsNullOrWhiteSpace(ReferenceNr) && Result == "OK" && ServiceId != "" && CallbackUrl != "" && !tSQLBankManager.HasNotificationTransaction(hareketDetay.HareketSirano) && !tSQLBankManager.HasPaymentTransferPool(hareketDetay.HareketSirano) && (isTrusted || hareketDetay.Miktar <= AutoTransferLimit) && !isHaveBlockedWord && !isCaughtInFraudControl)
                                        {
                                            string transactionId = DateTime.Now.Ticks.ToString("D16");

                                            var transactionNr = tSQLBankManager.AddNotificationTransaction(DateTime.Now, dt3, dt3, bankIdIs, ServiceId, transactionId, hareketDetay.HareketSirano, hareketDetay.Miktar, hareketDetay.Aciklama, "00000000-0000-0000-0000-000000000000", hareketDetay.KarsiHesSahipAdUnvan.Trim(), "11111111111", false, true);

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
                                                        transfer_date = dt3,
                                                        amount = hareketDetay.Miktar
                                                    },
                                                    user_entered_data = new
                                                    {
                                                        sender_name = hareketDetay.KarsiHesSahipAdUnvan.Trim(),
                                                        bank_description = hareketDetay.Aciklama
                                                    }
                                                };

                                                var pyID = tSQLBankManager.AddAutoPaymentNotification(dt3, bankIdIs, hareketDetay.KarsiHesSahipAdUnvan.Trim(), ServiceId, transactionId, hareketDetay.HareketSirano, hareketDetay.Miktar, "Otomatik Bakiye Yükleme İşlemi Bildirimi", "", companyBankAccountID, isCaughtInFraudControl, fraudDescription);

                                                var pyTransactionNr = tSQLBankManager.GetPaymentNotificationTransactionNr(pyID);

                                                var IDOutAuto = tSQLBankManager.AddPaymentTransferPoolWithReference(dt3, bankIdIs, hareketDetay.KarsiHesSahipAdUnvan.Trim(), "", hareketDetay.Miktar, hareketDetay.HareketSirano, hareketDetay.Aciklama, true, companyBankAccountID, pyTransactionNr, transactionId, isCaughtInFraudControl, fraudDescription);


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
                                            if (!tSQLBankManager.HasPaymentTransferPool(hareketDetay.HareketSirano) && !tSQLBankManager.HasNotificationTransaction(hareketDetay.HareketSirano))
                                            {

                                                var IDOut = tSQLBankManager.AddPaymentTransferPool(dt3, bankIdIs, hareketDetay.KarsiHesSahipAdUnvan.Trim() ?? "", "", hareketDetay.Miktar, hareketDetay.HareketSirano, hareketDetay.Aciklama, companyBankAccountID, isHaveBlockedWord ? (byte)Enums.StatusType.Risk : 1, isCaughtInFraudControl, fraudDescription);

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



                #endregion

                endDate = DateTime.Now;
                startDate = endDate.AddHours(transactionRangHour * -1);
                Console.WriteLine(
                string.Concat(Environment.NewLine, Environment.NewLine,
                              $"Bankaya Atılan Sorgu Başlangıç Tarihi: {startDate}\n",
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

    }
}