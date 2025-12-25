using Coravel.Invocable;
using StilPay.BLL.Abstract;
using StilPay.DAL.Concrete;
using StilPay.Utility.Helper;
using System.Collections.Generic;
using ZiraatBankPaymentService;
using System.Threading.Tasks;
using System.Linq;
using StilPay.Utility.Worker;
using DocumentFormat.OpenXml.Wordprocessing;
using RestSharp;
using static StilPay.Utility.Helper.Enums;
using StilPay.Entities.Concrete;
using StilPay.Utility.KuveytTurk.KuveytTurkAccountTransaction;
using StilPay.Utility.KuveytTurk;
using StilPay.Utility.Models;
using System.Globalization;
using System.Text.Json;
using System;
using StilPay.Utility.KuveytTurk.KuveytTurkToken;
using Microsoft.Extensions.Configuration;

namespace StilPay.BLL.Jobs
{
    public class WithdrawalPoolJob : IInvocable
    {
        public readonly ICompanyWithdrawalRequestManager _companyWithdrawalRequestManager;
        public readonly ICompanyManager _companyManager;
        public readonly ICompanyIntegrationManager _companyIntegrationManager;
        public readonly ICallbackResponseLogManager _callbackResponseLogManager;
        public readonly ISystemSettingManager _systemSettingManager;
        private readonly ICompanyBankAccountManager _companyBankAccountManager;
        private readonly IWithdrawalPoolManager _withdrawalPoolManager;
        private readonly SettingDAL _settingDAL = new SettingDAL();

        IConfiguration _configuration;

        NkyParaTransferiWSSoapClient ziraatService = new NkyParaTransferiWSSoapClient(NkyParaTransferiWSSoapClient.EndpointConfiguration.NkyParaTransferiWSSoap);
        SecuredWebServiceHeader securedWebServiceHeader = new SecuredWebServiceHeader();
        public WithdrawalPoolJob(ICompanyWithdrawalRequestManager companyWithdrawalRequestManager, ICompanyManager companyManager, ISystemSettingManager systemSettingManager, ICompanyIntegrationManager companyIntegrationManager, ICallbackResponseLogManager callbackResponseLogManager, ICompanyBankAccountManager companyBankAccountManager, IWithdrawalPoolManager withdrawalPoolManager, IConfiguration configuration)
        {
            _companyWithdrawalRequestManager = companyWithdrawalRequestManager;
            _companyManager = companyManager;
            _companyIntegrationManager = companyIntegrationManager;
            _callbackResponseLogManager = callbackResponseLogManager;
            _systemSettingManager = systemSettingManager;
            _companyBankAccountManager = companyBankAccountManager;
            _withdrawalPoolManager = withdrawalPoolManager;
            _configuration = configuration;
        }

