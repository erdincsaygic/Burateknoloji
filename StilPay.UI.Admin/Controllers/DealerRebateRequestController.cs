using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2013.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using ParamPosLiveReference;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.BLL.Concrete;
using StilPay.DAL.Abstract;
using StilPay.DAL.Concrete;
using StilPay.Entities;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.IsBankSanalPos;
using StilPay.Utility.IsBankSanalPos.IsBankSanalPOSCancelModel;
using StilPay.Utility.IsBankSanalPos.IsBankSanalPOSRefundModel;
using StilPay.Utility.IsBankTransferService;
using StilPay.Utility.KuveytTurk;
using StilPay.Utility.KuveytTurk.KuveytTurkTransfer;
using StilPay.Utility.Models;
using StilPay.Utility.Paybull.PaybullPayment;
using StilPay.Utility.Paybull;
using StilPay.Utility.PayNKolay;
using StilPay.Utility.PayNKolay.Models.CancelRefundPayment;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using ZiraatBankPaymentService;
using static StilPay.Utility.Helper.Enums;
using static StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPayment.IsBankPaymentRequestModel;
using static StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPaymentValidation.IsBankPaymentValidationRequestModel;
using StilPay.Utility.Paybull.PaybullRefund;
using StilPay.Utility.AKODESanalPOS.Models.AKODECancel;
using Org.BouncyCastle.Crypto.Prng;
using StilPay.Utility.AKODESanalPOS.Models.AKODECreditCardInfo;
using StilPay.Utility.AKODESanalPOS;
using StilPay.Utility.AKODESanalPOS.Models.AKODERefund;
using DocumentFormat.OpenXml.Vml.Office;
using System.Reflection;
using StilPay.Utility.ToslaSanalPos.Models.ToslaRefund;
using StilPay.Utility.ToslaSanalPos;
using System.Security.Claims;
using StilPay.Utility.EsnekPos.Models.EsnekPosCancelAndRefund;
using StilPay.Utility.EsnekPos;
using StilPay.Utility.LidioPos.Models.LidioPosCancel;
using StilPay.Utility.LidioPos;
using StilPay.Utility.LidioPos.Models.LidioPosRefund;
using StilPay.Utility.EfixPos;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "PendingProcess")]
    public class DealerRebateRequestController : BaseController<CompanyRebateRequest>
    {
        private readonly ICompanyRebateRequestManager _manager;
        private readonly IPaymentNotificationManager _paymentNotificationManager;
        private readonly ICompanyIntegrationManager _companyIntegrationManager;
        private readonly ICreditCardPaymentNotificationManager _creditCardPaymentNotificationManager;
        private readonly ICallbackResponseLogManager _callbackResponseLogManager;
        private readonly ISystemSettingManager _systemSettingManager;
        private readonly IForeignCreditCardPaymentNotificationManager _foreignCreditCardPaymentNotificationManager;
        private readonly ICompanyTransactionManager _companyTransactionManager;
        private readonly IPaymentInstitutionManager _paymentInstitutionManager;
        private readonly ICompanyBankAccountManager _companyBankAccountManager;
        private readonly SettingDAL _settingDAL = new SettingDAL();
        public DealerRebateRequestController(ICompanyRebateRequestManager manager, IPaymentNotificationManager paymentNotificationManager, ICreditCardPaymentNotificationManager creditCardPaymentNotificationManager, ICompanyIntegrationManager companyIntegrationManager, ICallbackResponseLogManager callbackResponseLogManager, ISystemSettingManager systemSettingManager, IForeignCreditCardPaymentNotificationManager foreignCreditCardPaymentNotificationManager, ICompanyTransactionManager companyTransactionManager, IHttpContextAccessor httpContext, IPaymentInstitutionManager paymentInstitutionManager, ICompanyBankAccountManager companyBankAccountManager) : base(httpContext)
        {
            _manager = manager;
            _paymentNotificationManager = paymentNotificationManager;
            _companyIntegrationManager = companyIntegrationManager;
            _creditCardPaymentNotificationManager = creditCardPaymentNotificationManager;
            _callbackResponseLogManager = callbackResponseLogManager;
            _systemSettingManager = systemSettingManager;
            _foreignCreditCardPaymentNotificationManager = foreignCreditCardPaymentNotificationManager;
            _companyTransactionManager = companyTransactionManager;
            _paymentInstitutionManager = paymentInstitutionManager;
            _companyBankAccountManager = companyBankAccountManager;
        }

        public override IBaseBLL<CompanyRebateRequest> Manager()
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
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetStatus(CompanyRebateRequest entity)
        {
            if (entity.Status == (byte)Enums.StatusType.Canceled)
                return Json(_manager.SetStatus(entity));

            var py = _paymentNotificationManager.GetSingleByTransactionID(entity.TransactionID);
            var creditCardEntity = _creditCardPaymentNotificationManager.GetSingleByTransactionID(entity.TransactionID);
            var foreignCreditCardEntity = _foreignCreditCardPaymentNotificationManager.GetSingleByTransactionID(entity.TransactionID);

            var callbackEntity = new CallbackResponseLog();
            var opt = new JsonSerializerOptions() { WriteIndented = true };

            if (foreignCreditCardEntity != null && py == null && creditCardEntity == null)
            {
                var paymentInstitutions = _paymentInstitutionManager.GetList(null).FirstOrDefault(x => x.ID == foreignCreditCardEntity.CreditCardPaymentMethodID.ToString());

                foreignCreditCardEntity.Amount =  entity.PartialRebatePaymentInstitutionTotalAmount;

                if (paymentInstitutions != null)
                {
                    Type type = typeof(DealerRebateRequestController);
                    MethodInfo methodInfo = type.GetMethod(paymentInstitutions.RedirectToActionRebateMethod);

                    if (methodInfo != null)
                    {
                        GenericResponse result = (GenericResponse)methodInfo.Invoke(this, new object[] { foreignCreditCardEntity });

                        if (result.Status == "OK")
                            return Json(_manager.SetStatus(entity));
                        else
                            return Json(result);
                    }
                }

                //var foreignCreditCardIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "PayNKolayForeignCC") });

                //if (foreignCreditCardEntity.TransactionReferenceCode != null || foreignCreditCardEntity.TransactionReferenceCode != "")
                //{
                //    var companyIntegration = _companyIntegrationManager.GetByServiceId(foreignCreditCardEntity.ServiceID);

                //    if (foreignCreditCardEntity.ActionDate == DateTime.Now.Date)
                //    {
                //        var cancelPaymentRequestModel = new CancelRefundPaymentRequestModel.CancelPaymentRequestModel
                //        {
                //            Amount = foreignCreditCardEntity.Amount,
                //            ReferenceCode = foreignCreditCardEntity.TransactionReferenceCode,
                //            Sx = foreignCreditCardIntegrationValues.FirstOrDefault(f => f.ParamDef == "cancel_sx").ParamVal,
                //            SecretKey = foreignCreditCardIntegrationValues.FirstOrDefault(f => f.ParamDef == "mercant_secret_key").ParamVal,
                //        };

                //        String str = cancelPaymentRequestModel.Sx + cancelPaymentRequestModel.ReferenceCode + cancelPaymentRequestModel.Type + foreignCreditCardEntity.Amount.ToString(CultureInfo.InvariantCulture) + cancelPaymentRequestModel.SecretKey;
                //        System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                //        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(str);
                //        byte[] hashingbytes = sha.ComputeHash(bytes);
                //        String hashData = Convert.ToBase64String(hashingbytes);
                //        cancelPaymentRequestModel.HashData = hashData;

                //        var cancelPaymentRequestResponse = CancelRefundPayment.CancelPaymentRequest(cancelPaymentRequestModel);

                //        callbackEntity.TransactionID = entity.TransactionID;
                //        callbackEntity.ServiceType = "PayNKolay";
                //        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(cancelPaymentRequestResponse, opt);
                //        callbackEntity.IDCompany = companyIntegration.ID;
                //        callbackEntity.TransactionType = "KREDI KARTI ODEMESI IPTAL";
                //        _callbackResponseLogManager.Insert(callbackEntity);

                //        if (cancelPaymentRequestResponse.Status == "OK")
                //        {
                //            if (cancelPaymentRequestResponse.Data.RESPONSE_CODE == 2)
                //                return Json(_manager.SetStatus(entity));
                //            else
                //                return Json(new GenericResponse { Status = "ERROR", Message = cancelPaymentRequestResponse.Data.RESPONSE_DATA });
                //        }
                //        else
                //            return Json(new GenericResponse { Status = "ERROR", Message = cancelPaymentRequestResponse.Message });

                //    }
                //    else
                //    {
                //        var refundPaymentRequestModel = new CancelRefundPaymentRequestModel.RefundRequestModel
                //        {
                //            Amount = foreignCreditCardEntity.Amount,
                //            ReferenceCode = foreignCreditCardEntity.TransactionReferenceCode,
                //            TrxDate = foreignCreditCardEntity.CDate.ToString("yyyy.MM.dd"),
                //            Sx = foreignCreditCardIntegrationValues.FirstOrDefault(f => f.ParamDef == "cancel_sx").ParamVal,
                //            SecretKey = foreignCreditCardIntegrationValues.FirstOrDefault(f => f.ParamDef == "mercant_secret_key").ParamVal,
                //        };

                //        String str = refundPaymentRequestModel.Sx + refundPaymentRequestModel.ReferenceCode + refundPaymentRequestModel.Type + foreignCreditCardEntity.Amount.ToString(CultureInfo.InvariantCulture) + refundPaymentRequestModel.TrxDate + refundPaymentRequestModel.SecretKey;
                //        System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                //        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(str);
                //        byte[] hashingbytes = sha.ComputeHash(bytes);
                //        String hashData = Convert.ToBase64String(hashingbytes);
                //        refundPaymentRequestModel.HashData = hashData;

                //        var refundPaymentRequestResponse = CancelRefundPayment.RefundPaymentRequest(refundPaymentRequestModel);

                //        callbackEntity.TransactionID = entity.TransactionID;
                //        callbackEntity.ServiceType = "PayNKolay";
                //        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(refundPaymentRequestResponse, opt);
                //        callbackEntity.IDCompany = companyIntegration.ID;
                //        callbackEntity.TransactionType = "KREDI KARTI ODEMESI IADE";
                //        _callbackResponseLogManager.Insert(callbackEntity);

                //        if (refundPaymentRequestResponse.Status == "OK")
                //        {
                //            if (refundPaymentRequestResponse.Data.RESPONSE_CODE == 2)
                //                return Json(_manager.SetStatus(entity));
                //            else
                //                return Json(new GenericResponse { Status = "ERROR", Message = refundPaymentRequestResponse.Data.RESPONSE_DATA });
                //        }
                //        else
                //            return Json(new GenericResponse { Status = "ERROR", Message = refundPaymentRequestResponse.Message });
                //    }
                //}

                //else
                //    return Json(new GenericResponse { Status = "ERROR", Message = "Bir Hata Meydana Geldi" });
            }

            if (py == null && foreignCreditCardEntity == null && creditCardEntity != null)
            {
                var paymentInstitutions = _paymentInstitutionManager.GetList(null).FirstOrDefault(x => x.ID == creditCardEntity.CreditCardPaymentMethodID.ToString());

                creditCardEntity.Amount = entity.PartialRebatePaymentInstitutionTotalAmount;

                if(paymentInstitutions != null)
                {
                    Type type = typeof(DealerRebateRequestController);
                    MethodInfo methodInfo = type.GetMethod(paymentInstitutions.RedirectToActionRebateMethod);

                    if (methodInfo != null)
                    {
                        GenericResponse result = (GenericResponse)methodInfo.Invoke(this, new object[] { creditCardEntity });

                        if(result.Status == "OK")
                            return Json(_manager.SetStatus(entity));
                        else
                            return Json(result);
                    }
                }
            }

            if (py != null && creditCardEntity == null && foreignCreditCardEntity == null)
            {
                var activeBank = _companyBankAccountManager.GetActiveList(new List<FieldParameter>() { new FieldParameter("IDCompany", Enums.FieldType.NVarChar, "1312E00F-E83E-45B4-85C6-892396D12331") }).Where(x => x.IsActiveByDefaultExpenseAccount).FirstOrDefault();

                entity.Amount = entity.PartialRebatePaymentInstitutionTotalAmount;

                //creditCardEntity.Amount = entity.IsPartialRebate ? entity.PartialRebatePaymentInstitutionTotalAmount : creditCardEntity.Amount;

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

                var companyIntegration = _companyIntegrationManager.GetByServiceId(py.ServiceID);

                if (useZiraatBank)
                {
                    var ziraatService = new NkyParaTransferiWSSoapClient(NkyParaTransferiWSSoapClient.EndpointConfiguration.NkyParaTransferiWSSoap);
                    var securedWebServiceHeader = new SecuredWebServiceHeader();

                    if (entity.IDBank == "08")
                    {
                        var havaleResponse = ziraatService.HavaleYapAsync(securedWebServiceHeader, "97736040", "5002", "", "", py.Iban, "TRY", entity.Amount.ToString(), $"{DateTime.Now.ToString()} Tarihinde {py.Company} Üye İşyeri Tarafından {py.Member} Kullanıcısına İade", py.TransactionID, "", "", "", "", "", "", "", "", "", "", "").Result;

                        callbackEntity.TransactionID = entity.TransactionID;
                        callbackEntity.ServiceType = "Ziraat Bankası Havale";
                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(havaleResponse.HavaleYapResult, opt);
                        callbackEntity.IDCompany = companyIntegration.ID;
                        callbackEntity.TransactionType = "IADE";
                        _callbackResponseLogManager.Insert(callbackEntity);

                        if (havaleResponse.HavaleYapResult.CevapKodu != "0")
                            return Json(new GenericResponse { Status = "ERROR", Message = havaleResponse.HavaleYapResult.CevapMesaji });

                        else
                        {
                            entity.SIDBank = idBank;
                            entity.CompanyBankAccountID = companyBankAccountID;
                            return Json(_manager.SetStatus(entity));
                        }
                    }
                    else
                    {
                        var eftResponse = ziraatService.EftYapAsync(securedWebServiceHeader, "97736040", "5002", entity.Bank, "", "", py.Iban, py.SenderName, DateTime.Now.ToString("yyyyMMdd"), entity.Amount.ToString(), py.TransactionID, $"{py.SenderName} Çekim Talebi", "", "", "", "", "", "", "", "", "", "").Result;

                        callbackEntity.TransactionID = entity.TransactionID;
                        callbackEntity.ServiceType = "Ziraat Bankası Havale";
                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(eftResponse.EftYapResult, opt);
                        callbackEntity.IDCompany = companyIntegration.ID;
                        callbackEntity.TransactionType = "IADE";
                        _callbackResponseLogManager.Insert(callbackEntity);

                        if (eftResponse.EftYapResult.CevapKodu != "0")
                            return Json(new GenericResponse { Status = "ERROR", Message = eftResponse.EftYapResult.CevapMesaji });

                        else
                        {
                            entity.SIDBank = idBank;
                            entity.CompanyBankAccountID = companyBankAccountID;
                            return Json(_manager.SetStatus(entity));
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
                            creditor_iban = py.Iban.Replace(" ", ""),
                            description = $"{py.Company} İade",
                            payment_reference_id = py.TransactionNr,
                            customer_ip = "89.252.138.236"
                        };

                        var isBankRemittancePaymentValidationResponse = IsBankPaymentValidation.IsBankPaymentValidationRequest(isBankRemittancePaymentValidationRequest);

                        callbackEntity.TransactionID = py.TransactionID;
                        callbackEntity.ServiceType = "İş Bankası Payment Validation";
                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankRemittancePaymentValidationResponse, opt);
                        callbackEntity.IDCompany = companyIntegration.ID;
                        callbackEntity.TransactionType = "HAVALE PARA GONDERIMI ON ONAY SONUC";
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
                                creditor_iban = py.Iban.Replace(" ", ""),
                                description = $"{py.Company} İade",
                                payment_reference_id = py.TransactionNr,
                                query_number = isBankRemittancePaymentValidationResponse.Data.data.query_number,
                                customer_ip = "89.252.138.236",
                                idempotency_key = py.ID,
                            };

                            var isBankRemittancePaymentRequestResponse = IsBankPayment.IsBankPaymentRequest(isBankRemittancePaymentRequest);

                            callbackEntity.TransactionID = py.TransactionID;
                            callbackEntity.ServiceType = "İş Bankası Payment Response";
                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankRemittancePaymentRequestResponse, opt);
                            callbackEntity.IDCompany = companyIntegration.ID;
                            callbackEntity.TransactionType = "HAVALE PARA GONDERIMI SONUC";
                            _callbackResponseLogManager.Insert(callbackEntity);

                            if (isBankRemittancePaymentRequestResponse.Status == "OK")
                            {
                                entity.IsBankQueryNr = isBankRemittancePaymentRequestResponse.Data.data.query_number;
                                entity.SIDBank = idBank;
                                entity.CompanyBankAccountID = companyBankAccountID;
                                return Json(_manager.SetStatus(entity));
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
                            creditor_name = py.SenderName,
                            creditor_iban = py.Iban.Replace(" ", ""),
                            description = $"{py.Company} İade",
                            payment_reference_id = py.TransactionNr,
                            customer_ip = "89.252.138.236",
                        };

                        var isBankEftPaymentValidationRequestResponse = IsBankPaymentValidation.IsBankPaymentValidationRequest(isBankEftPaymentValidationRequest);

                        callbackEntity.TransactionID = py.TransactionID;
                        callbackEntity.ServiceType = "İş Bankası Payment Validation";
                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankEftPaymentValidationRequestResponse, opt);
                        callbackEntity.IDCompany = companyIntegration.ID;
                        callbackEntity.TransactionType = "EFT PARA GONDERIMI ON ONAY SONUC";
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
                                creditor_name = py.SenderName,
                                creditor_iban = py.Iban.Replace(" ", ""),
                                description = $"{py.Company} İade",
                                payment_reference_id = py.TransactionNr,
                                idempotency_key = py.ID,
                                query_number = isBankEftPaymentValidationRequestResponse.Data.data.query_number,
                                customer_ip = "89.252.138.236",
                            };

                            var isBankEftPaymentRequestResponse = IsBankPayment.IsBankPaymentRequest(isBankEftPaymentRequest);

                            callbackEntity.TransactionID = py.TransactionID;
                            callbackEntity.ServiceType = "İş Bankası Payment Response";
                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankEftPaymentRequestResponse, opt);
                            callbackEntity.IDCompany = companyIntegration.ID;
                            callbackEntity.TransactionType = "EFT PARA GONDERIMI SONUC";
                            _callbackResponseLogManager.Insert(callbackEntity);

                            if (isBankEftPaymentRequestResponse.Status == "OK")
                            {
                                entity.SIDBank = idBank;
                                entity.CompanyBankAccountID = companyBankAccountID;
                                entity.IsBankQueryNr = isBankEftPaymentRequestResponse.Data.data.query_number;
                                return Json(_manager.SetStatus(entity));
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
                                    creditor_name = py.SenderName,
                                    creditor_iban = py.Iban.Replace(" ", ""),
                                    description = $"{py.Company} İade",
                                    payment_reference_id = py.TransactionNr,
                                    customer_ip = "89.252.138.236",
                                };

                                var isBankFastPaymentValidationRequestResponse = IsBankPaymentValidation.IsBankPaymentValidationRequest(isBankFastPaymentValidationRequest);

                                callbackEntity.TransactionID = py.TransactionID;
                                callbackEntity.ServiceType = "İş Bankası Payment Validation";
                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankFastPaymentValidationRequestResponse, opt);
                                callbackEntity.IDCompany = companyIntegration.ID;
                                callbackEntity.TransactionType = "FAST PARA GONDERIMI ON ONAY SONUC";
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
                                        creditor_name = py.SenderName,
                                        creditor_iban = py.Iban.Replace(" ", ""),
                                        description = $"{py.Company} İade",
                                        payment_reference_id = py.TransactionNr,
                                        idempotency_key = py.ID,
                                        query_number = isBankEftPaymentValidationRequestResponse.Data.data.query_number,
                                        customer_ip = "89.252.138.236",
                                    };

                                    var isBankFastPaymentRequestResponse = IsBankPayment.IsBankPaymentRequest(isBankFastPaymentRequest);

                                    callbackEntity.TransactionID = py.TransactionID;
                                    callbackEntity.ServiceType = "İş Bankası Payment Response";
                                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankFastPaymentRequestResponse, opt);
                                    callbackEntity.IDCompany = companyIntegration.ID;
                                    callbackEntity.TransactionType = "FAST PARA GONDERIMI SONUC";
                                    _callbackResponseLogManager.Insert(callbackEntity);

                                    if (isBankFastPaymentRequestResponse.Status == "OK")
                                    {
                                        entity.SIDBank = idBank;
                                        entity.CompanyBankAccountID = companyBankAccountID;
                                        entity.IsBankQueryNr = isBankFastPaymentRequestResponse.Data.data.query_number;
                                        return Json(_manager.SetStatus(entity));
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
                        ReceiverIBAN = py.Iban.Replace(" ", ""),
                        ReceiverName = py.SenderName,
                        MoneyTransferDescription = $"{py.TransactionNr} - {py.Company} İade",
                        TransactionGuid = py.TransactionNr
                    };

                    var kuveytTurkTransferResponse = KuveytTurkMoneyTransfer.MoneyTranfer(kuveytTurkTransferRequestModel);

                    callbackEntity.TransactionID = py.TransactionID;
                    callbackEntity.ServiceType = "Kuveyt Turk Bankası Payment Response";
                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(kuveytTurkTransferResponse, opt);
                    callbackEntity.IDCompany = companyIntegration.ID;
                    callbackEntity.TransactionType = "PARA GONDERIMI SONUC";
                    _callbackResponseLogManager.Insert(callbackEntity);

                    entity.Status = (byte)Enums.StatusType.Process;
                    entity.Description = "İşleme Alındı";
                    entity.MDate = DateTime.Now;
                    entity.MUser = "00000000-0000-0000-0000-000000000000";
                    entity.SIDBank = idBank;
                    entity.CompanyBankAccountID = companyBankAccountID;
                    entity.IsBankQueryNr = kuveytTurkTransferResponse?.Data?.value?.moneyTransferTransactionId != null ? kuveytTurkTransferResponse?.Data?.value?.moneyTransferTransactionId.ToString() : "-";
                    return Json(_manager.SetStatus(entity));
                }

                else
                    return Json(new GenericResponse { Status = "ERROR", Message = "Para Gönderilecek Banka Seçiminde Bir Hata Mevcut. Lütfen Yetkiliniz İle İletişime Geçin" });
            }

            return Json(new GenericResponse { Status = "ERROR", Message = "Şu anda İşlem Gerçekleştirilemiyor. Lütfen Bizimle İletişime Geçin." });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RebateForFraud(string id)
        {
            var creditCardEntity = _creditCardPaymentNotificationManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, id)});

            var callbackEntity = new CallbackResponseLog();
            var opt = new JsonSerializerOptions() { WriteIndented = true };

            if(creditCardEntity != null)
            {
                var paymentInstitutions = _paymentInstitutionManager.GetList(null).FirstOrDefault(x => x.ID == creditCardEntity.CreditCardPaymentMethodID.ToString());

                if (paymentInstitutions != null)
                {
                    Type type = typeof(DealerRebateRequestController);
                    MethodInfo methodInfo = type.GetMethod(paymentInstitutions.RedirectToActionRebateMethod);

                    if (methodInfo != null)
                    {
                        GenericResponse result = (GenericResponse)methodInfo.Invoke(this, new object[] { creditCardEntity });

                        if (result.Status == "OK")
                        {
                            creditCardEntity.MDate = DateTime.Now;
                            creditCardEntity.MUser = IDUser;
                            creditCardEntity.Status = creditCardEntity.Status == (byte)Enums.StatusType.PayPool ? (byte)Enums.StatusType.Refunded : (byte)Enums.StatusType.Fraud;
                            creditCardEntity.Description = creditCardEntity.Status == (byte)Enums.StatusType.PayPool ? "İade Edildi" : "Fraud İadesi";

                            return Json(_creditCardPaymentNotificationManager.SetStatus(creditCardEntity));
                        }
                        else
                            return Json(result);
                    }
                }
            }
            else
            {
                var foreignCreditCardEntity = _foreignCreditCardPaymentNotificationManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, id) });

                callbackEntity = new CallbackResponseLog();
                opt = new JsonSerializerOptions() { WriteIndented = true };

                if (foreignCreditCardEntity != null)
                {
                    var paymentInstitutions = _paymentInstitutionManager.GetList(null).FirstOrDefault(x => x.ID == foreignCreditCardEntity.CreditCardPaymentMethodID.ToString());

                    if (paymentInstitutions != null)
                    {
                        Type type = typeof(DealerRebateRequestController);
                        MethodInfo methodInfo = type.GetMethod(paymentInstitutions.RedirectToActionRebateMethod);

                        if (methodInfo != null)
                        {
                            GenericResponse result = (GenericResponse)methodInfo.Invoke(this, new object[] { foreignCreditCardEntity });

                            if (result.Status == "OK")
                            {
                                foreignCreditCardEntity.MDate = DateTime.Now;
                                foreignCreditCardEntity.MUser = IDUser;
                                foreignCreditCardEntity.Status = foreignCreditCardEntity.Status == (byte)Enums.StatusType.PayPool ? (byte)Enums.StatusType.Refunded : (byte)Enums.StatusType.Fraud;
                                foreignCreditCardEntity.Description = foreignCreditCardEntity.Status == (byte)Enums.StatusType.PayPool ? "İade Edildi" : "Fraud İadesi";

                                return Json(_foreignCreditCardPaymentNotificationManager.SetStatus(foreignCreditCardEntity));
                            }
                            else
                                return Json(result);
                        }
                    }
                }
            }

            return Json(new GenericResponse { Status = "ERROR", Message = "Şu anda İşlem Gerçekleştirilemiyor. Lütfen Bizimle İletişime Geçin." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RebateForFraudWithoutAPI(string id)
        {
            var creditCardEntity = _creditCardPaymentNotificationManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, id) });

            var callbackEntity = new CallbackResponseLog();
            var opt = new JsonSerializerOptions() { WriteIndented = true };

            if (creditCardEntity != null)
            {
                creditCardEntity.MDate = DateTime.Now;
                creditCardEntity.MUser = IDUser;
                creditCardEntity.Description = creditCardEntity.Status == (byte)Enums.StatusType.PayPool ? "İade Edildi" : "Fraud İadesi";
                creditCardEntity.Status = creditCardEntity.Status == (byte)Enums.StatusType.PayPool ? (byte)Enums.StatusType.Refunded : (byte)Enums.StatusType.Fraud;

                return Json(_creditCardPaymentNotificationManager.SetStatus(creditCardEntity));
            }
            else
            {
                var foreignCreditCardEntity = _foreignCreditCardPaymentNotificationManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, id) });

                if(foreignCreditCardEntity != null)
                {
                    foreignCreditCardEntity.MDate = DateTime.Now;
                    foreignCreditCardEntity.MUser = IDUser;
                    foreignCreditCardEntity.Description = foreignCreditCardEntity.Status == (byte)Enums.StatusType.PayPool ? "İade Edildi" : "Fraud İadesi";
                    foreignCreditCardEntity.Status = foreignCreditCardEntity.Status == (byte)Enums.StatusType.PayPool ? (byte)Enums.StatusType.Refunded : (byte)Enums.StatusType.Fraud;

                    return Json(_foreignCreditCardPaymentNotificationManager.SetStatus(foreignCreditCardEntity));

                }
                else
                    return Json(new GenericResponse { Status = "ERROR", Message = "İşlem bulunamadı" });
            }

            //return Json(new GenericResponse { Status = "ERROR", Message = "Şu anda İşlem Gerçekleştirilemiyor. Lütfen Bizimle İletişime Geçin." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetStatusWithoutAPI(CompanyRebateRequest entity)
        {
            return Json(_manager.SetStatus(entity));
        }

        #region Credit Card Rebate Methods

        public GenericResponse ParamRefundMethod(CreditCardPaymentNotification creditCardEntity)
        {
            var callbackEntity = new CallbackResponseLog();
            var opt = new JsonSerializerOptions() { WriteIndented = true };

            var companyIntegration = _companyIntegrationManager.GetByServiceId(creditCardEntity.ServiceID);

            var turkPosWSTESTSoapClient = new TurkPosWSPRODSoapClient(TurkPosWSPRODSoapClient.EndpointConfiguration.TurkPos_x0020_WS_x0020_PRODSoap);
            var stws = new ST_WS_Guvenlik();

            if (creditCardEntity.ActionDate == DateTime.Now.Date)
            {
                var result = turkPosWSTESTSoapClient.TP_Islem_Iptal_Iade2Async(stws, "DEC15F33-FC05-405E-BD0F-489381CE9AC3", "IPTAL", creditCardEntity.TransactionID);

                callbackEntity.TransactionID = creditCardEntity.TransactionID;
                callbackEntity.ServiceType = "PARAM";
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(result, opt);
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.TransactionType = "KREDİ KARTI ÖDEMESİ İPTAL";
                _callbackResponseLogManager.Insert(callbackEntity);

                var s = Convert.ToInt32(result.Result.Sonuc);
                if (s > 0)
                    return new GenericResponse { Status = "OK" };

                else
                    return new GenericResponse { Status = "ERROR", Message = result.Result.Sonuc_Str };
            }
            else
            {
                var result = turkPosWSTESTSoapClient.TP_Islem_Iptal_Iade2Async(stws, "DEC15F33-FC05-405E-BD0F-489381CE9AC3", "IADE", creditCardEntity.TransactionID);

                callbackEntity.TransactionID = creditCardEntity.TransactionID;
                callbackEntity.ServiceType = "PARAM";
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(result, opt);
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.TransactionType = "KREDİ KARTI ÖDEMESİ İADE";
                _callbackResponseLogManager.Insert(callbackEntity);

                var s = Convert.ToInt32(result.Result.Sonuc);
                if (s > 0)
                    return new GenericResponse { Status = "OK" };

                else
                    return new GenericResponse { Status = "ERROR", Message = result.Result.Sonuc_Str };
            }
        }

        public GenericResponse PayNKolayRefundMethod(CreditCardPaymentNotification creditCardEntity)
        {
            var callbackEntity = new CallbackResponseLog();
            var opt = new JsonSerializerOptions() { WriteIndented = true };

            var companyIntegration = _companyIntegrationManager.GetByServiceId(creditCardEntity.ServiceID);

            var creditCardIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "PayNKolayCreditCard") });

            if (creditCardEntity.ActionDate == DateTime.Now.Date)
            {

                var cancelPaymentRequestModel = new CancelRefundPaymentRequestModel.CancelPaymentRequestModel
                {
                    Amount = creditCardEntity.Amount,
                    ReferenceCode = creditCardEntity.TransactionReferenceCode,
                    Sx = creditCardIntegrationValues.FirstOrDefault(f => f.ParamDef == "cancel_sx").ParamVal,
                    SecretKey = creditCardIntegrationValues.FirstOrDefault(f => f.ParamDef == "mercant_secret_key").ParamVal,
                };

                String str = cancelPaymentRequestModel.Sx + cancelPaymentRequestModel.ReferenceCode + cancelPaymentRequestModel.Type + creditCardEntity.Amount.ToString(CultureInfo.InvariantCulture) + cancelPaymentRequestModel.SecretKey;
                System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(str);
                byte[] hashingbytes = sha.ComputeHash(bytes);
                String hashData = Convert.ToBase64String(hashingbytes);
                cancelPaymentRequestModel.HashData = hashData;

                var cancelPaymentRequestResponse = CancelRefundPayment.CancelPaymentRequest(cancelPaymentRequestModel);

                callbackEntity.TransactionID = creditCardEntity.TransactionID;
                callbackEntity.ServiceType = "PayNKolay";
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(cancelPaymentRequestResponse, opt);
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.TransactionType = "KREDI KARTI ODEMESI IPTAL";
                _callbackResponseLogManager.Insert(callbackEntity);

                if (cancelPaymentRequestResponse.Status == "OK")
                {
                    if (cancelPaymentRequestResponse.Data.RESPONSE_CODE == 2)
                        return new GenericResponse { Status = "OK" };
                    else
                        return new GenericResponse { Status = "ERROR", Message = cancelPaymentRequestResponse.Data.RESPONSE_DATA };
                }
                else
                    return new GenericResponse { Status = "ERROR", Message = cancelPaymentRequestResponse.Message };

            }
            else
            {
                var refundPaymentRequestModel = new CancelRefundPaymentRequestModel.RefundRequestModel
                {
                    Amount = creditCardEntity.Amount,
                    ReferenceCode = creditCardEntity.TransactionReferenceCode,
                    TrxDate = creditCardEntity.CDate.ToString("yyyy.MM.dd"),
                    Sx = creditCardIntegrationValues.FirstOrDefault(f => f.ParamDef == "cancel_sx").ParamVal,
                    SecretKey = creditCardIntegrationValues.FirstOrDefault(f => f.ParamDef == "mercant_secret_key").ParamVal,
                };

                String str = refundPaymentRequestModel.Sx + refundPaymentRequestModel.ReferenceCode + refundPaymentRequestModel.Type + creditCardEntity.Amount.ToString(CultureInfo.InvariantCulture) + refundPaymentRequestModel.TrxDate + refundPaymentRequestModel.SecretKey;
                System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(str);
                byte[] hashingbytes = sha.ComputeHash(bytes);
                String hashData = Convert.ToBase64String(hashingbytes);
                refundPaymentRequestModel.HashData = hashData;

                var refundPaymentRequestResponse = CancelRefundPayment.RefundPaymentRequest(refundPaymentRequestModel);

                callbackEntity.TransactionID = creditCardEntity.TransactionID;
                callbackEntity.ServiceType = "PayNKolay";
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(refundPaymentRequestResponse, opt);
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.TransactionType = "KREDI KARTI ODEMESI IADE";
                _callbackResponseLogManager.Insert(callbackEntity);

                if (refundPaymentRequestResponse.Status == "OK")
                {
                    if (refundPaymentRequestResponse.Data.RESPONSE_CODE == 2)
                        return new GenericResponse { Status = "OK" };
                    else
                        return new GenericResponse { Status = "ERROR", Message = refundPaymentRequestResponse.Data.RESPONSE_DATA };
                }
                else
                    return new GenericResponse { Status = "ERROR", Message = refundPaymentRequestResponse.Message };
            }
        }

        public GenericResponse IsBankRefundMethod(CreditCardPaymentNotification creditCardEntity)
        {
            var isBankSanalPosIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "IsBankSanalPos") });

            var callbackEntity = new CallbackResponseLog();
            var opt = new JsonSerializerOptions() { WriteIndented = true };

            var companyIntegration = _companyIntegrationManager.GetByServiceId(creditCardEntity.ServiceID);

            if (creditCardEntity.ActionDate == DateTime.Now.Date)
            {
                var isBankSanalPOSCancelRequestModel = new IsBankSanalPOSCancelRequestModel()
                {
                    apiUserName = isBankSanalPosIntegrationValues.FirstOrDefault(f => f.ParamDef == "apiUserName").ParamVal,
                    apiUserPassword = isBankSanalPosIntegrationValues.FirstOrDefault(f => f.ParamDef == "apiUserPassword").ParamVal,
                    clientid = isBankSanalPosIntegrationValues.FirstOrDefault(f => f.ParamDef == "clientid").ParamVal,
                    oid = creditCardEntity.TransactionID
                };


                var isBankCancelPaymentRequestResponse = IsBankSanalPOSCancel.CancelRequestXML(isBankSanalPOSCancelRequestModel);

                callbackEntity.TransactionID = creditCardEntity.TransactionID;
                callbackEntity.ServiceType = "IsBankSanalPOS";
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankCancelPaymentRequestResponse, opt);
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.TransactionType = "KREDI KARTI ODEMESI IPTAL";
                _callbackResponseLogManager.Insert(callbackEntity);

                if (isBankCancelPaymentRequestResponse.Status == "OK" && isBankCancelPaymentRequestResponse.Data.Response == "Approved")
                    return new GenericResponse { Status = "OK" };

                else
                    return new GenericResponse { Status = "ERROR", Message = isBankCancelPaymentRequestResponse.Message };

            }
            else
            {
                var isBankRefundPaymentRequestModel = new IsBankSanalPOSRefundRequestModel()
                {
                    apiUserName = isBankSanalPosIntegrationValues.FirstOrDefault(f => f.ParamDef == "apiUserName").ParamVal,
                    apiUserPassword = isBankSanalPosIntegrationValues.FirstOrDefault(f => f.ParamDef == "apiUserPassword").ParamVal,
                    clientid = isBankSanalPosIntegrationValues.FirstOrDefault(f => f.ParamDef == "clientid").ParamVal,
                    oid = creditCardEntity.TransactionID
                };

                var isBankRefundPaymentRequestResponse = IsBankSanalPOSRefund.RefundRequestXML(isBankRefundPaymentRequestModel);

                callbackEntity.TransactionID = creditCardEntity.TransactionID;
                callbackEntity.ServiceType = "IsBankSanalPOS";
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankRefundPaymentRequestResponse, opt);
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.TransactionType = "KREDI KARTI ODEMESI IADE";
                _callbackResponseLogManager.Insert(callbackEntity);

                if (isBankRefundPaymentRequestResponse.Status == "OK" && isBankRefundPaymentRequestResponse.Data.Response == "Approved")
                {
                    return new GenericResponse { Status = "OK" };
                }
                else
                    return new GenericResponse { Status = "ERROR", Message = isBankRefundPaymentRequestResponse.Message };
            }
        }

        public GenericResponse PaybullRefundMethod(CreditCardPaymentNotification creditCardEntity)
        {
            var paybullSanalPOSIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "PaybullCreditCard") });

            var callbackEntity = new CallbackResponseLog();
            var opt = new JsonSerializerOptions() { WriteIndented = true };

            var companyIntegration = _companyIntegrationManager.GetByServiceId(creditCardEntity.ServiceID);

            var paybullRefundRequestModel = new PaybullRefundRequestModel()
            {
                invoice_id = creditCardEntity.TransactionID,
                amount = creditCardEntity.Amount
            };

            var hashKey = PaybullGenerateRefundHashKey.GenerateRefundHashKey(string.Empty, paybullRefundRequestModel.invoice_id, paybullSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "secret_key").ParamVal, paybullSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "app_password").ParamVal);

            paybullRefundRequestModel.hash_key = hashKey;

            var paybullRefundRequest = PaybullRefundRequest.RefundRequest(paybullRefundRequestModel);

            callbackEntity.TransactionID = creditCardEntity.TransactionID;
            callbackEntity.ServiceType = "Paybull";
            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(paybullRefundRequest, opt);
            callbackEntity.IDCompany = companyIntegration.ID;
            callbackEntity.TransactionType = "KREDI KARTI ODEMESI IADE";
            _callbackResponseLogManager.Insert(callbackEntity);

            if (paybullRefundRequest.Status == "OK" && paybullRefundRequest.Data != null)
            {
                return new GenericResponse { Status = "OK" };
            }
            else
                return new GenericResponse { Status = "ERROR", Message = paybullRefundRequest.Message };
        }

        public GenericResponse AKODERefundMethod(CreditCardPaymentNotification creditCardEntity)
        {
            var callbackEntity = new CallbackResponseLog();
            var opt = new JsonSerializerOptions() { WriteIndented = true };

            var companyIntegration = _companyIntegrationManager.GetByServiceId(creditCardEntity.ServiceID);

            var akOdeSanalPOSIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "AKODECreditCard") });

            if (creditCardEntity.ActionDate == DateTime.Now.Date)
            {
                var randomGenerator = new Random();
                var rnd = randomGenerator.Next(1, 1000000).ToString();

                var akOdeCancelRequestModel = new AKODECancelRequestModel()
                {
                    ApiUser = akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_user").ParamVal,
                    ClientId = akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "client_id").ParamVal,
                    OrderId = creditCardEntity.TransactionID,
                    Rnd = rnd,
                    TimeSpan = DateTime.Now.ToString("yyyyMMddHHmmss"),
                };

                akOdeCancelRequestModel.Hash = AKODECreateHash.CreateHash(akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_pass").ParamVal, akOdeCancelRequestModel.ClientId, akOdeCancelRequestModel.ApiUser, akOdeCancelRequestModel.Rnd, akOdeCancelRequestModel.TimeSpan);

                var akOdeCancelRequestResponse = AKODECancelRequest.CancelRequest(akOdeCancelRequestModel);

                callbackEntity.TransactionID = creditCardEntity.TransactionID;
                callbackEntity.ServiceType = "AKODE";
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(akOdeCancelRequestResponse, opt);
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.TransactionType = "KREDI KARTI ODEMESI IPTAL";
                _callbackResponseLogManager.Insert(callbackEntity);

                if (akOdeCancelRequestResponse.Status == "OK" && akOdeCancelRequestResponse.Data != null && akOdeCancelRequestResponse.Data.Code == 0)
                    return new GenericResponse { Status = "OK" };
                else
                    return new GenericResponse { Status = "ERROR", Message = akOdeCancelRequestResponse.Message ?? akOdeCancelRequestResponse.Data.Message };

            }
            else
            {
                var randomGenerator = new Random();
                var rnd = randomGenerator.Next(1, 1000000).ToString();

                var akOdeRefundRequestModel = new AKODERefundRequestModel()
                {
                    ApiUser = akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_user").ParamVal,
                    ClientId = akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "client_id").ParamVal,
                    OrderId = creditCardEntity.TransactionID,
                    Rnd = rnd,
                    TimeSpan = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Amount = (long)(creditCardEntity.Amount * 100),
                };

                akOdeRefundRequestModel.Hash = AKODECreateHash.CreateHash(akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_pass").ParamVal, akOdeRefundRequestModel.ClientId, akOdeRefundRequestModel.ApiUser, akOdeRefundRequestModel.Rnd, akOdeRefundRequestModel.TimeSpan);

                var akOdeRefundRequestResponse = AKODERefundRequest.RefundRequest(akOdeRefundRequestModel);

                callbackEntity.TransactionID = creditCardEntity.TransactionID;
                callbackEntity.ServiceType = "AKODE";
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(akOdeRefundRequestResponse, opt);
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.TransactionType = "KREDI KARTI ODEMESI IADE";
                _callbackResponseLogManager.Insert(callbackEntity);

                if (akOdeRefundRequestResponse.Status == "OK" && akOdeRefundRequestResponse.Data != null && akOdeRefundRequestResponse.Data.Code == 0)
                {
                    return new GenericResponse { Status = "OK" };
                }
                else
                    return new GenericResponse { Status = "ERROR", Message = akOdeRefundRequestResponse.Message ?? akOdeRefundRequestResponse.Data.Message };
            }
        }

        public GenericResponse ToslaRefundMethod(CreditCardPaymentNotification creditCardEntity)
        {
            var callbackEntity = new CallbackResponseLog();
            var opt = new JsonSerializerOptions() { WriteIndented = true };

            var companyIntegration = _companyIntegrationManager.GetByServiceId(creditCardEntity.ServiceID);

            var toslaIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "Tosla") });

            var toslaRefundRequestModel = new ToslaRefundRequestModel()
            {
                amount = creditCardEntity.Amount,
                companyId = int.Parse(toslaIntegrationValues.FirstOrDefault(x => x.ParamDef == "companyId").ParamVal),
                description = $"{creditCardEntity.TransactionNr} numaralı işlemin iptali",
                externalTransactionId = creditCardEntity.TransactionNr,
                processId = creditCardEntity.TransactionID,
                phoneNumber = "9" + creditCardEntity.Phone,
            };

            var toslaRefundRequestResponse = ToslaRefundRequest.RefundRequest(toslaRefundRequestModel);

            callbackEntity.TransactionID = creditCardEntity.TransactionID;
            callbackEntity.ServiceType = "TOSLA";
            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(toslaRefundRequestResponse, opt);
            callbackEntity.IDCompany = companyIntegration.ID;
            callbackEntity.TransactionType = "TOSLA ODEMESI IADE TALEBI";
            _callbackResponseLogManager.Insert(callbackEntity);

            if (toslaRefundRequestResponse != null && toslaRefundRequestResponse.Status == "OK" && toslaRefundRequestResponse.Data != null && toslaRefundRequestResponse.Data.CommandId != null)
            {
                var toslaExecudeCommandResponse = ToslaRefundExecuteRequest.RefundExecuteRequest(toslaRefundRequestResponse.Data.CommandId, toslaRefundRequestResponse.Data.Token);

                callbackEntity.TransactionID = creditCardEntity.TransactionID;
                callbackEntity.ServiceType = "TOSLA";
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(toslaExecudeCommandResponse, opt);
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.TransactionType = "TOSLA ODEMESI IADE ONAY";
                _callbackResponseLogManager.Insert(callbackEntity);

                if (toslaExecudeCommandResponse != null && toslaExecudeCommandResponse.Status == "OK")
                    return new GenericResponse { Status = "OK" };          
                else
                    return new GenericResponse { Status = "ERROR", Message = toslaRefundRequestResponse.Message ?? "İade Başarısız" };
            }
            else
                return new GenericResponse { Status = "ERROR", Message = toslaRefundRequestResponse.Message ?? "İade Başarısız" };
        }

        public GenericResponse EsnekPosRefundMethod(CreditCardPaymentNotification creditCardEntity)
        {
            var callbackEntity = new CallbackResponseLog();
            var opt = new JsonSerializerOptions() { WriteIndented = true };

            var companyIntegration = _companyIntegrationManager.GetByServiceId(creditCardEntity.ServiceID);

            var esnekPosIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "EsnekPos") });

            var esnekPosCancelAndRefundRequestModel = new EsnekPosCancelAndRefundRequestModel()
            {
                ORDER_REF_NUMBER = creditCardEntity.TransactionID,
                AMOUNT = creditCardEntity.Amount
            };

            var esnekPosCancelAndRefundRequestResponseModel = EsnekPosCancelAndRefundRequest.CancelAndRefundRequest(esnekPosCancelAndRefundRequestModel);

            callbackEntity.TransactionID = creditCardEntity.TransactionID;
            callbackEntity.ServiceType = "EsnekPos";
            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(esnekPosCancelAndRefundRequestResponseModel, opt);
            callbackEntity.IDCompany = companyIntegration.ID;
            callbackEntity.TransactionType = "KREDI KARTI ODEMESI IPTAL";
            _callbackResponseLogManager.Insert(callbackEntity);

            if (esnekPosCancelAndRefundRequestResponseModel.Status == "OK" && esnekPosCancelAndRefundRequestResponseModel.Data != null)
                return new GenericResponse { Status = "OK" };
            else
                return new GenericResponse { Status = "ERROR", Message = esnekPosCancelAndRefundRequestResponseModel.Message ?? esnekPosCancelAndRefundRequestResponseModel.Data.RETURN_MESSAGE };
        }

        public GenericResponse LidioPosRefundMethod(CreditCardPaymentNotification creditCardEntity)
        {
            var callbackEntity = new CallbackResponseLog();
            var opt = new JsonSerializerOptions() { WriteIndented = true };

            var companyIntegration = _companyIntegrationManager.GetByServiceId(creditCardEntity.ServiceID);

            var lidioPosIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "LidioPos") });

            if (creditCardEntity.ActionDate == DateTime.Now.Date)
            {
                var lidioPosCancelRequestModel = new LidioPosCancelRequestModel()
                {
                    orderId = creditCardEntity.TransactionID,
                    paymentInstrument = "NewCard"
                };

                var lidioPosCancelRequestResponseModel = LidioPosCancelRequest.CancelRequest(lidioPosCancelRequestModel);

                callbackEntity.TransactionID = creditCardEntity.TransactionID;
                callbackEntity.ServiceType = "LidioPos";
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(lidioPosCancelRequestResponseModel, opt);
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.TransactionType = "KREDI KARTI ODEMESI IPTAL";
                _callbackResponseLogManager.Insert(callbackEntity);

                if (lidioPosCancelRequestResponseModel.Status == "OK" && lidioPosCancelRequestResponseModel.Data != null)
                    return new GenericResponse { Status = "OK" };
                else
                    return new GenericResponse { Status = "ERROR", Message = lidioPosCancelRequestResponseModel.Message ?? lidioPosCancelRequestResponseModel.Data.ResultMessage };
            }
            else
            {
                var lidioPosRefundRequestModel = new LidioPosRefundRequestModel()
                {
                    orderId = creditCardEntity.TransactionID,
                    totalAmount = creditCardEntity.Amount,
                    currency = "TRY"
                };

                var lidioPosRefundRequestResponseModel = LidioPosRefundRequest.RefundRequest(lidioPosRefundRequestModel);

                callbackEntity.TransactionID = creditCardEntity.TransactionID;
                callbackEntity.ServiceType = "LidioPos";
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(lidioPosRefundRequestResponseModel, opt);
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.TransactionType = "KREDI KARTI ODEMESI IADE";
                _callbackResponseLogManager.Insert(callbackEntity);

                if (lidioPosRefundRequestResponseModel.Status == "OK" && lidioPosRefundRequestResponseModel.Data != null)
                    return new GenericResponse { Status = "OK" };
                else
                    return new GenericResponse { Status = "ERROR", Message = lidioPosRefundRequestResponseModel.Message ?? lidioPosRefundRequestResponseModel.Data.ResultMessage };
            }
        }

        public GenericResponse LidioPosForeignCardRefundMethod(ForeignCreditCardPaymentNotification foreignCreditCardEntity)
        {
            var callbackEntity = new CallbackResponseLog();
            var opt = new JsonSerializerOptions() { WriteIndented = true };

            var companyIntegration = _companyIntegrationManager.GetByServiceId(foreignCreditCardEntity.ServiceID);

            var lidioPosIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "LidioPosYD") });

            if (foreignCreditCardEntity.ActionDate == DateTime.Now.Date)
            {
                var lidioPosCancelRequestModel = new LidioPosCancelRequestModel()
                {
                    orderId = foreignCreditCardEntity.TransactionID,
                    paymentInstrument = "NewCard"
                };

                var lidioPosCancelRequestResponseModel = LidioPosCancelRequest.CancelRequest(lidioPosCancelRequestModel, true);

                callbackEntity.TransactionID = foreignCreditCardEntity.TransactionID;
                callbackEntity.ServiceType = "LidioPosYD";
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(lidioPosCancelRequestResponseModel, opt);
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.TransactionType = "KREDI KARTI ODEMESI IPTAL";
                _callbackResponseLogManager.Insert(callbackEntity);

                if (lidioPosCancelRequestResponseModel.Status == "OK" && lidioPosCancelRequestResponseModel.Data != null)
                    return new GenericResponse { Status = "OK" };
                else
                    return new GenericResponse { Status = "ERROR", Message = lidioPosCancelRequestResponseModel.Message ?? lidioPosCancelRequestResponseModel.Data.ResultMessage };
            }
            else
            {
                var lidioPosRefundRequestModel = new LidioPosRefundRequestModel()
                {
                    orderId = foreignCreditCardEntity.TransactionID,
                    totalAmount = foreignCreditCardEntity.Amount,
                    currency = foreignCreditCardEntity.CurrencyCode,
                };

                var lidioPosRefundRequestResponseModel = LidioPosRefundRequest.RefundRequest(lidioPosRefundRequestModel, true);

                callbackEntity.TransactionID = foreignCreditCardEntity.TransactionID;
                callbackEntity.ServiceType = "LidioPosYD";
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(lidioPosRefundRequestResponseModel, opt);
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.TransactionType = "KREDI KARTI ODEMESI IADE";
                _callbackResponseLogManager.Insert(callbackEntity);

                if (lidioPosRefundRequestResponseModel.Status == "OK" && lidioPosRefundRequestResponseModel.Data != null)
                    return new GenericResponse { Status = "OK" };
                else
                    return new GenericResponse { Status = "ERROR", Message = lidioPosRefundRequestResponseModel.Message ?? lidioPosRefundRequestResponseModel.Data.ResultMessage };
            }
        }

        public GenericResponse EfixPosRefundMethod(CreditCardPaymentNotification creditCardEntity)
        {
            var callbackEntity = new CallbackResponseLog();
            var opt = new JsonSerializerOptions() { WriteIndented = true };

            var companyIntegration = _companyIntegrationManager.GetByServiceId(creditCardEntity.ServiceID);

            if (creditCardEntity.ActionDate == DateTime.Now.Date)
            {
                var responseReverse = EfixPosRebateAndRefundRequest.CreateReverse(int.Parse(creditCardEntity.TransactionReferenceCode));

                callbackEntity.TransactionID = creditCardEntity.TransactionID;
                callbackEntity.ServiceType = "EfixPos";
                callbackEntity.Callback = JsonSerializer.Serialize(responseReverse, opt);
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.TransactionType = "KREDI KARTI ODEMESI IPTAL";
                _callbackResponseLogManager.Insert(callbackEntity);

                if (responseReverse.Status == "OK" && responseReverse.Data != null)
                    return new GenericResponse { Status = "OK" };
                else
                    return new GenericResponse { Status = "ERROR", Message = responseReverse.Message ?? responseReverse.Data.ErrorMessage };
            }
            else
            {
                var responseRefund = EfixPosRebateAndRefundRequest.CreateRefund(creditCardEntity.Amount, int.Parse(creditCardEntity.TransactionReferenceCode));

                callbackEntity.TransactionID = creditCardEntity.TransactionID;
                callbackEntity.ServiceType = "EfixPos";
                callbackEntity.Callback = JsonSerializer.Serialize(responseRefund, opt);
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.TransactionType = "KREDI KARTI ODEMESI IADE";
                _callbackResponseLogManager.Insert(callbackEntity);

                if (responseRefund.Status == "OK" && responseRefund.Data != null)
                    return new GenericResponse { Status = "OK" };
                else
                    return new GenericResponse { Status = "ERROR", Message = responseRefund.Message ?? responseRefund.Data.ErrorMessage };
            }
        }
        #endregion
    }
}
