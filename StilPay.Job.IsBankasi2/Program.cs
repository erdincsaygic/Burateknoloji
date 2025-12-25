using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using RestSharp;
using StilPay.Job.IsBankasi.Models;
using StilPay.Utility.Models;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.Xml;
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
            var companyBankAccountID = startUp.IsApi.companyBankAccountID;

            Console.WriteLine(
                    string.Concat("-------------------------------------------------",
                                   Environment.NewLine, Environment.NewLine,
                                  "            STILPAY-OVG İŞ-BANK WORKING..            ",
                                   Environment.NewLine, Environment.NewLine,
                                  "-------------------------------------------------")
            );

            while (true)
            {
                #region Is Bankası Api
                try
                {
                    int responseStatus = 0;

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
                                    if (!tSQLBankManager.HasPaymentTransferPool(hareketDetay.HareketSirano))
                                    {
                                        var (Result, ReferenceNr, ServiceId, CallbackUrl, AutoTransferLimit) = tSQLBankManager.CheckReferenceNr(hareketDetay.Aciklama);

                                        if (!string.IsNullOrEmpty(Result) && !string.IsNullOrWhiteSpace(Result) && !string.IsNullOrEmpty(ReferenceNr) && !string.IsNullOrWhiteSpace(ReferenceNr) && Result == "OK" && ServiceId != "" && CallbackUrl != "" && !tSQLBankManager.HasNotificationTransaction(hareketDetay.HareketSirano) && hareketDetay.Miktar <= AutoTransferLimit)
                                        {
                                            string transactionId = DateTime.Now.Ticks.ToString("D16");

                                            var transactionNr = tSQLBankManager.AddNotificationTransaction(DateTime.Now, Convert.ToDateTime(hareketDetay.Tarih), Convert.ToDateTime(hareketDetay.Tarih), bankIdIs, ServiceId, transactionId, hareketDetay.HareketSirano, hareketDetay.Miktar, hareketDetay.Aciklama, "00000000-0000-0000-0000-000000000000", hareketDetay.KarsiHesSahipAdUnvan.Trim(), "11111111111", false, true);

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
                                                        transfer_date = Convert.ToDateTime(hareketDetay.Tarih),
                                                        amount = hareketDetay.Miktar
                                                    },
                                                    user_entered_data = new
                                                    {
                                                        sender_name = hareketDetay.KarsiHesSahipAdUnvan.Trim(),
                                                        bank_description = hareketDetay.Aciklama
                                                    }
                                                };

                                                var pyID = tSQLBankManager.AddAutoPaymentNotification(Convert.ToDateTime(hareketDetay.Tarih), bankIdIs, hareketDetay.KarsiHesSahipAdUnvan.Trim(), ServiceId, transactionId, hareketDetay.HareketSirano, hareketDetay.Miktar, "Otomatik Bakiye Yükleme İşlemi Bildirimi", "", companyBankAccountID);

                                                var pyTransactionNr = tSQLBankManager.GetPaymentNotificationTransactionNr(pyID);

                                                var IDOutAuto = tSQLBankManager.AddPaymentTransferPoolWithReference(Convert.ToDateTime(hareketDetay.Tarih), bankIdIs, hareketDetay.KarsiHesSahipAdUnvan.Trim(), "", hareketDetay.Miktar, hareketDetay.HareketSirano, hareketDetay.Aciklama, true, companyBankAccountID, pyTransactionNr, transactionId);

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
                                        else
                                        {
                                            var IDOut = tSQLBankManager.AddPaymentTransferPool(Convert.ToDateTime(hareketDetay.Tarih), bankIdIs, hareketDetay.KarsiHesSahipAdUnvan.Trim() ?? "", "", hareketDetay.Miktar, hareketDetay.HareketSirano, hareketDetay.Aciklama, companyBankAccountID);

                                            tableInsertionErrorCount = string.IsNullOrEmpty(IDOut) ? tableInsertionErrorCount + 1 : tableInsertionErrorCount;
                                            tableInsertionSuccessCount = string.IsNullOrEmpty(IDOut) ? tableInsertionSuccessCount : tableInsertionSuccessCount + 1;
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