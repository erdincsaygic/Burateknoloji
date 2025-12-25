using HalkbankWSDL;
using Newtonsoft.Json.Linq;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading;
using static StilPay.Utility.Helper.Enums;

namespace StilPay.Job.Halkbank
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

            var startup = new Startup();

            var service = new HesapEkstreOrtakClient(HesapEkstreOrtakClient.EndpointConfiguration.basicHttpEndpoint);
            service.ClientCredentials.UserName.UserName = "43279407EPDUSR";
            service.ClientCredentials.UserName.Password = "D5!t/JM6Jz";

            var halkbankId = startup.HalkbankApi.bank_id;
            var queryperiodintervalsecond = startup.HalkbankApi.query_period_interval_second;
            var transactionRangeHour = startup.HalkbankApi.transaction_range_hour;
            var notificationRangeMinute = startup.HalkbankApi.notification_range_minute;

            var startDate = startup.HalkbankApi.startDate;
            var endDate = startup.HalkbankApi.endDate;

            var halkbankEndDate = DateTime.Now;
            var halkbankStartDate = halkbankEndDate.AddHours(transactionRangeHour * -1);

            var response = "";
            var companyBankAccountID = startup.HalkbankApi.companyBankAccountID;
            Console.WriteLine(
                    string.Concat("-------------------------------------------------",
                                   Environment.NewLine, Environment.NewLine,
                                  "            STILPAY HALKBANK WORKING..            ",
                                   Environment.NewLine, Environment.NewLine,
                                  "-------------------------------------------------"));

            while (true)
            {
                #region Halkbank Api
                try
                {
                    var req = new HesapEkstreRequest()
                    {
                        BaslangicTarihi = startDate == "" ? halkbankStartDate : Convert.ToDateTime(startDate),
                        BitisTarihi = endDate == "" ? halkbankEndDate : Convert.ToDateTime(endDate),
                    };

                    var transactionModel = service.EkstreSorgulamaAsync(req).Result;
                    response = transactionModel.HataAciklama;

                    if (transactionModel != null && transactionModel.HataKodu == "0" && transactionModel.Hesaplar != null && transactionModel.Hesaplar.FirstOrDefault().Hareketler != null)
                    {
                        accountTransactionCount = transactionModel.Hesaplar.FirstOrDefault().Hareketler.Count();

                        foreach (var hareketDetay in transactionModel.Hesaplar.FirstOrDefault().Hareketler.OrderByDescending(o => Convert.ToDateTime(o.Tarih)))
                        {
                            hareketDetay.ReferansNo = hareketDetay.ReferansNo == "HV988000000" ? hareketDetay.DekontNo + hareketDetay.ReferansNo : hareketDetay.ReferansNo;

                            if (!tSQLBankManager.HasPaymentTransferPool(hareketDetay.ReferansNo))
                            {
                                var IDOut = tSQLBankManager.AddPaymentTransferPool(DateTime.ParseExact($"{hareketDetay.Tarih} {hareketDetay.Saat}", "dd/MM/yyyy HH:mm:ss", null), halkbankId, hareketDetay.KarsiAdSoyad, hareketDetay.KarsiHesapIBAN ?? "", Convert.ToDecimal(hareketDetay.HareketTutari.Replace(",", "."), CultureInfo.InvariantCulture), hareketDetay.ReferansNo, hareketDetay.Aciklama + " - " + hareketDetay.EkstreAciklama, companyBankAccountID);

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
                #endregion

                halkbankEndDate = DateTime.Now;
                halkbankStartDate = halkbankEndDate.AddHours(transactionRangeHour * -1);
                Console.WriteLine(
                string.Concat(Environment.NewLine, Environment.NewLine,
                              $"Bankaya Atılan Sorgu Başlangıç Tarihi: {halkbankStartDate}\n",
                              $"Bankaya Atılan Sorgu Bitiş Tarihi: {halkbankEndDate}\n",
                              $"Hesap Hareketleri Sayısı : {accountTransactionCount}\n",
                              $"Tabloya Kayıt Edilen Başarılı İşlem Sayısı : {tableInsertionSuccessCount}\n",
                              $"Tabloya Kayıt Edilen Hatalı İşlem Sayısı Sayısı: {tableInsertionErrorCount}\n",
                              $"Zaman Aşımına Uğrayan Bildirim Sayısı: {timeoutNotificationsCount}\n",
                              $"Sorgu Durumu: {response}\n",
                              "-------------------------------------------------"));
                response = "";
                Thread.Sleep(queryperiodintervalsecond * 1000);
            }
        }

    }
}