        public async Task Invoke()
        {
            //var companyBankAccountID = _configuration.GetValue<string>("WithdrawalPoolBankSettings:KuveytTurkApi:companyBankAccountID");

            //var withdrawalRequest = _companyWithdrawalRequestManager.GetList(new List<FieldParameter>()
            //{
            //    new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null),
            //    new FieldParameter("Status", Enums.FieldType.Tinyint, (int)Enums.StatusType.Process),
            //    new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
            //    new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
            //    new FieldParameter("PageLenght", Enums.FieldType.Int, 9999),
            //    new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
            //    new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null),
            //    new FieldParameter("IsProcess", Enums.FieldType.Bit, true)
            //}).Where(x => x.CompanyBankAccountID == companyBankAccountID).ToList();

            //if (withdrawalRequest.Count > 0)
            //{
            //    foreach (var item in withdrawalRequest)
            //    {
            //        //Dictionary<string, string> header = new Dictionary<string, string>();
            //        //Dictionary<string, object> body = new Dictionary<string, object>();

            //        //var activeBank = _companyBankAccountManager.GetActiveList(new List<FieldParameter>() { new FieldParameter("IDCompany", Enums.FieldType.NVarChar, "1312E00F-E83E-45B4-85C6-892396D12331") }).Where(x => x.IsActiveByDefaultExpenseAccount).FirstOrDefault();

            //        var transaction_url = _configuration.GetValue<string>("WithdrawalPoolBankSettings:KuveytTurkApi:transaction_url");
            //        var bankId = _configuration.GetValue<string>("WithdrawalPoolBankSettings:KuveytTurkApi:bank_id");

            //        //string previousDay = null;
            //        //DateTime dateNow = DateTime.Now;
            //        //TimeSpan timeLimit = new TimeSpan(10, 0, 0);

            //        //if (dateNow.TimeOfDay < timeLimit)
            //        //    previousDay = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
            //        //else
            //        //    previousDay = null;

            //        var systemSettingValues = tSQLBankManager.GetSystemSettingValues("KuveytTurkClient");

            //        var tokenModel = new KuveytTurkTokenRequestModel()
            //        {
            //            client_id = systemSettingValues.FirstOrDefault(f => f.ParamDef == "kuveytturk_client_id").ParamVal,
            //            client_secret = systemSettingValues.FirstOrDefault(f => f.ParamDef == "kuveytturk_client_secret").ParamVal
            //        };
            //        var token = KuveytTurkGetToken.GetAccessToken(tokenModel);

            //        if (token != null && token.Status == "OK" && token.Data != null)
            //        {
            //            var url = String.Concat(transaction_url, $"?beginDate={item.MDate.Value:yyyy-MM-dd}&endDate={item.MDate.Value:yyyy-MM-dd}");

            //            var rsa = KuveytTurkRSAKeyGenerator.RSAKeyGenerator(systemSettingValues.FirstOrDefault(f => f.ParamDef == "kuveytturk_rsa_private_key").ParamVal, token.Data.access_token, "GET", null, url);
            //            var accClass = new KuveytTurkAccountTransactionRequestModel()
            //            {
            //                Authorization = token.Data.access_token,
            //                Signature = rsa,
            //                url = url
            //            };

            //            var transactionModel = KuveytTurkGetAccountTransaction.GetAccountTransaction(accClass);

            //            if (transactionModel != null && transactionModel.Data != null && transactionModel.Status == "OK")
            //            {
            //                foreach (var hareketDetay in transactionModel.Data.value.accountActivities.Where(x => x.date <= item.MDate.Value.AddMinutes(-2) && x.date >= item.MDate.Value.AddMinutes(2)).OrderByDescending(o => Convert.ToDateTime(o.date)))
            //                {
            //                    string[] splitTransactionKey = hareketDetay.description.Split(new string[] { "-" }, StringSplitOptions.None);

            //                    var transactionKey = "";
            //                    if (splitTransactionKey.Length > 1)
            //                        transactionKey = splitTransactionKey[0].Trim();

            //                    if(!string.IsNullOrEmpty(transactionKey))
            //                    {
            //                        if (!tSQLBankManager.HasWithdrawalPool(transactionKey))
            //                        {
            //                            var entity = new WithdrawalPool()
            //                            {
            //                                Amount = Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture),
            //                                CDate = DateTime.Now,
            //                                CompanyBankAccountID = companyBankAccountID,
            //                                Description = hareketDetay.description,
            //                                IDBank = bankId,
            //                                ReceiverIban = item.IBAN,
            //                                ReceiverName = item.Title,
            //                                TransactionDate = Convert.ToDateTime(hareketDetay.date),
            //                                TransactionKey = transactionKey,
            //                                Status = (int)Enums.StatusType.Pending
            //                            };
                                        
            //                            var respInsert = _withdrawalPoolManager.Insert(entity);

            //                            if (respInsert == null || respInsert.Status != "OK" || !string.IsNullOrEmpty(respInsert.Data.ToString()) ) break;
            //                        }

            //                        var withdrawalPoolEntity = _withdrawalPoolManager.GetSingle(new List<FieldParameter>() { new FieldParameter("TransactionKey", Enums.FieldType.NVarChar, transactionKey) });

            //                        if (withdrawalPoolEntity != null && withdrawalPoolEntity.Status == (int)Enums.StatusType.Pending && item.Status == (int)Enums.StatusType.Process
            //                            && Math.Abs(withdrawalPoolEntity.Amount) == item.Amount && withdrawalPoolEntity.TransactionKey == item.RequestNr && withdrawalPoolEntity.Description.Contains(item.Title))
            //                        {
            //                            item.Status = (byte)Enums.StatusType.Confirmed;
            //                            item.Description = "Çekim Limiti Aşılmadığı İçin Otomatik Onaylandı";
            //                            item.MDate = DateTime.Now;
            //                            item.MUser = "00000000-0000-0000-0000-000000000000";
            //                            var response = _companyWithdrawalRequestManager.SetStatus(item);

            //                            if (response.Status == "OK")
            //                            {
            //                                withdrawalPoolEntity.Status = (int)Enums.StatusType.Confirmed;
            //                                withdrawalPoolEntity.MDate = DateTime.Now;
            //                                withdrawalPoolEntity.RequestNr = transactionKey;
            //                                withdrawalPoolEntity.ResponseTransactionNr = item.TransactionNr;
            //                                withdrawalPoolEntity.IDCompany = item.IDCompany;
            //                                _withdrawalPoolManager.SetStatus(withdrawalPoolEntity);

            //                                var companyIntegration = _companyIntegrationManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, item.IDCompany) });

            //                                var dataCallback = new
            //                                {
            //                                    status_code = "OK",
            //                                    service_id = companyIntegration.ServiceID,
            //                                    status_type = 2,
            //                                    ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
            //                                    data = new { transaction_id = item.RequestNr, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = "Çekim Talebi İşlemi Başarıyla Tamamlandı" }
            //                                };

            //                                var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

            //                                var callbackEntity = new CallbackResponseLog();
            //                                var opt = new JsonSerializerOptions() { WriteIndented = true };

            //                                callbackEntity.TransactionID = item.RequestNr;
            //                                callbackEntity.ServiceType = "STILPAY";
            //                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
            //                                callbackEntity.IDCompany = item.IDCompany;
            //                                callbackEntity.TransactionType = "STILPAY PARA GÖNDERİMİ CALLBACK";
            //                                callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
            //                                _callbackResponseLogManager.Insert(callbackEntity);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }             
            //    }
            //}
        }
    }
}
