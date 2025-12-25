using Coravel.Invocable;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using StilPay.BLL.Abstract;
using StilPay.BLL.Concrete;
using StilPay.DAL.Concrete;
using StilPay.Entities;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.IsBankTransferService;
using StilPay.Utility.KuveytTurk.KuveytTurkTransfer;
using StilPay.Utility.KuveytTurk;
using StilPay.Utility.Models;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ZiraatBankPaymentService;
using static StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPayment.IsBankPaymentRequestModel;
using static StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPaymentValidation.IsBankPaymentValidationRequestModel;
using static System.Net.WebRequestMethods;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.EMMA;

namespace StilPay.BLL.Jobs
{

    public class AutoWithdrawal : IInvocable
    {
        public readonly ICompanyWithdrawalRequestManager _companyWithdrawalRequestManager;
        public readonly ICompanyManager _companyManager;
        public readonly ICompanyIntegrationManager _companyIntegrationManager;
        public readonly ICallbackResponseLogManager _callbackResponseLogManager;
        public readonly ISystemSettingManager _systemSettingManager;
        private readonly ICompanyBankAccountManager _companyBankAccountManager;
        private readonly SettingDAL _settingDAL = new SettingDAL();

        NkyParaTransferiWSSoapClient ziraatService = new NkyParaTransferiWSSoapClient(NkyParaTransferiWSSoapClient.EndpointConfiguration.NkyParaTransferiWSSoap);
        SecuredWebServiceHeader securedWebServiceHeader = new SecuredWebServiceHeader();
        public AutoWithdrawal(ICompanyWithdrawalRequestManager companyWithdrawalRequestManager, ICompanyManager companyManager, ISystemSettingManager systemSettingManager, ICompanyIntegrationManager companyIntegrationManager, ICallbackResponseLogManager callbackResponseLogManager, ICompanyBankAccountManager companyBankAccountManager)
        {
            _companyWithdrawalRequestManager = companyWithdrawalRequestManager;
            _companyManager = companyManager;
            _companyIntegrationManager = companyIntegrationManager;
            _callbackResponseLogManager = callbackResponseLogManager;
            _systemSettingManager = systemSettingManager;
            _companyBankAccountManager = companyBankAccountManager;
        }

