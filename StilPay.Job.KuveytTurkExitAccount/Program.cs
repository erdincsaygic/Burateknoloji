using DocumentFormat.OpenXml.Bibliography;
using Org.BouncyCastle.Asn1.Ocsp;
using RestSharp;
using StilPay.Job.KuveytTurk.Models;
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


            var reqEndDate = DateTime.Now;
            var today = DateTime.Today.ToString("yyyy-MM-dd");
            string previousDay = null;
            var companyBankAccountID = startUp.KuveytTurkApi.companyBankAccountID;
            Console.WriteLine(
                    string.Concat("-------------------------------------------------",
                                   Environment.NewLine, Environment.NewLine,
                                  "            STILPAY KUVEYTTURK-EXIT-BANK WORKING..            ",
                                   Environment.NewLine, Environment.NewLine,
                                  "-------------------------------------------------")
            );

            while (true)
            {
                #region KuveytTurk Api
                try
                {

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
                            foreach (var hareketDetay in transactionModel.Data.value.accountActivities.OrderByDescending(o => o.date))
                            {
                                if (!tSQLBankManager.HasWithdrawalPool(hareketDetay.businessKey))
                                {
                                    string[] splitReceiverName = hareketDetay.description.Split(new string[] { "Alıcı:" }, StringSplitOptions.None);

                                    var senderName = "";
                                    if (splitReceiverName.Length > 1)
                                        senderName = splitReceiverName[1].Split(',')[0].Trim();


                                    string[] splitRequestNr = hareketDetay.description.Split(new string[] { "-" }, StringSplitOptions.None);

                                    var requestNr = "";
                                    if (splitRequestNr.Length > 1)
                                        requestNr = splitRequestNr[0].Trim();


                                    var IDOut = tSQLBankManager.AddWithdrawalPool(DateTime.Now, hareketDetay.date, bankId, senderName, Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture), hareketDetay.description, hareketDetay.iban ?? "", hareketDetay.businessKey, companyBankAccountID, requestNr);

                                    tableInsertionErrorCount = string.IsNullOrEmpty(IDOut) ? tableInsertionErrorCount + 1 : tableInsertionErrorCount;
                                    tableInsertionSuccessCount = string.IsNullOrEmpty(IDOut) ? tableInsertionSuccessCount : tableInsertionSuccessCount + 1;                                

                                }
                            }

                            var list = tSQLBankManager.GetInProcessWithdrawalAndRebateRequests();

                            foreach (var item in list)
                            {
                                var getWithdrawalPool = tSQLBankManager.GetWithdrawalPool();

                                if (getWithdrawalPool != null)
                                {
                                    //var withdrawalPools = getWithdrawalPool.OrderByDescending(o => Convert.ToDateTime(o.TransactionDate)).Where(w =>
                                    //    (!string.IsNullOrEmpty(w.ReceiverName) && w.ReceiverName.Contains(item.Title))
                                    //    || (!string.IsNullOrEmpty(w.ReceiverName) && w.ReceiverName.Equals(item.Title))
                                    //    || (!string.IsNullOrEmpty(w.Description) && w.Description.Contains(item.Title))
                                    //    || (!string.IsNullOrEmpty(w.Description) && w.Description.Equals(item.Title))
                                    //    || (!string.IsNullOrEmpty(w.Description) && w.Description.Contains(item.RequestNr))
                                    //    && w.Amount == (item.Amount * - 1 )
                                    //    && w.TransactionDate >= item.MDate.AddMinutes(-2) && w.TransactionDate <= item.MDate.AddMinutes(2)).ToList();

                                    var withdrawalPools = getWithdrawalPool.OrderByDescending(o => Convert.ToDateTime(o.TransactionDate)).Where(w =>
                                        !string.IsNullOrEmpty(w.Description)
                                        && w.Description.Contains(item.RequestNr)
                                        && w.Amount == (item.Amount * -1)
                                        && (w.ReceiverName.Replace(" ", "").ToLower() == item.Title.Replace(" ", "").ToLower() || w.Description.Contains(item.Title.Replace(" ", "").ToLower()))
                                        && w.TransactionDate >= item.MDate.AddMinutes(-2) && w.TransactionDate <= item.MDate.AddMinutes(2)).ToList();

                                    foreach (var withdrawalPool in withdrawalPools)
                                    {
                                        item.Description = "İşlem Başarılı";
                                        if (item.IsRebate)
                                        {
                                            var IDOutRebate = tSQLBankManager.RebateRequestSetStatus(item.ID, item.Description, item.SIDBank, item.CompanyBankAccountID, (byte)Enums.StatusType.Confirmed);

                                            if (IDOutRebate != null)
                                                tSQLBankManager.WithdrawalPoolSetStatus(withdrawalPool.ID, item.RequestNr, item.IDCompany, true, (byte)Enums.StatusType.Confirmed);
                                        }
                                        else
                                        {
                                            var hasReverse = false;
                                            for (int i = 0; i < 5; i++)
                                            {
                                                hasReverse = tSQLBankManager.HasWithdrawalPoolReverse(item.RequestNr);

                                                if (hasReverse)
                                                {
                                                    var reverseID = tSQLBankManager.GetWithdrawalPoolReverseID(item.RequestNr);

                                                    if (reverseID != null)
                                                    {
                                                        item.Description = "İşlem başarısız, alıcı ad soyad veya iban hatalı";
                                                        var IDOutWithdrawal = tSQLBankManager.WithdrawalRequestSetStatus(item.ID, item.Description, item.SIDBank, item.CompanyBankAccountID, (byte)Enums.StatusType.Canceled);

                                                        if (IDOutWithdrawal != null)
                                                        {
                                                            tSQLBankManager.WithdrawalPoolSetStatus(withdrawalPool.ID, item.RequestNr, item.IDCompany, false, (byte)Enums.StatusType.Canceled, item.Description);
                                                            tSQLBankManager.WithdrawalPoolSetStatus(reverseID, item.RequestNr, item.IDCompany, false, (byte)Enums.StatusType.Canceled, item.Description);

                                                            var companyIntegration = tSQLBankManager.GetCompanyIntegrationByID(item.IDCompany);

                                                            var dataCallback = new
                                                            {
                                                                status_code = "ERROR",
                                                                service_id = companyIntegration.ServiceID,
                                                                status_type = 2,
                                                                ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                                                data = new { transaction_id = item.RequestNr, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = item.Description }
                                                            };

                                                            var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                                                            var opt = new JsonSerializerOptions() { WriteIndented = true };
                                                            tSQLBankManager.AddCallbackResponseLog(item.RequestNr, "STILPAY", System.Text.Json.JsonSerializer.Serialize(dataCallback, opt), companyIntegration.ID, "STILPAY PARA GÖNDERİMİ CALLBACK", 1);
                                                        }
                                                    }

                                                    break;
                                                }
                                            }

                                            if (!hasReverse)
                                            {
                                                var IDOutWithdrawal = tSQLBankManager.WithdrawalRequestSetStatus(item.ID, item.Description, item.SIDBank, item.CompanyBankAccountID, (byte)Enums.StatusType.Confirmed);

                                                if (IDOutWithdrawal != null)
                                                {
                                                    tSQLBankManager.WithdrawalPoolSetStatus(withdrawalPool.ID, item.RequestNr, item.IDCompany, false, (byte)Enums.StatusType.Confirmed);

                                                    var companyIntegration = tSQLBankManager.GetCompanyIntegrationByID(item.IDCompany);

                                                    var dataCallback = new
                                                    {
                                                        status_code = "OK",
                                                        service_id = companyIntegration.ServiceID,
                                                        status_type = 2,
                                                        ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                                        data = new { transaction_id = item.RequestNr, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = "Çekim Talebi İşlemi Başarıyla Tamamlandı" }
                                                    };

                                                    var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                                                    var opt = new JsonSerializerOptions() { WriteIndented = true };
                                                    tSQLBankManager.AddCallbackResponseLog(item.RequestNr, "STILPAY", System.Text.Json.JsonSerializer.Serialize(dataCallback, opt), companyIntegration.ID, "STILPAY PARA GÖNDERİMİ CALLBACK", 1);
                                                }
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
                              "-------------------------------------------------"));

                Thread.Sleep(queryperiodintervalsecond * 1000);

                //#endregion

            }
        }
    }
}
