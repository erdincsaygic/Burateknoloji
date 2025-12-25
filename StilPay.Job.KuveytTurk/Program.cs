using StilPay.Utility.Helper;
using StilPay.Utility.KuveytTurk;
using StilPay.Utility.KuveytTurk.KuveytTurkAccountTransaction;
using StilPay.Utility.KuveytTurk.KuveytTurkToken;
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

namespace StilPay.Job.KuveytTurk
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
                                  "            STILPAY KUVEYTTURK-BANK WORKING..            ",
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
                            foreach (var hareketDetay in transactionModel.Data.value.accountActivities.OrderByDescending(o => Convert.ToDateTime(o.date)).Where(a=> a.amount == 400d))
                            {
                                if (!tSQLBankManager.HasPaymentTransferPool(hareketDetay.businessKey) && !tSQLBankManager.HasNotificationTransaction(hareketDetay.businessKey))
                                {
                                    string[] splitSenderName = hareketDetay.description.Split(new string[] { "Gönderen:", "Gönderen=" }, StringSplitOptions.None);

                                    var senderName = "";
                                    if (splitSenderName.Length > 1)
                                        senderName = splitSenderName.Length > 1 ? splitSenderName[1].Split(',')[0].Trim() : " ";

                                    var isCaughtInFraudControl = false;
                                    var fraudDescription = "Fraud kontrolleri başarıyla tamamlandı.";
                                    var isTrusted = false;
                                    var (Result, ReferenceNr, ServiceId, CallbackUrl, AutoTransferLimit) = tSQLBankManager.CheckReferenceNr(hareketDetay.description);

                                    var trCulture = new CultureInfo("tr-TR");
                                    string normalizedDescription = hareketDetay.description != null
                                                                   ? hareketDetay.description.Replace(" ", "").ToLower(trCulture)
                                                                   : "";

                                    var paymentTransferPoolDescriptionControlList = tSQLBankManager.GetPaymentTransferPoolDescriptionControls();
                                    var isHaveBlockedWord = paymentTransferPoolDescriptionControlList.Any(x =>
                                        !string.IsNullOrEmpty(x.Name) &&
                                        normalizedDescription.Contains(x.Name.Replace(" ", "").ToLower(trCulture))
                                    );

                                    if (!string.IsNullOrEmpty(Result) && !string.IsNullOrEmpty(ReferenceNr) && Result != "" && ReferenceNr != "" && ServiceId != "")
                                    {
                                        var (IsTrusted, FraudResult, FraudDescription) = tSQLBankManager.TransferCheckFraudControl(null, ReferenceNr, senderName, ServiceId, Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture));
                                        isCaughtInFraudControl = !FraudResult;
                                        fraudDescription = IsTrusted ? "Aynı kişi 24 saat veya öncesinde ödeme yaptı, kişi güvenilir olduğu için fraud kontrolü yapılmadı" : FraudDescription;
                                        isTrusted = IsTrusted;
                                    }

                                    if (!string.IsNullOrEmpty(Result) && !string.IsNullOrWhiteSpace(Result) && !string.IsNullOrEmpty(ReferenceNr) && !string.IsNullOrWhiteSpace(ReferenceNr) && Result == "OK" && ServiceId != "" && CallbackUrl != "" && !tSQLBankManager.HasNotificationTransaction(hareketDetay.businessKey) && !tSQLBankManager.HasPaymentTransferPool(hareketDetay.businessKey) && ( isTrusted || Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture) <= AutoTransferLimit) && !isHaveBlockedWord && !isCaughtInFraudControl)
                                    {
                                        string transactionId = DateTime.Now.Ticks.ToString("D16");

                                        var transactionNr = tSQLBankManager.AddNotificationTransaction(DateTime.Now, Convert.ToDateTime(hareketDetay.date), Convert.ToDateTime(hareketDetay.date), bankId, ServiceId, transactionId, hareketDetay.businessKey, Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture), hareketDetay.description, "00000000-0000-0000-0000-000000000000", senderName, "11111111111", false, true);

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
                                                    transfer_date = Convert.ToDateTime(hareketDetay.date),
                                                    amount = Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture)
                                                },
                                                user_entered_data = new
                                                {
                                                    sender_name = senderName,
                                                    bank_description = hareketDetay.description
                                                }
                                            };

                                            var pyID = tSQLBankManager.AddAutoPaymentNotification(Convert.ToDateTime(hareketDetay.date), bankId, senderName, ServiceId, transactionId, hareketDetay.businessKey, Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture), "Otomatik Bakiye Yükleme İşlemi Bildirimi", "", companyBankAccountID, isCaughtInFraudControl, fraudDescription);

                                            var pyTransactionNr = tSQLBankManager.GetPaymentNotificationTransactionNr(pyID);

                                            var IDOutAuto = tSQLBankManager.AddPaymentTransferPoolWithReference(Convert.ToDateTime(hareketDetay.date), bankId, senderName, "", Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture), hareketDetay.businessKey, hareketDetay.description, true, companyBankAccountID, pyTransactionNr, transactionId, isCaughtInFraudControl, fraudDescription);


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
                                        if (!tSQLBankManager.HasPaymentTransferPool(hareketDetay.businessKey) && !tSQLBankManager.HasNotificationTransaction(hareketDetay.businessKey))
                                        {
                                            var IDOut = tSQLBankManager.AddPaymentTransferPool(Convert.ToDateTime(hareketDetay.date), bankId, senderName, hareketDetay.iban ?? "", Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture), hareketDetay.businessKey, hareketDetay.description, companyBankAccountID, isHaveBlockedWord ? (byte)Enums.StatusType.Risk : 1, isCaughtInFraudControl, fraudDescription);

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
