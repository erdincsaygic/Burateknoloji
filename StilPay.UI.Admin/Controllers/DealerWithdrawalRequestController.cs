using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.BLL.Concrete;
using StilPay.DAL.Concrete;
using StilPay.Entities.Concrete;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.IsBankTransferService;
using StilPay.Utility.KuveytTurk;
using StilPay.Utility.KuveytTurk.KuveytTurkTransfer;
using StilPay.Utility.Models;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ZiraatBankPaymentService;
using static StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPayment.IsBankPaymentRequestModel;
using static StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPaymentValidation.IsBankPaymentValidationRequestModel;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "PendingProcess")]
    public class DealerWithdrawalRequestController : BaseController<CompanyWithdrawalRequest>
    {
        private readonly ICompanyWithdrawalRequestManager _manager;
        private readonly ICompanyIntegrationManager _companyIntegrationManager;
        private readonly ICallbackResponseLogManager _callbackResponseLogManager;
        private readonly ISystemSettingManager _systemSettingManager;
        private readonly ICompanyBankAccountManager _companyBankAccountManager;
        private readonly SettingDAL _settingDAL = new SettingDAL();

        public DealerWithdrawalRequestController(ICompanyWithdrawalRequestManager manager, ISystemSettingManager systemSettingManager, ICompanyIntegrationManager companyIntegrationManager, IHttpContextAccessor httpContext, ICallbackResponseLogManager callbackResponseLogManager, ICompanyBankAccountManager companyBankAccountManager) : base(httpContext)
        {
            _manager = manager;
            _companyIntegrationManager = companyIntegrationManager;
            _callbackResponseLogManager = callbackResponseLogManager;
            _systemSettingManager = systemSettingManager;
            _companyBankAccountManager = companyBankAccountManager;
        }

        public override IBaseBLL<CompanyWithdrawalRequest> Manager()
        {
            return _manager;
        }

        public IActionResult GetData()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = GetData(
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.Pending),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("PageLenght", Enums.FieldType.Int, length),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue),
                new FieldParameter("WithProcess", Enums.FieldType.Bit, true)
            );

            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;

            var result = new
            {
                recordsFiltered = recordsTotal,
                data = list
            };

            return Json(result);
        }

        public override EditViewModel<CompanyWithdrawalRequest> InitEditViewModel(string id = null)
        {
            var model = new EditViewModel<CompanyWithdrawalRequest>();

            var entity = Manager().GetSingle(new List<FieldParameter>()
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, id)
            });

            model.entity = entity;

            var companyBankAccounts = _companyBankAccountManager.GetActiveList(new List<FieldParameter>()
            {
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, "1312E00F-E83E-45B4-85C6-892396D12331"),
            });

            ViewBag.CompanyBankAccount = companyBankAccounts;

            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetStatus(CompanyWithdrawalRequest entity)
        {
            //if (entity.Amount > 50000)
            //    return Json(new GenericResponse { Status = "ERROR", Message = "Maximum 50.000 TL İşlem Yapabilirsiniz." });

            var checkStatus = _manager.GetSingle(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, entity.ID) });

            if (checkStatus != null && (checkStatus.Status != (byte)Enums.StatusType.Pending && checkStatus.Status != (byte)Enums.StatusType.Process))
                return Json(new GenericResponse { Status = "ERROR", Message = "İşlem yapılmış bildirim tekrar değiştirilemez." });

            if (checkStatus != null && (checkStatus.Status == (byte)Enums.StatusType.Process && entity.Status == (byte)Enums.StatusType.Confirmed))
                return Json(new GenericResponse { Status = "ERROR", Message = "İşlemde olan bildirim onaylanamaz." });

            var companyIntegration = _companyIntegrationManager.GetSingle(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, entity.IDCompany) });
            var callbackEntity = new CallbackResponseLog();
            var opt = new JsonSerializerOptions() { WriteIndented = true };

            if (entity.Status == (byte)Enums.StatusType.Canceled)
            {
                var response = _manager.SetStatus(entity);
                if (response.Status == "OK")
                {
                    var dataCallback = new
                    {
                        status_code = "ERROR",
                        service_id = companyIntegration.ServiceID,
                        status_type = 2,
                        ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                        data = new { transaction_id = entity.RequestNr, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = entity.Description }
                    };

                    var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                    callbackEntity.TransactionID = entity.RequestNr;
                    callbackEntity.ServiceType = "STILPAY";
                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                    callbackEntity.IDCompany = companyIntegration.ID;
                    callbackEntity.TransactionType = "ÜYE İŞYERİ ÇEKİM TALEBİ";
                    callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                    _callbackResponseLogManager.Insert(callbackEntity);
                }

                return Json(response);
            }

            var activeBank = _companyBankAccountManager.GetActiveList(new List<FieldParameter>() { new FieldParameter("IDCompany", Enums.FieldType.NVarChar, "1312E00F-E83E-45B4-85C6-892396D12331") }).Where(x => x.IsActiveByDefaultExpenseAccount).FirstOrDefault();

            var useZiraatBank = false;
            var useIsBank = false;
            var useKuveytTurk = false;
            var idBank = "";
            var companyBankAccountID = "";

            if (activeBank != null)
            {
                switch (activeBank.IDBank)
                {
                    case "08":
                        useZiraatBank = true;
                        companyBankAccountID = activeBank.ID;
                        idBank = "08";
                        break;

                    case "03":
                        useIsBank = true;
                        companyBankAccountID = activeBank.ID;
                        idBank = "03";
                        break;

                    case "07":
                        useKuveytTurk = true;
                        companyBankAccountID = activeBank.ID;
                        idBank = "07";
                        break;

                    case "36":
                        useKuveytTurk = true;
                        companyBankAccountID = activeBank.ID;
                        idBank = "36";
                        break;

                }
            }

            if (useZiraatBank)
            {
                var ziraatService = new NkyParaTransferiWSSoapClient(NkyParaTransferiWSSoapClient.EndpointConfiguration.NkyParaTransferiWSSoap);
                var securedWebServiceHeader = new SecuredWebServiceHeader();

                if (entity.IDBank == "08")
                {
                    var havaleResponse = ziraatService.HavaleYapAsync(securedWebServiceHeader, "97736040", "5002", "", "", entity.IBAN.Replace(" ", ""), "TRY", entity.Amount.ToString(), string.IsNullOrWhiteSpace(entity.DealerDescription) ? $"{entity.Company} Çekim Talebi" : entity.DealerDescription, entity.RequestNr, "", "", "", "", "", "", "", "", "", "", "").Result;

                    callbackEntity.TransactionID = entity.RequestNr;
                    callbackEntity.ServiceType = "Ziraat Bankası Havale";
                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(havaleResponse.HavaleYapResult, opt);
                    callbackEntity.IDCompany = companyIntegration.ID;
                    callbackEntity.TransactionType = "Havale";
                    _callbackResponseLogManager.Insert(callbackEntity);

                    if (havaleResponse.HavaleYapResult.CevapKodu != "0")
                    {
                        return Json(new GenericResponse { Status = "ERROR", Message = havaleResponse.HavaleYapResult.CevapMesaji });
                    }

                    else
                    {

                        entity.SIDBank = idBank;
                        entity.CompanyBankAccountID = companyBankAccountID;
                        var response = _manager.SetStatus(entity);
                        if (response.Status == "OK")
                        {
                            var dataCallback = new
                            {
                                status_code = "OK",
                                service_id = companyIntegration.ServiceID,
                                status_type = 2,
                                ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                data = new { transaction_id = entity.RequestNr, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = "Çekim Talebi Başarıyla Gerçekleştirildi" }
                            };

                            var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                            callbackEntity.TransactionID = entity.RequestNr;
                            callbackEntity.ServiceType = "STILPAY";
                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                            callbackEntity.IDCompany = entity.IDCompany;
                            callbackEntity.TransactionType = "ÜYE İŞYERİ ÇEKİM TALEBİ";
                            callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                            _callbackResponseLogManager.Insert(callbackEntity);
                        }

                        return Json(response);
                    }
                }

                else
                {
                    var eftResponse = ziraatService.EftYapAsync(securedWebServiceHeader, "97736040", "5002", entity.Bank, "", "", entity.IBAN.Replace(" ", ""), entity.Title, DateTime.Now.ToString("yyyyMMdd"), entity.Amount.ToString(), entity.RequestNr, string.IsNullOrWhiteSpace(entity.DealerDescription) ? $"{entity.Company} Çekim Talebi" : entity.DealerDescription, "", "", "", "", "", "", "", "", "", "").Result;

                    callbackEntity.TransactionID = entity.RequestNr;
                    callbackEntity.ServiceType = "Ziraat Bankası EFT";
                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(eftResponse.EftYapResult, opt);
                    callbackEntity.IDCompany = companyIntegration.ID;
                    callbackEntity.TransactionType = "EFT";
                    _callbackResponseLogManager.Insert(callbackEntity);

                    if (eftResponse.EftYapResult.CevapKodu != "0")
                    {
                        return Json(new GenericResponse { Status = "ERROR", Message = eftResponse.EftYapResult.CevapMesaji });
                    }

                    else
                    {

                        entity.SIDBank = idBank;
                        entity.CompanyBankAccountID = companyBankAccountID;
                        var response = _manager.SetStatus(entity);
                        if (response.Status == "OK")
                        {
                            var dataCallback = new
                            {
                                status_code = "OK",
                                service_id = companyIntegration.ServiceID,
                                status_type = 2,
                                ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                data = new { transaction_id = entity.RequestNr, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = "Çekim Talebi Başarıyla Gerçekleştirildi" }
                            };

                            var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                            callbackEntity.TransactionID = entity.RequestNr;
                            callbackEntity.ServiceType = "STILPAY";
                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                            callbackEntity.IDCompany = companyIntegration.ID;
                            callbackEntity.TransactionType = "ÜYE İŞYERİ ÇEKİM TALEBİ";
                            callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                            _callbackResponseLogManager.Insert(callbackEntity);
                        }

                        return Json(response);
                    }
                }
            }

            if (useIsBank)
            {
                var isBankIntegrationValues = _settingDAL.GetList(null).Where(x => x.ParamType == "IsBankTransfer").ToList();

                if (entity.IDBank == "03")
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
                        amount = entity.Amount,
                        creditor_iban = entity.IBAN.Replace(" ", ""),
                        description = string.IsNullOrWhiteSpace(entity.DealerDescription) ? $"{entity.Company} Çekim Talebi" : entity.DealerDescription,
                        payment_reference_id = entity.RequestNr,
                        customer_ip = "89.252.138.236"
                    };

                    var isBankRemittancePaymentValidationResponse = IsBankPaymentValidation.IsBankPaymentValidationRequest(isBankRemittancePaymentValidationRequest);

                    callbackEntity.TransactionID = entity.RequestNr;
                    callbackEntity.ServiceType = "İş Bankası Payment Validation";
                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankRemittancePaymentValidationResponse, opt);
                    callbackEntity.IDCompany = companyIntegration.ID;
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
                            amount = entity.Amount,
                            creditor_iban = entity.IBAN.Replace(" ", ""),
                            description = string.IsNullOrWhiteSpace(entity.DealerDescription) ? $"{entity.Company} Çekim Talebi" : entity.DealerDescription,
                            payment_reference_id = entity.RequestNr,
                            query_number = isBankRemittancePaymentValidationResponse.Data.data.query_number,
                            customer_ip = "89.252.138.236",
                            idempotency_key = entity.ID,
                        };

                        var isBankRemittancePaymentRequestResponse = IsBankPayment.IsBankPaymentRequest(isBankRemittancePaymentRequest);

                        callbackEntity.TransactionID = entity.RequestNr;
                        callbackEntity.ServiceType = "İş Bankası Payment Response";
                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankRemittancePaymentRequestResponse, opt);
                        callbackEntity.IDCompany = companyIntegration.ID;
                        callbackEntity.TransactionType = "HAVALE PARA GÖNDERİMİ SONUC";
                        _callbackResponseLogManager.Insert(callbackEntity);

                        if (isBankRemittancePaymentRequestResponse.Status == "OK")
                        {
                            entity.IsBankQueryNr = isBankRemittancePaymentRequestResponse.Data.data.query_number;
                            entity.SIDBank = idBank;
                            entity.CompanyBankAccountID = companyBankAccountID;
                            var response = _manager.SetStatus(entity);
                            if (response.Status == "OK")
                            {
                                var dataCallback = new
                                {
                                    status_code = "OK",
                                    service_id = companyIntegration.ServiceID,
                                    status_type = 2,
                                    ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                    data = new { transaction_id = entity.RequestNr, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = "Çekim Talebi Başarıyla Gerçekleştirildi" }
                                };

                                var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                                callbackEntity.TransactionID = entity.RequestNr;
                                callbackEntity.ServiceType = "STILPAY";
                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                callbackEntity.IDCompany = companyIntegration.ID;
                                callbackEntity.TransactionType = "ÜYE İŞYERİ ÇEKİM TALEBİ";
                                callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                _callbackResponseLogManager.Insert(callbackEntity);
                            }

                            return Json(response);
                        }
                        else
                            return Json(new GenericResponse { Status = "ERROR", Message = isBankRemittancePaymentRequestResponse.Message });
                    }
                    else
                        return Json(new GenericResponse { Status = "ERROR", Message = isBankRemittancePaymentValidationResponse.Message });
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
                        amount = entity.Amount,
                        creditor_name = entity.Title,
                        creditor_iban = entity.IBAN.Replace(" ", ""),
                        description = string.IsNullOrWhiteSpace(entity.DealerDescription) ? $"{entity.Company} Çekim Talebi" : entity.DealerDescription,
                        payment_reference_id = entity.RequestNr,
                        customer_ip = "89.252.138.236",
                    };

                    var isBankEftPaymentValidationRequestResponse = IsBankPaymentValidation.IsBankPaymentValidationRequest(isBankEftPaymentValidationRequest);

                    callbackEntity.TransactionID = entity.RequestNr;
                    callbackEntity.ServiceType = "İş Bankası Payment Validation";
                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankEftPaymentValidationRequestResponse, opt);
                    callbackEntity.IDCompany = companyIntegration.ID;
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
                            amount = entity.Amount,
                            creditor_name = entity.Title,
                            creditor_iban = entity.IBAN.Replace(" ", ""),
                            description = string.IsNullOrWhiteSpace(entity.DealerDescription) ? $"{entity.Company} Çekim Talebi" : entity.DealerDescription,
                            payment_reference_id = entity.RequestNr,
                            idempotency_key = entity.ID,
                            query_number = isBankEftPaymentValidationRequestResponse.Data.data.query_number,
                            customer_ip = "89.252.138.236",
                        };

                        var isBankEftPaymentRequestResponse = IsBankPayment.IsBankPaymentRequest(isBankEftPaymentRequest);

                        callbackEntity.TransactionID = entity.RequestNr;
                        callbackEntity.ServiceType = "İş Bankası Payment Response";
                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankEftPaymentRequestResponse, opt);
                        callbackEntity.IDCompany = companyIntegration.ID;
                        callbackEntity.TransactionType = "EFT PARA GÖNDERİMİ SONUC";
                        _callbackResponseLogManager.Insert(callbackEntity);

                        if (isBankEftPaymentRequestResponse.Status == "OK")
                        {
                            entity.IsBankQueryNr = isBankEftPaymentRequestResponse.Data.data.query_number;
                            entity.SIDBank = idBank;
                            entity.CompanyBankAccountID = companyBankAccountID;
                            var response = _manager.SetStatus(entity);
                            if (response.Status == "OK")
                            {
                                var dataCallback = new
                                {
                                    status_code = "OK",
                                    service_id = companyIntegration.ServiceID,
                                    status_type = 2,
                                    ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                    data = new { transaction_id = entity.RequestNr, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = "Çekim Talebi Başarıyla Gerçekleştirildi" }
                                };

                                var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                                callbackEntity.TransactionID = entity.RequestNr;
                                callbackEntity.ServiceType = "STILPAY";
                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                callbackEntity.IDCompany = companyIntegration.ID;
                                callbackEntity.TransactionType = "ÜYE İŞYERİ ÇEKİM TALEBİ";
                                callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                _callbackResponseLogManager.Insert(callbackEntity);
                            }

                            return Json(response);
                        }
                        else
                            return Json(new GenericResponse { Status = "ERROR", Message = isBankEftPaymentRequestResponse.Message });
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
                                amount = entity.Amount,
                                creditor_name = entity.Title,
                                creditor_iban = entity.IBAN.Replace(" ", ""),
                                description = string.IsNullOrWhiteSpace(entity.DealerDescription) ? $"{entity.Company} Çekim Talebi" : entity.DealerDescription,
                                payment_reference_id = entity.RequestNr,
                                customer_ip = "89.252.138.236",
                            };

                            var isBankFastPaymentValidationRequestResponse = IsBankPaymentValidation.IsBankPaymentValidationRequest(isBankFastPaymentValidationRequest);

                            callbackEntity.TransactionID = entity.RequestNr;
                            callbackEntity.ServiceType = "İş Bankası Payment Validation";
                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankFastPaymentValidationRequestResponse, opt);
                            callbackEntity.IDCompany = companyIntegration.ID;
                            callbackEntity.TransactionType = "FAST PARA GÖNDERİMİ ÖN ONAY SONUC";
                            _callbackResponseLogManager.Insert(callbackEntity);

                            if (isBankFastPaymentValidationRequestResponse.Status == "OK")
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
                                    amount = entity.Amount,
                                    creditor_name = entity.Title,
                                    creditor_iban = entity.IBAN.Replace(" ", ""),
                                    description = string.IsNullOrWhiteSpace(entity.DealerDescription) ? $"{entity.Company} Çekim Talebi" : entity.DealerDescription,
                                    payment_reference_id = entity.RequestNr,
                                    idempotency_key = entity.ID,
                                    query_number = isBankFastPaymentValidationRequestResponse.Data.data.query_number,
                                    customer_ip = "89.252.138.236",
                                };

                                var isBankFastPaymentRequestResponse = IsBankPayment.IsBankPaymentRequest(isBankFastPaymentRequest);

                                callbackEntity.TransactionID = entity.RequestNr;
                                callbackEntity.ServiceType = "İş Bankası Payment Response";
                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankFastPaymentRequestResponse, opt);
                                callbackEntity.IDCompany = companyIntegration.ID;
                                callbackEntity.TransactionType = "FAST PARA GÖNDERİMİ SONUC";
                                _callbackResponseLogManager.Insert(callbackEntity);

                                if (isBankFastPaymentRequestResponse.Status == "OK")
                                {
                                    entity.IsBankQueryNr = isBankFastPaymentRequestResponse.Data.data.query_number;
                                    entity.SIDBank = idBank;
                                    entity.CompanyBankAccountID = companyBankAccountID;
                                    var response = _manager.SetStatus(entity);
                                    if (response.Status == "OK")
                                    {
                                        var dataCallback = new
                                        {
                                            status_code = "OK",
                                            service_id = companyIntegration.ServiceID,
                                            status_type = 2,
                                            ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                            data = new { transaction_id = entity.RequestNr, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = "Çekim Talebi Başarıyla Gerçekleştirildi" }
                                        };

                                        var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                                        callbackEntity.TransactionID = entity.RequestNr;
                                        callbackEntity.ServiceType = "STILPAY";
                                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                        callbackEntity.IDCompany = companyIntegration.ID;
                                        callbackEntity.TransactionType = "ÜYE İŞYERİ ÇEKİM TALEBİ";
                                        callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                        _callbackResponseLogManager.Insert(callbackEntity);
                                    }

                                    return Json(response);
                                }
                                else
                                    return Json(isBankFastPaymentRequestResponse);
                            }
                            else
                                return Json(isBankFastPaymentValidationRequestResponse);
                        }
                        else
                            return Json(isBankEftPaymentValidationRequestResponse);
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
                    MoneyTransferAmount = entity.Amount,
                    ReceiverIBAN = entity.IBAN.Replace(" ", ""),
                    ReceiverName = entity.Title,
                    MoneyTransferDescription = string.IsNullOrWhiteSpace(entity.DealerDescription) ? $"{entity.Company} Çekim Talebi" : entity.DealerDescription,
                    TransactionGuid = entity.RequestNr
                };

                var kuveytTurkTransferResponse = KuveytTurkMoneyTransfer.MoneyTranfer(kuveytTurkTransferRequestModel);

                callbackEntity.TransactionID = entity.RequestNr;
                callbackEntity.ServiceType = "Kuveyt Türk Bankası Payment Response";
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(kuveytTurkTransferResponse, opt);
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.TransactionType = "PARA GÖNDERİMİ SONUC";
                _callbackResponseLogManager.Insert(callbackEntity);

                if (kuveytTurkTransferResponse.Status == "OK")
                {
                    entity.IsBankQueryNr = kuveytTurkTransferResponse.Data.value.moneyTransferTransactionId.ToString();
                    entity.SIDBank = idBank;
                    entity.CompanyBankAccountID = companyBankAccountID;
                    var response = _manager.SetStatus(entity);
                    if (response.Status == "OK")
                    {
                        var dataCallback = new
                        {
                            status_code = "OK",
                            service_id = companyIntegration.ServiceID,
                            status_type = 2,
                            ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                            data = new { transaction_id = entity.RequestNr, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = "Çekim Talebi Başarıyla Gerçekleştirildi" }
                        };

                        var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.WithdrawalRequestCallback, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

                        callbackEntity.TransactionID = entity.RequestNr;
                        callbackEntity.ServiceType = "STILPAY";
                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                        callbackEntity.IDCompany = companyIntegration.ID;
                        callbackEntity.TransactionType = "ÜYE İŞYERİ ÇEKİM TALEBİ";
                        callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                        _callbackResponseLogManager.Insert(callbackEntity);
                    }

                    return Json(response);
                }
                else
                    return Json(kuveytTurkTransferResponse);
            }

            else
                return Json(new GenericResponse { Status = "ERROR", Message = "Para Gönderilecek Banka Seçiminde Bir Hata Mevcut. Lütfen Yetkiliniz İle İletişime Geçin" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetStatusWithoutAPI(CompanyWithdrawalRequest entity)
        {
            if (string.IsNullOrEmpty(entity.CompanyBankAccountID))
                return Json(new GenericResponse { Status = "ERROR", Message = "Lütfen Banka Seçiniz" });

            var checkStatus = _manager.GetSingle(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, entity.ID) });

            if (checkStatus != null && checkStatus.Status == (byte)Enums.StatusType.Confirmed)
                return Json(new GenericResponse { Status = "ERROR", Message = "Bildirim zaten onaylanmış" });

            var companyBankAccount = _companyBankAccountManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, entity.CompanyBankAccountID) });
            entity.SIDBank = companyBankAccount.IDBank;
            return Json(_manager.SetStatus(entity));
        }
    }
}
