using StilPay.Utility.Helper;
using StilPay.Utility.KuveytTurk;
using StilPay.Utility.KuveytTurk.KuveytTurkAccountTransaction;
using StilPay.Utility.KuveytTurk.KuveytTurkToken;
using StilPay.Utility.Models;
using StilPay.Utility.Worker;
using System.Globalization;
using System.Text;
using System.Text.Json;
using static StilPay.Utility.Helper.Enums;

namespace StilPay.Job.TangoKuveytturk
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var startUp = new Startup();

            Dictionary<string, string> header = new Dictionary<string, string>();
            Dictionary<string, object> body = new Dictionary<string, object>();


            var bankId = startUp.KuveytTurkApi.bank_id;
            var transaction_url = startUp.KuveytTurkApi.transaction_url;
            var startDate = startUp.KuveytTurkApi.startDate; 
            var endDate = startUp.KuveytTurkApi.endDate; 
            var queryperiodintervalsecond = startUp.KuveytTurkApi.query_period_interval_second;
            var notificationRangeMinute = startUp.KuveytTurkApi.notification_range_minute; 
            var serviceID = startUp.KuveytTurkApi.serviceID;
            var auth_transaction_url = startUp.KuveytTurkToken.transaction_url;
            var auth_grant_type = startUp.KuveytTurkToken.grant_type;
            var auth_scope = startUp.KuveytTurkToken.scope;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            int accountTransactionCount = 0;
            int tableInsertionErrorCount = 0;
            int tableInsertionSuccessCount = 0;
            int timeoutNotificationsCount = 0;


            var reqEndDate = DateTime.Now;
            var today = DateTime.Today.ToString("yyyy-MM-dd");
            string previousDay = null;
            var companyBankAccountID = startUp.KuveytTurkApi.companyBankAccountID;
            Console.WriteLine(
                    string.Concat("-------------------------------------------------",
                                   Environment.NewLine, Environment.NewLine,
                                  "            STILPAY TANGO-KUVEYTTURK-BANK WORKING..            ",
                                   Environment.NewLine, Environment.NewLine,
                                  "-------------------------------------------------")
            );

            while (true)
            {
                #region KuveytTurk Api
                try
                {
                    int responseStatus = 0;

                    DateTime dateNow = DateTime.Now;
                    TimeSpan timeLimit = new TimeSpan(10, 0, 0);

                    if (dateNow.TimeOfDay < timeLimit)
                        previousDay = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
                    else
                        previousDay = null;

                    var systemSettingValues = tSQLBankManager.GetSystemSettingValues("KuveytTurkClient");

                    var tokenModel = new KuveytTurkTokenRequestModel()
                    {
                        client_id = systemSettingValues.FirstOrDefault(f => f.ParamDef == "kuveytturk_client_id").ParamVal,
                        client_secret = systemSettingValues.FirstOrDefault(f => f.ParamDef == "kuveytturk_client_secret").ParamVal
                    };

                    var token = KuveytTurkGetToken.GetAccessToken(tokenModel);

                    if(token != null && token.Status == "OK" && token.Data != null)
                    {
                        var url = !string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate) ? String.Concat(transaction_url, $"?beginDate={startDate}&endDate={endDate}")
                            : String.Concat(transaction_url, $"?beginDate={previousDay??today}&endDate={today}");

                        var rsa = KuveytTurkRSAKeyGenerator.RSAKeyGenerator(systemSettingValues.FirstOrDefault(f => f.ParamDef == "kuveytturk_rsa_private_key").ParamVal, token.Data.access_token, "GET", null, url);
                        var accClass = new KuveytTurkAccountTransactionRequestModel()
                        {
                            Authorization = token.Data.access_token,
                            Signature = rsa,
                            url = url
                        };

                        var transactionModel = KuveytTurkGetAccountTransaction.GetAccountTransaction(accClass);

                        if (transactionModel != null && transactionModel.Data != null && transactionModel.Status == "OK")
                        {
                            foreach (var hareketDetay in transactionModel.Data.value.accountActivities.OrderByDescending(o => Convert.ToDateTime(o.date)))
                            {
                                if (!tSQLBankManager.HasPaymentTransferPool(hareketDetay.businessKey) && !tSQLBankManager.HasNotificationTransaction(hareketDetay.businessKey) && hareketDetay.amount > 0)
                                {
                                    string[] splitSenderName = hareketDetay.description.Split(new string[] { "Gönderen:", "Gönderen=" }, StringSplitOptions.None);

                                    var senderName = "";
                                    if (splitSenderName.Length > 1)
                                        senderName = splitSenderName.Length > 1 ? splitSenderName[1].Split(',')[0].Trim() : " ";
                          
                                    string transactionId = DateTime.Now.Ticks.ToString("D16");

                                    var transactionNr = tSQLBankManager.AddNotificationTransaction(DateTime.Now, Convert.ToDateTime(hareketDetay.date), Convert.ToDateTime(hareketDetay.date), bankId, serviceID, transactionId, hareketDetay.businessKey, Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture), hareketDetay.description, "00000000-0000-0000-0000-000000000000", senderName, "11111111111", false, true);

                                    if (!string.IsNullOrEmpty(transactionNr))
                                    {
                                        var companyIntegration = tSQLBankManager.GetCompanyIntegration(serviceID);

                                        var pyID = tSQLBankManager.AddAutoPaymentNotification(Convert.ToDateTime(hareketDetay.date), bankId, senderName, serviceID, transactionId, hareketDetay.businessKey, Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture), "Otomatik Bakiye Yükleme İşlemi Bildirimi", "", companyBankAccountID, false);

                                        var pyTransactionNr = tSQLBankManager.GetPaymentNotificationTransactionNr(pyID);

                                        var IDOutAuto = tSQLBankManager.AddPaymentTransferPoolWithReference(Convert.ToDateTime(hareketDetay.date), bankId, senderName, "", Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture), hareketDetay.businessKey, hareketDetay.description, true, companyBankAccountID, pyTransactionNr, transactionId, false);


                                        if (pyID != null && IDOutAuto != null)
                                        {
                                            tSQLBankManager.SetPaymentTransactionStatus(pyID, (int)StatusType.Confirmed, "Otomatik Bakiye Yükleme İşlemi Bildirimi");

                                            var phones = new List<string>
                                            {
                                                "05468130828",
                                                "05425575091",
                                                "05465508493",
                                                "05527364171"
                                            };

                                            string formattedDate = hareketDetay.date.ToString("dd.MM.yyyy HH:mm");
                                            string formattedAmount = Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture).ToString("N2", new CultureInfo("tr-TR"));

                                            foreach (var item in phones)
                                            {
                                                tSmsSender sender = new tSmsSender();
                                                string msg = $"TR900020500009814244300005 {formattedDate} {senderName} {formattedAmount}";
                                                sender.SendSms(item, msg);
                                            }


                                            tableInsertionErrorCount = string.IsNullOrEmpty(IDOutAuto) ? tableInsertionErrorCount + 1 : tableInsertionErrorCount;
                                            tableInsertionSuccessCount = string.IsNullOrEmpty(IDOutAuto) ? tableInsertionSuccessCount : tableInsertionSuccessCount + 1;
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



                //#endregion

            }
        }
    }
}