        public async Task Invoke()
        {
            var withdrawalRequest = _companyWithdrawalRequestManager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null),
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.Pending),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("PageLenght", Enums.FieldType.Int, 9999),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null),
                new FieldParameter("IsProcess", Enums.FieldType.Bit, false),
                new FieldParameter("WithProcess", Enums.FieldType.Bit, false)
            });

            if(withdrawalRequest.Count  > 0)
            {
                foreach(var wt in withdrawalRequest)
                {
                    wt.IsProcess = true;
                    _companyWithdrawalRequestManager.SetStatus(wt);
                }

                var activeBank = _companyBankAccountManager.GetActiveList(new List<FieldParameter>() { new FieldParameter("IDCompany", Enums.FieldType.NVarChar, "1312E00F-E83E-45B4-85C6-892396D12331") }).Where(x => x.IsActiveByDefaultExpenseAccount).FirstOrDefault();

                var useZiraatBank = false;
                var useIsBank = false;
                var useKuveytTurk = false;
                var sIdBank = "";

                var companyBankAccountID = "";

                if (activeBank != null)
                {
                    switch (activeBank.IDBank)
                    {
                        case "08":
                            useZiraatBank = true;
                            companyBankAccountID = activeBank.ID;
                            sIdBank = "08";
                            break;

                        case "03":
                            useIsBank = true;
                            companyBankAccountID = activeBank.ID;
                            sIdBank = "03";
                            break;

                        case "07":
                            useKuveytTurk = true;
                            companyBankAccountID = activeBank.ID;
                            sIdBank = "07";
                            break;

                        case "36":
                            useKuveytTurk = true;
                            companyBankAccountID = activeBank.ID;
                            sIdBank = "36";
                            break;

                    }
                }

                foreach (var item in withdrawalRequest)
                {
                    var company = _companyManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, item.IDCompany) });

                    if (company.AutoWithdrawalLimit >= item.Amount)
                    {
                        var companyIntegration = _companyIntegrationManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, item.IDCompany) });

                        var callbackEntity = new CallbackResponseLog();
                        var opt = new JsonSerializerOptions() { WriteIndented = true };

                        if (useZiraatBank)
                        {
                            if (item.IDBank == "08")
                            {
                                var havaleResponse = ziraatService.HavaleYapAsync(securedWebServiceHeader, "97736040", "5002", "", "", item.IBAN.Replace(" ", ""), "TRY", item.Amount.ToString(), string.IsNullOrWhiteSpace(item.DealerDescription) ? $"{company.Title} Çekim Talebi" : item.DealerDescription, item.RequestNr, "", "", "", "", "", "", "", "", "", "", "").Result;

                                callbackEntity.TransactionID = item.RequestNr;
                                callbackEntity.ServiceType = "Ziraat Bankası Havale";
                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(havaleResponse.HavaleYapResult, opt);
                                callbackEntity.IDCompany = company.ID;
                                callbackEntity.TransactionType = "Havale";
                                _callbackResponseLogManager.Insert(callbackEntity);

                                if (havaleResponse.HavaleYapResult.CevapKodu != "0")
                                {
                                    if (havaleResponse.HavaleYapResult.CevapKodu == "25")
                                    {
                                        string[] phones = { "05466113687" };
                                        tSmsSender sender = new tSmsSender();
                                        string msg = string.Concat("Stilpay Ziraat Bankası 5002 Hesabında Bakiye Yetersiz Olduğu İçin Çekim Talebi İşlemleri Yapılamıyor");
                                        foreach (var phone in phones)
                                        {
                                            var smsResp = sender.SendSms(phone, msg);
                                        }
                                    }

                                    item.Status = 3;
                                    item.Description = havaleResponse.HavaleYapResult.CevapMesaji;
                                    item.MUser = "00000000-0000-0000-0000-000000000000";
                                    var response = _companyWithdrawalRequestManager.SetStatus(item);
                                    if (response.Status == "OK")
                                    {
                                        var dataCallback = new
                                        {
                                            status_code = "ERROR",
                                            service_id = companyIntegration.ServiceID,
                                            status_type = 2,
                                            ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                            data = new { transaction_id = item.RequestNr, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = havaleResponse.HavaleYapResult.CevapMesaji }
                                        };

                                        var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                                        callbackEntity.TransactionID = item.RequestNr;
                                        callbackEntity.ServiceType = "STILPAY";
                                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                        callbackEntity.IDCompany = item.IDCompany;
                                        callbackEntity.TransactionType = "ÇEKİM TALEBİ";
                                        callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                        _callbackResponseLogManager.Insert(callbackEntity);
                                    }
                                }
                                else
                                {
                                    item.Status = 2;
                                    item.MUser = "00000000-0000-0000-0000-000000000000";
                                    item.Description = "Çekim Limiti Aşılmadığı İçin Otomatik Onaylandı";
                                    item.SIDBank = "08";
                                    item.CompanyBankAccountID = companyBankAccountID;
                                    var response = _companyWithdrawalRequestManager.SetStatus(item);
                                    if (response.Status == "OK")
                                    {
                                        var dataCallback = new
                                        {
                                            status_code = "OK",
                                            service_id = companyIntegration.ServiceID,
                                            status_type = 2,
                                            ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                            data = new { transaction_id = item.RequestNr, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = "Çekim Talebi İşlemi Başarıyla Tamamlandı" }
                                        };

                                       var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                                        callbackEntity.TransactionID = item.RequestNr;
                                        callbackEntity.ServiceType = "STILPAY";
                                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                        callbackEntity.TransactionType = "ÇEKİM TALEBİ";
                                        callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                        _callbackResponseLogManager.Insert(callbackEntity);
                                    }
                                }
                            }

                            else
                            {
                                var eftResponse = ziraatService.EftYapAsync(securedWebServiceHeader, "97736040", "5002", item.Bank, "", "", item.IBAN.Replace(" ", ""), item.Title, DateTime.Now.ToString("yyyyMMdd"), item.Amount.ToString(), item.RequestNr, string.IsNullOrWhiteSpace(item.DealerDescription) ? $"{company.Title} Çekim Talebi" : item.DealerDescription, "", "", "", "", "", "", "", "", "", "").Result;

                                callbackEntity.TransactionID = item.RequestNr;
                                callbackEntity.ServiceType = "Ziraat Bankası EFT";
                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(eftResponse.EftYapResult, opt);
                                callbackEntity.IDCompany = company.ID;
                                callbackEntity.TransactionType = "EFT";
                                _callbackResponseLogManager.Insert(callbackEntity);

                                if (eftResponse.EftYapResult.CevapKodu != "0")
                                {
                                    if (eftResponse.EftYapResult.CevapKodu == "25")
                                    {
                                        string[] phones = { "05466113687" };
                                        tSmsSender sender = new tSmsSender();
                                        string msg = string.Concat("Stilpay Ziraat Bankası 5002 Hesabında Bakiye Yetersiz Olduğu İçin Çekim Talebi İşlemleri Yapılamıyor");
                                        foreach (var phone in phones)
                                        {
                                            var smsResp = sender.SendSms(phone, msg);
                                        }
                                    }

                                    item.Status = 3;
                                    item.Description = eftResponse.EftYapResult.CevapMesaji;
                                    item.MUser = "00000000-0000-0000-0000-000000000000";
                                    _companyWithdrawalRequestManager.SetStatus(item);


                                    var dataCallback = new
                                    {
                                        status_code = "ERROR",
                                        service_id = companyIntegration.ServiceID,
                                        status_type = 2,
                                        ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                        data = new { transaction_id = item.RequestNr, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = eftResponse.EftYapResult.CevapMesaji }
                                    };

                                    var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                                    callbackEntity.TransactionID = item.RequestNr;
                                    callbackEntity.ServiceType = "STILPAY";
                                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                    callbackEntity.IDCompany = item.IDCompany;
                                    callbackEntity.TransactionType = "ÇEKİM TALEBİ";
                                    callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                    _callbackResponseLogManager.Insert(callbackEntity);
                                }
                                else
                                {
                                    item.Status = 2;
                                    item.MUser = "00000000-0000-0000-0000-000000000000";
                                    item.Description = "Çekim Limiti Aşılmadığı İçin Otomatik Onaylandı";
                                    item.SIDBank = "08";
                                    item.CompanyBankAccountID = companyBankAccountID;
                                    _companyWithdrawalRequestManager.SetStatus(item);

                                    var dataCallback = new
                                    {
                                        status_code = "OK",
                                        service_id = companyIntegration.ServiceID,
                                        status_type = 2,
                                        ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                        data = new { transaction_id = item.RequestNr, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = "Çekim Talebi İşlemi Başarıyla Tamamlandı" }
                                    };

                                    var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                                    callbackEntity.TransactionID = item.RequestNr;
                                    callbackEntity.ServiceType = "STILPAY";
                                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                    callbackEntity.IDCompany = item.IDCompany;
                                    callbackEntity.TransactionType = "ÇEKİM TALEBİ";
                                    callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                    _callbackResponseLogManager.Insert(callbackEntity);
                                }
                            }
                        }

                        if (useIsBank)
                        {
                            var isBankIntegrationValues = _settingDAL.GetList(null).Where(x => x.ParamType == "IsBankTransfer").ToList();

                            if (item.IDBank == "03")
                            {
                                var isBankRemittancePaymentValidationRequest = new IsBankPaymentValidationRequest
                                {
                                    apiUrl = "/api/isbank/v1/remittance-validations",
                                    debtor_account_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "debtor_account_id").ParamVal,
                                    isbank_client_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_id").ParamVal,
                                    isbank_client_secret = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_secret").ParamVal,
                                    isbank_client_certificate = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_certificate").ParamVal,
                                    authorization = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_auth").ParamVal,
                                    username = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_username").ParamVal,
                                    password = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_password").ParamVal,
                                    amount = item.Amount,
                                    creditor_iban = item.IBAN.Replace(" ", ""),
                                    description = string.IsNullOrWhiteSpace(item.DealerDescription) ? $"{company.Title} Çekim Talebi" : item.DealerDescription,
                                    payment_reference_id = item.RequestNr,
                                    customer_ip = "89.252.138.236"
                                };

                                var isBankRemittancePaymentValidationResponse = IsBankPaymentValidation.IsBankPaymentValidationRequest(isBankRemittancePaymentValidationRequest);

                                callbackEntity.TransactionID = item.RequestNr;
                                callbackEntity.ServiceType = "İş Bankası Payment Validation";
                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankRemittancePaymentValidationResponse, opt);
                                callbackEntity.IDCompany = company.ID;
                                callbackEntity.TransactionType = "HAVALE PARA GÖNDERİMİ ÖN ONAY SONUC";
                                _callbackResponseLogManager.Insert(callbackEntity);

                                if (isBankRemittancePaymentValidationResponse.Status == "OK")
                                {
                                    var isBankRemittancePaymentRequest = new IsBankPaymentRequest
                                    {
                                        apiUrl = "/api/isbank/v1/remittance-payments",
                                        debtor_account_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "debtor_account_id").ParamVal,
                                        isbank_client_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_id").ParamVal,
                                        isbank_client_secret = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_secret").ParamVal,
                                        isbank_client_certificate = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_certificate").ParamVal,
                                        authorization = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_auth").ParamVal,
                                        username = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_username").ParamVal,
                                        password = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_password").ParamVal,
                                        amount = item.Amount,
                                        creditor_iban = item.IBAN.Replace(" ", ""),
                                        description = string.IsNullOrWhiteSpace(item.DealerDescription) ? $"{company.Title} Çekim Talebi" : item.DealerDescription,
                                        payment_reference_id = item.RequestNr,
                                        query_number = isBankRemittancePaymentValidationResponse.Data.data.query_number,
                                        customer_ip = "89.252.138.236",
                                        idempotency_key = item.ID,
                                    };

                                    var isBankRemittancePaymentRequestResponse = IsBankPayment.IsBankPaymentRequest(isBankRemittancePaymentRequest);

                                    callbackEntity.TransactionID = item.RequestNr;
                                    callbackEntity.ServiceType = "İş Bankası Payment Response";
                                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankRemittancePaymentRequestResponse, opt);
                                    callbackEntity.IDCompany = company.ID;
                                    callbackEntity.TransactionType = "HAVALE PARA GÖNDERİMİ SONUC";
                                    _callbackResponseLogManager.Insert(callbackEntity);

                                    if (isBankRemittancePaymentRequestResponse.Status == "OK")
                                    {
                                        item.Status = (byte)Enums.StatusType.Confirmed;
                                        item.Description = "Çekim Limiti Aşılmadığı İçin Otomatik Onaylandı";
                                        item.MDate = DateTime.Now;
                                        item.MUser = "00000000-0000-0000-0000-000000000000";
                                        item.IsBankQueryNr = isBankRemittancePaymentRequestResponse.Data.data.query_number;
                                        item.SIDBank = "03";
                                        item.CompanyBankAccountID = companyBankAccountID;
                                        var response = _companyWithdrawalRequestManager.SetStatus(item);
                                        if (response.Status == "OK")
                                        {
                                            var dataCallback = new
                                            {
                                                status_code = "OK",
                                                service_id = companyIntegration.ServiceID,
                                                status_type = 2,
                                                ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                                data = new { transaction_id = item.RequestNr, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = "Çekim Talebi İşlemi Başarıyla Tamamlandı" }
                                            };

                                            var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                                            callbackEntity.TransactionID = item.RequestNr;
                                            callbackEntity.ServiceType = "STILPAY";
                                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                            callbackEntity.IDCompany = item.IDCompany;
                                            callbackEntity.TransactionType = "STILPAY HAVALE PARA GÖNDERİMİ CALLBACK";
                                            callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                            _callbackResponseLogManager.Insert(callbackEntity);
                                        }
                                    }
                                    else
                                    {
                                        item.Status = (byte)Enums.StatusType.Canceled;
                                        item.Description = isBankRemittancePaymentRequestResponse.Message;
                                        item.MUser = "00000000-0000-0000-0000-000000000000";
                                        var response = _companyWithdrawalRequestManager.SetStatus(item);
                                        if (response.Status == "OK")
                                        {
                                            var dataCallback = new
                                            {
                                                status_code = "ERROR",
                                                service_id = companyIntegration.ServiceID,
                                                status_type = 2,
                                                ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                                data = new { transaction_id = item.RequestNr, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = isBankRemittancePaymentRequestResponse.Message }
                                            };

                                             var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                                            callbackEntity.TransactionID = item.RequestNr;
                                            callbackEntity.ServiceType = "STILPAY";
                                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                            callbackEntity.IDCompany = item.IDCompany;
                                            callbackEntity.TransactionType = "STILPAY HAVALE PARA GÖNDERİMİ CALLBACK";
                                            callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                            _callbackResponseLogManager.Insert(callbackEntity);
                                        }
                                    }
                                }
                                else
                                {
                                    if (isBankRemittancePaymentValidationResponse.Data != null && isBankRemittancePaymentValidationResponse.Data.infos != null && isBankRemittancePaymentValidationResponse.Data.infos.FirstOrDefault().code == "30080")
                                    {
                                        string[] phones = { "05466113687" };
                                        tSmsSender sender = new tSmsSender();
                                        string msg = string.Concat("Stilpay İş Bankası 6745 Hesabında Bakiye Yetersiz Olduğu İçin Çekim Talebi İşlemleri Yapılamıyor");
                                        foreach (var phone in phones)
                                        {
                                            var smsResp = sender.SendSms(phone, msg);
                                        }                                
                                    }

                                    item.Status = (byte)Enums.StatusType.Canceled;
                                    item.Description = isBankRemittancePaymentValidationResponse.Message;
                                    item.MUser = "00000000-0000-0000-0000-000000000000";
                                    var response = _companyWithdrawalRequestManager.SetStatus(item);
                                    if (response.Status == "OK")
                                    {
                                        var dataCallback = new
                                        {
                                            status_code = "ERROR",
                                            service_id = companyIntegration.ServiceID,
                                            status_type = 2,
                                            ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                            data = new { transaction_id = item.RequestNr, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = isBankRemittancePaymentValidationResponse.Message }
                                        };

                                        var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                                        callbackEntity.TransactionID = item.RequestNr;
                                        callbackEntity.ServiceType = "STILPAY";
                                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                        callbackEntity.IDCompany = item.IDCompany;
                                        callbackEntity.TransactionType = "STILPAY HAVALE PARA GÖNDERİMİ CALLBACK";
                                        callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                        _callbackResponseLogManager.Insert(callbackEntity);
                                    }
                                }
                            }
                            else
                            {
                                var isBankEftPaymentValidationRequest = new IsBankPaymentValidationRequest
                                {
                                    apiUrl = "/api/isbank/v1/eft-validations",
                                    debtor_account_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "debtor_account_id").ParamVal,
                                    isbank_client_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_id").ParamVal,
                                    isbank_client_secret = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_secret").ParamVal,
                                    isbank_client_certificate = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_certificate").ParamVal,
                                    authorization = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_auth").ParamVal,
                                    username = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_username").ParamVal,
                                    password = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_password").ParamVal,
                                    amount = item.Amount,
                                    creditor_name = item.Title,
                                    creditor_iban = item.IBAN.Replace(" ", ""),
                                    description = string.IsNullOrWhiteSpace(item.DealerDescription) ? $"{company.Title} Çekim Talebi" : item.DealerDescription,
                                    payment_reference_id = item.RequestNr,
                                    customer_ip = "89.252.138.236",
                                };

                                var isBankEftPaymentValidationRequestResponse = IsBankPaymentValidation.IsBankPaymentValidationRequest(isBankEftPaymentValidationRequest);

                                callbackEntity.TransactionID = item.RequestNr;
                                callbackEntity.ServiceType = "İş Bankası Payment Validation";
                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankEftPaymentValidationRequestResponse, opt);
                                callbackEntity.IDCompany = company.ID;
                                callbackEntity.TransactionType = "EFT PARA GÖNDERİMİ ÖN ONAY SONUC";
                                _callbackResponseLogManager.Insert(callbackEntity);

                                if (isBankEftPaymentValidationRequestResponse.Status == "OK")
                                {
                                    var isBankEftPaymentRequest = new IsBankPaymentRequest
                                    {
                                        apiUrl = "/api/isbank/v1/eft-payments",
                                        debtor_account_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "debtor_account_id").ParamVal,
                                        isbank_client_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_id").ParamVal,
                                        isbank_client_secret = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_secret").ParamVal,
                                        isbank_client_certificate = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_certificate").ParamVal,
                                        authorization = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_auth").ParamVal,
                                        username = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_username").ParamVal,
                                        password = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_password").ParamVal,
                                        amount = item.Amount,
                                        creditor_name = item.Title,
                                        creditor_iban = item.IBAN.Replace(" ", ""),
                                        description = string.IsNullOrWhiteSpace(item.DealerDescription) ? $"{company.Title} Çekim Talebi" : item.DealerDescription,
                                        payment_reference_id = item.RequestNr,
                                        idempotency_key = item.ID,
                                        query_number = isBankEftPaymentValidationRequestResponse.Data.data.query_number,
                                        customer_ip = "89.252.138.236",
                                    };

                                    var isBankEftPaymentRequestResponse = IsBankPayment.IsBankPaymentRequest(isBankEftPaymentRequest);

                                    callbackEntity.TransactionID = item.RequestNr;
                                    callbackEntity.ServiceType = "İş Bankası Payment Response";
                                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankEftPaymentRequestResponse, opt);
                                    callbackEntity.IDCompany = company.ID;
                                    callbackEntity.TransactionType = "EFT PARA GÖNDERİMİ SONUC";
                                    _callbackResponseLogManager.Insert(callbackEntity);

                                    if (isBankEftPaymentRequestResponse.Status == "OK")
                                    {
                                        item.Status = (byte)Enums.StatusType.Confirmed;
                                        item.Description = "Çekim Limiti Aşılmadığı İçin Otomatik Onaylandı";
                                        item.MDate = DateTime.Now;
                                        item.MUser = "00000000-0000-0000-0000-000000000000";
                                        item.IsBankQueryNr = isBankEftPaymentRequestResponse.Data.data.query_number;
                                        item.SIDBank = "03";
                                        item.CompanyBankAccountID = companyBankAccountID;
                                        var response = _companyWithdrawalRequestManager.SetStatus(item);
                                        if (response.Status == "OK")
                                        {
                                            var dataCallback = new
                                            {
                                                status_code = "OK",
                                                service_id = companyIntegration.ServiceID,
                                                status_type = 2,
                                                ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                                data = new { transaction_id = item.RequestNr, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = "Çekim Talebi İşlemi Başarıyla Tamamlandı" }
                                            };

                                            var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                                            callbackEntity.TransactionID = item.RequestNr;
                                            callbackEntity.ServiceType = "STILPAY";
                                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                            callbackEntity.IDCompany = item.IDCompany;
                                            callbackEntity.TransactionType = "STILPAY EFT PARA GÖNDERİMİ CALLBACK";
                                            callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                            _callbackResponseLogManager.Insert(callbackEntity);
                                        }
                                    }
                                    else
                                    {
                                        item.Status = (byte)Enums.StatusType.Canceled;
                                        item.Description = isBankEftPaymentRequestResponse.Message;
                                        item.MUser = "00000000-0000-0000-0000-000000000000";
                                        var response = _companyWithdrawalRequestManager.SetStatus(item);
                                        if (response.Status == "OK")
                                        {
                                            var dataCallback = new
                                            {
                                                status_code = "ERROR",
                                                service_id = companyIntegration.ServiceID,
                                                status_type = 2,
                                                ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                                data = new { transaction_id = item.RequestNr, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = isBankEftPaymentRequestResponse.Message }
                                            };

                                            var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                                            callbackEntity.TransactionID = item.RequestNr;
                                            callbackEntity.ServiceType = "STILPAY";
                                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                            callbackEntity.IDCompany = item.IDCompany;
                                            callbackEntity.TransactionType = "STILPAY EFT PARA GÖNDERİMİ CALLBACK";
                                            callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                            _callbackResponseLogManager.Insert(callbackEntity);
                                        }
                                    }
                                }

                                else
                                {
                                    if (isBankEftPaymentValidationRequestResponse.Data != null && isBankEftPaymentValidationRequestResponse.Data.infos != null &&
                                        (isBankEftPaymentValidationRequestResponse.Data.infos.FirstOrDefault().code == "MRD110" || isBankEftPaymentValidationRequestResponse.Data.infos.FirstOrDefault().code == "MRD132"))
                                    {
                                        var isBankFastPaymentValidationRequest = new IsBankPaymentValidationRequest
                                        {
                                            apiUrl = "/api/isbank/v1/fast-validations",
                                            debtor_account_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "debtor_account_id").ParamVal,
                                            isbank_client_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_id").ParamVal,
                                            isbank_client_secret = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_secret").ParamVal,
                                            isbank_client_certificate = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_certificate").ParamVal,
                                            authorization = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_auth").ParamVal,
                                            username = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_username").ParamVal,
                                            password = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_password").ParamVal,
                                            amount = item.Amount,
                                            creditor_name = item.Title,
                                            creditor_iban = item.IBAN.Replace(" ", ""),
                                            description = string.IsNullOrWhiteSpace(item.DealerDescription) ? $"{company.Title} Çekim Talebi" : item.DealerDescription,
                                            payment_reference_id = item.RequestNr,
                                            customer_ip = "89.252.138.236",
                                        };

                                        var isBankFastPaymentValidationResponse = IsBankPaymentValidation.IsBankPaymentValidationRequest(isBankFastPaymentValidationRequest);

                                        callbackEntity.TransactionID = item.RequestNr;
                                        callbackEntity.ServiceType = "İş Bankası Payment Validation";
                                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankFastPaymentValidationResponse, opt);
                                        callbackEntity.IDCompany = company.ID;
                                        callbackEntity.TransactionType = "FAST PARA GÖNDERİMİ ÖN ONAY SONUC";
                                        _callbackResponseLogManager.Insert(callbackEntity);

                                        if (isBankFastPaymentValidationResponse.Status == "OK")
                                        {
                                            var isBankFastPaymentRequest = new IsBankPaymentRequest
                                            {
                                                apiUrl = "/api/isbank/v1/fast-payments",
                                                debtor_account_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "debtor_account_id").ParamVal,
                                                isbank_client_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_id").ParamVal,
                                                isbank_client_secret = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_secret").ParamVal,
                                                isbank_client_certificate = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_certificate").ParamVal,
                                                authorization = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_auth").ParamVal,
                                                username = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_username").ParamVal,
                                                password = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_password").ParamVal,
                                                amount = item.Amount,
                                                creditor_name = item.Title,
                                                creditor_iban = item.IBAN.Replace(" ", ""),
                                                description = string.IsNullOrWhiteSpace(item.DealerDescription) ? $"{company.Title} Çekim Talebi" : item.DealerDescription,
                                                payment_reference_id = item.RequestNr,
                                                idempotency_key = item.ID,
                                                query_number = isBankFastPaymentValidationResponse.Data.data.query_number,
                                                customer_ip = "89.252.138.236",
                                            };

                                            var isBankFastPaymentRequestResponse = IsBankPayment.IsBankPaymentRequest(isBankFastPaymentRequest);

                                            callbackEntity.TransactionID = item.RequestNr;
                                            callbackEntity.ServiceType = "İş Bankası Payment Response";
                                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankFastPaymentRequestResponse, opt);
                                            callbackEntity.IDCompany = company.ID;
                                            callbackEntity.TransactionType = "FAST PARA GÖNDERİMİ SONUC";
                                            _callbackResponseLogManager.Insert(callbackEntity);

                                            if (isBankFastPaymentRequestResponse.Status == "OK")
                                            {
                                                item.Status = (byte)Enums.StatusType.Confirmed;
                                                item.Description = "Çekim Limiti Aşılmadığı İçin Otomatik Onaylandı";
                                                item.MDate = DateTime.Now;
                                                item.MUser = "00000000-0000-0000-0000-000000000000";
                                                item.IsBankQueryNr = isBankFastPaymentRequestResponse.Data.data.query_number;
                                                item.SIDBank = "03";
                                                item.CompanyBankAccountID = companyBankAccountID;
                                                var response = _companyWithdrawalRequestManager.SetStatus(item);
                                                if (response.Status == "OK")
                                                {
                                                    var dataCallback = new
                                                    {
                                                        status_code = "OK",
                                                        service_id = companyIntegration.ServiceID,
                                                        status_type = 2,
                                                        ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                                        data = new { transaction_id = item.RequestNr, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = "Çekim Talebi İşlemi Başarıyla Tamamlandı" }
                                                    };

                                                    var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                                                    callbackEntity.TransactionID = item.RequestNr;
                                                    callbackEntity.ServiceType = "STILPAY";
                                                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                                    callbackEntity.IDCompany = item.IDCompany;
                                                    callbackEntity.TransactionType = "STILPAY FAST PARA GÖNDERİMİ CALLBACK";
                                                    callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                                    _callbackResponseLogManager.Insert(callbackEntity);
                                                }
                                            }
                                            else
                                            {
                                                item.Status = (byte)Enums.StatusType.Canceled;
                                                item.Description = isBankFastPaymentRequestResponse.Message;
                                                item.MUser = "00000000-0000-0000-0000-000000000000";
                                                var response = _companyWithdrawalRequestManager.SetStatus(item);
                                                if (response.Status == "OK")
                                                {
                                                    var dataCallback = new
                                                    {
                                                        status_code = "ERROR",
                                                        service_id = companyIntegration.ServiceID,
                                                        status_type = 2,
                                                        ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                                        data = new { transaction_id = item.RequestNr, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = isBankFastPaymentRequestResponse.Message }
                                                    };

                                                    var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                                                    callbackEntity.TransactionID = item.RequestNr;
                                                    callbackEntity.ServiceType = "STILPAY";
                                                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                                    callbackEntity.IDCompany = item.IDCompany;
                                                    callbackEntity.TransactionType = "STILPAY FAST PARA GÖNDERİMİ CALLBACK";
                                                    callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                                    _callbackResponseLogManager.Insert(callbackEntity);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (isBankFastPaymentValidationResponse.Data != null && isBankFastPaymentValidationResponse.Data.infos != null && isBankFastPaymentValidationResponse.Data.infos.FirstOrDefault().code == "30080")
                                            {
                                                string[] phones = { "05466113687" };
                                                tSmsSender sender = new tSmsSender();
                                                string msg = string.Concat("Stilpay İş Bankası 6745 Hesabında Bakiye Yetersiz Olduğu İçin Çekim Talebi İşlemleri Yapılamıyor");
                                                foreach (var phone in phones)
                                                {
                                                    var smsResp = sender.SendSms(phone, msg);
                                                }
                                            }

                                            item.Status = (byte)Enums.StatusType.Canceled;
                                            item.Description = isBankFastPaymentValidationResponse.Message;
                                            item.MUser = "00000000-0000-0000-0000-000000000000";
                                            var response = _companyWithdrawalRequestManager.SetStatus(item);
                                            if (response.Status == "OK")
                                            {
                                                var dataCallback = new
                                                {
                                                    status_code = "ERROR",
                                                    service_id = companyIntegration.ServiceID,
                                                    status_type = 2,
                                                    ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                                    data = new { transaction_id = item.RequestNr, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = isBankFastPaymentValidationResponse.Message }
                                                };

                                                var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                                                callbackEntity.TransactionID = item.RequestNr;
                                                callbackEntity.ServiceType = "STILPAY";
                                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                                callbackEntity.IDCompany = item.IDCompany;
                                                callbackEntity.TransactionType = "STILPAY FAST PARA GÖNDERİMİ CALLBACK";
                                                callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                                _callbackResponseLogManager.Insert(callbackEntity);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (isBankEftPaymentValidationRequestResponse.Data != null && isBankEftPaymentValidationRequestResponse.Data.infos != null && isBankEftPaymentValidationRequestResponse.Data.infos.FirstOrDefault().code == "30080")
                                        {
                                            string[] phones = { "05466113687" };
                                            tSmsSender sender = new tSmsSender();
                                            string msg = string.Concat("Stilpay İş Bankası 6745 Hesabında Bakiye Yetersiz Olduğu İçin Çekim Talebi İşlemleri Yapılamıyor");
                                            foreach (var phone in phones)
                                            {
                                                var smsResp = sender.SendSms(phone, msg);
                                            }
                                        }

                                        item.Status = (byte)Enums.StatusType.Canceled;
                                        item.Description = isBankEftPaymentValidationRequestResponse.Message;
                                        item.MUser = "00000000-0000-0000-0000-000000000000";
                                        var response = _companyWithdrawalRequestManager.SetStatus(item);
                                        if (response.Status == "OK")
                                        {
                                            var dataCallback = new
                                            {
                                                status_code = "ERROR",
                                                service_id = companyIntegration.ServiceID,
                                                status_type = 2,
                                                ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                                data = new { transaction_id = item.RequestNr, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = isBankEftPaymentValidationRequestResponse.Message }
                                            };

                                            var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                                            callbackEntity.TransactionID = item.RequestNr;
                                            callbackEntity.ServiceType = "STILPAY";
                                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                            callbackEntity.IDCompany = item.IDCompany;
                                            callbackEntity.TransactionType = "STILPAY EFT PARA GÖNDERİMİ CALLBACK";
                                            callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                            _callbackResponseLogManager.Insert(callbackEntity);
                                        }
                                    }
                                }
                            }
                        }

                        if (useKuveytTurk)
                        {
                            var kuveyTurkIntegrationValues = _settingDAL.GetList(null).Where(x => x.ParamType == "KuveytTurkTransfer").ToList();

                            var kuveytTurkTransferRequestModel = new KuveytTurkTransferRequestModel()
                            {
                                CorporateWebUserName = kuveyTurkIntegrationValues.FirstOrDefault(f => f.ParamDef == "web_username").ParamVal,
                                SenderAccountSuffix = int.Parse(kuveyTurkIntegrationValues.FirstOrDefault(f => f.ParamDef == "account_suffix").ParamVal),
                                TransferType = int.Parse(kuveyTurkIntegrationValues.FirstOrDefault(f => f.ParamDef == "transfer_type").ParamVal),
                                MoneyTransferAmount = item.Amount,
                                ReceiverIBAN = item.IBAN.Replace(" ", ""),
                                ReceiverName = item.Title,
                                MoneyTransferDescription = string.IsNullOrWhiteSpace(item.DealerDescription) ? $"{item.RequestNr} - {item.Company} Çekim Talebi" : item.RequestNr + " / " + item.DealerDescription,
                                TransactionGuid = item.RequestNr
                            };

                            var kuveytTurkTransferResponse = KuveytTurkMoneyTransfer.MoneyTranfer(kuveytTurkTransferRequestModel);

                            callbackEntity.TransactionID = item.RequestNr;
                            callbackEntity.ServiceType = "Kuveyt Türk Payment Response";
                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(kuveytTurkTransferResponse, opt);
                            callbackEntity.IDCompany = companyIntegration.ID;
                            callbackEntity.TransactionType = "KUVEYT TURK PARA GONDERIMI SONUC AUTO";
                            _callbackResponseLogManager.Insert(callbackEntity);
                            
                            if(kuveytTurkTransferResponse != null && kuveytTurkTransferResponse.Status == "ERROR" && (kuveytTurkTransferResponse.Message.Contains("Bakiye yetersiz") || kuveytTurkTransferResponse.Message.Contains("Bakiye Yetersiz")
                                || kuveytTurkTransferResponse.Message.Contains("Yetersiz") || kuveytTurkTransferResponse.Message.Contains("yetersiz")))
                            {
                                item.Status = (byte)Enums.StatusType.Canceled;
                                item.Description = kuveytTurkTransferResponse.Message;
                                item.MDate = DateTime.Now;
                                item.MUser = "00000000-0000-0000-0000-000000000000";
                                item.IsBankQueryNr = kuveytTurkTransferResponse?.Data?.value?.moneyTransferTransactionId != null ? kuveytTurkTransferResponse?.Data?.value?.moneyTransferTransactionId.ToString() : "-";
                                item.SIDBank = "36";
                                item.CompanyBankAccountID = companyBankAccountID;
                                var responseErr = _companyWithdrawalRequestManager.SetStatus(item);

                                if(responseErr != null && responseErr.Status == "OK")
                                {
                                    var dataCallback = new
                                    {
                                        status_code = "ERROR",
                                        service_id = companyIntegration.ServiceID,
                                        status_type = 2,
                                        ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                        data = new { transaction_id = item.RequestNr, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = "Çekim Talebi Başarısız" }
                                    };

                                    var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                                    callbackEntity.TransactionID = item.RequestNr;
                                    callbackEntity.ServiceType = "STILPAY";
                                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                    callbackEntity.IDCompany = item.IDCompany;
                                    callbackEntity.TransactionType = "STILPAY PARA GÖNDERİMİ CALLBACK";
                                    callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                    _callbackResponseLogManager.Insert(callbackEntity);
                                }
                            }
                            else
                            {
                                item.Status = (byte)Enums.StatusType.Process;
                                item.Description = "İşleme Alındı";
                                item.MDate = DateTime.Now;
                                item.MUser = "00000000-0000-0000-0000-000000000000";
                                item.IsBankQueryNr = kuveytTurkTransferResponse?.Data?.value?.moneyTransferTransactionId != null ? kuveytTurkTransferResponse?.Data?.value?.moneyTransferTransactionId.ToString() : "-";
                                item.SIDBank = "36";
                                item.CompanyBankAccountID = companyBankAccountID;
                                var response = _companyWithdrawalRequestManager.SetStatus(item);
                            }


                            //if (response.Status == "OK")
                            //{
                            //    var dataCallback = new
                            //    {
                            //        status_code = "OK",
                            //        service_id = companyIntegration.ServiceID,
                            //        status_type = 2,
                            //        ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                            //        data = new { transaction_id = item.RequestNr, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = "Çekim Talebi İşlemi Başarıyla Tamamlandı" }
                            //    };

                            //    var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                            //    callbackEntity.TransactionID = item.RequestNr;
                            //    callbackEntity.ServiceType = "STILPAY";
                            //    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                            //    callbackEntity.IDCompany = item.IDCompany;
                            //    callbackEntity.TransactionType = "STILPAY PARA GÖNDERİMİ CALLBACK";
                            //    callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                            //    _callbackResponseLogManager.Insert(callbackEntity);
                            //}                           
                        }
                    }
                }
            }
        }
    }
}
