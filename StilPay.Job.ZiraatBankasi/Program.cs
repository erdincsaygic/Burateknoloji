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
using ZiraatBankReferance;
using static StilPay.Utility.Helper.Enums;

namespace StilPay.Job.ZiraatBankasi
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
            var service = new HesapHareketleriSoapClient(HesapHareketleriSoapClient.EndpointConfiguration.HesapHareketleriSoap);

            var ziraatBankId = startup.ZiraatApi.bank_id;
            var queryperiodintervalsecond = startup.ZiraatApi.query_period_interval_second;
            var transactionRangeHour = startup.ZiraatApi.transaction_range_hour;
            var notificationRangeMinute = startup.ZiraatApi.notification_range_minute;

            var ziraatEndDate = DateTime.Now;
            var ziraatStartDate = ziraatEndDate.AddHours(transactionRangeHour * -1);

            Console.WriteLine(
                    string.Concat("-------------------------------------------------",
                                   Environment.NewLine, Environment.NewLine,
                                  "            STILPAY ZİRAAT-BANK WORKING..            ",
                                   Environment.NewLine, Environment.NewLine,
                                  "-------------------------------------------------"));

            while (true)
            {
                #region Ziraat Bankası Api
                try
                {
                    var transactionModel = service.SorgulaHesapHareketZamanIleHizliAsync(startup.ZiraatAccount.CustomerNo, startup.ZiraatAccount.BranchNo, ziraatStartDate, ziraatEndDate, startup.ZiraatAccount.BranchCode, startup.ZiraatAccount.Password).Result;

                    accountTransactionCount = transactionModel.hareketdetay.Count();

                    if (transactionModel != null && transactionModel.hareketdetay != null)
                    {
                        foreach (var hareketDetay in transactionModel.hareketdetay.OrderByDescending(o => Convert.ToDateTime(o.islemTarihi)))
                        {
                            if (!tSQLBankManager.HasPaymentTransferPool(hareketDetay.dekontNo))
                            {
                                //var IDOut = tSQLBankManager.AddPaymentTransferPool(Convert.ToDateTime(hareketDetay.islemTarihi), ziraatBankId, hareketDetay.adUnvan, hareketDetay.iban??"", Convert.ToDecimal(hareketDetay.tutar, CultureInfo.InvariantCulture), hareketDetay.dekontNo, hareketDetay.aciklama);

                                //tableInsertionErrorCount = string.IsNullOrEmpty(IDOut) ? tableInsertionErrorCount + 1 : tableInsertionErrorCount;
                                //tableInsertionSuccessCount = string.IsNullOrEmpty(IDOut) ? tableInsertionSuccessCount : tableInsertionSuccessCount + 1;
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

                ziraatEndDate = DateTime.Now;
                ziraatStartDate = ziraatEndDate.AddHours(transactionRangeHour * -1);
                Console.WriteLine(
                string.Concat(Environment.NewLine, Environment.NewLine,
                              $"Bankaya Atılan Sorgu Başlangıç Tarihi: {ziraatStartDate}\n",
                              $"Bankaya Atılan Sorgu Bitiş Tarihi: {ziraatEndDate}\n",
                              $"Hesap Hareketleri Sayısı : {accountTransactionCount}\n",
                              $"Tabloya Kayıt Edilen Başarılı İşlem Sayısı : {tableInsertionSuccessCount}\n",
                              $"Tabloya Kayıt Edilen Hatalı İşlem Sayısı Sayısı: {tableInsertionErrorCount}\n",
                              $"Zaman Aşımına Uğrayan Bildirim Sayısı: {timeoutNotificationsCount}\n",
                              "-------------------------------------------------"));
                Thread.Sleep(queryperiodintervalsecond * 1000);
            }
        }

    }
}