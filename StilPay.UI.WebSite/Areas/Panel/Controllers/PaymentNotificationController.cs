using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ParamPosLiveReference;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.DAL.Concrete;
using StilPay.Entities;
using StilPay.Entities.Concrete;
using StilPay.UI.WebSite.Areas.Panel.Infrastructures;
using StilPay.UI.WebSite.Areas.Panel.Models;
using StilPay.Utility.AKODESanalPOS;
using StilPay.Utility.AKODESanalPOS.Models.AKODECreditCardInfo;
using StilPay.Utility.AKODESanalPOS.Models.AKODEGetSession;
using StilPay.Utility.AKODESanalPOS.Models.AKODEGetTransactions;
using StilPay.Utility.AKODESanalPOS.Models.AKODEPaymentRequest;
using StilPay.Utility.EfixPos;
using StilPay.Utility.EfixPos.Models.EfixGetTransaction;
using StilPay.Utility.EfixPos.Models.EfixPosAddTransactionDetail;
using StilPay.Utility.EfixPos.Models.EfixPosCheckout;
using StilPay.Utility.EsnekPos;
using StilPay.Utility.EsnekPos.Models.EsnekPosPaymentRequest;
using StilPay.Utility.EsnekPos.Models.EsnekPosThreeDResultResponse;
using StilPay.Utility.EsnekPos.Models.EsnekPosTransactionQuery;
using StilPay.Utility.Helper;
using StilPay.Utility.IsBankSanalPos;
using StilPay.Utility.IsBankSanalPos.IsBankPaymentModel;
using StilPay.Utility.LidioPos;
using StilPay.Utility.LidioPos.Models.LidioPosFinishPaymentRequestModel;
using StilPay.Utility.LidioPos.Models.LidioPosPaymentRequest;
using StilPay.Utility.LidioPos.Models.LidioPosTransactionQuery;
using StilPay.Utility.Models;
using StilPay.Utility.Paybull;
using StilPay.Utility.Paybull.PaybullCheckStatus;
using StilPay.Utility.Paybull.PaybullPayment;
using StilPay.Utility.PayNKolay;
using StilPay.Utility.PayNKolay.Models;
using StilPay.Utility.PayNKolay.Models.ComplatePayment;
using StilPay.Utility.PayNKolay.Models.PaymentRequest;
using StilPay.Utility.ToslaSanalPos;
using StilPay.Utility.ToslaSanalPos.Models.ToslaGetRefCode;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Xml.Linq;
using static StilPay.UI.WebSite.Areas.Panel.Models.PaymentNotificationViewModel;
using static StilPay.Utility.Helper.Enums;


namespace StilPay.UI.WebSite.Areas.Panel.Controllers
{
    [Area("Panel")]
    public class PaymentNotificationController : BaseController<PaymentNotification>
    {
        private readonly IPaymentNotificationManager _manager;
        private readonly IMemberManager _memberManager;
        private readonly ICompanyIntegrationManager _companyIntegration;
        private readonly ICompanyBankManager _companyBankManager;
        private readonly IBankManager _bankManager;
        private readonly ICreditCardPaymentNotificationManager _creditCardPaymentNotificationManager;
        private readonly ICompanyManager _companyManager;
        private readonly ICallbackResponseLogManager _callbackResponseLogManager;
        private readonly IForeignCreditCardPaymentNotificationManager _foreignCreditCardPaymentNotificationManager;
        private readonly IPaymentInstitutionManager _paymentInstitutionManager;
        private readonly ICompanyPaymentInstitutionManager _companyPaymentInstitutionManager;
        private readonly ICompanyBankAccountManager _companyBankAccountManager;
        private readonly ICurrencyManager _currencyManager;
        private readonly IMemberTypeManager _memberTypeManager;
        private readonly ICompanyFraudControlManager _companyFraudControlManager;
        private readonly IPaymentTransferPoolDescriptionControlManager _paymentTransferPoolDescriptionControlManager;
        private readonly TurkPosWSPRODSoapClient turkPosWSPRODSoapClient = new TurkPosWSPRODSoapClient(TurkPosWSPRODSoapClient.EndpointConfiguration.TurkPos_x0020_WS_x0020_PRODSoap);
        private readonly ST_WS_Guvenlik stws = new ST_WS_Guvenlik();
        private readonly SettingDAL _settingDAL = new SettingDAL();

        public PaymentNotificationController(IPaymentNotificationManager manager, IMemberManager memberManager, ICompanyIntegrationManager companyIntegration, ICompanyBankManager companyBankManager, IBankManager bankManager, ICreditCardPaymentNotificationManager creditCardPaymentNotificationManager,IHttpContextAccessor httpContext, ICallbackResponseLogManager callbackResponseLogManager, IForeignCreditCardPaymentNotificationManager foreignCreditCardPaymentNotificationManager, IPaymentInstitutionManager paymentInstitutionManager, ICompanyPaymentInstitutionManager companyPaymentInstitutionManager, ICompanyBankAccountManager companyBankAccountManager, ICurrencyManager currencyManager, ICompanyManager companyManager, IMemberTypeManager memberTypeManager, ICompanyFraudControlManager companyFraudControlManager, IPaymentTransferPoolDescriptionControlManager paymentTransferPoolDescriptionControlManager) : base(httpContext)
        {
            _manager = manager;
            _memberManager = memberManager;
            _companyIntegration = companyIntegration;
            _companyBankManager = companyBankManager;
            _bankManager = bankManager;
            _creditCardPaymentNotificationManager = creditCardPaymentNotificationManager;
            _companyManager = companyManager;
            _callbackResponseLogManager = callbackResponseLogManager;
            _foreignCreditCardPaymentNotificationManager = foreignCreditCardPaymentNotificationManager;
            _paymentInstitutionManager = paymentInstitutionManager;
            _companyPaymentInstitutionManager = companyPaymentInstitutionManager;
            _companyBankAccountManager = companyBankAccountManager;
            _currencyManager = currencyManager;
            _memberTypeManager = memberTypeManager;
            _companyFraudControlManager = companyFraudControlManager;
            _paymentTransferPoolDescriptionControlManager = paymentTransferPoolDescriptionControlManager;
        }

        public override IBaseBLL<PaymentNotification> Manager()
        {
            return _manager;
        }

        [HttpGet]
        public IActionResult Index(string service_id, string frame_id)
        {
            try
            {
                var integration = _companyIntegration.GetByServiceId(service_id);

                if (integration != null)
                {
                    var json = tMD5Manager.Decrypt(integration.SecretKey, frame_id);
                    var data = JsonConvert.DeserializeAnonymousType(json, new { transaction_id = "", amount = 0.0m, creation_time = DateTime.Now, payment_method_type_id = "", currencyCode = "", redirectUrl = "" });

                    if (!string.IsNullOrEmpty(data.transaction_id) && data.creation_time > DateTime.Now.AddSeconds(-50))
                    {
                        var model = new PaymentNotificationViewModel();
                        model.entity.ServiceID = service_id;
                        model.entity.TransactionID = data.transaction_id;
                        model.entity.ActionDate = DateTime.Now;
                        model.entity.ActionTime = DateTime.Now.ToString("HH:mm");
                        model.entity.Amount = data.amount;
                        model.entity.Status = (byte)Enums.StatusType.Pending;
                        model.RedirectUrl = data.redirectUrl ?? integration.RedirectUrl;
                        model.CurrencyCode = data.currencyCode ?? "TRY";
                        model.PaymentMethodID = data.payment_method_type_id;

                        if(model.CurrencyCode != "TRY")
                        {
                            model.IsForeignCreditCard = true;
                            var currency = _currencyManager.GetList(new List<FieldParameter> { new FieldParameter("CurrencyCode", Enums.FieldType.NVarChar, model.CurrencyCode) }).FirstOrDefault();

                            if(currency != null)
                            {
                                model.CurrencyName = currency.CurrencyName;
                                model.CurrencySymbol = currency.CurrencySymbol;
                            }
                        }

                        var systemSetting = _settingDAL.GetList(null).Where(x => x.ParamType == "IFrame" && x.ActivatedForGeneralUse).ToList();

                        if (systemSetting != null && systemSetting.Count > 0)
                        {
                            foreach (var item in systemSetting)
                            {
                                switch (item.ParamDef)
                                {
                                    case "CreditCard":
                                        model.ActivePaymentMethodModel.CreditCardBeUsed = integration.CreditCardBeUsed; break;

                                    case "ForeignCreditCard":
                                        model.ActivePaymentMethodModel.ForeignCreditCardBeUsed = integration.ForeignCreditCardBeUsed; break;

                                    case "Remittance":
                                        model.ActivePaymentMethodModel.TransferBeUsed = integration.TransferBeUsed; break;
                                }
                            }
                        }

                        //var companyPaymentInstitution = _companyPaymentInstitutionManager.GetSingle(new List<FieldParameter>()
                        //{
                        //    new FieldParameter("ID", FieldType.NVarChar, integration.ID),
                        //    //new FieldParameter("PaymentInstitutionID", FieldType.NVarChar, data.payment_method_type_id)
                        //});

                        //if (companyPaymentInstitution != null && companyPaymentInstitution.IsActive)
                        //{                           
                        //    model.CreditCardPaymentMethodID = Convert.ToByte(companyPaymentInstitution.PaymentInstitutionID);
                        //    model.CreditCardRedirectToActionGetThreeDView = companyPaymentInstitution.RedirectToActionGetThreeDView;
                        //    model.CreditCardRedirectToActionPaymentMethod = companyPaymentInstitution.RedirectToActionPaymentMethod;
                        //}
                        //else
                        //{
                        //    // Ödeme kuruluşlarını al ve aktif olanları filtrele
                        //    var paymentInstitutions = _paymentInstitutionManager.GetList(null).Where(x => x.IsActive).ToList();

                        //    if (paymentInstitutions.Count >= 2)
                        //    {
                        //        PaymentInstitution currentInstitution = null;

                        //        // Mevcut aktif ödeme kuruluşunu belirle
                        //        foreach (var institution in paymentInstitutions)
                        //        {
                        //            if (institution.CurrentTransactionCount < institution.ConsecutiveTransactionLimit)
                        //            {
                        //                currentInstitution = institution;
                        //                break;
                        //            }
                        //        }

                        //        // Eğer tüm kurumların işlem sayısı sınırı dolmuşsa sıfırla
                        //        if (currentInstitution == null)
                        //        {
                        //            foreach (var institution in paymentInstitutions)
                        //            {
                        //                institution.CurrentTransactionCount = 0;
                        //                _paymentInstitutionManager.Update(institution);
                        //            }
                        //            currentInstitution = paymentInstitutions.First();
                        //        }


                        //        // İşlemi ilgili ödeme kuruluşuna yönlendir
                        //        model.CreditCardPaymentMethodID = Convert.ToByte(currentInstitution.ID);
                        //        model.CreditCardRedirectToActionGetThreeDView = currentInstitution.RedirectToActionGetThreeDView;
                        //        model.CreditCardRedirectToActionPaymentMethod = currentInstitution.RedirectToActionPaymentMethod;

                        //        // İşlem sayısını artır
                        //        currentInstitution.CurrentTransactionCount++;
                        //        _paymentInstitutionManager.Update(currentInstitution);

                        //    }
                        //    else
                        //    {
                        //        var paymentInstitution = _paymentInstitutionManager.GetList(null).FirstOrDefault(x => x.IsActive);

                        //        if (paymentInstitution != null)
                        //        {
                        //            model.CreditCardPaymentMethodID = Convert.ToByte(paymentInstitution.ID);
                        //            model.CreditCardRedirectToActionGetThreeDView = paymentInstitution.RedirectToActionGetThreeDView;
                        //            model.CreditCardRedirectToActionPaymentMethod = paymentInstitution.RedirectToActionPaymentMethod;
                        //        }
                        //        else
                        //            return View("NotAllowed");
                        //    }
                        //}

                        _httpContext.HttpContext.Session.Write("Payment_Notification", model);

                        return View(model);
                    }
                }
            }
            catch { }

            return View("NotAllowed");
        }

        [HttpGet]
        public IActionResult Transfer(string service_id, string frame_id)
        {
            try
            {
                if (service_id == null || frame_id == null)
                {
                    var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                    if (model != null && model.entity != null)
                    {
                        var integration = _companyIntegration.GetByServiceId(model.entity.ServiceID);

                        model.CompanyBanks = _companyBankManager.GetActiveList(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, integration.ID) }).ToList();

                        if (model.CompanyBanks == null || model.CompanyBanks.Count() == 0 )
                        {
                            model.CompanyBanks = _companyBankManager.GetList(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, integration.ID) }).ToList();
                        }
                        //if (_bankManager.GetBanksForIframeSetting().Any(x => x.ActivatedForGeneralUse))
                        //{
                        //    var systemBankListIds = _bankManager.GetBanksForIframeSetting().Where(x => x.ActivatedForGeneralUse).Select(s => s.ID).ToList();

                        //    model.CompanyBanks = _companyBankManager.GetList(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, integration.ID) }).Where(x => systemBankListIds.Contains(x.IDBank)).ToList();
                        //}
                        //else
                        //    return View("NotAllowed");

                        return View(model);
                    }
                }
                else
                {
                    var integration = _companyIntegration.GetByServiceId(service_id);

                    if (integration != null)
                    {
                        if (!integration.TransferBeUsed)
                            return View("PaymentMethodDeactive");

                        var json = tMD5Manager.Decrypt(integration.SecretKey, frame_id);
                        var data = JsonConvert.DeserializeAnonymousType(json, new { transaction_id = "", amount = 0.0m, creation_time = DateTime.Now, redirectUrl = "" });

                        if (!string.IsNullOrEmpty(data.transaction_id) && data.creation_time > DateTime.Now.AddSeconds(-50))
                        {
                            var model = new PaymentNotificationViewModel();
                            model.entity.ServiceID = service_id;
                            model.entity.TransactionID = data.transaction_id;
                            model.entity.ActionDate = DateTime.Now;
                            model.entity.ActionTime = DateTime.Now.ToString("HH:mm");
                            model.entity.Amount = data.amount;
                            model.entity.Status = (byte)Enums.StatusType.Pending;
                            model.RedirectUrl = data.redirectUrl ?? integration.RedirectUrl;

                            var systemSetting = _settingDAL.GetList(null).Where(x => x.ParamType == "IFrame" && x.ParamDef == "Remittance" && x.ActivatedForGeneralUse).FirstOrDefault();

                            if (systemSetting != null || integration.TransferBeUsed)
                            {
                                model.CompanyBanks = _companyBankManager.GetActiveList(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, integration.ID) }).ToList();


                                if (model.CompanyBanks == null || model.CompanyBanks.Count() == 0)
                                {
                                    model.CompanyBanks = _companyBankManager.GetList(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, integration.ID) }).ToList();
                                }
                                //if (_bankManager.GetBanksForIframeSetting().Any(x => x.ActivatedForGeneralUse))
                                //{
                                //    var systemBankListIds = _bankManager.GetBanksForIframeSetting().Where(x => x.ActivatedForGeneralUse).Select(s => s.ID).ToList();

                                //    model.CompanyBanks = _companyBankManager.GetList(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, integration.ID) }).Where(x => systemBankListIds.Contains(x.IDBank)).ToList();
                                //}
                                //else
                                //    return View("NotAllowed");

                                _httpContext.HttpContext.Session.Write("Payment_Notification", model);
                                return View(model);
                            }
                            else
                                return View("NotAllowed");
                        }
                    }
                    else
                        return View("NotAllowed");
                }

            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Transfer(PaymentNotification entity)
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                model.entity.Phone = "0" + entity.Phone;
                model.entity.IDBank = entity.IDBank;
                model.entity.ActionDate = entity.ActionDate;
                model.entity.ActionTime = entity.ActionTime;
                model.entity.SenderName = entity.SenderName;
                model.entity.SenderIdentityNr = entity.SenderIdentityNr;
                model.entity.CompanyBankAccountID = entity.CompanyBankAccountID;
                model.LastTime = DateTime.Now;
                _httpContext.HttpContext.Session.Write("Payment_Notification", model);

                return Json(new GenericResponse { Status = "OK" });

            }
            catch { }

            return Json(new GenericResponse { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." });
        }

        [HttpGet]
        public IActionResult CreditCard(string service_id, string frame_id)
        {
            try
            {
                if (service_id == null && frame_id == null)
                {
                    var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                    if (model != null && model.entity != null)
                    {
                        return View(model);
                    }
                }
                else
                {
                    var integration = _companyIntegration.GetByServiceId(service_id);

                    if (integration != null)
                    {
                        if (!integration.CreditCardBeUsed)
                            return View("PaymentMethodDeactive");

                        var json = tMD5Manager.Decrypt(integration.SecretKey, frame_id);
                        var data = JsonConvert.DeserializeAnonymousType(json, new { transaction_id = "", amount = 0.0m, creation_time = DateTime.Now, payment_method_type_id = "", redirectUrl = "" });

                        if (!string.IsNullOrEmpty(data.transaction_id) && data.creation_time > DateTime.Now.AddSeconds(-50))
                        {
                            var model = new PaymentNotificationViewModel();
                            model.entity.ServiceID = service_id;
                            model.entity.TransactionID = data.transaction_id;
                            model.entity.ActionDate = DateTime.Now;
                            model.entity.ActionTime = DateTime.Now.ToString("HH:mm");
                            model.entity.Amount = data.amount;
                            model.entity.Status = (byte)Enums.StatusType.Pending;
                            model.RedirectUrl = data.redirectUrl ?? integration.RedirectUrl;
                            model.PaymentMethodID = data.payment_method_type_id;

                            //var companyPaymentInstitution = _companyPaymentInstitutionManager.GetSingle(new List<FieldParameter>()
                            //{
                            //    new FieldParameter("ID", FieldType.NVarChar, integration.ID),
                            //    //new FieldParameter("PaymentInstitutionID", FieldType.NVarChar, data.payment_method_type_id)
                            //});

                            //if (companyPaymentInstitution != null && companyPaymentInstitution.IsActive)
                            //{
                            //    model.CreditCardPaymentMethodID = Convert.ToByte(companyPaymentInstitution.PaymentInstitutionID);
                            //    model.CreditCardRedirectToActionGetThreeDView = companyPaymentInstitution.RedirectToActionGetThreeDView;
                            //    model.CreditCardRedirectToActionPaymentMethod = companyPaymentInstitution.RedirectToActionPaymentMethod;
                            //}
                            //else
                            //{
                            //    // Ödeme kuruluşlarını al ve aktif olanları filtrele
                            //    var paymentInstitutions = _paymentInstitutionManager.GetList(null).Where(x => x.IsActive).ToList();

                            //    if (paymentInstitutions.Count >= 2)
                            //    {
                            //        PaymentInstitution currentInstitution = null;

                            //        // Mevcut aktif ödeme kuruluşunu belirle
                            //        foreach (var institution in paymentInstitutions)
                            //        {
                            //            if (institution.CurrentTransactionCount < institution.ConsecutiveTransactionLimit)
                            //            {
                            //                currentInstitution = institution;
                            //                break;
                            //            }
                            //        }

                            //        // Eğer tüm kurumların işlem sayısı sınırı dolmuşsa sıfırla
                            //        if (currentInstitution == null)
                            //        {
                            //            foreach (var institution in paymentInstitutions)
                            //            {
                            //                institution.CurrentTransactionCount = 0;
                            //                _paymentInstitutionManager.Update(institution);
                            //            }
                            //            currentInstitution = paymentInstitutions.First();
                            //        }


                            //        // İşlemi ilgili ödeme kuruluşuna yönlendir
                            //        model.CreditCardPaymentMethodID = Convert.ToByte(currentInstitution.ID);
                            //        model.CreditCardRedirectToActionGetThreeDView = currentInstitution.RedirectToActionGetThreeDView;
                            //        model.CreditCardRedirectToActionPaymentMethod = currentInstitution.RedirectToActionPaymentMethod;

                            //        // İşlem sayısını artır
                            //        currentInstitution.CurrentTransactionCount++;
                            //        _paymentInstitutionManager.Update(currentInstitution);

                            //    }
                            //    else
                            //    {
                            //        var paymentInstitution = _paymentInstitutionManager.GetList(null).FirstOrDefault(x => x.IsActive);

                            //        if (paymentInstitution != null)
                            //        {
                            //            model.CreditCardPaymentMethodID = Convert.ToByte(paymentInstitution.ID);
                            //            model.CreditCardRedirectToActionGetThreeDView = paymentInstitution.RedirectToActionGetThreeDView;
                            //            model.CreditCardRedirectToActionPaymentMethod = paymentInstitution.RedirectToActionPaymentMethod;
                            //        }
                            //        else
                            //            return View("NotAllowed");
                            //    }
                            //}

                            _httpContext.HttpContext.Session.Write("Payment_Notification", model);
                            return View(model);

                        }
                    }
                    else
                        return View("NotAllowed");

                }
            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreditCard(PaymentNotificationViewModel paymentNotificationViewModel)
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                var integration = _companyIntegration.GetByServiceId(model.entity.ServiceID);
                model.IsForeignCreditCard = false;
                var hasEntity = _creditCardPaymentNotificationManager.GetSingleByTransactionID(model.entity.TransactionID);

                if(hasEntity != null)
                {
                    ContentResult result = new ContentResult
                    {
                        Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{model.RedirectUrl}"),
                        ContentType = "text/html"
                    };

                    return Json(new GenericResponse { Status = "ERROR", Data = model.RedirectUrl, Message = "hasEntity" });
                }

                model.CreditCardModel = paymentNotificationViewModel.CreditCardModel;
                model.CreditCardModel.PhoneNumber = paymentNotificationViewModel.CreditCardModel.CountryCode + paymentNotificationViewModel.CreditCardModel.PhoneNumber;
                model.entity.Phone = model.CreditCardModel.PhoneNumber;

                var member = _memberManager.GetMember(model.CreditCardModel.PhoneNumber);
                if (member != null)
                {
                    model.entity.IDMember = member.ID;
                    model.entity.SenderIdentityNr = member.IdentityNr;
                    //model.entity.Phone = member.Phone;
                }

                if (model.CreditCardModel.InstallmentMonth == null)
                    return Json(new GenericResponse { Status = "ERROR", Message = "Lütfen kredi kartı bilgilerini kontrol ediniz.." });

                var callbackEntity = new CallbackResponseLog();
                var opt = new JsonSerializerOptions() { WriteIndented = true };

                // KREDİ KARTI BIN NUMARASINA GÖRE KART TİPİ SORGUSU 
                var lidio = _paymentInstitutionManager.GetList(null).FirstOrDefault(x => x.ID == Convert.ToString((int)CreditCardPaymentMethodType.LidioPos));

                if (model.CreditCardPaymentMethodID == (int)CreditCardPaymentMethodType.EfixPos)
                    model.CreditCardModel.CardTypeId = (int)CreditCardType.CreditCard;

                else
                {

                    var lidioErr = false;

                    if (lidio != null && lidio.IsActive)
                    {
                        var lidioPosBinQueryRequestResponseModel = LidioPosBinQueryRequest.BinQueryRequest(model.CreditCardModel.CardNumber.Replace(" ", "")[..6]);

                        callbackEntity = new CallbackResponseLog();
                        opt = new JsonSerializerOptions() { WriteIndented = true };

                        callbackEntity.TransactionID = model.entity.TransactionID;
                        callbackEntity.ServiceType = "LidioPos";
                        callbackEntity.IDCompany = integration.ID;
                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(lidioPosBinQueryRequestResponseModel, opt);
                        callbackEntity.TransactionType = "KREDI KARTI BIN QUERY RESPONSE";
                        _callbackResponseLogManager.Insert(callbackEntity);

                        if (lidioPosBinQueryRequestResponseModel.Status == "OK" && lidioPosBinQueryRequestResponseModel.Data != null && lidioPosBinQueryRequestResponseModel.Data.result == "Success")
                        {
                            model.CreditCardModel.CardTypeId = lidioPosBinQueryRequestResponseModel.Data.isDebitCard ? (int)CreditCardType.BankCard : (int)CreditCardType.CreditCard;
                            model.CreditCardModel.CardBankId = lidioPosBinQueryRequestResponseModel.Data.bankCode;
                        }
                        else if(lidioPosBinQueryRequestResponseModel.Status == "OK" && lidioPosBinQueryRequestResponseModel.Data != null && lidioPosBinQueryRequestResponseModel.Data.result == "NotFound")
                        {
                            //return Json(new GenericResponse { Status = "ERROR", Message = "Lütfen TR kartı ile ödeme yapınız" });

                            model.CreditCardModel.CardTypeId =  (int)CreditCardType.CreditCard;

                        }
                        else
                            lidioErr = true;
                    }

                    if(lidio == null || !lidio.IsActive || lidioErr)
                    {
                        //ilk esnekposa sorgu atılıyor

                        var esnekPosBinQueryRequestResponseModel = EsnekPosBinQueryRequest.BinQueryRequest(model.CreditCardModel.CardNumber.Replace(" ", "")[..6]);
                        callbackEntity = new CallbackResponseLog();
                        opt = new JsonSerializerOptions() { WriteIndented = true };

                        callbackEntity.TransactionID = model.entity.TransactionID;
                        callbackEntity.ServiceType = "EsnekPos";
                        callbackEntity.IDCompany = integration.ID;
                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(esnekPosBinQueryRequestResponseModel, opt);
                        callbackEntity.TransactionType = "KREDI KARTI BIN QUERY RESPONSE";
                        _callbackResponseLogManager.Insert(callbackEntity);

                        if (esnekPosBinQueryRequestResponseModel.Status == "OK" && esnekPosBinQueryRequestResponseModel.Data != null && esnekPosBinQueryRequestResponseModel.Data.Card_Type != null && !string.IsNullOrEmpty(esnekPosBinQueryRequestResponseModel.Data.Card_Type))
                        {
                            model.CreditCardModel.CardTypeId = esnekPosBinQueryRequestResponseModel.Data.Card_Type == "CREDIT" ? (int)CreditCardType.CreditCard : (int)CreditCardType.BankCard;
                            model.CreditCardModel.CardBankId = esnekPosBinQueryRequestResponseModel.Data.Bank_Code;
                        }
                        else
                        {
                            // Esnekpos olumsuz döndüğü veya kart bilgisi dönmediği durumda aködeye istek atıyoruz.

                            var akOdeSanalPOSIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "AKODECreditCard") });

                            var randomGeneratorBin = new Random();
                            var rndBin = randomGeneratorBin.Next(1, 1000000).ToString();
                            var akOdeCreditCardInfoRequestModel = new AKODECreditCardInfoRequestModel()
                            {
                                ApiUser = akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_user").ParamVal,
                                ClientId = akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "client_id").ParamVal,
                                TimeSpan = DateTime.Now.ToString("yyyyMMddHHmmss"),
                                Rnd = rndBin,
                                Bin = int.Parse(model.CreditCardModel.CardNumber.Replace(" ", "")[..6])
                            };

                            akOdeCreditCardInfoRequestModel.Hash = AKODECreateHash.CreateHash(akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_pass").ParamVal, akOdeCreditCardInfoRequestModel.ClientId, akOdeCreditCardInfoRequestModel.ApiUser, akOdeCreditCardInfoRequestModel.Rnd, akOdeCreditCardInfoRequestModel.TimeSpan);

                            var akOdeCreditCardInfoResponse = AKODECreditCardInfoRequest.CreditCardInfoRequest(akOdeCreditCardInfoRequestModel);

                            callbackEntity = new CallbackResponseLog();
                            opt = new JsonSerializerOptions() { WriteIndented = true };

                            callbackEntity.TransactionID = model.entity.TransactionID;
                            callbackEntity.ServiceType = "AKODE";
                            callbackEntity.IDCompany = integration.ID;
                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(akOdeCreditCardInfoResponse, opt);
                            callbackEntity.TransactionType = "KREDI KARTI BIN QUERY RESPONSE";
                            _callbackResponseLogManager.Insert(callbackEntity);

                            if (akOdeCreditCardInfoResponse.Status == "OK" && akOdeCreditCardInfoResponse.Data != null)
                            {
                                model.CreditCardModel.CardTypeId = akOdeCreditCardInfoResponse.Data.CardClass == "Kredi Kartı" ? (int)CreditCardType.CreditCard : (int)CreditCardType.BankCard;          
                                model.CreditCardModel.CardBankId = akOdeCreditCardInfoResponse.Data.BankCode;
                            }
                            else
                                model.CreditCardModel.CardTypeId = (int)CreditCardType.CreditCard;
                        }
                    }
                }



                // KREDİ KARTI BIN NUMARASINA GÖRE KART TİPİ SORGUSU SON

                if (!string.IsNullOrEmpty(model.PaymentMethodID) || !string.IsNullOrWhiteSpace(model.PaymentMethodID))
                {
                    var paymentInstitution = _paymentInstitutionManager.GetList(null).FirstOrDefault(x => x.ID == model.PaymentMethodID);

                    model.CreditCardPaymentMethodID = Convert.ToByte(paymentInstitution.ID);
                    model.CreditCardRedirectToActionGetThreeDView = paymentInstitution.RedirectToActionGetThreeDView;
                    model.CreditCardRedirectToActionPaymentMethod = paymentInstitution.RedirectToActionPaymentMethod;
                }
                else
                {
                    if ((model.CreditCardModel.CardBankId == "010" || model.CreditCardModel.CardBankId == "0010" || model.CreditCardModel.CardBankId == "10") && lidio != null && lidio.IsActive)
                    {
                        model.CreditCardPaymentMethodID = Convert.ToByte(lidio.ID);
                        model.CreditCardRedirectToActionGetThreeDView = lidio.RedirectToActionGetThreeDView;
                        model.CreditCardRedirectToActionPaymentMethod = lidio.RedirectToActionPaymentMethod;
                    }
                    else
                    {
                        var companyPaymentInstitution = _companyPaymentInstitutionManager.GetSingle(new List<FieldParameter>()
                        {
                            new FieldParameter("ID", FieldType.NVarChar, integration.ID),
                            //new FieldParameter("PaymentInstitutionID", FieldType.NVarChar, data.payment_method_type_id)
                        });

                        if (companyPaymentInstitution != null && companyPaymentInstitution.IsActive)
                        {
                            model.CreditCardPaymentMethodID = Convert.ToByte(companyPaymentInstitution.PaymentInstitutionID);
                            model.CreditCardRedirectToActionGetThreeDView = companyPaymentInstitution.RedirectToActionGetThreeDView;
                            model.CreditCardRedirectToActionPaymentMethod = companyPaymentInstitution.RedirectToActionPaymentMethod;
                        }
                        else
                        {
                            // Ödeme kuruluşlarını al ve aktif olanları filtrele
                            var paymentInstitutions = _paymentInstitutionManager.GetList(null).Where(x => x.IsActive && !x.UseForForeignCard).ToList();

                            if (paymentInstitutions.Count >= 2)
                            {
                                PaymentInstitution currentInstitution = null;

                                // Mevcut aktif ödeme kuruluşunu belirle
                                foreach (var institution in paymentInstitutions)
                                {
                                    if (institution.CurrentTransactionCount < institution.ConsecutiveTransactionLimit)
                                    {
                                        currentInstitution = institution;
                                        break;
                                    }
                                }

                                // Eğer tüm kurumların işlem sayısı sınırı dolmuşsa sıfırla
                                if (currentInstitution == null)
                                {
                                    foreach (var institution in paymentInstitutions)
                                    {
                                        institution.CurrentTransactionCount = 0;
                                        _paymentInstitutionManager.Update(institution);
                                    }
                                    currentInstitution = paymentInstitutions.First();
                                }


                                // İşlemi ilgili ödeme kuruluşuna yönlendir
                                model.CreditCardPaymentMethodID = Convert.ToByte(currentInstitution.ID);
                                model.CreditCardRedirectToActionGetThreeDView = currentInstitution.RedirectToActionGetThreeDView;
                                model.CreditCardRedirectToActionPaymentMethod = currentInstitution.RedirectToActionPaymentMethod;

                                // İşlem sayısını artır
                                currentInstitution.CurrentTransactionCount++;
                                _paymentInstitutionManager.Update(currentInstitution);

                            }
                            else
                            {
                                var paymentInstitution = _paymentInstitutionManager.GetList(null).FirstOrDefault(x => x.IsActive && !x.UseForForeignCard);

                                if (paymentInstitution != null)
                                {
                                    model.CreditCardPaymentMethodID = Convert.ToByte(paymentInstitution.ID);
                                    model.CreditCardRedirectToActionGetThreeDView = paymentInstitution.RedirectToActionGetThreeDView;
                                    model.CreditCardRedirectToActionPaymentMethod = paymentInstitution.RedirectToActionPaymentMethod;
                                }
                                else
                                    return View("NotAllowed");
                            }
                        }
                    }
                }
              
                _httpContext.HttpContext.Session.Write("Payment_Notification", model);

                Type type = typeof(PaymentNotificationController);
                MethodInfo methodInfo = type.GetMethod(model.CreditCardRedirectToActionPaymentMethod);

                if (methodInfo != null)
                {
                    object result = methodInfo.Invoke(this, null);
                    return Json(result);
                }
            }
            catch { }

            return Json(new GenericResponse { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." });
        }

        #region Credit Card General Installment Options

        [HttpGet]
        public IActionResult InstallmentOptions(string creditCardNumber)
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");

                model.PayNKolayEncodedValues.Clear();

                #region Old Code
                //if (model.CreditCardPaymentMethodModel.Param)
                //{
                //    var bank = turkPosWSPRODSoapClient.BIN_SanalPosAsync(stws, creditCardNumber);
                //    var bankResultStr = bank.Result.Sonuc_Str;
                //    var bankResultList = bank.Result.DT_Bilgi.Any1.InnerXml;
                //    XmlDocument bankDoc = new XmlDocument();
                //    bankDoc.LoadXml(bankResultList);
                //    XmlNodeList bankNode = bankDoc.GetElementsByTagName("SanalPOS_ID");
                //    string bankSanalPosId = bankNode[0].InnerText;

                //    var ozelOranList = turkPosWSPRODSoapClient.TP_Ozel_Oran_ListeAsync(stws, "DEC15F33-FC05-405E-BD0F-489381CE9AC3");
                //    var s = Convert.ToInt32(ozelOranList.Result.Sonuc);
                //    if (s > 0)
                //    {
                //        var list = ozelOranList.Result.DT_Bilgi.Any1.InnerXml;
                //        XmlDocument doc = new XmlDocument();
                //        doc.LoadXml(list);
                //        var xpath = String.Format(@"//DT_Ozel_Oranlar[./SanalPOS_ID[text()='{0}']]", bankSanalPosId);

                //        model.SanalPosId = int.Parse(bankSanalPosId);
                //        _httpContext.HttpContext.Session.Write("Payment_Notification", model);

                //        var response = new Dictionary<string, string>();
                //        if (doc.SelectSingleNode(xpath) != null)
                //        {
                //            var installmentPlansNode = doc.SelectSingleNode(xpath).ChildNodes;

                //            for (int i = 7; i < 8; i++)
                //            {
                //                var key = int.Parse(Regex.Replace(installmentPlansNode[i].Name, @"[^\d]", ""));
                //                var value = decimal.Parse(installmentPlansNode[i].InnerText, CultureInfo.InvariantCulture).ToString("N2");

                //                var amount = key == 1 ? model.entity.Amount : model.entity.Amount + ((model.entity.Amount * decimal.Parse(value)) / 100);

                //                /* https://dev.param.com.tr/tr/api/oedeme-v2 Komisyon Dahil Sipariş Tutarı Hesaplama
                //                 Toplam_Tutar = Islem_Tutar + ((Islem_Tutar x Komisyon Oran) / 100)  */

                //                if (decimal.Parse(value) > 0 && key == 1)
                //                {
                //                    response.Add($"{key} Taksit", amount.ToString("N2"));
                //                }
                //            }

                //            return Json(response);
                //        }
                //        else { return Json(new GenericResponse { Status = "ERROR", Message = "Şu anda İşleminiz Gerçekleştirilemiyor. Lütfen Bir Süre Sonra Tekrar Deneyiniz." }); }
                //    }
                //    else
                //    {
                //        return Json(new GenericResponse { Status = "ERROR", Message = ozelOranList.Result.Sonuc_Str });
                //    }
                //}

                //if (model.CreditCardPaymentMethodModel.IsBankSanalPOS)
                //{
                //    var response = new Dictionary<string, string>();
                //    var key = 1;
                //    response.Add($"{key} Taksit", model.entity.Amount.ToString("N2"));

                //    return Json(response);
                //}

                //if (model.CreditCardPaymentMethodModel.Paybull)
                //{
                //    var response = new Dictionary<string, string>();
                //    var key = 1;
                //    response.Add($"{key} Taksit", model.entity.Amount.ToString("N2"));

                //    return Json(response);
                //}

                //if (model.CreditCardPaymentMethodModel.AKODE)
                //{
                //    var response = new Dictionary<string, string>();
                //    var key = 1;
                //    response.Add($"{key} Taksit", model.entity.Amount.ToString("N2"));

                //    return Json(response);
                //}

                #endregion

                if (model.CreditCardPaymentMethodID == (int)Enums.CreditCardPaymentMethodType.PayNKolay)
                {
                    var creditCardIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "PayNKolayCreditCard") });

                    var paymentInstallmentsRequestModel = new PaymentInstallmentRequestModel
                    {
                        Amount = model.entity.Amount,
                        CardNumber = creditCardNumber,
                        Sx = creditCardIntegrationValues.FirstOrDefault(f => f.ParamDef == "sx").ParamVal,
                    };

                    var paymentInstallmentsRequestResponse = PayNKolayPaymentInstallments.PaymentInstallmentsRequest(paymentInstallmentsRequestModel);

                    if (paymentInstallmentsRequestResponse.Status == "OK" && paymentInstallmentsRequestResponse.Data != null && paymentInstallmentsRequestResponse.Data.PAYMENT_BANK_LIST.Count != 0)
                    {
                        var responsePayNKolay = new Dictionary<string, string>();

                        foreach (var item in paymentInstallmentsRequestResponse.Data.PAYMENT_BANK_LIST.Where(x => x.INSTALLMENT == 1))
                        {
                            var keyPayNKolay = item.INSTALLMENT;
                            var value = item.INSTALLMENT_AMOUNT;

                            if (value > 0)
                            {
                                responsePayNKolay.Add($"{keyPayNKolay} Taksit", value.ToString("N2"));
                                model.PayNKolayEncodedValues.Add(new PayNKolayEncodedValueList { InstallmentMonth = keyPayNKolay, EncodedValue = item.EncodedValue });
                            }
                        }
                        _httpContext.HttpContext.Session.Write("Payment_Notification", model);
                        return Json(responsePayNKolay);
                    }
                    else
                        if (paymentInstallmentsRequestResponse.Message.Contains("valid"))
                        return Json(new GenericResponse { Status = "ERROR", Message = "Lütfen Kredi Kartı Bilgilerinizi Kontrol Ediniz." });
                    else
                        return Json(new GenericResponse { Status = "ERROR", Message = "Şu anda İşleminiz Gerçekleştirilemiyor. Lütfen Bir Süre Sonra Tekrar Deneyiniz." });
                }

                else
                {
                    var response = new Dictionary<string, string>();
                    var key = 1;
                    response.Add($"{key} Taksit", model.entity.Amount.ToString("N2"));

                    return Json(response);
                }

            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }

        #endregion

        #region Foreing CreditCard

        [HttpGet]
        public IActionResult ForeignCreditCard(string service_id, string frame_id)
        {
            try
            {
                if (service_id == null && frame_id == null)
                {
                    var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                    if (model != null && model.entity != null)
                    {
                        model.IsForeignCreditCard = true;
                        _httpContext.HttpContext.Session.Write("Payment_Notification", model);
                        return View(model);
                    }
                }
                else
                {
                    var integration = _companyIntegration.GetByServiceId(service_id);

                    if (integration != null)
                    {
                        if (!integration.ForeignCreditCardBeUsed)
                            return View("PaymentMethodDeactive");

                        var json = tMD5Manager.Decrypt(integration.SecretKey, frame_id);
                        var data = JsonConvert.DeserializeAnonymousType(json, new { transaction_id = "", amount = 0.0m, creation_time = DateTime.Now, currencyCode = "", redirectUrl = "" });

                        if (!string.IsNullOrEmpty(data.transaction_id) && data.creation_time > DateTime.Now.AddSeconds(-50))
                        {
                            var model = new PaymentNotificationViewModel();
                            model.entity.ServiceID = service_id;
                            model.entity.TransactionID = data.transaction_id;
                            model.entity.ActionDate = DateTime.Now;
                            model.entity.ActionTime = DateTime.Now.ToString("HH:mm");
                            model.entity.Amount = data.amount;
                            model.entity.Status = (byte)Enums.StatusType.Pending;
                            model.RedirectUrl = data.redirectUrl ?? integration.RedirectUrl;
                            model.IsForeignCreditCard = true;
                            model.CurrencyCode = data.currencyCode ?? "TRY";

                            if (model.CurrencyCode != "TRY")
                            {
                                var currency = _currencyManager.GetList(new List<FieldParameter> { new FieldParameter("CurrencyCode", Enums.FieldType.NVarChar, model.CurrencyCode) }).FirstOrDefault();

                                if (currency != null)
                                {
                                    model.CurrencyName = currency.CurrencyName;
                                    model.CurrencySymbol = currency.CurrencySymbol;
                                }
                            }

                            _httpContext.HttpContext.Session.Write("Payment_Notification", model);
                            return View(model);

                        }
                        else
                            return View("NotAllowed");
                    }

                }
            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForeignCreditCard(PaymentNotificationViewModel paymentNotificationViewModel)
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                var integration = _companyIntegration.GetByServiceId(model.entity.ServiceID);

                var hasEntity = _creditCardPaymentNotificationManager.GetSingleByTransactionID(model.entity.TransactionID);

                if (hasEntity != null)
                {
                    ContentResult result = new ContentResult
                    {
                        Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{model.RedirectUrl}"),
                        ContentType = "text/html"
                    };

                    return Json(new GenericResponse { Status = "ERROR", Data = model.RedirectUrl, Message = "hasEntity" });
                }
                model.CreditCardModel = paymentNotificationViewModel.CreditCardModel;
                model.CreditCardModel.PhoneNumber = paymentNotificationViewModel.CreditCardModel.CountryCode == "+90" ? "0" + paymentNotificationViewModel.CreditCardModel.PhoneNumber.Replace(" ", "") : paymentNotificationViewModel.CreditCardModel.PhoneNumber.Replace(" ", "");
                model.entity.Phone = model.CreditCardModel.PhoneNumber;

                //var checkBin = tSQLBankManager.CheckCardBinNumber(model.CreditCardModel.CardNumber.Replace(" ", "")[..6]);

                //if (checkBin)
                //{
                //    ContentResult result = new ContentResult
                //    {
                //        Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{model.RedirectUrl}"),
                //        ContentType = "text/html"
                //    };

                //    return Json(new GenericResponse { Status = "ERROR", Data = model.RedirectUrl, Message = "Lütfen kredi kartı bilgilerini kontrol ediniz.." });
                //}

                var member = _memberManager.GetMember(model.CreditCardModel.PhoneNumber);
                if (member != null)
                {
                    model.entity.IDMember = member.ID;
                    model.entity.SenderIdentityNr = member.IdentityNr;
                    //model.entity.Phone = member.Phone;
                }

                if (model.CreditCardModel.InstallmentMonth == null)
                    return Json(new GenericResponse { Status = "ERROR", Message = "Lütfen kredi kartı bilgilerini kontrol ediniz.." });

                var callbackEntity = new CallbackResponseLog();
                var opt = new JsonSerializerOptions() { WriteIndented = true };

                var lidio = model.CurrencyCode != "TRY" ? _paymentInstitutionManager.GetList(null).FirstOrDefault(x => x.ID == Convert.ToString((int)CreditCardPaymentMethodType.ForeignLidioPosCurrency)) :
                 _paymentInstitutionManager.GetList(null).FirstOrDefault(x => x.ID == Convert.ToString((int)CreditCardPaymentMethodType.ForeignLidioPosTL));

                if (lidio != null && lidio.IsActive)
                {
                    var lidioPosBinQueryRequestResponseModel = LidioPosBinQueryRequest.BinQueryRequest(model.CreditCardModel.CardNumber.Replace(" ", "")[..6], true);

                    callbackEntity = new CallbackResponseLog();
                    opt = new JsonSerializerOptions() { WriteIndented = true };

                    callbackEntity.TransactionID = model.entity.TransactionID;
                    callbackEntity.ServiceType = "LidioPosYD";
                    callbackEntity.IDCompany = integration.ID;
                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(lidioPosBinQueryRequestResponseModel, opt);
                    callbackEntity.TransactionType = "KREDI KARTI BIN QUERY RESPONSE";
                    _callbackResponseLogManager.Insert(callbackEntity);

                    if (lidioPosBinQueryRequestResponseModel.Status == "OK" && lidioPosBinQueryRequestResponseModel.Data != null && lidioPosBinQueryRequestResponseModel.Data.result == "Success")
                    {
                        return Json(new GenericResponse { Status = "ERROR", Message = "Lütfen Yurt Dışı kartı ile ödeme yapınız" });
                        //model.CreditCardModel.CardTypeId = lidioPosBinQueryRequestResponseModel.Data.isDebitCard ? (int)CreditCardType.BankCard : (int)CreditCardType.CreditCard;
                    }
                    else if (lidioPosBinQueryRequestResponseModel.Status == "OK" && lidioPosBinQueryRequestResponseModel.Data != null && lidioPosBinQueryRequestResponseModel.Data.result == "NotFound")
                        model.CreditCardModel.CardTypeId = (int)CreditCardType.CreditCard;
                    else
                        model.CreditCardModel.CardTypeId = (int)CreditCardType.CreditCard;                  
                }

                model.CreditCardPaymentMethodID = Convert.ToByte(lidio.ID);
                model.CreditCardRedirectToActionGetThreeDView = lidio.RedirectToActionGetThreeDView;
                model.CreditCardRedirectToActionPaymentMethod = lidio.RedirectToActionPaymentMethod;

                _httpContext.HttpContext.Session.Write("Payment_Notification", model);
                Type type = typeof(PaymentNotificationController);
                MethodInfo methodInfo = type.GetMethod(model.CreditCardRedirectToActionPaymentMethod);

                if (methodInfo != null)
                {
                    object result = methodInfo.Invoke(this, null);
                    return Json(result);
                }
            }
            catch { }

            return Json(new GenericResponse { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." });
        }

        [HttpGet]
        public IActionResult ForeignCreditCardInstallmentOptions(string creditCardNumber)
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                model.PayNKolayEncodedValues.Clear();

                var response = new Dictionary<string, string>();
                var key = 1;
                response.Add($"{key} Taksit", model.entity.Amount.ToString("N2"));

                return Json(response);
            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }
        #endregion

        #region PARAM
        public GenericResponse ParamPaymentMethod()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");

                var installmentMonth = int.Parse(model.CreditCardModel.InstallmentMonth);
                var expDateMonth = model.CreditCardModel.ExpirationDate[..2];
                var expDateSub = model.CreditCardModel.ExpirationDate[3..];
                var expDateYear = DateTime.Now.Year.ToString()[..2] + expDateSub;

                var hash_ = "";
                var odemeUrl = "https://burateknoloji.com/panel/paymentnotification/creditcard";
                var hataUrl = "https://burateknoloji.com/panel/paymentnotification/threedsecureresult";
                var basariliUrl = "https://burateknoloji.com/panel/paymentnotification/threedsecureresult";
                //var odemeUrl = "http://localhost:63352/Panel/paymentnotification/creditCard";
                //var hataUrl = "http://localhost:63352/Panel/paymentnotification/threedsecure";
                //var basariliUrl = "http://localhost:63352/Panel/paymentnotification/threedsecure";
                var Islem_Hash = stws.CLIENT_CODE + "DEC15F33-FC05-405E-BD0F-489381CE9AC3" + model.SanalPosId + installmentMonth.ToString() + model.entity.Amount.ToString("n2") + model.CreditCardModel.InstallmentAmount + model.entity.TransactionID + hataUrl + basariliUrl;
                hash_ = turkPosWSPRODSoapClient.SHA2B64Async(Islem_Hash).Result;

                var stSonucPreIslemOdeme = turkPosWSPRODSoapClient.TP_Islem_OdemeAsync(stws, model.SanalPosId, "DEC15F33-FC05-405E-BD0F-489381CE9AC3", model.CreditCardModel.SenderName, model.CreditCardModel.CardNumber, expDateMonth, expDateYear, model.CreditCardModel.SecurityCode, model.CreditCardModel.PhoneNumber, hataUrl, basariliUrl, model.entity.TransactionID, "sipariş açıklama", installmentMonth, model.entity.Amount.ToString("n2"), model.CreditCardModel.InstallmentAmount, hash_, "", "127.0.0.1", odemeUrl, "", "", "", "", "");

                var s = Convert.ToInt32(stSonucPreIslemOdeme.Result.Sonuc);
                if (s > 0)
                {
                    model.CreditCardModel.UCD_URL = stSonucPreIslemOdeme.Result.UCD_URL;
                    model.CreditCardModel.Description = stSonucPreIslemOdeme.Result.Sonuc_Str;

                    model.LastTime = DateTime.Now;
                    _httpContext.HttpContext.Session.Write("Payment_Notification", model);

                    return new GenericResponse { Status = "OK" };
                }
                else
                {
                    var creditCardPaymentNotification = new CreditCardPaymentNotification
                    {
                        TransactionID = model.entity.TransactionID,
                        ActionDate = model.entity.ActionDate,
                        ActionTime = model.entity.ActionTime,
                        Amount = decimal.Parse(model.entity.Amount.ToString()),
                        Description = stSonucPreIslemOdeme.Result.Sonuc_Str,
                        IDMember = model.entity.IDMember,
                        Phone = model.CreditCardModel.PhoneNumber,
                        Status = (int)StatusType.Canceled,
                        SenderName = model.CreditCardModel.SenderName,
                        SenderIdentityNr = model.entity.SenderIdentityNr,
                        ServiceID = model.entity.ServiceID,
                        CardNumber = model.CreditCardModel.CardNumber.Replace(" ", "")[..4] + "****" + "****" + model.CreditCardModel.CardNumber.Replace(" ", "")[12..],
                        MUser = "00000000-0000-0000-0000-000000000000"
                    };

                    var response = _creditCardPaymentNotificationManager.Insert(creditCardPaymentNotification);
                    model.LastTime = DateTime.Now;
                    _httpContext.HttpContext.Session.Write("Payment_Notification", model);
                    return new GenericResponse { Status = "ERROR", Message = stSonucPreIslemOdeme.Result.Sonuc_Str };

                }
            }
            catch { }

            return new GenericResponse { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." };
        }

        [HttpGet]
        public IActionResult ThreeDSecure()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                _httpContext.HttpContext.Session.Write("Payment_Notification", model);

                return View(model);

            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }

        [HttpPost]
        public IActionResult ThreeDSecure(ThreeDSecureResult threeDSecureResult)
        {
            try
            {
                return RedirectToAction("ThreeDSecureResult", "PaymentNotification", threeDSecureResult);
            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }

        [HttpPost]
        public IActionResult ThreeDSecureResult(ThreeDSecureResult threeDSecureResult)
        {
            try
            {
                var entity = _creditCardPaymentNotificationManager.GetSingleByTransactionID(threeDSecureResult.TURKPOS_RETVAL_Siparis_ID);
                var integration = _companyIntegration.GetByServiceId(entity.ServiceID);

                var callbackEntity = new CallbackResponseLog();
                var opt = new JsonSerializerOptions() { WriteIndented = true };
                callbackEntity.TransactionID = entity.TransactionID;
                callbackEntity.ServiceType = "PARAM";
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(threeDSecureResult, opt);
                callbackEntity.IDCompany = integration.ID;
                callbackEntity.TransactionType = "KREDI KARTI ODEMESI";
                _callbackResponseLogManager.Insert(callbackEntity);


                if (entity.Status != (byte)Enums.StatusType.Pending)
                {
                    ContentResult result = new ContentResult
                    {
                        Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{integration.RedirectUrl}"),
                        ContentType = "text/html"
                    };
                    return result;
                }

                if (int.Parse(threeDSecureResult.TURKPOS_RETVAL_Sonuc) > 0 && entity.Status == (byte)Enums.StatusType.Pending)
                {
                    entity.Status = entity.IsAutoNotification ? (byte)StatusType.Confirmed : (byte)StatusType.Pending;
                    entity.MUser = "00000000-0000-0000-0000-000000000000";
                    entity.MDate = DateTime.Now;
                    entity.Description = threeDSecureResult.TURKPOS_RETVAL_Sonuc_Str;
                    var response = _creditCardPaymentNotificationManager.SetStatus(entity);
                    if (response.Status == "OK")
                    {
                        var connection = _httpContext.HttpContext.Connection;

                        _creditCardPaymentNotificationManager.SetMemberIPAdress(entity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                        if (entity.IsAutoNotification)
                        {
                            var dataCallback = new
                            {
                                status_code = "OK",
                                service_id = entity.ServiceID,
                                status_type = 1,
                                ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                data = new { transaction_id = entity.TransactionID, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = "Ödeme İşlemi Başarılı" },
                                user_entered_data = new { member = entity.Member, sender_name = entity.SenderName, action_date = entity.ActionDate, action_time = entity.ActionTime, creditCard = entity.CardNumber, amount = entity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
                            };

                            var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                            callbackEntity.ServiceType = "STILPAY";
                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                            callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                            _callbackResponseLogManager.Insert(callbackEntity);
                        }
                    }
                    else
                        return Json(new GenericResponse { Status = "ERROR", Message = response.Message });
                }
                if (int.Parse(threeDSecureResult.TURKPOS_RETVAL_Sonuc) < 0 && entity.Status == (byte)Enums.StatusType.Pending)
                {
                    entity.Status = (int)StatusType.Canceled;
                    entity.MUser = "00000000-0000-0000-0000-000000000000";
                    entity.MDate = DateTime.Now;
                    entity.Description = threeDSecureResult.TURKPOS_RETVAL_Sonuc_Str;
                    var response = _creditCardPaymentNotificationManager.SetStatus(entity);
                    if (response.Status == "OK")
                    {
                        var connection = _httpContext.HttpContext.Connection;
                        _creditCardPaymentNotificationManager.SetMemberIPAdress(entity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                        var dataCallback = new
                        {
                            status_code = "ERROR",
                            service_id = entity.ServiceID,
                            status_type = 1,
                            ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                            data = new { transaction_id = entity.TransactionID, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = entity.Description },
                            user_entered_data = new { member = entity.Member, sender_name = entity.SenderName, action_date = entity.ActionDate, action_time = entity.ActionTime, creditCard = entity.CardNumber, amount = entity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
                        };

                        var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });
                        callbackEntity.ServiceType = "STILPAY";
                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                        callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                        _callbackResponseLogManager.Insert(callbackEntity);
                    }
                    else
                        return Json(new GenericResponse { Status = "ERROR", Message = response.Message });

                }

                var payment = new PaymentNotificationViewModel
                {
                    ThreeDSecureResultModel = threeDSecureResult,
                    IsAutoTransaction = entity.IsAutoNotification ? "true" : "false",
                    RedirectUrl = integration.RedirectUrl
                };

                //var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                //model.IsAutoTransaction = entity.IsAutoNotification;
                //_httpContext.HttpContext.Session.Write("Payment_Notification", model);

                return View(payment);

            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }
        #endregion

        #region PayNKolay
        public GenericResponse PayNKolayPaymentMethod()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");

                var expDateMonth = model.CreditCardModel.ExpirationDate[..2];
                var expDateSub = model.CreditCardModel.ExpirationDate[3..];
                var expDateYear = DateTime.Now.Year.ToString()[..2] + expDateSub;
                var installmentMonth = int.Parse(model.CreditCardModel.InstallmentMonth);

                var creditCardIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "PayNKolayCreditCard") });

                var paymentRequestModel = new PaymentRequestModel
                {
                    SenderName = model.CreditCardModel.SenderName,
                    CardNumber = model.CreditCardModel.CardNumber,
                    ExpirationDateMonth = expDateMonth,
                    ExpirationDateYear = expDateYear,
                    SecurityCode = model.CreditCardModel.SecurityCode,
                    InstallmentMonth = installmentMonth.ToString(),
                    InstallmentAmount = Convert.ToDecimal(model.CreditCardModel.InstallmentAmount),
                    EncodedValue = model.PayNKolayEncodedValues.Single(x => x.InstallmentMonth == installmentMonth).EncodedValue,
                    ClientRefCode = model.entity.TransactionID,
                    Sx = creditCardIntegrationValues.FirstOrDefault(f => f.ParamDef == "sx").ParamVal,
                    MerchantSecretKey = creditCardIntegrationValues.FirstOrDefault(f => f.ParamDef == "mercant_secret_key").ParamVal,
                };

                String rnd = DateTime.Now.ToString();
                String str = paymentRequestModel.Sx + paymentRequestModel.ClientRefCode + paymentRequestModel.InstallmentAmount.ToString(CultureInfo.InvariantCulture) + paymentRequestModel.SuccessUrl + paymentRequestModel.FailUrl + rnd + paymentRequestModel.MerchantSecretKey;
                SHA1 sha = new SHA1CryptoServiceProvider();
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(str);
                byte[] hashingbytes = sha.ComputeHash(bytes);
                String hashData = Convert.ToBase64String(hashingbytes);
                paymentRequestModel.HashData = hashData;
                paymentRequestModel.Rnd = rnd;

                var paymentRequestResponse = PayNKolayPayment.PaymentRequest(paymentRequestModel);

                if (paymentRequestResponse.Status == "OK" && paymentRequestResponse.Data != null)
                {
                    if (paymentRequestResponse.Data.RESPONSE_CODE == 2)
                    {
                        model.CreditCardModel.UCD_URL = paymentRequestResponse.Data.BANK_REQUEST_MESSAGE;
                        model.CreditCardModel.Description = paymentRequestResponse.Message;

                        model.LastTime = DateTime.Now;
                        _httpContext.HttpContext.Session.Write("Payment_Notification", model);
                        return new GenericResponse { Status = "OK" };
                    }
                    else
                        return new GenericResponse { Status = "ERROR", Message = paymentRequestResponse.Message };

                }
                else
                    return new GenericResponse { Status = "ERROR", Message = paymentRequestResponse.Message };
            }
            catch { }

            return new GenericResponse { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." };
        }

        [HttpGet]
        public IActionResult PayNKolayThreeDSecure()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                _httpContext.HttpContext.Session.Write("Payment_Notification", model);

                return View(model);

            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }

        [HttpPost]
        public IActionResult PayNKolayThreeDSecureResult(PayNKolayThreeDSecureResult payNKolayThreeDSecureResult)
        {
            try
            {
                var view = new PaymentNotificationViewModel();

                var entity = _creditCardPaymentNotificationManager.GetSingleByTransactionID(payNKolayThreeDSecureResult.CLIENT_REFERENCE_CODE);
                var callbackEntity = new CallbackResponseLog();
                var opt = new JsonSerializerOptions() { WriteIndented = true };

                if (entity != null)
                {
                    var integration = _companyIntegration.GetByServiceId(entity.ServiceID);

                    var checkCallback = _callbackResponseLogManager.GetSingle(new List<FieldParameter>()
                    {
                        new FieldParameter("TransactionID", Enums.FieldType.NVarChar, entity.TransactionID),
                        new FieldParameter("TransactionType", Enums.FieldType.NVarChar, "KREDI KARTI ODEMESI PAYMENT")
                    });

                    callbackEntity.TransactionID = entity.TransactionID;
                    callbackEntity.ServiceType = "PayNKolay";
                    callbackEntity.IDCompany = integration.ID;
                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(payNKolayThreeDSecureResult, opt);
                    callbackEntity.TransactionType = "KREDI KARTI ODEMESI PAYMENT";
                    _callbackResponseLogManager.Insert(callbackEntity);


                    if (checkCallback != null)
                    {
                        return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
                    }

                    var creditCardIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "PayNKolayCreditCard") });

                    if (entity.Status != (byte)Enums.StatusType.Pending)
                    {
                        ContentResult result = new ContentResult
                        {
                            Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{integration.RedirectUrl}"),
                            ContentType = "text/html"
                        };
                        return result;
                    }

                    if (entity.Status == (byte)Enums.StatusType.Pending && payNKolayThreeDSecureResult.RESPONSE_CODE == 2)
                    {
                        var complatePaymentRequestModel = new ComplatePaymentRequestModel
                        {
                            ReferenceCode = payNKolayThreeDSecureResult.REFERENCE_CODE,
                            Sx = creditCardIntegrationValues.FirstOrDefault(f => f.ParamDef == "sx").ParamVal,
                        };

                        var complatePaymentRequest = PayNKolayCompletePayment.CompletePaymentRequest(complatePaymentRequestModel);

                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(complatePaymentRequest, opt);
                        callbackEntity.TransactionType = "KREDI KARTI ODEMESI COMPLATEPAYMENT";
                        _callbackResponseLogManager.Insert(callbackEntity);

                        if (complatePaymentRequest.Status == "OK" && complatePaymentRequest.Data.AUTH_CODE != null)
                        {
                            entity.Status = entity.IsAutoNotification ? (byte)StatusType.Confirmed : (byte)StatusType.Pending;
                            entity.MUser = "00000000-0000-0000-0000-000000000000";
                            entity.MDate = DateTime.Now;
                            entity.Description = complatePaymentRequest.Message;
                            entity.TransactionReferenceCode = complatePaymentRequest.Data.REFERENCE_CODE;
                            var response = _creditCardPaymentNotificationManager.SetStatus(entity);
                            if (response.Status == "OK")
                            {

                                var connection = _httpContext.HttpContext.Connection;
                                _creditCardPaymentNotificationManager.SetMemberIPAdress(entity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                                if (entity.IsAutoNotification)
                                {
                                    var dataCallback = new
                                    {
                                        status_code = "OK",
                                        service_id = entity.ServiceID,
                                        status_type = 1,
                                        ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                        data = new { transaction_id = entity.TransactionID, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = "Ödeme İşlemi Başarılı" },
                                        user_entered_data = new { member = entity.Member, sender_name = entity.SenderName, action_date = entity.ActionDate, action_time = entity.ActionTime, creditCard = entity.CardNumber, amount = entity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
                                    };

                                   var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                                    callbackEntity.ServiceType = "STILPAY";
                                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                    callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                    _callbackResponseLogManager.Insert(callbackEntity);
                                }
                            }
                            else
                                return Json(new GenericResponse { Status = "ERROR", Message = response.Message });
                        }
                        else
                        {
                            entity.Status = (int)StatusType.Canceled;
                            entity.MUser = "00000000-0000-0000-0000-000000000000";
                            entity.MDate = DateTime.Now;
                            entity.Description = complatePaymentRequest.Message;
                            entity.TransactionReferenceCode = complatePaymentRequest.Data.REFERENCE_CODE;
                            var response = _creditCardPaymentNotificationManager.SetStatus(entity);
                            if (response.Status == "OK")
                            {
                                var connection = _httpContext.HttpContext.Connection;
                                _creditCardPaymentNotificationManager.SetMemberIPAdress(entity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                                var companyIntegration = _companyIntegration.GetByServiceId(entity.ServiceID);

                                var dataCallback = new
                                {
                                    status_code = "ERROR",
                                    service_id = entity.ServiceID,
                                    status_type = 1,
                                    ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                    data = new { transaction_id = entity.TransactionID, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = entity.Description },
                                    user_entered_data = new { member = entity.Member, sender_name = entity.SenderName, action_date = entity.ActionDate, action_time = entity.ActionTime, creditCard = entity.CardNumber, amount = entity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
                                };

                                var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });
                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                callbackEntity.ServiceType = "STILPAY";
                                callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                _callbackResponseLogManager.Insert(callbackEntity);
                            }
                            else
                                return Json(new GenericResponse { Status = "ERROR", Message = response.Message });
                        }

                        view.PayNKolayThreeDSecureResultModel.RESPONSE_CODE = complatePaymentRequest.Data.RESPONSE_CODE;
                        view.PayNKolayThreeDSecureResultModel.RESPONSE_DATA = complatePaymentRequest.Message;
                        view.PayNKolayThreeDSecureResultModel.TimeStamp = payNKolayThreeDSecureResult.TimeStamp;
                        view.IsAutoTransaction = entity.IsAutoNotification ? "true" : "false";
                        view.RedirectUrl = integration.RedirectUrl;

                        return View(view);
                    }

                    if (entity.Status == (byte)Enums.StatusType.Pending && payNKolayThreeDSecureResult.RESPONSE_CODE != 2)
                    {
                        entity.Status = (byte)StatusType.Canceled;
                        entity.MUser = "00000000-0000-0000-0000-000000000000";
                        entity.MDate = DateTime.Now;
                        entity.Description = payNKolayThreeDSecureResult.RESPONSE_DATA;
                        var response = _creditCardPaymentNotificationManager.SetStatus(entity);
                        if (response.Status == "OK")
                        {
                            var connection = _httpContext.HttpContext.Connection;
                            _creditCardPaymentNotificationManager.SetMemberIPAdress(entity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                            var dataCallback = new
                            {
                                status_code = "ERROR",
                                service_id = entity.ServiceID,
                                status_type = 1,
                                ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                data = new { transaction_id = entity.TransactionID, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = entity.Description },
                                user_entered_data = new { member = entity.Member, sender_name = entity.SenderName, action_date = entity.ActionDate, action_time = entity.ActionTime, creditCard = entity.CardNumber, amount = entity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
                            };

                            var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                            callbackEntity.ServiceType = "STILPAY";
                            callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                            _callbackResponseLogManager.Insert(callbackEntity);
                        }
                        else
                            return Json(new GenericResponse { Status = "ERROR", Message = response.Message });
                    }

                    view.PayNKolayThreeDSecureResultModel.RESPONSE_CODE = payNKolayThreeDSecureResult.RESPONSE_CODE;
                    view.PayNKolayThreeDSecureResultModel.RESPONSE_DATA = payNKolayThreeDSecureResult.RESPONSE_DATA;
                    view.PayNKolayThreeDSecureResultModel.TimeStamp = payNKolayThreeDSecureResult.TimeStamp;
                    view.IsAutoTransaction = entity.IsAutoNotification ? "true" : "false";
                    view.RedirectUrl = integration.RedirectUrl;
                    return View(view);
                }

                else
                {
                    var foreignCreditCardEntity = _foreignCreditCardPaymentNotificationManager.GetSingleByTransactionID(payNKolayThreeDSecureResult.CLIENT_REFERENCE_CODE);

                    var integration = _companyIntegration.GetByServiceId(foreignCreditCardEntity.ServiceID);



                    var checkCallback = _callbackResponseLogManager.GetSingle(new List<FieldParameter>()
                    {
                        new FieldParameter("TransactionID", Enums.FieldType.NVarChar, foreignCreditCardEntity.TransactionID),
                        new FieldParameter("TransactionType", Enums.FieldType.NVarChar, "KREDI KARTI ODEMESI PAYMENT")
                    });

                    callbackEntity.TransactionID = foreignCreditCardEntity.TransactionID;
                    callbackEntity.ServiceType = "PayNKolay";
                    callbackEntity.IDCompany = integration.ID;
                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(payNKolayThreeDSecureResult, opt);
                    callbackEntity.TransactionType = "KREDI KARTI ODEMESI PAYMENT";
                    _callbackResponseLogManager.Insert(callbackEntity);


                    if (checkCallback != null)
                    {
                        return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
                    }

                    var foreignCreditCardIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "PayNKolayForeignCC") });

                    if (foreignCreditCardEntity.Status != (byte)Enums.StatusType.Pending)
                    {
                        ContentResult result = new ContentResult
                        {
                            Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{integration.RedirectUrl}"),
                            ContentType = "text/html"
                        };
                        return result;
                    }

                    if (foreignCreditCardEntity.Status == (byte)Enums.StatusType.Pending && payNKolayThreeDSecureResult.RESPONSE_CODE == 2)
                    {
                        var complatePaymentRequestModel = new ComplatePaymentRequestModel
                        {
                            ReferenceCode = payNKolayThreeDSecureResult.REFERENCE_CODE,
                            Sx = foreignCreditCardIntegrationValues.FirstOrDefault(f => f.ParamDef == "sx").ParamVal,
                        };

                        var complatePaymentRequest = PayNKolayCompletePayment.CompletePaymentRequest(complatePaymentRequestModel);

                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(complatePaymentRequest, opt);
                        callbackEntity.TransactionType = "KREDI KARTI ODEMESI COMPLATEPAYMENT";
                        _callbackResponseLogManager.Insert(callbackEntity);

                        if (complatePaymentRequest.Status == "OK" && complatePaymentRequest.Data.AUTH_CODE != null)
                        {
                            foreignCreditCardEntity.Status = foreignCreditCardEntity.IsAutoNotification ? (byte)StatusType.Confirmed : (byte)StatusType.Pending;
                            foreignCreditCardEntity.MUser = "00000000-0000-0000-0000-000000000000";
                            foreignCreditCardEntity.MDate = DateTime.Now;
                            foreignCreditCardEntity.Description = complatePaymentRequest.Message;
                            foreignCreditCardEntity.TransactionReferenceCode = complatePaymentRequest.Data.REFERENCE_CODE;
                            var response = _foreignCreditCardPaymentNotificationManager.SetStatus(foreignCreditCardEntity);
                            if (response.Status == "OK")
                            {

                                var connection = _httpContext.HttpContext.Connection;
                                _foreignCreditCardPaymentNotificationManager.SetMemberIPAdress(foreignCreditCardEntity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                                if (foreignCreditCardEntity.IsAutoNotification)
                                {
                                    var dataCallback = new
                                    {
                                        status_code = "OK",
                                        service_id = foreignCreditCardEntity.ServiceID,
                                        status_type = 1,
                                        ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                        data = new { transaction_id = foreignCreditCardEntity.TransactionID, sp_transactionNr = foreignCreditCardEntity.TransactionNr, amount = foreignCreditCardEntity.Amount, sp_id = foreignCreditCardEntity.ID, message = "Ödeme İşlemi Başarılı" },
                                        user_entered_data = new { member = foreignCreditCardEntity.Member, sender_name = foreignCreditCardEntity.SenderName, action_date = foreignCreditCardEntity.ActionDate, action_time = foreignCreditCardEntity.ActionTime, creditCard = foreignCreditCardEntity.CardNumber, amount = foreignCreditCardEntity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
                                    };

                                    var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                                    callbackEntity.ServiceType = "STILPAY";
                                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                    callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                    _callbackResponseLogManager.Insert(callbackEntity);
                                }
                            }
                            else
                                return Json(new GenericResponse { Status = "ERROR", Message = response.Message });
                        }
                        else
                        {
                            foreignCreditCardEntity.Status = (int)StatusType.Canceled;
                            foreignCreditCardEntity.MUser = "00000000-0000-0000-0000-000000000000";
                            foreignCreditCardEntity.MDate = DateTime.Now;
                            foreignCreditCardEntity.Description = complatePaymentRequest.Message;
                            foreignCreditCardEntity.TransactionReferenceCode = complatePaymentRequest.Data.REFERENCE_CODE;
                            var response = _foreignCreditCardPaymentNotificationManager.SetStatus(foreignCreditCardEntity);
                            if (response.Status == "OK")
                            {
                                var connection = _httpContext.HttpContext.Connection;
                                _foreignCreditCardPaymentNotificationManager.SetMemberIPAdress(foreignCreditCardEntity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                                var companyIntegration = _companyIntegration.GetByServiceId(foreignCreditCardEntity.ServiceID);

                                var dataCallback = new
                                {
                                    status_code = "ERROR",
                                    service_id = foreignCreditCardEntity.ServiceID,
                                    status_type = 1,
                                    ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                    data = new { transaction_id = foreignCreditCardEntity.TransactionID, sp_transactionNr = foreignCreditCardEntity.TransactionNr, amount = foreignCreditCardEntity.Amount, sp_id = foreignCreditCardEntity.ID, message = foreignCreditCardEntity.Description },
                                    user_entered_data = new { member = foreignCreditCardEntity.Member, sender_name = foreignCreditCardEntity.SenderName, action_date = foreignCreditCardEntity.ActionDate, action_time = foreignCreditCardEntity.ActionTime, creditCard = foreignCreditCardEntity.CardNumber, amount = foreignCreditCardEntity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
                                };

                                var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });
                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                callbackEntity.ServiceType = "STILPAY";
                                callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                _callbackResponseLogManager.Insert(callbackEntity);
                            }
                            else
                                return Json(new GenericResponse { Status = "ERROR", Message = response.Message });
                        }

                        view.PayNKolayThreeDSecureResultModel.RESPONSE_CODE = complatePaymentRequest.Data.RESPONSE_CODE;
                        view.PayNKolayThreeDSecureResultModel.RESPONSE_DATA = complatePaymentRequest.Message;
                        view.PayNKolayThreeDSecureResultModel.TimeStamp = payNKolayThreeDSecureResult.TimeStamp;
                        view.IsAutoTransaction = foreignCreditCardEntity.IsAutoNotification ? "true" : "false";
                        view.RedirectUrl = integration.RedirectUrl;

                        return View(view);
                    }

                    if (foreignCreditCardEntity.Status == (byte)Enums.StatusType.Pending && payNKolayThreeDSecureResult.RESPONSE_CODE != 2)
                    {
                        foreignCreditCardEntity.Status = (byte)StatusType.Canceled;
                        foreignCreditCardEntity.MUser = "00000000-0000-0000-0000-000000000000";
                        foreignCreditCardEntity.MDate = DateTime.Now;
                        foreignCreditCardEntity.Description = payNKolayThreeDSecureResult.RESPONSE_DATA;
                        var response = _foreignCreditCardPaymentNotificationManager.SetStatus(foreignCreditCardEntity);
                        if (response.Status == "OK")
                        {
                            var connection = _httpContext.HttpContext.Connection;
                            _foreignCreditCardPaymentNotificationManager.SetMemberIPAdress(foreignCreditCardEntity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                            var dataCallback = new
                            {
                                status_code = "ERROR",
                                service_id = foreignCreditCardEntity.ServiceID,
                                status_type = 1,
                                ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                data = new { transaction_id = foreignCreditCardEntity.TransactionID, sp_transactionNr = foreignCreditCardEntity.TransactionNr, amount = foreignCreditCardEntity.Amount, sp_id = foreignCreditCardEntity.ID, message = foreignCreditCardEntity.Description },
                                user_entered_data = new { member = foreignCreditCardEntity.Member, sender_name = foreignCreditCardEntity.SenderName, action_date = foreignCreditCardEntity.ActionDate, action_time = foreignCreditCardEntity.ActionTime, creditCard = foreignCreditCardEntity.CardNumber, amount = foreignCreditCardEntity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
                            };

                            var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                            callbackEntity.ServiceType = "STILPAY";
                            callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                            _callbackResponseLogManager.Insert(callbackEntity);
                        }
                        else
                            return Json(new GenericResponse { Status = "ERROR", Message = response.Message });
                    }

                    view.PayNKolayThreeDSecureResultModel.RESPONSE_CODE = payNKolayThreeDSecureResult.RESPONSE_CODE;
                    view.PayNKolayThreeDSecureResultModel.RESPONSE_DATA = payNKolayThreeDSecureResult.RESPONSE_DATA;
                    view.PayNKolayThreeDSecureResultModel.TimeStamp = payNKolayThreeDSecureResult.TimeStamp;
                    view.IsAutoTransaction = foreignCreditCardEntity.IsAutoNotification ? "true" : "false";
                    view.RedirectUrl = integration.RedirectUrl;
                    return View(view);
                }

            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }
        #endregion

        #region IsBank
        public GenericResponse IsBankPaymentMethod()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");

                var expDateMonth = model.CreditCardModel.ExpirationDate[..2];
                var expDateSub = model.CreditCardModel.ExpirationDate[3..];

                var isBankSanalPOSIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "IsBankSanalPos") });

                var isBankPaymentRequestModel = new IsBankSanalPosPaymentRequestModel
                {
                    amount = model.entity.Amount.ToString("n2"),
                    clientid = isBankSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "clientid").ParamVal,
                    Ecom_Payment_Card_ExpDate_Month = expDateMonth,
                    Ecom_Payment_Card_ExpDate_Year = expDateSub,
                    oid = model.entity.TransactionID,
                    pan = model.CreditCardModel.CardNumber
                };

                string hashVal = "";

                SortedDictionary<string, string> param = new SortedDictionary<string, string>()
                    {
                        { "clientid", isBankPaymentRequestModel.clientid},
                        { "oid", isBankPaymentRequestModel.oid  },
                        { "amount", isBankPaymentRequestModel.amount.ToString(CultureInfo.InvariantCulture)},
                        { "okUrl", isBankPaymentRequestModel.okUrl  },
                        { "failUrl", isBankPaymentRequestModel.failUrl  },
                        { "islemtipi", isBankPaymentRequestModel.islemtipi },
                        { "currency", isBankPaymentRequestModel.currency },
                        { "lang", isBankPaymentRequestModel.lang },
                        { "pan", isBankPaymentRequestModel.pan.Replace(" ", "") },
                        { "Ecom_Payment_Card_ExpDate_Year", isBankPaymentRequestModel.Ecom_Payment_Card_ExpDate_Year },
                        { "Ecom_Payment_Card_ExpDate_Month", isBankPaymentRequestModel.Ecom_Payment_Card_ExpDate_Month },
                        { "hashAlgorithm", isBankPaymentRequestModel.hashAlgorithm },
                        { "storetype", isBankPaymentRequestModel.storetype }
                    };

                foreach (KeyValuePair<String, String> pair in param)
                {
                    String escapedValue = pair.Value.Replace("\\", "\\\\").Replace("|", "\\|");
                    String lowerValue = pair.Key.ToLower(new System.Globalization.CultureInfo("en-US", false));
                    if (!"encoding".Equals(lowerValue) && !"hash".Equals(lowerValue))
                    {
                        hashVal += escapedValue + "|";
                    }
                }

                hashVal += isBankSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "storeKey").ParamVal;

                System.Security.Cryptography.SHA512 sha = new System.Security.Cryptography.SHA512CryptoServiceProvider();
                byte[] hashbytes = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(hashVal);
                byte[] inputbytes = sha.ComputeHash(hashbytes);
                String hash = System.Convert.ToBase64String(inputbytes);

                isBankPaymentRequestModel.hash = hash;

                var response = IsBankSanalPOS.PaymentRequest(isBankPaymentRequestModel);

                if (response != null)
                {
                    model.LastTime = DateTime.Now;
                    model.CreditCardModel.UCD_URL = response;
                    _httpContext.HttpContext.Session.Write("Payment_Notification", model);
                    return new GenericResponse { Status = "OK" };
                }
                else
                    return new GenericResponse { Status = "ERROR", Message = response };
            }
            catch { }

            return new GenericResponse { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." };
        }

        [HttpGet]
        public IActionResult IsBankThreeDSecure()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                _httpContext.HttpContext.Session.Write("Payment_Notification", model);

                return View(model);

            }
            catch { }

            return Json(new GenericResponse { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." });
        }

        [HttpPost]
        public IActionResult IsBankThreeDSecureResult(IsBankSanalPosPayment3DResponseModel isBankSanalPosPayment3DResponseModel)
        {
            try
            {
                var view = new PaymentNotificationViewModel();

                var entity = _creditCardPaymentNotificationManager.GetSingleByTransactionID(isBankSanalPosPayment3DResponseModel.oid);
                var callbackEntity = new CallbackResponseLog();
                var opt = new JsonSerializerOptions() { WriteIndented = true };

                var integration = _companyIntegration.GetByServiceId(entity.ServiceID);

                var checkCallback = _callbackResponseLogManager.GetSingle(new List<FieldParameter>()
                    {
                        new FieldParameter("TransactionID", Enums.FieldType.NVarChar, entity.TransactionID),
                        new FieldParameter("TransactionType", Enums.FieldType.NVarChar, "KREDI KARTI ODEMESI PAYMENT")
                    });

                callbackEntity.TransactionID = entity.TransactionID;
                callbackEntity.ServiceType = "IsBankSanalPOS";
                callbackEntity.IDCompany = integration.ID;
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankSanalPosPayment3DResponseModel, opt);
                callbackEntity.TransactionType = "KREDI KARTI ODEMESI PAYMENT";
                _callbackResponseLogManager.Insert(callbackEntity);

                if (checkCallback != null)
                {
                    return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
                }

                if (entity.Status != (byte)Enums.StatusType.Pending)
                {
                    ContentResult result = new ContentResult
                    {
                        Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{integration.RedirectUrl}"),
                        ContentType = "text/html"
                    };
                    return result;
                }

                string[] mdStatusValues = { "1", "2", "3", "4" };

                if (entity.Status == (byte)Enums.StatusType.Pending && mdStatusValues.Contains(isBankSanalPosPayment3DResponseModel.mdStatus) && isBankSanalPosPayment3DResponseModel.mdErrorMsg == "Success")
                {
                    var connection = _httpContext.HttpContext.Connection;
                    _creditCardPaymentNotificationManager.SetMemberIPAdress(entity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                    var isBankSanalPOSIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "IsBankSanalPos") });

                    isBankSanalPosPayment3DResponseModel.apiUserName = isBankSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "apiUserName").ParamVal;
                    isBankSanalPosPayment3DResponseModel.apiUserPassword = isBankSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "apiUserPassword").ParamVal;

                    var isBankPaymentRequestXmlResponse = IsBankSanalPOSComplatePaymentXML.PaymentRequestXML(isBankSanalPosPayment3DResponseModel);

                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankPaymentRequestXmlResponse, opt);
                    callbackEntity.TransactionType = "KREDI KARTI ODEMESI COMPLATEPAYMENT";
                    _callbackResponseLogManager.Insert(callbackEntity);

                    if (isBankPaymentRequestXmlResponse.Status == "OK" && isBankPaymentRequestXmlResponse.Data.Response == "Approved")
                    {
                        entity.Status = entity.IsAutoNotification ? (byte)StatusType.Confirmed : (byte)StatusType.Pending;
                        entity.MUser = "00000000-0000-0000-0000-000000000000";
                        entity.MDate = DateTime.Now;
                        entity.Description = isBankPaymentRequestXmlResponse.Message;

                        var response = _creditCardPaymentNotificationManager.SetStatus(entity);
                        if (response.Status == "OK")
                        {
                            view.GenericCreditCardPaymentResponseModel.Success = true;
                            view.GenericCreditCardPaymentResponseModel.IsAutoTransaction = entity.IsAutoNotification ? "true" : "false";
                            view.GenericCreditCardPaymentResponseModel.Message = isBankPaymentRequestXmlResponse.Message;
                            view.GenericCreditCardPaymentResponseModel.TimeStamp = DateTime.ParseExact(isBankPaymentRequestXmlResponse.Data.Extra.TRXDATE, "yyyyMMdd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).ToString(("yyyy-MM-dd HH:mm:ss"));
                            view.GenericCreditCardPaymentResponseModel.RedirectUrl = integration.RedirectUrl;

                            if (entity.IsAutoNotification)
                            {
                                var dataCallback = new
                                {
                                    status_code = "OK",
                                    service_id = entity.ServiceID,
                                    status_type = 1,
                                    ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                    data = new { transaction_id = entity.TransactionID, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = "Ödeme İşlemi Başarılı" },
                                    user_entered_data = new { member = entity.Member, sender_name = entity.SenderName, action_date = entity.ActionDate, action_time = entity.ActionTime, creditCard = entity.CardNumber, amount = entity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
                                };

                                var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                                callbackEntity.ServiceType = "STILPAY";
                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                _callbackResponseLogManager.Insert(callbackEntity);
                            }
                        }
                        else
                            return Json(new GenericResponse { Status = "ERROR", Message = response.Message });
                    }
                    else
                    {
                        entity.Status = (int)StatusType.Canceled;
                        entity.MUser = "00000000-0000-0000-0000-000000000000";
                        entity.MDate = DateTime.Now;
                        entity.Description = isBankPaymentRequestXmlResponse.Message;

                        var response = _creditCardPaymentNotificationManager.SetStatus(entity);
                        if (response.Status == "OK")
                        {
                            view.GenericCreditCardPaymentResponseModel.Success = false;
                            view.GenericCreditCardPaymentResponseModel.IsAutoTransaction = entity.IsAutoNotification ? "true" : "false";
                            view.GenericCreditCardPaymentResponseModel.Message = isBankPaymentRequestXmlResponse.Message;
                            view.GenericCreditCardPaymentResponseModel.TimeStamp = DateTime.ParseExact(isBankPaymentRequestXmlResponse.Data.Extra.TRXDATE, "yyyyMMdd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).ToString(("yyyy-MM-dd HH:mm:ss"));
                            view.GenericCreditCardPaymentResponseModel.RedirectUrl = integration.RedirectUrl;

                            var dataCallback = new
                            {
                                status_code = "ERROR",
                                service_id = entity.ServiceID,
                                status_type = 1,
                                ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                data = new { transaction_id = entity.TransactionID, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = entity.Description },
                                user_entered_data = new { member = entity.Member, sender_name = entity.SenderName, action_date = entity.ActionDate, action_time = entity.ActionTime, creditCard = entity.CardNumber, amount = entity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
                            };

                            var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });
                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                            callbackEntity.ServiceType = "STILPAY";
                            callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                            _callbackResponseLogManager.Insert(callbackEntity);
                        }
                        else
                            return Json(new GenericResponse { Status = "ERROR", Message = response.Message });
                    }
                }
                else
                {
                    view.GenericCreditCardPaymentResponseModel.Success = false;
                    view.GenericCreditCardPaymentResponseModel.IsAutoTransaction = entity.IsAutoNotification ? "true" : "false";
                    view.GenericCreditCardPaymentResponseModel.Message = isBankSanalPosPayment3DResponseModel.mdErrorMsg == null || isBankSanalPosPayment3DResponseModel.mdErrorMsg == "" ? "İşlem Başarısız" : isBankSanalPosPayment3DResponseModel.mdErrorMsg;
                    view.GenericCreditCardPaymentResponseModel.TimeStamp = DateTime.Now.ToString("n2");
                    view.GenericCreditCardPaymentResponseModel.RedirectUrl = integration.RedirectUrl;
                }
                return View(view);
            }

            catch { }

            return Json(new GenericResponse { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." });
        }

        #endregion

        #region Paybull
        public GenericResponse PaybullPaymentMethod()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");

                var expDateMonth = model.CreditCardModel.ExpirationDate[..2];
                var expDateSub = model.CreditCardModel.ExpirationDate[3..];
                var expDateYear = DateTime.Now.Year.ToString()[..2] + expDateSub;

                var paybullSanalPOSIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "PaybullCreditCard") });

                string[] senderNameParts = model.CreditCardModel.SenderName.Split(' ');
                var paybullPaymentRequestModel = new PaybullPaymentRequestModel
                {
                    cc_holder_name = model.CreditCardModel.SenderName,
                    cc_no = model.CreditCardModel.CardNumber.Replace(" ", ""),
                    currency_code = "TRY",
                    cvv = model.CreditCardModel.SecurityCode,
                    expiry_month = expDateMonth,
                    expiry_year = expDateYear,
                    installments_number = 1,
                    invoice_id = model.entity.TransactionID,
                    name = senderNameParts[0],
                    surname = senderNameParts[1],
                    total = model.entity.Amount,
                    merchant_key = paybullSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "secret_key").ParamVal,
                    invoice_description = "test"
                };

                var hashKey = PaybullGenerateHashKey.GenerateHashKey(paybullPaymentRequestModel.total.ToString(CultureInfo.InvariantCulture), paybullPaymentRequestModel.installments_number.ToString(), paybullPaymentRequestModel.currency_code, paybullPaymentRequestModel.merchant_key, paybullPaymentRequestModel.invoice_id, paybullSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "app_password").ParamVal);

                paybullPaymentRequestModel.hash_key = hashKey;

                var paymentRequest = PaybullPaymentRequest.PaymentRequest(paybullPaymentRequestModel);

                if (paymentRequest != null && !paymentRequest.Contains("1001"))
                {
                    model.CreditCardModel.UCD_URL = paymentRequest;
                    model.CreditCardModel.Description = "Ön Ödeme Süreci Başarılı";

                    model.LastTime = DateTime.Now;
                    _httpContext.HttpContext.Session.Write("Payment_Notification", model);
                    return new GenericResponse { Status = "OK" };
                }
                else
                    return new GenericResponse { Status = "ERROR", Message = paymentRequest };

            }
            catch { }

            return new GenericResponse { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." };       
        }

        [HttpGet]
        public IActionResult PaybullThreeDSecure()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                _httpContext.HttpContext.Session.Write("Payment_Notification", model);

                return View(model);

            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }

        [HttpGet]
        public IActionResult PaybullThreeDSecureResult(PaybullPaymentResponseModel.Root paybullPaymentResponseModel)
        {
            try
            {
                var view = new PaymentNotificationViewModel();
                byte status = 0;
                var response = "";
                var orderId = "";
                var entity = _creditCardPaymentNotificationManager.GetSingleByTransactionID(paybullPaymentResponseModel.invoice_id);
                var callbackEntity = new CallbackResponseLog();
                var opt = new JsonSerializerOptions() { WriteIndented = true };

                var integration = _companyIntegration.GetByServiceId(entity.ServiceID);

                var checkCallback = _callbackResponseLogManager.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("TransactionID", Enums.FieldType.NVarChar, entity.TransactionID),
                    new FieldParameter("TransactionType", Enums.FieldType.NVarChar, "KREDI KARTI ODEMESI PAYMENT")
                });

                callbackEntity.TransactionID = entity.TransactionID;
                callbackEntity.ServiceType = "Paybull";
                callbackEntity.IDCompany = integration.ID;
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(paybullPaymentResponseModel, opt);
                callbackEntity.TransactionType = "KREDI KARTI ODEMESI PAYMENT";
                _callbackResponseLogManager.Insert(callbackEntity);

                if (checkCallback != null)
                {
                    return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
                }

                var getRequestData = _callbackResponseLogManager.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("TransactionID", Enums.FieldType.NVarChar, entity.TransactionID),
                    new FieldParameter("TransactionType", Enums.FieldType.NVarChar, "Get IFrame Request")
                });

                JObject jsonObject = JObject.Parse(getRequestData.Callback);

                var redirectUrl = jsonObject["data"]?["redirectUrl"]?.ToString();

                if (string.IsNullOrEmpty(redirectUrl))
                {
                    redirectUrl = integration.RedirectUrl;
                }

                if (entity.Status != (byte)Enums.StatusType.Pending)
                {
                    ContentResult result = new ContentResult
                    {
                        Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{redirectUrl}"),
                        ContentType = "text/html"
                    };

                    return result;
                }

                if (entity.Status == (byte)Enums.StatusType.Pending)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        var paybullCheckStatusRequestModel = new PaybullCheckStatusRequestModel()
                        {
                            merchant_key = _settingDAL.GetList(null).FirstOrDefault(x => x.ParamType == "PaybullCreditCard" && x.ParamDef == "secret_key").ParamVal,
                            invoice_id = entity.TransactionID,
                            include_pending_status = false,
                        };

                        var paybullCheckStatusResponseModel = PaybullCheckStatusRequest.CheckStatus(paybullCheckStatusRequestModel);

                        callbackEntity.TransactionID = entity.TransactionID;
                        callbackEntity.ServiceType = "Paybull";
                        callbackEntity.IDCompany = integration.ID;
                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(paybullCheckStatusResponseModel, opt);
                        callbackEntity.TransactionType = "KREDI KARTI ODEMESI QUERY RESPONSE";
                        _callbackResponseLogManager.Insert(callbackEntity);

                        if (paybullCheckStatusResponseModel.Data != null && paybullCheckStatusResponseModel.Data.status_code == 100)
                        {
                            status = paybullCheckStatusResponseModel.Data.transaction_status == "Completed" ? (byte)Enums.StatusType.Confirmed : paybullCheckStatusResponseModel.Data.transaction_status == "Failed" ? (byte)Enums.StatusType.Canceled : (byte)Enums.StatusType.Pending;

                            orderId = paybullCheckStatusResponseModel.Data.order_id;
                        }
                        
                        if (status == (byte)Enums.StatusType.Confirmed)
                        {
                            response = "Ödeme İşlemi Başarılı";
                            break; 
                        }
                        else
                            response = paybullCheckStatusResponseModel.Data != null ? 
                                paybullCheckStatusResponseModel.Data.bank_status_description ?? paybullCheckStatusResponseModel.Data.original_bank_error_description : paybullPaymentResponseModel.original_bank_error_description;

                        Thread.Sleep(1000);
                    }

                    if(status == (byte)Enums.StatusType.Confirmed)
                    {
                        var connection = _httpContext.HttpContext.Connection;
                        _creditCardPaymentNotificationManager.SetMemberIPAdress(entity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                        entity.Status = entity.IsAutoNotification ? (byte)StatusType.Confirmed : (byte)StatusType.Pending;
                        entity.MUser = "00000000-0000-0000-0000-000000000000";
                        entity.MDate = DateTime.Now;
                        entity.Description = response;
                        entity.TransactionReferenceCode = orderId;

                        var responseSetStatus = _creditCardPaymentNotificationManager.SetStatus(entity);
                        if (responseSetStatus.Status == "OK")
                        {
                            view.GenericCreditCardPaymentResponseModel.Success = true;
                            view.GenericCreditCardPaymentResponseModel.IsAutoTransaction = entity.IsAutoNotification ? "true" : "false";
                            view.GenericCreditCardPaymentResponseModel.Message = "Ödeme Başarılı";
                            view.GenericCreditCardPaymentResponseModel.TimeStamp = DateTime.Now.ToString();
                            view.GenericCreditCardPaymentResponseModel.RedirectUrl = redirectUrl;

                            if (entity.IsAutoNotification)
                            {
                                var dataCallback = new
                                {
                                    status_code = "OK",
                                    service_id = entity.ServiceID,
                                    status_type = 1,
                                    ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                    data = new { transaction_id = entity.TransactionID, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = "Ödeme İşlemi Başarılı" },
                                    user_entered_data = new { member = entity.Member, sender_name = entity.SenderName, action_date = entity.ActionDate, action_time = entity.ActionTime, creditCard = entity.CardNumber, amount = entity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
                                };

                                var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                                callbackEntity.ServiceType = "STILPAY";
                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                _callbackResponseLogManager.Insert(callbackEntity);
                            }
                        }
                        else
                            return Json(new GenericResponse { Status = "ERROR", Message = responseSetStatus.Message });
                    }
                    else
                    {
                        entity.Status = (byte)StatusType.Canceled;
                        entity.MUser = "00000000-0000-0000-0000-000000000000";
                        entity.MDate = DateTime.Now;
                        entity.Description = response;
                        var responseSetStatus = _creditCardPaymentNotificationManager.SetStatus(entity);
                        if (responseSetStatus.Status == "OK")
                        {
                            view.GenericCreditCardPaymentResponseModel.Success = false;
                            view.GenericCreditCardPaymentResponseModel.IsAutoTransaction = entity.IsAutoNotification ? "true" : "false";
                            view.GenericCreditCardPaymentResponseModel.Message = paybullPaymentResponseModel.original_bank_error_description ?? paybullPaymentResponseModel.error; ;
                            view.GenericCreditCardPaymentResponseModel.TimeStamp = DateTime.Now.ToString("n2");
                            view.GenericCreditCardPaymentResponseModel.RedirectUrl = redirectUrl;

                            var connection = _httpContext.HttpContext.Connection;
                            _creditCardPaymentNotificationManager.SetMemberIPAdress(entity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                            var dataCallback = new
                            {
                                status_code = "ERROR",
                                service_id = entity.ServiceID,
                                status_type = 1,
                                ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                data = new { transaction_id = entity.TransactionID, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = entity.Description },
                                user_entered_data = new { member = entity.Member, sender_name = entity.SenderName, action_date = entity.ActionDate, action_time = entity.ActionTime, creditCard = entity.CardNumber, amount = entity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
                            };

                            var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                            callbackEntity.ServiceType = "STILPAY";
                            callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                            _callbackResponseLogManager.Insert(callbackEntity);
                        }
                    }
                }
                else
                {
                    ContentResult result = new ContentResult
                    {
                        Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{redirectUrl}"),
                        ContentType = "text/html"
                    };

                    return result;
                }

                return View(view);
            }

            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }

        #endregion

        #region AKODE
        public GenericResponse AKODEPaymentMethod()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                var akOdeSanalPOSIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "AKODECreditCard") });

                var expDateMonth = model.CreditCardModel.ExpirationDate[..2];
                var expDateSub = model.CreditCardModel.ExpirationDate[3..];

                var randomGenerator = new Random();
                var rnd = randomGenerator.Next(1, 1000000).ToString();
                var akOdeGetSessionRequestModel = new AKODEGetSessionRequestModel()
                {
                    apiUser = akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_user").ParamVal,
                    clientId = akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "client_id").ParamVal,
                    amount = (long)(model.entity.Amount * 100),
                    orderId = model.entity.TransactionID,
                    Rnd = rnd,
                    timeSpan = DateTime.Now.ToString("yyyyMMddHHmmss")
                };

                akOdeGetSessionRequestModel.Hash = AKODECreateHash.CreateHash(akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_pass").ParamVal, akOdeGetSessionRequestModel.clientId, akOdeGetSessionRequestModel.apiUser, akOdeGetSessionRequestModel.Rnd, akOdeGetSessionRequestModel.timeSpan);

                var akOdeGetSessionResponseModel = AKODEGetSessionRequest.GetSessionRequest(akOdeGetSessionRequestModel);

                if (akOdeGetSessionResponseModel != null && akOdeGetSessionResponseModel.Data != null && akOdeGetSessionResponseModel.Status == "OK")
                {
                    var akOdePaymentRequestModel = new AKODEPaymentRequestModel()
                    {
                        ThreeDSessionId = akOdeGetSessionResponseModel.Data.ThreeDSessionId,
                        CardHolderName = model.CreditCardModel.SenderName,
                        CardNo = model.CreditCardModel.CardNumber.Replace(" ", ""),
                        Cvv = model.CreditCardModel.SecurityCode,
                        ExpireDate = expDateMonth + expDateSub,
                    };

                    var akOdePaymentRequestResponse = AKODEPaymentRequest.PaymentRequest(akOdePaymentRequestModel);

                    if (akOdePaymentRequestResponse != null && !akOdePaymentRequestResponse.Contains("1001"))
                    {
                        model.CreditCardModel.UCD_URL = akOdePaymentRequestResponse;
                        model.CreditCardModel.Description = "Ön Ödeme Süreci Başarılı";

                        model.LastTime = DateTime.Now;
                        _httpContext.HttpContext.Session.Write("Payment_Notification", model);
                        return new GenericResponse { Status = "OK" };
                    }
                    else
                        return new GenericResponse { Status = "ERROR", Message = akOdePaymentRequestResponse };
                }
                else
                    return new GenericResponse { Status = "ERROR", Message = akOdeGetSessionResponseModel.Message };
            }
            catch { }

            return new GenericResponse { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." };

        }

        [HttpGet]
        public IActionResult AKODEThreeDSecure()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                _httpContext.HttpContext.Session.Write("Payment_Notification", model);

                return View(model);

            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }

        [HttpPost]
        public IActionResult AKODEThreeDSecureResult(AKODEPaymentResponseModel akOdePaymentResponseModel)
        {
            try
            {
                var view = new PaymentNotificationViewModel();
                byte status = 0;
                var response = "";
                var orderId = "";
                decimal? paymentInstitutionCommissionRate = 0.0M;
                decimal? paymentInstitutionNetAmount = 0.0M;
                var entity = _creditCardPaymentNotificationManager.GetSingleByTransactionID(akOdePaymentResponseModel.OrderId);
                var callbackEntity = new CallbackResponseLog();
                var opt = new JsonSerializerOptions() { WriteIndented = true };

                var integration = _companyIntegration.GetByServiceId(entity.ServiceID);

                var checkCallback = _callbackResponseLogManager.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("TransactionID", Enums.FieldType.NVarChar, entity.TransactionID),
                    new FieldParameter("TransactionType", Enums.FieldType.NVarChar, "KREDI KARTI ODEMESI RESPONSE")
                });

                callbackEntity.TransactionID = entity.TransactionID;
                callbackEntity.ServiceType = "AKODE";
                callbackEntity.IDCompany = integration.ID;
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(akOdePaymentResponseModel, opt);
                callbackEntity.TransactionType = "KREDI KARTI ODEMESI RESPONSE";
                _callbackResponseLogManager.Insert(callbackEntity);

                if (checkCallback != null)
                {
                    return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
                }

                var getRequestData = _callbackResponseLogManager.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("TransactionID", Enums.FieldType.NVarChar, entity.TransactionID),
                    new FieldParameter("TransactionType", Enums.FieldType.NVarChar, "Get IFrame Request")
                });

                JObject jsonObject = JObject.Parse(getRequestData.Callback);

                var redirectUrl = jsonObject["data"]?["redirectUrl"]?.ToString();

                if (string.IsNullOrEmpty(redirectUrl))
                {
                    redirectUrl = integration.RedirectUrl;
                }

                if (entity.Status != (byte)Enums.StatusType.Pending)
                {
                    ContentResult result = new ContentResult
                    {
                        Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{redirectUrl}"),
                        ContentType = "text/html"
                    };

                    return result;
                }

                if (entity.Status == (byte)Enums.StatusType.Pending)
                {
                    var akOdeSanalPOSIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "AKODECreditCard") });

                    for (int i = 0; i < 5; i++)
                    {
                        var randomGenerator = new Random();
                        var rnd = randomGenerator.Next(1, 1000000).ToString();
                        var akOdeGetTransactionRequestModel = new AKODEGetTransactionRequestModel()
                        {
                            ApiUser = akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_user").ParamVal,
                            ClientId = akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "client_id").ParamVal,
                            Rnd = rnd,
                            TimeSpan = DateTime.Now.ToString("yyyyMMddHHmmss"),
                            TransactionDate = int.Parse(DateTime.Now.ToString("yyyyMMdd")),
                            Page = 1,
                            PageSize = int.MaxValue,
                            OrderId = entity.TransactionID
                        };

                        akOdeGetTransactionRequestModel.Hash = AKODECreateHash.CreateHash(akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_pass").ParamVal, akOdeGetTransactionRequestModel.ClientId, akOdeGetTransactionRequestModel.ApiUser, akOdeGetTransactionRequestModel.Rnd, akOdeGetTransactionRequestModel.TimeSpan);

                        var akOdeTransactionQueryResponseModel = AKODEGetTransactionRequest.GetTransactionList(akOdeGetTransactionRequestModel);

                        //var randomGenerator = new Random();
                        //var rnd = randomGenerator.Next(1, 1000000).ToString();

                        //var akOdeTransactionQueryRequestModel = new AKODETransactionQueryRequestModel()
                        //{
                        //    ApiUser = akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_user").ParamVal,
                        //    ClientId = akOdePaymentResponseModel.ClientId,
                        //    OrderId = akOdePaymentResponseModel.OrderId,
                        //    Rnd = rnd,
                        //    TimeSpan = DateTime.Now.ToString("yyyyMMddHHmmss")
                        //};

                        //akOdeTransactionQueryRequestModel.Hash = AKODECreateHash.CreateHash(akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_pass").ParamVal, akOdeTransactionQueryRequestModel.ClientId, akOdeTransactionQueryRequestModel.ApiUser, akOdeTransactionQueryRequestModel.Rnd, akOdeTransactionQueryRequestModel.TimeSpan);

                        //var akOdeTransactionQueryResponseModel = AKODETransactionQueryRequest.TransactionQueryRequest(akOdeTransactionQueryRequestModel);

                        callbackEntity.TransactionID = entity.TransactionID;
                        callbackEntity.ServiceType = "AKODE";
                        callbackEntity.IDCompany = integration.ID;
                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(akOdeTransactionQueryResponseModel, opt);
                        callbackEntity.TransactionType = "KREDI KARTI ODEMESI QUERY RESPONSE";
                        _callbackResponseLogManager.Insert(callbackEntity);

                        if (akOdeTransactionQueryResponseModel.Data != null && akOdeTransactionQueryResponseModel.Status == "OK")
                        {
                            foreach(var item in akOdeTransactionQueryResponseModel.Data.Transactions.Where(x => x.TransactionType == 1 && x.RequestStatus != 11))
                            {
                                status = item.BankResponseCode == "00" && item.RequestStatus == 1 ? (byte)Enums.StatusType.Confirmed : item.RequestStatus == 0 ? (byte)Enums.StatusType.Canceled : (byte)Enums.StatusType.Pending;
                                paymentInstitutionNetAmount = Convert.ToDecimal(item.NetAmount) / 100;
                                paymentInstitutionCommissionRate = item.MerchantCommissionRate;
                                orderId = item.HostReferenceNumber;

                                response = item.BankResponseMessage;
                            }
                        }

                        if (status == (byte)Enums.StatusType.Confirmed)
                        {
                            break;
                        }

                        Thread.Sleep(1000);
                    }

                    if (status == (byte)Enums.StatusType.Confirmed)
                    {
                        var connection = _httpContext.HttpContext.Connection;
                        _creditCardPaymentNotificationManager.SetMemberIPAdress(entity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                        entity.Status = entity.IsAutoNotification ? (byte)StatusType.Confirmed : (byte)StatusType.Pending;
                        entity.MUser = "00000000-0000-0000-0000-000000000000";
                        entity.MDate = DateTime.Now;
                        entity.Description = response;
                        entity.PaymentInstitutionCommissionRate = paymentInstitutionCommissionRate;
                        entity.PaymentInstitutionNetAmount = paymentInstitutionNetAmount;
                        entity.TransactionReferenceCode = orderId;

                        var responseSetStatus = _creditCardPaymentNotificationManager.SetStatus(entity);
                        if (responseSetStatus.Status == "OK")
                        {
                            view.GenericCreditCardPaymentResponseModel.Success = true;
                            view.GenericCreditCardPaymentResponseModel.IsAutoTransaction = entity.IsAutoNotification ? "true" : "false";
                            view.GenericCreditCardPaymentResponseModel.Message = "Ödeme Başarılı";
                            view.GenericCreditCardPaymentResponseModel.TimeStamp = DateTime.Now.ToString();
                            view.GenericCreditCardPaymentResponseModel.RedirectUrl = redirectUrl;

                            if (entity.IsAutoNotification)
                            {
                                var dataCallback = new
                                {
                                    status_code = "OK",
                                    service_id = entity.ServiceID,
                                    status_type = 1,
                                    ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                    data = new { transaction_id = entity.TransactionID, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = "Ödeme İşlemi Başarılı" },
                                    user_entered_data = new { member = entity.Member, sender_name = entity.SenderName, action_date = entity.ActionDate, action_time = entity.ActionTime, creditCard = entity.CardNumber, amount = entity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
                                };

                                var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                                callbackEntity.ServiceType = "STILPAY";
                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                _callbackResponseLogManager.Insert(callbackEntity);
                            }
                        }
                        else
                            return Json(new GenericResponse { Status = "ERROR", Message = responseSetStatus.Message });
                    }
                    else
                    {
                        entity.Status = (byte)StatusType.Canceled;
                        entity.MUser = "00000000-0000-0000-0000-000000000000";
                        entity.MDate = DateTime.Now;
                        entity.Description = response;
                        var responseSetStatus = _creditCardPaymentNotificationManager.SetStatus(entity);
                        if (responseSetStatus.Status == "OK")
                        {
                            view.GenericCreditCardPaymentResponseModel.Success = false;
                            view.GenericCreditCardPaymentResponseModel.IsAutoTransaction = entity.IsAutoNotification ? "true" : "false";
                            //view.GenericCreditCardPaymentResponseModel.Message = paybullPaymentResponseModel.original_bank_error_description ?? paybullPaymentResponseModel.error; ;
                            view.GenericCreditCardPaymentResponseModel.TimeStamp = DateTime.Now.ToString("n2");
                            view.GenericCreditCardPaymentResponseModel.RedirectUrl = redirectUrl;

                            var connection = _httpContext.HttpContext.Connection;
                            _creditCardPaymentNotificationManager.SetMemberIPAdress(entity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                            var dataCallback = new
                            {
                                status_code = "ERROR",
                                service_id = entity.ServiceID,
                                status_type = 1,
                                ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                data = new { transaction_id = entity.TransactionID, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = entity.Description },
                                user_entered_data = new { member = entity.Member, sender_name = entity.SenderName, action_date = entity.ActionDate, action_time = entity.ActionTime, creditCard = entity.CardNumber, amount = entity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
                            };

                            var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                            callbackEntity.ServiceType = "STILPAY";
                            callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                            _callbackResponseLogManager.Insert(callbackEntity);
                        }
                    }
                }
                else
                {
                    ContentResult result = new ContentResult
                    {
                        Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{redirectUrl}"),
                        ContentType = "text/html"
                    };

                    return result;
                }

                return View(view);
            }

            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }

        #endregion

        #region Tosla
        public GenericResponse ToslaPaymentMethod()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");

                var toslaGetRefCodeRequestModel = new ToslaGetRefCodeRequestModel()
                {
                    amount = model.entity.Amount,
                    phoneNumber = "9" + model.CreditCardModel.PhoneNumber,
                    processId = model.entity.TransactionID
                };

                var toslaGetRefCodeResponseModel = ToslaGetRefCodeRequest.GetRefCode(toslaGetRefCodeRequestModel);

                if(toslaGetRefCodeResponseModel.Status == "OK" && toslaGetRefCodeResponseModel.Data != null )
                    return new GenericResponse { Status = "OK" };
                else
                    return new GenericResponse { Status = "ERROR", Message = toslaGetRefCodeResponseModel.Data.errorMessage ?? toslaGetRefCodeResponseModel.Message };

            }
            catch { }

            return new GenericResponse { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." };
        }

        [HttpGet]
        public IActionResult ToslaFinish()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");


                model.GenericCreditCardPaymentResponseModel.Success = true;
                model.GenericCreditCardPaymentResponseModel.IsAutoTransaction = "false";
                model.GenericCreditCardPaymentResponseModel.Message = "Ön Ödeme Başarılı Onay Bekliyor";
                model.GenericCreditCardPaymentResponseModel.TimeStamp = DateTime.Now.ToString();
                model.GenericCreditCardPaymentResponseModel.RedirectUrl = model.RedirectUrl;

                _httpContext.HttpContext.Session.Write("Payment_Notification", model);
                return View("CreditCardFinish", model);

            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }

        #endregion

        #region EsnekPos
        public GenericResponse EsnekPosPaymentMethod()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                var esnekPosIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "EsnekPos") });

                var expDateMonth = model.CreditCardModel.ExpirationDate[..2];
                var expDateSub = model.CreditCardModel.ExpirationDate[3..];


                var esnekPosPaymentRequest = new EsnekPosPaymentRequestModel()
                {
                    Config = new Config()
                    {
                        ORDER_AMOUNT = model.entity.Amount.ToString().Replace(".", ","),
                        ORDER_REF_NUMBER = model.entity.TransactionID,
                    },
                    CreditCard= new Utility.EsnekPos.Models.EsnekPosPaymentRequest.CreditCard()
                    {
                        CC_CVV = model.CreditCardModel.SecurityCode,
                        CC_NUMBER =  model.CreditCardModel.CardNumber.Replace(" ", ""),
                        CC_OWNER =  model.CreditCardModel.SenderName,
                        EXP_MONTH = expDateMonth,
                        EXP_YEAR = expDateSub,
                        INSTALLMENT_NUMBER = "1",
                    },
                    Customer= new Utility.EsnekPos.Models.EsnekPosPaymentRequest.Customer()
                    {
                        ADDRESS= "ESKİŞEHİR",
                        CITY = "ESKİŞEHİR",
                        FIRST_NAME = "ESKİŞEHİR",
                        LAST_NAME = "ESKİŞEHİR",
                        MAIL = "eskişehir@gmail.com",
                        STATE = "ESKİŞEHİR",
                        PHONE = model.entity.Phone,
                        CLIENT_IP =  _httpContext.HttpContext.Connection.RemoteIpAddress.ToString(),
                    },
                    Product = new Product()
                    {
                       PRODUCT_ID = new Random().Next(1,9999).ToString(),
                       PRODUCT_AMOUNT = model.entity.Amount.ToString().Replace(".", ","),
                       PRODUCT_CATEGORY= "EPIN",
                       PRODUCT_DESCRIPTION = "EPIN",
                       PRODUCT_NAME = "EPIN URUNU"
                    },
                };


                var esnekPosPaymentRequestResponseModel = EsnekPosPaymentRequest.PaymentRequest(esnekPosPaymentRequest);

                if (esnekPosPaymentRequestResponseModel != null && esnekPosPaymentRequestResponseModel.Data != null && esnekPosPaymentRequestResponseModel.Status == "OK")
                {   
                    model.CreditCardModel.UCD_URL = esnekPosPaymentRequestResponseModel.Data.URL_3DS;
                    model.CreditCardModel.Description = "Ön Ödeme Süreci Başarılı";

                    model.LastTime = DateTime.Now;
                    _httpContext.HttpContext.Session.Write("Payment_Notification", model);
                    return new GenericResponse { Status = "OK" };
                }
                else
                    return new GenericResponse { Status = "ERROR", Message = esnekPosPaymentRequestResponseModel.Message };
            }
            catch { }

            return new GenericResponse { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." };

        }

        [HttpGet]
        public IActionResult EsnekPosThreeDSecure()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                _httpContext.HttpContext.Session.Write("Payment_Notification", model);

                return View(model);

            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
            }

        [HttpPost]
        public IActionResult EsnekPosThreeDSecureResult(EsnekPosThreeDResultResponseModel esnekPosThreeDResultResponseModel)
        {
            try
            {
                var view = new PaymentNotificationViewModel();
                byte status = 0;
                var response = "";
                var refNo = "";
                decimal? paymentInstitutionCommissionRate = 0.0M;
                decimal? paymentInstitutionNetAmount = 0.0M;
                var entity = _creditCardPaymentNotificationManager.GetSingleByTransactionID(esnekPosThreeDResultResponseModel.ORDER_REF_NUMBER);
                var callbackEntity = new CallbackResponseLog();
                var opt = new JsonSerializerOptions() { WriteIndented = true };

                var integration = _companyIntegration.GetByServiceId(entity.ServiceID);

                var checkCallback = _callbackResponseLogManager.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("TransactionID", Enums.FieldType.NVarChar, entity.TransactionID),
                    new FieldParameter("TransactionType", Enums.FieldType.NVarChar, "KREDI KARTI ODEMESI RESPONSE")
                });

                callbackEntity.TransactionID = entity.TransactionID;
                callbackEntity.ServiceType = "EsnekPos";
                callbackEntity.IDCompany = integration.ID;
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(esnekPosThreeDResultResponseModel, opt);
                callbackEntity.TransactionType = "KREDI KARTI ODEMESI RESPONSE";
                _callbackResponseLogManager.Insert(callbackEntity);

                if (checkCallback != null)
                {
                    return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
                }

                var getRequestData = _callbackResponseLogManager.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("TransactionID", Enums.FieldType.NVarChar, entity.TransactionID),
                    new FieldParameter("TransactionType", Enums.FieldType.NVarChar, "Get IFrame Request")
                });

                JObject jsonObject = JObject.Parse(getRequestData.Callback);

                var redirectUrl = jsonObject["data"]?["redirectUrl"]?.ToString();

                if (string.IsNullOrEmpty(redirectUrl))
                {
                    redirectUrl = integration.RedirectUrl;
                }
                
                if (entity.Status != (byte)Enums.StatusType.Pending)
                {
                    ContentResult result = new ContentResult
                    {
                        Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{redirectUrl}"),
                        ContentType = "text/html"
                    };

                    return result;
                }

                if (entity.Status == (byte)Enums.StatusType.Pending)
                {
                    var esnekPosIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "EsnekPos") });

                    for (int i = 0; i < 5; i++)
                    {
                        var esnekPosTransactionQueryRequestModel = new EsnekPosTransactionQueryRequestModel()
                        {
                            ORDER_REF_NUMBER = entity.TransactionID
                        };

                        var esnekPosTransactionQueryRequestResponseModel = EsnekPosTransactionQueryRequest.TransactionQueryRequest(esnekPosTransactionQueryRequestModel);

                        callbackEntity.TransactionID = entity.TransactionID;
                        callbackEntity.ServiceType = "EsnekPos";
                        callbackEntity.IDCompany = integration.ID;
                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(esnekPosTransactionQueryRequestResponseModel, opt);
                        callbackEntity.TransactionType = "KREDI KARTI ODEMESI QUERY RESPONSE";
                        _callbackResponseLogManager.Insert(callbackEntity);

                        if (esnekPosTransactionQueryRequestResponseModel.Data != null && esnekPosTransactionQueryRequestResponseModel.Data.TRANSACTIONS.Count > 0 && esnekPosTransactionQueryRequestResponseModel.Status == "OK")
                        {
                            foreach (var item in esnekPosTransactionQueryRequestResponseModel.Data.TRANSACTIONS)
                            {
                                status = item.STATUS_ID == 3 && item.STATUS_NAME == "Ödeme - Başarılı" ? (byte)Enums.StatusType.Confirmed : item.STATUS_ID == 4 ? (byte)Enums.StatusType.Canceled : (byte)Enums.StatusType.Pending;
                                paymentInstitutionNetAmount = Convert.ToDecimal(item.AMOUNT) - Convert.ToDecimal(esnekPosThreeDResultResponseModel.COMMISSION);
                                paymentInstitutionCommissionRate = decimal.Parse(esnekPosThreeDResultResponseModel.COMMISSION_RATE);
                                refNo = esnekPosTransactionQueryRequestResponseModel.Data.REFNO;
                                response = item.STATUS_NAME;
                            }
                        }
                        else
                        {
                            var xmlStartHasValue = true;
                            int xmlStartIndex = esnekPosTransactionQueryRequestResponseModel.Message.IndexOf("<?xml");

                            if (xmlStartIndex == -1)
                            {
                                response = "Ödeme Başarısız";
                                xmlStartHasValue = false;
                            }

                            if (xmlStartHasValue)
                            {
                                string xmlData = esnekPosTransactionQueryRequestResponseModel.Message.Substring(xmlStartIndex);

                                // XML verisini ayrıştırma
                                XDocument xmlDoc = XDocument.Parse(xmlData);

                                var errMsg = xmlDoc.Root.Element("ErrMsg")?.Value;
                                response = errMsg;
                            }
                        }


                        if (status == (byte)Enums.StatusType.Confirmed)
                        {
                            break;
                        }

                        Thread.Sleep(1000);
                    }

                    if (status == (byte)Enums.StatusType.Confirmed)
                    {
                        var connection = _httpContext.HttpContext.Connection;
                        _creditCardPaymentNotificationManager.SetMemberIPAdress(entity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                        entity.Status = entity.IsAutoNotification ? (byte)StatusType.Confirmed : (byte)StatusType.Pending;
                        entity.MUser = "00000000-0000-0000-0000-000000000000";
                        entity.MDate = DateTime.Now;
                        entity.Description = response;
                        entity.PaymentInstitutionCommissionRate = paymentInstitutionCommissionRate;
                        entity.PaymentInstitutionNetAmount = paymentInstitutionNetAmount;
                        entity.TransactionReferenceCode = refNo;

                        var responseSetStatus = _creditCardPaymentNotificationManager.SetStatus(entity);
                        if (responseSetStatus.Status == "OK")
                        {
                            view.GenericCreditCardPaymentResponseModel.Success = true;
                            view.GenericCreditCardPaymentResponseModel.IsAutoTransaction = entity.IsAutoNotification ? "true" : "false";
                            view.GenericCreditCardPaymentResponseModel.Message = "Ödeme Başarılı";
                            view.GenericCreditCardPaymentResponseModel.TimeStamp = DateTime.Now.ToString();
                            view.GenericCreditCardPaymentResponseModel.RedirectUrl = redirectUrl;

                            if (entity.IsAutoNotification)
                            {
                                var dataCallback = new
                                {
                                    status_code = "OK",
                                    service_id = entity.ServiceID,
                                    status_type = 1,
                                    ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                    data = new { transaction_id = entity.TransactionID, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = "Ödeme İşlemi Başarılı" },
                                    user_entered_data = new { member = entity.Member, sender_name = entity.SenderName, action_date = entity.ActionDate, action_time = entity.ActionTime, creditCard = entity.CardNumber, amount = entity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
                                };

                                var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                                callbackEntity.ServiceType = "STILPAY";
                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                _callbackResponseLogManager.Insert(callbackEntity);
                            }
                        }
                        else
                            return Json(new GenericResponse { Status = "ERROR", Message = responseSetStatus.Message });
                    }
                    else
                    {
                        entity.Status = (byte)StatusType.Canceled;
                        entity.MUser = "00000000-0000-0000-0000-000000000000";
                        entity.MDate = DateTime.Now;
                        entity.Description = response;
                        var responseSetStatus = _creditCardPaymentNotificationManager.SetStatus(entity);
                        if (responseSetStatus.Status == "OK")
                        {
                            view.GenericCreditCardPaymentResponseModel.Success = false;
                            view.GenericCreditCardPaymentResponseModel.IsAutoTransaction = entity.IsAutoNotification ? "true" : "false";
                            //view.GenericCreditCardPaymentResponseModel.Message = paybullPaymentResponseModel.original_bank_error_description ?? paybullPaymentResponseModel.error;
                            view.GenericCreditCardPaymentResponseModel.Message = response;
                            view.GenericCreditCardPaymentResponseModel.TimeStamp = DateTime.Now.ToString();
                            view.GenericCreditCardPaymentResponseModel.RedirectUrl = redirectUrl;

                            var connection = _httpContext.HttpContext.Connection;
                            _creditCardPaymentNotificationManager.SetMemberIPAdress(entity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                            var dataCallback = new
                            {
                                status_code = "ERROR",
                                service_id = entity.ServiceID,
                                status_type = 1,
                                ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                data = new { transaction_id = entity.TransactionID, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = entity.Description },
                                user_entered_data = new { member = entity.Member, sender_name = entity.SenderName, action_date = entity.ActionDate, action_time = entity.ActionTime, creditCard = entity.CardNumber, amount = entity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
                            };

                            var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                            callbackEntity.ServiceType = "STILPAY";
                            callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                            _callbackResponseLogManager.Insert(callbackEntity);
                        }
                    }
                }
                else
                {
                    ContentResult result = new ContentResult
                    {
                        Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{redirectUrl}"),
                        ContentType = "text/html"
                    };

                    return result;
                }

                return View(view);
            }

            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }
        #endregion

        #region LidioPos
        public GenericResponse LidioPosPaymentMethod()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");

                var expDateMonth = model.CreditCardModel.ExpirationDate[..2];
                var expDateYear = DateTime.Now.Year.ToString()[..2] + model.CreditCardModel.ExpirationDate[3..];

                var lidioPosPaymentRequestModel = new LidioPosPaymentRequestModel()
                {
                    orderId = model.entity.TransactionID,
                    totalAmount = (double)model.entity.Amount,
                    customerInfo = new Utility.LidioPos.Models.LidioPosPaymentRequest.CustomerInfo()
                    {
                        customerId = model.entity.TransactionID
                    },
                    paymentInstrument = "NewCard",
                    paymentInstrumentInfo = new Utility.LidioPos.Models.LidioPosPaymentRequest.PaymentInstrumentInfo()
                    {
                        newCard = new NewCard()
                        {
                            processType = "sales",
                            cardInfo = new Utility.LidioPos.Models.LidioPosPaymentRequest.CardInfo()
                            {
                                cardNumber = model.CreditCardModel.CardNumber.Replace(" ", ""),
                                cardHolderName = model.CreditCardModel.SenderName,
                                lastMonth = expDateMonth,
                                lastYear = expDateYear
                            },
                            use3DSecure = true,
                            loyaltyPointUsage = "None"
                        }
                    },
                    basketItems = new Utility.LidioPos.Models.LidioPosPaymentRequest.BasketItem()
                    {
                        name = "epin",
                        quantity = 1,
                        unitPrice = 1
                    },
                    currency = model.CurrencyCode
                };

                var lidioPosPaymentRequestResponseModel = LidioPosPaymentRequest.PaymentRequest(lidioPosPaymentRequestModel, model.IsForeignCreditCard);

                if (lidioPosPaymentRequestResponseModel != null && lidioPosPaymentRequestResponseModel.Data != null && lidioPosPaymentRequestResponseModel.Status == "OK")
                {
                    model.CreditCardModel.UCD_URL = lidioPosPaymentRequestResponseModel.Data.redirectForm;
                    model.CreditCardModel.Description = "Ön Ödeme Süreci Başarılı";

                    model.LastTime = DateTime.Now;
                    _httpContext.HttpContext.Session.Write("Payment_Notification", model);
                    return new GenericResponse { Status = "OK" };
                }
                else
                    return new GenericResponse { Status = "ERROR", Message = lidioPosPaymentRequestResponseModel.Message };
            }
            catch { }

            return new GenericResponse { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." };

        }

        [HttpGet]
        public IActionResult LidioPosThreeDSecure()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                _httpContext.HttpContext.Session.Write("Payment_Notification", model);

                return View(model);
            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }

        [HttpGet]
        public IActionResult LidioPosThreeDSecureResult([FromQuery] string OrderId, [FromQuery] string SystemTransId, [FromQuery] string Result, [FromQuery] decimal TotalAmount, [FromQuery] int InstallmentCount, [FromQuery] string Hash, [FromQuery] int MDStatus)
        {
            try
            {
                var entity = _creditCardPaymentNotificationManager.GetSingleByTransactionID(OrderId);

                if (entity != null)
                {
                    var lidioPosIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "LidioPos") });

                    string hashData = OrderId + ":" + lidioPosIntegrationValues.FirstOrDefault(f => f.ParamDef == "merchant_key").ParamVal + ":" + String.Format(new CultureInfo("en-us"), "{0:#0.00}", TotalAmount) + ":" + Result + ":" + OrderId;

                    SHA256CryptoServiceProvider cryptoServiceProvider = new SHA256CryptoServiceProvider();
                    string spHash = Convert.ToBase64String(cryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(hashData)));

                    if (spHash != Hash)
                        return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });

                    var view = new PaymentNotificationViewModel();
                    byte status = 0;
                    var response = "";
                    var refNo = "";
                    var lidioSenderName = "";
                    decimal? paymentInstitutionCommissionRate = 0.0M;
                    decimal? paymentInstitutionNetAmount = 0.0M;
                    var callbackEntity = new CallbackResponseLog();
                    var opt = new JsonSerializerOptions() { WriteIndented = true };

                    var integration = _companyIntegration.GetByServiceId(entity.ServiceID);

                    var checkCallback = _callbackResponseLogManager.GetSingle(new List<FieldParameter>()
                    {
                        new FieldParameter("TransactionID", Enums.FieldType.NVarChar, entity.TransactionID),
                        new FieldParameter("TransactionType", Enums.FieldType.NVarChar, "KREDI KARTI ODEMESI ON RESPONSE")
                    });

                    var paymentResponse = new
                    {
                        OrderId,
                        SystemTransId,
                        Result,
                        TotalAmount,
                        InstallmentCount,
                        Hash,
                        MDStatus
                    };

                    callbackEntity.TransactionID = entity.TransactionID;
                    callbackEntity.ServiceType = "LidioPos";
                    callbackEntity.IDCompany = integration.ID;
                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(paymentResponse, opt);
                    callbackEntity.TransactionType = "KREDI KARTI ODEMESI ON RESPONSE";
                    _callbackResponseLogManager.Insert(callbackEntity);

                    if (checkCallback != null)
                    {
                        return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
                    }

                    var getRequestData = _callbackResponseLogManager.GetSingle(new List<FieldParameter>()
                    {
                        new FieldParameter("TransactionID", Enums.FieldType.NVarChar, entity.TransactionID),
                        new FieldParameter("TransactionType", Enums.FieldType.NVarChar, "Get IFrame Request")
                    });

                    JObject jsonObject = JObject.Parse(getRequestData.Callback);

                    var redirectUrl = jsonObject["data"]?["redirectUrl"]?.ToString();

                    if (string.IsNullOrEmpty(redirectUrl))
                    {
                        redirectUrl = integration.RedirectUrl;
                    }
                    

                    if (entity.Status != (byte)Enums.StatusType.Pending)
                    {
                        ContentResult result = new ContentResult
                        {
                            Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{redirectUrl}"),
                            ContentType = "text/html"
                        };

                        return result;
                    }

                    if (entity.Status == (byte)Enums.StatusType.Pending)
                    {
                        var lidioPosFinishPaymentRequestModel = new LidioPosFinishPaymentRequestModel()
                        {
                            orderId = OrderId,
                            systemTransId = SystemTransId,
                            paymentInstrument = "NewCard",
                            paymentInstrumentInfo = new Utility.LidioPos.Models.LidioPosFinishPaymentRequestModel.PaymentInstrumentInfo()
                            {
                                newCard = new NewCardInfo()
                                {
                                    posAccount = new PosAccountInfo()
                                    {
                                        id = 0
                                    }
                                }
                            },
                            totalAmount = (double)TotalAmount,
                            currency = "TRY"
                        };

                        var lidioPosFinishPaymentRequestResponseModel = LidioPosFinishPaymentRequest.FinishPaymentRequest(lidioPosFinishPaymentRequestModel);

                        callbackEntity = new CallbackResponseLog();
                        opt = new JsonSerializerOptions() { WriteIndented = true };

                        callbackEntity.TransactionID = entity.TransactionID;
                        callbackEntity.ServiceType = "LidioPos";
                        callbackEntity.IDCompany = integration.ID;
                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(lidioPosFinishPaymentRequestResponseModel, opt);
                        callbackEntity.TransactionType = "KREDI KARTI ODEMESI FINISH RESPONSE";
                        _callbackResponseLogManager.Insert(callbackEntity);

                        if (lidioPosFinishPaymentRequestResponseModel.Data != null && lidioPosFinishPaymentRequestResponseModel.Status == "OK" && lidioPosFinishPaymentRequestResponseModel.Data.result == "Success")
                        {
                            status = (byte)Enums.StatusType.Confirmed;
                            paymentInstitutionNetAmount = (decimal?)lidioPosFinishPaymentRequestResponseModel.Data.paymentInfo.paybackTransactionList[0].paybackAmount;
                            paymentInstitutionCommissionRate = (decimal?)lidioPosFinishPaymentRequestResponseModel.Data.paymentInfo.paybackTransactionList[0].bankTotalCommissionRate;
                            refNo = lidioPosFinishPaymentRequestResponseModel.Data.paymentInfo.systemTransId;
                            response = lidioPosFinishPaymentRequestResponseModel.Data.paymentInfo.resultCategory.categoryName;
                            lidioSenderName = lidioPosFinishPaymentRequestResponseModel.Data.paymentInfo.acquirerResultDetail.pos.cardHolderNameFromBank;
                        }
                        else
                        {
                            status = (byte)Enums.StatusType.Canceled;
                            response = lidioPosFinishPaymentRequestResponseModel.Data.paymentInfo.resultCategory.recommendedUIMessageTR ?? lidioPosFinishPaymentRequestResponseModel.Data.paymentInfo.resultCategory.categoryName;
                        }

                        //if (lidioPosFinishPaymentRequestResponseModel.Status == "OK" && lidioPosFinishPaymentRequestResponseModel.Data != null)
                        //{
                        //    for (int i = 0; i < 5; i++)
                        //    {
                        //        var lidioPosTransactionQueryRequestModel = new LidioPosTransactionQueryRequestModel()
                        //        {
                        //            orderId = OrderId,
                        //            paymentInstrument = "NewCard",
                        //            totalAmount = TotalAmount,
                        //            paymentInquiryInstrumentInfo = new PaymentInquiryInstrumentInfo()
                        //            {
                        //                Card = new PaymentInquiryCard()
                        //                {
                        //                    ProcessType = "sales"
                        //                }
                        //            }
                        //        };

                        //        var lidioPosTransactionQueryRequestResponseModel = LidioPosTransactionQuery.TransactionQueryRequest(lidioPosTransactionQueryRequestModel);

                        //        callbackEntity.TransactionID = entity.TransactionID;
                        //        callbackEntity.ServiceType = "LidioPos";
                        //        callbackEntity.IDCompany = integration.ID;
                        //        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(lidioPosTransactionQueryRequestResponseModel, opt);
                        //        callbackEntity.TransactionType = "KREDI KARTI ODEMESI QUERY RESPONSE";
                        //        _callbackResponseLogManager.Insert(callbackEntity);

                        //        if (lidioPosTransactionQueryRequestResponseModel.Data != null && lidioPosTransactionQueryRequestResponseModel.Status == "OK")
                        //        {
                        //            status = (byte)Enums.StatusType.Confirmed;
                        //            paymentInstitutionNetAmount = (decimal?)lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.PaybackTransactionList[0].PaybackAmount;
                        //            paymentInstitutionCommissionRate = (decimal?)lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.PaybackTransactionList[0].BankTotalCommissionRate;
                        //            refNo = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.SystemTransId;
                        //            response = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.ResultCategory.CategoryName;
                        //            lidioSenderName = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.AcquirerResultDetail.Pos.CardHolderNameFromBank;
                        //        }

                        //        if (status != (byte)Enums.StatusType.Confirmed && lidioPosTransactionQueryRequestResponseModel.Data != null && lidioPosTransactionQueryRequestResponseModel.Status == "ERROR")
                        //        {
                        //            status = (byte)Enums.StatusType.Canceled;
                        //            response = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.ResultCategory.RecommendedUIMessageTR ?? lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.ResultCategory.CategoryName;
                        //        }

                        //        if (status == (byte)Enums.StatusType.Confirmed)
                        //        {
                        //            break;
                        //        }

                        //        Thread.Sleep(1000);
                        //    }
                        //}

                        if (status == (byte)Enums.StatusType.Confirmed)
                        {
                            var connection = _httpContext.HttpContext.Connection;
                            _creditCardPaymentNotificationManager.SetMemberIPAdress(entity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                            entity.Status = entity.IsAutoNotification ? (byte)StatusType.Confirmed : (byte)StatusType.Pending;
                            entity.MUser = "00000000-0000-0000-0000-000000000000";
                            entity.MDate = DateTime.Now;
                            entity.Description = lidioSenderName == "" ? response : response + " Banka Gönderici Adı: " + lidioSenderName;
                            entity.PaymentInstitutionCommissionRate = paymentInstitutionCommissionRate;
                            entity.PaymentInstitutionNetAmount = paymentInstitutionNetAmount;
                            entity.TransactionReferenceCode = refNo;

                            var responseSetStatus = _creditCardPaymentNotificationManager.SetStatus(entity);
                            if (responseSetStatus.Status == "OK")
                            {
                                view.GenericCreditCardPaymentResponseModel.Success = true;
                                view.GenericCreditCardPaymentResponseModel.IsAutoTransaction = entity.IsAutoNotification ? "true" : "false";
                                view.GenericCreditCardPaymentResponseModel.Message = "Ödeme Başarılı";
                                view.GenericCreditCardPaymentResponseModel.TimeStamp = DateTime.Now.ToString();
                                view.GenericCreditCardPaymentResponseModel.RedirectUrl = redirectUrl;

                                if (entity.IsAutoNotification)
                                {
                                    var dataCallback = new
                                    {
                                        status_code = "OK",
                                        service_id = entity.ServiceID,
                                        status_type = 1,
                                        ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                        data = new { transaction_id = entity.TransactionID, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = "Ödeme İşlemi Başarılı" },
                                        user_entered_data = new { member = entity.Member, sender_name = entity.SenderName, action_date = entity.ActionDate, action_time = entity.ActionTime, creditCard = entity.CardNumber, amount = entity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString(), cardSenderNameFromBank = lidioSenderName }
                                    };

                                    var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                                    callbackEntity.ServiceType = "STILPAY";
                                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                    callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                    _callbackResponseLogManager.Insert(callbackEntity);
                                }
                            }
                            else
                                return Json(new GenericResponse { Status = "ERROR", Message = responseSetStatus.Message });
                        }

                        else
                        {
                            if (response == "")
                                response = lidioPosFinishPaymentRequestResponseModel.Data.paymentInfo.resultCategory.recommendedUIMessageTR ?? lidioPosFinishPaymentRequestResponseModel.Data.paymentInfo.resultCategory.categoryName;


                            entity.Status = (byte)StatusType.Canceled;
                            entity.MUser = "00000000-0000-0000-0000-000000000000";
                            entity.MDate = DateTime.Now;
                            entity.Description = response;
                            var responseSetStatus = _creditCardPaymentNotificationManager.SetStatus(entity);
                            if (responseSetStatus.Status == "OK")
                            {
                                view.GenericCreditCardPaymentResponseModel.Success = false;
                                view.GenericCreditCardPaymentResponseModel.IsAutoTransaction = entity.IsAutoNotification ? "true" : "false";
                                //view.GenericCreditCardPaymentResponseModel.Message = paybullPaymentResponseModel.original_bank_error_description ?? paybullPaymentResponseModel.error;
                                view.GenericCreditCardPaymentResponseModel.Message = response;
                                view.GenericCreditCardPaymentResponseModel.TimeStamp = DateTime.Now.ToString();
                                view.GenericCreditCardPaymentResponseModel.RedirectUrl = redirectUrl;

                                var connection = _httpContext.HttpContext.Connection;
                                _creditCardPaymentNotificationManager.SetMemberIPAdress(entity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                                var dataCallback = new
                                {
                                    status_code = "ERROR",
                                    service_id = entity.ServiceID,
                                    status_type = 1,
                                    ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                    data = new { transaction_id = entity.TransactionID, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = entity.Description },
                                    user_entered_data = new { member = entity.Member, sender_name = entity.SenderName, action_date = entity.ActionDate, action_time = entity.ActionTime, creditCard = entity.CardNumber, amount = entity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
                                };

                                var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                callbackEntity.ServiceType = "STILPAY";
                                callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                _callbackResponseLogManager.Insert(callbackEntity);
                            }
                        }
                    }
                    else
                    {
                        ContentResult result = new ContentResult
                        {
                            Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{redirectUrl}"),
                            ContentType = "text/html"
                        };

                        return result;
                    }

                    return View(view);
                }

                else
                {
                    var foreignEntity = _foreignCreditCardPaymentNotificationManager.GetSingleByTransactionID(OrderId);

                    var lidioPosYDIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "LidioPosYD") });

                    string hashData = OrderId + ":" + lidioPosYDIntegrationValues.FirstOrDefault(f => f.ParamDef == "merchant_key").ParamVal + ":" + String.Format(new CultureInfo("en-us"), "{0:#0.00}", TotalAmount) + ":" + Result + ":" + OrderId;

                    SHA256CryptoServiceProvider cryptoServiceProvider = new SHA256CryptoServiceProvider();
                    string spHash = Convert.ToBase64String(cryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(hashData)));

                    if (spHash != Hash)
                        return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });

                    var view = new PaymentNotificationViewModel();
                    byte status = 0;
                    var response = "";
                    var refNo = "";
                    var lidioSenderName = "";
                    decimal? paymentInstitutionCommissionRate = 0.0M;
                    decimal? paymentInstitutionNetAmount = 0.0M;
                    var callbackEntity = new CallbackResponseLog();
                    var opt = new JsonSerializerOptions() { WriteIndented = true };

                    var integration = _companyIntegration.GetByServiceId(foreignEntity.ServiceID);

                    var checkCallback = _callbackResponseLogManager.GetSingle(new List<FieldParameter>()
                    {
                        new FieldParameter("TransactionID", Enums.FieldType.NVarChar, foreignEntity.TransactionID),
                        new FieldParameter("TransactionType", Enums.FieldType.NVarChar, "KREDI KARTI ODEMESI ON RESPONSE")
                    });

                    var paymentResponse = new
                    {
                        OrderId,
                        SystemTransId,
                        Result,
                        TotalAmount,
                        InstallmentCount,
                        Hash,
                        MDStatus
                    };

                    callbackEntity.TransactionID = foreignEntity.TransactionID;
                    callbackEntity.ServiceType = "LidioPosYD";
                    callbackEntity.IDCompany = integration.ID;
                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(paymentResponse, opt);
                    callbackEntity.TransactionType = "KREDI KARTI ODEMESI ON RESPONSE";
                    _callbackResponseLogManager.Insert(callbackEntity);

                    if (checkCallback != null)
                    {
                        return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
                    }

                    var getRequestData = _callbackResponseLogManager.GetSingle(new List<FieldParameter>()
                    {
                        new FieldParameter("TransactionID", Enums.FieldType.NVarChar, foreignEntity.TransactionID),
                        new FieldParameter("TransactionType", Enums.FieldType.NVarChar, "Get IFrame Request")
                    });

                    JObject jsonObject = JObject.Parse(getRequestData.Callback);

                    var redirectUrl = jsonObject["data"]?["redirectUrl"]?.ToString();

                    if (string.IsNullOrEmpty(redirectUrl))
                    {
                        redirectUrl = integration.RedirectUrl;
                    }

                    if (foreignEntity.Status != (byte)Enums.StatusType.Pending)
                    {
                        ContentResult result = new ContentResult
                        {
                            Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{redirectUrl}"),
                            ContentType = "text/html"
                        };

                        return result;
                    }

                    if (foreignEntity.Status == (byte)Enums.StatusType.Pending)
                    {
                        var lidioPosFinishPaymentRequestModel = new LidioPosFinishPaymentRequestModel()
                        {
                            orderId = OrderId,
                            systemTransId = SystemTransId,
                            paymentInstrument = "NewCard",
                            paymentInstrumentInfo = new Utility.LidioPos.Models.LidioPosFinishPaymentRequestModel.PaymentInstrumentInfo()
                            {
                                newCard = new NewCardInfo()
                                {
                                    posAccount = new PosAccountInfo()
                                    {
                                        id = 0
                                    }
                                }
                            },
                            totalAmount = (double)TotalAmount,
                            currency = foreignEntity.CurrencyCode
                        };

                        var lidioPosFinishPaymentRequestResponseModel = LidioPosFinishPaymentRequest.FinishPaymentRequest(lidioPosFinishPaymentRequestModel, true);

                        callbackEntity = new CallbackResponseLog();
                        opt = new JsonSerializerOptions() { WriteIndented = true };

                        callbackEntity.TransactionID = foreignEntity.TransactionID;
                        callbackEntity.ServiceType = "LidioPosYD";
                        callbackEntity.IDCompany = integration.ID;
                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(lidioPosFinishPaymentRequestResponseModel, opt);
                        callbackEntity.TransactionType = "KREDI KARTI ODEMESI FINISH RESPONSE";
                        _callbackResponseLogManager.Insert(callbackEntity);

                        if (lidioPosFinishPaymentRequestResponseModel.Status == "OK" && lidioPosFinishPaymentRequestResponseModel.Data != null)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                var lidioPosTransactionQueryRequestModel = new LidioPosTransactionQueryRequestModel()
                                {
                                    orderId = OrderId,
                                    paymentInstrument = "NewCard",
                                    totalAmount = TotalAmount,
                                    paymentInquiryInstrumentInfo = new PaymentInquiryInstrumentInfo()
                                    {
                                        Card = new PaymentInquiryCard()
                                        {
                                            ProcessType = "sales"
                                        }
                                    }
                                };

                                var lidioPosTransactionQueryRequestResponseModel = LidioPosTransactionQuery.TransactionQueryRequest(lidioPosTransactionQueryRequestModel, true);

                                callbackEntity.TransactionID = foreignEntity.TransactionID;
                                callbackEntity.ServiceType = "LidioPosYD";
                                callbackEntity.IDCompany = integration.ID;
                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(lidioPosTransactionQueryRequestResponseModel, opt);
                                callbackEntity.TransactionType = "KREDI KARTI ODEMESI QUERY RESPONSE";
                                _callbackResponseLogManager.Insert(callbackEntity);

                                if (lidioPosTransactionQueryRequestResponseModel.Data != null && lidioPosTransactionQueryRequestResponseModel.Status == "OK")
                                {
                                    status = (byte)Enums.StatusType.Confirmed;
                                    paymentInstitutionNetAmount = (decimal?)lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.PaybackTransactionList[0].PaybackAmount;
                                    paymentInstitutionCommissionRate = (decimal?)lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.PaybackTransactionList[0].BankTotalCommissionRate;
                                    refNo = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.SystemTransId;
                                    response = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.ResultCategory.CategoryName;
                                    lidioSenderName = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.AcquirerResultDetail.Pos.CardHolderNameFromBank;
                                }

                                if (status != (byte)Enums.StatusType.Confirmed && lidioPosTransactionQueryRequestResponseModel.Data != null && lidioPosTransactionQueryRequestResponseModel.Status == "ERROR")
                                {
                                    status = (byte)Enums.StatusType.Canceled;
                                    response = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.ResultCategory.RecommendedUIMessageTR ?? lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.ResultCategory.CategoryName;
                                }

                                if (status == (byte)Enums.StatusType.Confirmed)
                                {
                                    break;
                                }

                                Thread.Sleep(1000);
                            }
                        }

                        if (status == (byte)Enums.StatusType.Confirmed)
                        {
                            var connection = _httpContext.HttpContext.Connection;
                            _foreignCreditCardPaymentNotificationManager.SetMemberIPAdress(foreignEntity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                            foreignEntity.Status = foreignEntity.IsAutoNotification ? (byte)StatusType.Confirmed : (byte)StatusType.Pending;
                            foreignEntity.MUser = "00000000-0000-0000-0000-000000000000";
                            foreignEntity.MDate = DateTime.Now;
                            foreignEntity.Description = lidioSenderName == "" ? response : response + " Banka Gönderici Adı: " + lidioSenderName;
                            foreignEntity.PaymentInstitutionCommissionRate = paymentInstitutionCommissionRate;
                            foreignEntity.PaymentInstitutionNetAmount = paymentInstitutionNetAmount;
                            foreignEntity.TransactionReferenceCode = refNo;

                            var responseSetStatus = _foreignCreditCardPaymentNotificationManager.SetStatus(foreignEntity);
                            if (responseSetStatus.Status == "OK")
                            {
                                view.GenericCreditCardPaymentResponseModel.Success = true;
                                view.GenericCreditCardPaymentResponseModel.IsAutoTransaction = foreignEntity.IsAutoNotification ? "true" : "false";
                                view.GenericCreditCardPaymentResponseModel.Message = "Ödeme Başarılı";
                                view.GenericCreditCardPaymentResponseModel.TimeStamp = DateTime.Now.ToString();
                                view.GenericCreditCardPaymentResponseModel.RedirectUrl = redirectUrl;

                                if (foreignEntity.IsAutoNotification)
                                {
                                    var dataCallback = new
                                    {
                                        status_code = "OK",
                                        service_id = foreignEntity.ServiceID,
                                        status_type = 1,
                                        ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                        data = new { transaction_id = foreignEntity.TransactionID, sp_transactionNr = foreignEntity.TransactionNr, amount = foreignEntity.Amount, sp_id = foreignEntity.ID, message = "Ödeme İşlemi Başarılı", currencyCode = foreignEntity.CurrencyCode },
                                        user_entered_data = new { member = foreignEntity.Member, sender_name = foreignEntity.SenderName, action_date = foreignEntity.ActionDate, action_time = foreignEntity.ActionTime, creditCard = foreignEntity.CardNumber, amount = foreignEntity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString(), cardSenderNameFromBank = lidioSenderName }
                                    };

                                    var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                                    callbackEntity.ServiceType = "STILPAY";
                                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                    callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                    _callbackResponseLogManager.Insert(callbackEntity);
                                }
                            }
                            else
                                return Json(new GenericResponse { Status = "ERROR", Message = responseSetStatus.Message });
                        }
                        else
                        {
                            if (response == "")
                                response = lidioPosFinishPaymentRequestResponseModel.Data.paymentInfo.resultCategory.recommendedUIMessageTR ?? lidioPosFinishPaymentRequestResponseModel.Data.paymentInfo.resultCategory.categoryName;


                            foreignEntity.Status = (byte)StatusType.Canceled;
                            foreignEntity.MUser = "00000000-0000-0000-0000-000000000000";
                            foreignEntity.MDate = DateTime.Now;
                            foreignEntity.Description = response;
                            var responseSetStatus = _foreignCreditCardPaymentNotificationManager.SetStatus(foreignEntity);
                            if (responseSetStatus.Status == "OK")
                            {
                                view.GenericCreditCardPaymentResponseModel.Success = false;
                                view.GenericCreditCardPaymentResponseModel.IsAutoTransaction = foreignEntity.IsAutoNotification ? "true" : "false";
                                //view.GenericCreditCardPaymentResponseModel.Message = paybullPaymentResponseModel.original_bank_error_description ?? paybullPaymentResponseModel.error;
                                view.GenericCreditCardPaymentResponseModel.Message = response;
                                view.GenericCreditCardPaymentResponseModel.TimeStamp = DateTime.Now.ToString();
                                view.GenericCreditCardPaymentResponseModel.RedirectUrl = redirectUrl;

                                var connection = _httpContext.HttpContext.Connection;
                                _foreignCreditCardPaymentNotificationManager.SetMemberIPAdress(foreignEntity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                                var dataCallback = new
                                {
                                    status_code = "ERROR",
                                    service_id = foreignEntity.ServiceID,
                                    status_type = 1,
                                    ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                    data = new { transaction_id = foreignEntity.TransactionID, sp_transactionNr = foreignEntity.TransactionNr, amount = foreignEntity.Amount, sp_id = foreignEntity.ID, message = foreignEntity.Description, currencyCode = foreignEntity.CurrencyCode },
                                    user_entered_data = new { member = foreignEntity.Member, sender_name = foreignEntity.SenderName, action_date = foreignEntity.ActionDate, action_time = foreignEntity.ActionTime, creditCard = foreignEntity.CardNumber, amount = foreignEntity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
                                };

                                var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                callbackEntity.ServiceType = "STILPAY";
                                callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                _callbackResponseLogManager.Insert(callbackEntity);
                            }
                        }
                    }
                    else
                    {
                        ContentResult result = new ContentResult
                        {
                            Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{redirectUrl}"),
                            ContentType = "text/html"
                        };

                        return result;
                    }

                    return View(view);
                }
            }

            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }
        #endregion

        #region EfixPos
        public GenericResponse EfixPosPaymentMethod()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");

                var expDateMonth = model.CreditCardModel.ExpirationDate[..2];
                var expDateSub = model.CreditCardModel.ExpirationDate[3..];
                var date = DateTime.Now.Year.ToString()[..2] + expDateSub;

                var addTransactionPaymentModel = new EfixPosAddTransactionDetailRequestModel()
                {
                    TotalAmount = model.entity.Amount,
                    ClientOrderId = model.entity.TransactionID
                };

                var addTransactionRequest = EfixPosAddTransactionDetailRequest.AddTransactionDetail(addTransactionPaymentModel);

                if (addTransactionRequest != null && addTransactionRequest.Data != null && addTransactionRequest.Status == "OK")
                {
                    var reqModel = new EfixPosCheckoutRequestModel()
                    {
                        Pan = model.CreditCardModel.CardNumber.Replace(" ", ""),
                        CardOwner = model.CreditCardModel.SenderName,
                        Amount = model.entity.Amount,
                        Cvv = model.CreditCardModel.SecurityCode,
                        ExpiryDate = date + expDateMonth,
                        ClientOrderId = model.entity.TransactionID,
                    };

                    var sendReq = EfixPosCheckoutRequest.Checkout(reqModel);

                    if(sendReq.Status == "OK")
                    {
                        model.CreditCardModel.UCD_URL = sendReq.Data.Url3ds;
                        model.CreditCardModel.Description = "Ön Ödeme Süreci Başarılı";
                    }

                    model.LastTime = DateTime.Now;
                    _httpContext.HttpContext.Session.Write("Payment_Notification", model);
                    return new GenericResponse { Status = "OK" };
                }
                else
                    return new GenericResponse { Status = "ERROR", Message = addTransactionRequest.Message };
            }
            catch { }

            return new GenericResponse { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." };

        }

        [HttpGet]
        public IActionResult EfixPosThreeDSecure()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                _httpContext.HttpContext.Session.Write("Payment_Notification", model);

                return View(model);
            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }

        [HttpGet]
        public IActionResult EfixPosThreeDSecureResult([FromQuery] string STATUS, [FromQuery] string data)       
        {
            try
            {
                var entity = _creditCardPaymentNotificationManager.GetSingleByTransactionID(data);

                if (entity != null)
                {
                    var view = new PaymentNotificationViewModel();
                    byte status = 0;
                    var response = "";
                    var refNo = "";
                    var lidioSenderName = "";
                    decimal? paymentInstitutionCommissionRate = 0.0M;
                    decimal? paymentInstitutionNetAmount = 0.0M;
                    var callbackEntity = new CallbackResponseLog();
                    var opt = new JsonSerializerOptions() { WriteIndented = true };

                    var integration = _companyIntegration.GetByServiceId(entity.ServiceID);

                    var checkCallback = _callbackResponseLogManager.GetSingle(new List<FieldParameter>()
                    {
                        new FieldParameter("TransactionID", Enums.FieldType.NVarChar, entity.TransactionID),
                        new FieldParameter("TransactionType", Enums.FieldType.NVarChar, "KREDI KARTI ODEMESI ON RESPONSE")
                    });

                    var respdata = new { status = STATUS, data };

                    callbackEntity.TransactionID = entity.TransactionID;
                    callbackEntity.ServiceType = "EfixPos";
                    callbackEntity.IDCompany = integration.ID;
                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(respdata, opt);
                    callbackEntity.TransactionType = "KREDI KARTI ODEMESI ON RESPONSE";
                    _callbackResponseLogManager.Insert(callbackEntity);

                    if (checkCallback != null)
                    {
                        return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
                    }

                    var getRequestData = _callbackResponseLogManager.GetSingle(new List<FieldParameter>()
                    {
                        new FieldParameter("TransactionID", Enums.FieldType.NVarChar, entity.TransactionID),
                        new FieldParameter("TransactionType", Enums.FieldType.NVarChar, "Get IFrame Request")
                    });

                    JObject jsonObject = JObject.Parse(getRequestData?.Callback ?? "{}");

                    var redirectUrl = jsonObject["data"]?["redirectUrl"]?.ToString();

                    if (string.IsNullOrEmpty(redirectUrl))
                    {
                        redirectUrl = integration.RedirectUrl;
                    }

                    if (entity.Status != (byte)Enums.StatusType.Pending)
                    {
                        ContentResult result = new ContentResult
                        {
                            Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{redirectUrl}"),
                            ContentType = "text/html"
                        };

                        return result;
                    }

                    if (entity.Status == (byte)Enums.StatusType.Pending)
                    {
                        callbackEntity = new CallbackResponseLog();
                        opt = new JsonSerializerOptions() { WriteIndented = true };

                        var efix = _paymentInstitutionManager.GetList(null).FirstOrDefault(x => x.ID == Convert.ToString((int)CreditCardPaymentMethodType.EfixPos));

                        for (int i = 0; i < 5; i++)
                        {
                            var getDetail = EfixPosTransactionDetailRequest.GetDetail(entity.TransactionID);

                            callbackEntity.TransactionID = entity.TransactionID;
                            callbackEntity.ServiceType = "EfixPos";
                            callbackEntity.IDCompany = integration.ID;
                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(getDetail, opt);
                            callbackEntity.TransactionType = "KREDI KARTI ODEMESI QUERY RESPONSE";
                            _callbackResponseLogManager.Insert(callbackEntity);

                            if (getDetail.Data != null && getDetail.Status == "OK")
                            {
                                decimal netAmount = Math.Round(getDetail.Data.TotalAmount - (getDetail.Data.TotalAmount * efix.Rate / 100m), 2);

                                status = STATUS == "SUCCESS" && getDetail.Data.ExternalStatusCode == "0000"? (byte)Enums.StatusType.Confirmed : (byte)Enums.StatusType.Pending;
                                paymentInstitutionNetAmount = netAmount;
                                paymentInstitutionCommissionRate = efix.Rate;
                                refNo = getDetail.Data.PaymentTransactionId.ToString();
                                response = getDetail.Data.ExternalStatusDesc;
                                lidioSenderName = "";
                            }

                            if (status != (byte)Enums.StatusType.Confirmed && getDetail.Data != null && getDetail.Status == "ERROR")
                            {
                                status = (byte)Enums.StatusType.Canceled;
                                response = getDetail.Data.ExternalStatusDesc;
                            }

                            if (status == (byte)Enums.StatusType.Confirmed)
                            {
                                break;
                            }

                            Thread.Sleep(1000);
                        }
                    

                        if (status == (byte)Enums.StatusType.Confirmed)
                        {
                            var connection = _httpContext.HttpContext.Connection;
                            _creditCardPaymentNotificationManager.SetMemberIPAdress(entity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                            entity.Status = entity.IsAutoNotification ? (byte)StatusType.Confirmed : (byte)StatusType.Pending;
                            entity.MUser = "00000000-0000-0000-0000-000000000000";
                            entity.MDate = DateTime.Now;
                            entity.Description = lidioSenderName == "" ? response : response + " Banka Gönderici Adı: " + lidioSenderName;
                            entity.PaymentInstitutionCommissionRate = paymentInstitutionCommissionRate;
                            entity.PaymentInstitutionNetAmount = paymentInstitutionNetAmount;
                            entity.TransactionReferenceCode = refNo;

                            var responseSetStatus = _creditCardPaymentNotificationManager.SetStatus(entity);
                            if (responseSetStatus.Status == "OK")
                            {
                                view.GenericCreditCardPaymentResponseModel.Success = true;
                                view.GenericCreditCardPaymentResponseModel.IsAutoTransaction = entity.IsAutoNotification ? "true" : "false";
                                view.GenericCreditCardPaymentResponseModel.Message = "Ödeme Başarılı";
                                view.GenericCreditCardPaymentResponseModel.TimeStamp = DateTime.Now.ToString();
                                view.GenericCreditCardPaymentResponseModel.RedirectUrl = redirectUrl;

                                if (entity.IsAutoNotification)
                                {
                                    var dataCallback = new
                                    {
                                        status_code = "OK",
                                        service_id = entity.ServiceID,
                                        status_type = 1,
                                        ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                        data = new { transaction_id = entity.TransactionID, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = "Ödeme İşlemi Başarılı" },
                                        user_entered_data = new { member = entity.Member, sender_name = entity.SenderName, action_date = entity.ActionDate, action_time = entity.ActionTime, creditCard = entity.CardNumber, amount = entity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString(), cardSenderNameFromBank = lidioSenderName }
                                    };

                                    var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                                    callbackEntity.ServiceType = "STILPAY";
                                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                    callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                    _callbackResponseLogManager.Insert(callbackEntity);
                                }
                            }
                            else
                                return Json(new GenericResponse { Status = "ERROR", Message = responseSetStatus.Message });
                        }

                        else
                        {
                            entity.Status = (byte)StatusType.Canceled;
                            entity.MUser = "00000000-0000-0000-0000-000000000000";
                            entity.MDate = DateTime.Now;
                            entity.Description = response;
                            var responseSetStatus = _creditCardPaymentNotificationManager.SetStatus(entity);
                            if (responseSetStatus.Status == "OK")
                            {
                                view.GenericCreditCardPaymentResponseModel.Success = false;
                                view.GenericCreditCardPaymentResponseModel.IsAutoTransaction = entity.IsAutoNotification ? "true" : "false";
                                //view.GenericCreditCardPaymentResponseModel.Message = paybullPaymentResponseModel.original_bank_error_description ?? paybullPaymentResponseModel.error;
                                view.GenericCreditCardPaymentResponseModel.Message = response;
                                view.GenericCreditCardPaymentResponseModel.TimeStamp = DateTime.Now.ToString();
                                view.GenericCreditCardPaymentResponseModel.RedirectUrl = redirectUrl;

                                var connection = _httpContext.HttpContext.Connection;
                                _creditCardPaymentNotificationManager.SetMemberIPAdress(entity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                                var dataCallback = new
                                {
                                    status_code = "ERROR",
                                    service_id = entity.ServiceID,
                                    status_type = 1,
                                    ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                    data = new { transaction_id = entity.TransactionID, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = entity.Description },
                                    user_entered_data = new { member = entity.Member, sender_name = entity.SenderName, action_date = entity.ActionDate, action_time = entity.ActionTime, creditCard = entity.CardNumber, amount = entity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
                                };

                                var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                                callbackEntity.ServiceType = "STILPAY";
                                callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                                _callbackResponseLogManager.Insert(callbackEntity);
                            }
                        }
                    }
                    else
                    {
                        ContentResult result = new ContentResult
                        {
                            Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{redirectUrl}"),
                            ContentType = "text/html"
                        };

                        return result;
                    }

                    return View(view);
                }
            }

            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }
        #endregion

        //#region NomuPay
        //public GenericResponse NomuPayPosPaymentMethod()
        //{
        //    try
        //    {
        //        var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");

        //        var expDateMonth = model.CreditCardModel.ExpirationDate[..2];
        //        var expDateYear = DateTime.Now.Year.ToString()[..2] + model.CreditCardModel.ExpirationDate[3..];

        //        var nomuPayPosIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "NomuPayPos") });

        //        var nomuPayPosPaymentRequestModel = new NomuPayPosPaymentRequestModel()
        //        {
        //            CardTokenization = new CardTokenization()
        //            {
        //                RequestType = 0,
        //                CustomerId = model.entity.TransactionID,
        //                ValidityPeriod = 0
        //            },
        //            Token = new Token()
        //            {
        //                Pin = nomuPayPosIntegrationValues.FirstOrDefault(f => f.ParamDef == "pin").ParamVal,
        //                UserCode = nomuPayPosIntegrationValues.FirstOrDefault(f => f.ParamDef == "user_code").ParamVal,
        //            },
        //            MPAY = model.entity.TransactionID,
        //            CreditCardInfo = new CreditCardInfo()
        //            {
        //                CreditCardNo = model.CreditCardModel.CardNumber.Replace(" ", ""),
        //                Cvv = model.CreditCardModel.SecurityCode,
        //                ExpireMonth = int.Parse(expDateMonth),
        //                ExpireYear = int.Parse(expDateYear),
        //                OwnerName = model.CreditCardModel.SenderName,
        //                Price = (long)(model.entity.Amount * 100),
        //            },
        //            PaymentContent = "EPIN",
        //            IPAddress = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString() == "::1" ? "176.88.30.213" : _httpContext.HttpContext.Connection.RemoteIpAddress.ToString()
        //        };

        //        var nomuPayPosPaymentRequestResponseModel = NomuPayPosPaymentRequest.PaymentRequest(nomuPayPosPaymentRequestModel);

        //        if (nomuPayPosPaymentRequestResponseModel != null && nomuPayPosPaymentRequestResponseModel.Data != null && nomuPayPosPaymentRequestResponseModel.Status == "OK")
        //        {
        //            model.CreditCardModel.UCD_URL = nomuPayPosPaymentRequestResponseModel.Data.RedirectUrl;
        //            model.CreditCardModel.Description = "Ön Ödeme Süreci Başarılı";

        //            model.LastTime = DateTime.Now;
        //            _httpContext.HttpContext.Session.Write("Payment_Notification", model);
        //            return new GenericResponse { Status = "OK" };
        //        }
        //        else
        //            return new GenericResponse { Status = "ERROR", Message = nomuPayPosPaymentRequestResponseModel.Message };
        //    }
        //    catch { }

        //    return new GenericResponse { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." };
        //}

        //[HttpGet]
        //public IActionResult NomuPayPosThreeDSecure()
        //{
        //    try
        //    {
        //        var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
        //        _httpContext.HttpContext.Session.Write("Payment_Notification", model);

        //        return View(model);
        //    }
        //    catch { }

        //    return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        //}

        //[HttpGet]
        //public IActionResult NomuPayPosThreeDSecureResult(NomuPayPosPaymentRequestResponseModel nomuPayPosPaymentRequestResponseModel)
        //{
        //    try
        //    {
        //        var view = new PaymentNotificationViewModel();
        //        byte status = 0;
        //        var response = "";
        //        var refNo = "";
        //        decimal? paymentInstitutionCommissionRate = 0.0M;
        //        decimal? paymentInstitutionNetAmount = 0.0M;
        //        var callbackEntity = new CallbackResponseLog();
        //        var opt = new JsonSerializerOptions() { WriteIndented = true };

        //        var entity = _creditCardPaymentNotificationManager.GetSingleByTransactionID(nomuPayPosPaymentRequestResponseModel.MPAY);

        //        var nomuPayPosIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "NomuPayPos") });

        //        var integration = _companyIntegration.GetByServiceId(entity.ServiceID);

        //        var checkCallback = _callbackResponseLogManager.GetSingle(new List<FieldParameter>()
        //        {
        //            new FieldParameter("TransactionID", Enums.FieldType.NVarChar, entity.TransactionID),
        //            new FieldParameter("TransactionType", Enums.FieldType.NVarChar, "KREDI KARTI ODEMESI RESPONSE")
        //        });

        //        callbackEntity.TransactionID = entity.TransactionID;
        //        callbackEntity.ServiceType = "NomuPayPos";
        //        callbackEntity.IDCompany = integration.ID;
        //        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(nomuPayPosPaymentRequestResponseModel, opt);
        //        callbackEntity.TransactionType = "KREDI KARTI ODEMESI RESPONSE";
        //        _callbackResponseLogManager.Insert(callbackEntity);

        //        if (checkCallback != null)
        //        {
        //            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        //        }

        //        if (entity.Status != (byte)Enums.StatusType.Pending)
        //        {
        //            ContentResult result = new ContentResult
        //            {
        //                Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{integration.RedirectUrl}"),
        //                ContentType = "text/html"
        //            };

        //            return result;
        //        }

        //        if (entity.Status == (byte)Enums.StatusType.Pending)
        //        {
        //            for (int i = 0; i < 5; i++)
        //            {
        //                var nomuPayPosTransactionQueryRequestResponseModel = NomuPayPosTransactionQueryRequest.TransactionQueryRequest(entity.TransactionID);

        //                callbackEntity.TransactionID = entity.TransactionID;
        //                callbackEntity.ServiceType = "NomuPayPos";
        //                callbackEntity.IDCompany = integration.ID;
        //                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(nomuPayPosTransactionQueryRequestResponseModel, opt);
        //                callbackEntity.TransactionType = "KREDI KARTI ODEMESI QUERY RESPONSE";
        //                _callbackResponseLogManager.Insert(callbackEntity);

        //                if (nomuPayPosTransactionQueryRequestResponseModel.Data != null && nomuPayPosTransactionQueryRequestResponseModel.Status == "OK")
        //                {
        //                    status = (byte)Enums.StatusType.Confirmed;
        //                    //paymentInstitutionNetAmount = (decimal?)lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.PaybackTransactionList[0].PaybackAmount;
        //                    //paymentInstitutionCommissionRate = (decimal?)lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.PaybackTransactionList[0].BankTotalCommissionRate;
        //                    //refNo = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.SystemTransId;
        //                    //response = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.ResultCategory.CategoryName;
        //                }

        //                //if (status != (byte)Enums.StatusType.Confirmed && lidioPosTransactionQueryRequestResponseModel.Data != null && lidioPosTransactionQueryRequestResponseModel.Status == "ERROR")
        //                //{
        //                //    status = (byte)Enums.StatusType.Canceled;
        //                //    response = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.ResultCategory.RecommendedUIMessageTR ?? lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.ResultCategory.CategoryName;
        //                //}

        //                if (status == (byte)Enums.StatusType.Confirmed)
        //                {
        //                    break;
        //                }

        //                Thread.Sleep(1000);
        //            }


        //            if (status == (byte)Enums.StatusType.Confirmed)
        //            {
        //                var connection = _httpContext.HttpContext.Connection;
        //                _creditCardPaymentNotificationManager.SetMemberIPAdress(entity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

        //                entity.Status = entity.IsAutoNotification ? (byte)StatusType.Confirmed : (byte)StatusType.Pending;
        //                entity.MUser = "00000000-0000-0000-0000-000000000000";
        //                entity.MDate = DateTime.Now;
        //                entity.Description = response;
        //                entity.PaymentInstitutionCommissionRate = paymentInstitutionCommissionRate;
        //                entity.PaymentInstitutionNetAmount = paymentInstitutionNetAmount;
        //                entity.TransactionReferenceCode = refNo;

        //                var responseSetStatus = _creditCardPaymentNotificationManager.SetStatus(entity);
        //                if (responseSetStatus.Status == "OK")
        //                {
        //                    view.GenericCreditCardPaymentResponseModel.Success = true;
        //                    view.GenericCreditCardPaymentResponseModel.IsAutoTransaction = entity.IsAutoNotification ? "true" : "false";
        //                    view.GenericCreditCardPaymentResponseModel.Message = "Ödeme Başarılı";
        //                    view.GenericCreditCardPaymentResponseModel.TimeStamp = DateTime.Now.ToString();
        //                    view.GenericCreditCardPaymentResponseModel.RedirectUrl = integration.RedirectUrl;

        //                    if (entity.IsAutoNotification)
        //                    {
        //                        var dataCallback = new
        //                        {
        //                            status_code = "OK",
        //                            service_id = entity.ServiceID,
        //                            status_type = 1,
        //                            ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
        //                            data = new { transaction_id = entity.TransactionID, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = "Ödeme İşlemi Başarılı" },
        //                            user_entered_data = new { member = entity.Member, sender_name = entity.SenderName, action_date = entity.ActionDate, action_time = entity.ActionTime, creditCard = entity.CardNumber, amount = entity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
        //                        };

        //                        var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

        //                        callbackEntity.ServiceType = "STILPAY";
        //                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
        //                        callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
        //                        _callbackResponseLogManager.Insert(callbackEntity);
        //                    }
        //                }
        //                else
        //                    return Json(new GenericResponse { Status = "ERROR", Message = responseSetStatus.Message });
        //            }
        //            else
        //            {
        //                entity.Status = (byte)StatusType.Canceled;
        //                entity.MUser = "00000000-0000-0000-0000-000000000000";
        //                entity.MDate = DateTime.Now;
        //                entity.Description = response;
        //                var responseSetStatus = _creditCardPaymentNotificationManager.SetStatus(entity);
        //                if (responseSetStatus.Status == "OK")
        //                {
        //                    view.GenericCreditCardPaymentResponseModel.Success = false;
        //                    view.GenericCreditCardPaymentResponseModel.IsAutoTransaction = entity.IsAutoNotification ? "true" : "false";
        //                    //view.GenericCreditCardPaymentResponseModel.Message = paybullPaymentResponseModel.original_bank_error_description ?? paybullPaymentResponseModel.error;
        //                    view.GenericCreditCardPaymentResponseModel.Message = response;
        //                    view.GenericCreditCardPaymentResponseModel.TimeStamp = DateTime.Now.ToString();
        //                    view.GenericCreditCardPaymentResponseModel.RedirectUrl = integration.RedirectUrl;

        //                    var connection = _httpContext.HttpContext.Connection;
        //                    _creditCardPaymentNotificationManager.SetMemberIPAdress(entity.ID, connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

        //                    var dataCallback = new
        //                    {
        //                        status_code = "ERROR",
        //                        service_id = entity.ServiceID,
        //                        status_type = 1,
        //                        ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
        //                        data = new { transaction_id = entity.TransactionID, sp_transactionNr = entity.TransactionNr, amount = entity.Amount, sp_id = entity.ID, message = entity.Description },
        //                        user_entered_data = new { member = entity.Member, sender_name = entity.SenderName, action_date = entity.ActionDate, action_time = entity.ActionTime, creditCard = entity.CardNumber, amount = entity.Amount, user_ip = connection.RemoteIpAddress.ToString(), user_port = connection.RemotePort.ToString() }
        //                    };

        //                    var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

        //                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
        //                    callbackEntity.ServiceType = "STILPAY";
        //                    callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
        //                    _callbackResponseLogManager.Insert(callbackEntity);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            ContentResult result = new ContentResult
        //            {
        //                Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{integration.RedirectUrl}"),
        //                ContentType = "text/html"
        //            };

        //            return result;
        //        }

        //        return View(view);
        //    }

        //    catch { }

        //    return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        //}

        //#endregion

        [HttpGet]
        public IActionResult Information()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                if (model != null && model.entity != null && model.LastTime >= DateTime.Now.AddSeconds(-30))
                {
                    model.SelectedBank = _companyBankAccountManager.GetSingle(new List<FieldParameter>
                    {
                        new FieldParameter("ID", Enums.FieldType.NVarChar, model.entity.CompanyBankAccountID)
                    });

                    _httpContext.HttpContext.Session.Write<PaymentNotificationViewModel>("Payment_Notification", model);

                    return View(model);
                }
            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }

        [HttpPost]
        public IActionResult Information(string id)
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");

                model.LastTime = DateTime.Now;

                _httpContext.HttpContext.Session.Write<PaymentNotificationViewModel>("Payment_Notification", model);

                return Json(new GenericResponse { Status = "OK" });

            }
            catch { }

            return Json(new GenericResponse { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." });
        }

        [HttpGet]
        public IActionResult Validation()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                //model.LastTime = DateTime.Now;
                _httpContext.HttpContext.Session.Write("Payment_Notification", model);

                var hasSent = _httpContext.HttpContext.Session.HasSentSms(model.entity.Phone, "PaymentNotification_Validation");
                if (hasSent)
                {
                    model.HasSendSms = true;
                    return View(model);
                }

                var integration = _companyIntegration.GetByServiceId(model.entity.ServiceID);

                var companyFraudControl = _companyFraudControlManager.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", FieldType.NVarChar, integration.ID),
                });

                if (companyFraudControl != null && model.CreditCardModel.CardNumber == null)
                {
                    if (companyFraudControl.IsSmsConfirmationActiveTransfer)
                    {
                        if (model != null && model.entity != null && model.LastTime >= DateTime.Now.AddSeconds(-360))
                        {
                            tSmsSender sender = new tSmsSender();
                            var smsResponse = sender.SendConfirmCode(model.entity.Phone, "PaymentNotification_Validation");
                            if (smsResponse.Status.Equals("OK"))
                            {
                                _httpContext.HttpContext.Session.SaveSms(model.entity.Phone, "PaymentNotification_Validation", smsResponse.ConfirmCode);
                                model.HasSendSms = false;
                                return View(model);
                            }
                        }
                        else
                        {
                            ContentResult result = new ContentResult
                            {
                                Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{integration.RedirectUrl}"),
                                ContentType = "text/html"
                            };

                            return result;
                        }
                    }
                    else
                        return RedirectToAction("Profile", "PaymentNotification", new { area = "Panel" });
                }
                if (companyFraudControl != null && model.CreditCardModel != null && !model.IsForeignCreditCard)
                {
                    if (companyFraudControl.IsSmsConfirmationActiveCreditCard)
                    {

                        if (model != null && model.entity != null && model.LastTime >= DateTime.Now.AddSeconds(-360))
                        {
                            tSmsSender sender = new tSmsSender();
                            var smsResponse = sender.SendConfirmCode(model.entity.Phone, "PaymentNotification_Validation");
                            if (smsResponse.Status.Equals("OK"))
                            {
                                _httpContext.HttpContext.Session.SaveSms(model.entity.Phone, "PaymentNotification_Validation", smsResponse.ConfirmCode);
                                model.HasSendSms = false;
                                return View(model);
                            }
                        }
                        else
                        {
                            ContentResult result = new ContentResult
                            {
                                Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{integration.RedirectUrl}"),
                                ContentType = "text/html"
                            };

                            return result;
                        }
                    }
                    else
                        return RedirectToAction("Profile", "PaymentNotification", new { area = "Panel" });
                }

                if (companyFraudControl != null && model.IsForeignCreditCard)
                {
                    if (companyFraudControl.IsSmsConfirmationActiveForeingCreditCard && model.CreditCardModel.CountryCode == "+90")
                    {
                        if (model != null && model.entity != null && model.LastTime >= DateTime.Now.AddSeconds(-360))
                        {
                            tSmsSender sender = new tSmsSender();
                            var smsResponse = sender.SendConfirmCode(model.entity.Phone, "PaymentNotification_Validation");
                            if (smsResponse.Status.Equals("OK"))
                            {
                                _httpContext.HttpContext.Session.SaveSms(model.entity.Phone, "PaymentNotification_Validation", smsResponse.ConfirmCode);
                                model.HasSendSms = false;
                                return View(model);
                            }
                        }
                        else
                        {
                            ContentResult result = new ContentResult
                            {
                                Content = string.Format("<script >window.parent.location.href = '{0}';</script>", $"{integration.RedirectUrl}"),
                                ContentType = "text/html"
                            };

                            return result;
                        }
                    }
                    else
                        return RedirectToAction("Profile", "PaymentNotification", new { area = "Panel" });
                }

            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Validation(int confirmCode)
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");

                model.LastTime = DateTime.Now;

                _httpContext.HttpContext.Session.Write("Payment_Notification", model);

                var genericResponse = _httpContext.HttpContext.Session.ValidateConfirmCode("PaymentNotification_Validation", confirmCode);

                return Json(genericResponse);

            }
            catch { }

            return Json(new GenericResponse { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." });
        }

        [HttpGet]
        public IActionResult Profile()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");

                var integration = _companyIntegration.GetByServiceId(model.entity.ServiceID);

                var companyFraudControl = _companyFraudControlManager.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", FieldType.NVarChar, integration.ID),
                });

                if (model != null && model.entity != null && model.LastTime >= DateTime.Now.AddSeconds(-30))
                {
                    var member = _memberManager.GetMember(model.entity.Phone);
                    //var memberTypes = _memberTypeManager.GetList(null).ToList();

                    if (member == null)
                    {
                        var newMember = new Entities.Concrete.Member();

                        newMember.CUser = newMember.MUser = "00000000-0000-0000-0000-000000000000";
                        newMember.Phone = model.CreditCardModel.PhoneNumber ?? model.entity.Phone;
                        newMember.StatusFlag = true;
                        newMember.IDMemberType = "00000000-0000-0000-0000-000000000000";
                        newMember.IdentityNr = "11111111111";
                        newMember.ServiceID = model.entity.ServiceID;
                        newMember.Name = model.CreditCardModel.SenderName ?? model.entity.SenderName;
                        newMember.Email = "otomatik@otomatik.com";
                        newMember.BirthYear = "2023";
                        newMember.CDate = DateTime.Now;
                        var response = _memberManager.Insert(newMember);

                        if (response.Status == "OK")
                        {
                            model.entity.IDMember = response.Data.ToString();
                            model.LastTime = DateTime.Now;
                            model.entity.SenderIdentityNr = newMember.IdentityNr;
                            _httpContext.HttpContext.Session.Write<PaymentNotificationViewModel>("Payment_Notification", model);
                        }
                    }
                    else
                    {
                        model.entity.IDMember = member.ID;
                        model.LastTime = DateTime.Now;
                        model.entity.SenderIdentityNr = member.IdentityNr;
                        _httpContext.HttpContext.Session.Write("Payment_Notification", model);
                    }

                    if (model.CreditCardModel.CardNumber != null)
                    {
                        var encryptedCardNumber = CardNumberHashingService.Hash(model.CreditCardModel.CardNumber.Replace(" ", ""));

                        var checkPaymentBlockSetting = _paymentTransferPoolDescriptionControlManager.PaymentWillBlocked(model.CreditCardModel.SenderName.Trim(), model.CreditCardModel.PhoneNumber.Trim(), model.CreditCardModel.CardNumber.Replace(" ", ""));

                        if (model.IsForeignCreditCard)
                        {
                            var foreignCreditCardPaymentNotification = new ForeignCreditCardPaymentNotification
                            {
                                TransactionID = model.entity.TransactionID,
                                ActionDate = model.entity.ActionDate,
                                ActionTime = model.entity.ActionTime,
                                Amount = decimal.Parse(model.entity.Amount.ToString()),
                                Description = model.CreditCardModel.Description,
                                IDMember = model.entity.IDMember,
                                Phone = model.CreditCardModel.PhoneNumber,
                                CountryCode = model.CreditCardModel.CountryCode,
                                Status = model.entity.Status,
                                SenderName = model.CreditCardModel.SenderName,
                                SenderIdentityNr = model.entity.SenderIdentityNr,
                                ServiceID = model.entity.ServiceID,
                                CardNumber = model.CreditCardModel.CardNumber.Replace(" ", "")[..4] + "****" + "****" + model.CreditCardModel.CardNumber.Replace(" ", "")[12..],
                                MUser = "00000000-0000-0000-0000-000000000000",
                                CurrencyCode = model.CurrencyCode,
                                CardTypeId = model.CreditCardModel.CardTypeId,
                                CreditCardPaymentMethodID = model.CreditCardPaymentMethodID,
                                EncryptedCardNumber = encryptedCardNumber
                            };

                            if(companyFraudControl != null && companyFraudControl.IsForeignCreditCardFraudControlActive)
                            {
                                var encryptedCardNumberData = _foreignCreditCardPaymentNotificationManager.GetEncryptedCardNumberData(encryptedCardNumber);

                                if (!encryptedCardNumberData.Any(x => x.IsTrustedCardNumber))
                                {
                                    if (encryptedCardNumberData.Any(x => x.IsCaughtInFraudControl && x.Status == (int)Enums.StatusType.Pending))
                                    {
                                        foreignCreditCardPaymentNotification.FraudControlDescription = "Daha önceki işlemi fraud kontrolüne takıldı, işlem durduruldu";
                                        foreignCreditCardPaymentNotification.IsCaughtInFraudControl = true;
                                    }
                                    else
                                    {
                                        if (!encryptedCardNumberData.Any(x => x.Status == (int)Enums.StatusType.Confirmed) && foreignCreditCardPaymentNotification.Amount >= companyFraudControl.ForeignCreditCardFirstTransactionLimit)
                                        {
                                            foreignCreditCardPaymentNotification.FraudControlDescription = $"İlk işlem {companyFraudControl.ForeignCreditCardFirstTransactionLimit:n2} veya üstü, işlem durduruldu";
                                            foreignCreditCardPaymentNotification.IsCaughtInFraudControl = true;
                                        }
                                        else
                                        {
                                            OkObjectResult fraudControl = FraudControl(true, encryptedCardNumber, foreignCreditCardPaymentNotification.Amount, companyFraudControl.ForeignCreditCardTimeSpanInRecentTransactionMinutesLimitAmount, companyFraudControl.ForeignCreditCardTimeSpanInRecentTransactionMinutes, companyFraudControl.ForeignCreditCardDailyTransactionCount, companyFraudControl.ForeignCreditCardDailyTransactionLimitAmount, companyFraudControl.BeStoppedForeignCreditCardDailyTransactionCount) as OkObjectResult;

                                            var resultValue = fraudControl.Value;
                                            var fraudControlResult = resultValue as dynamic;

                                            foreignCreditCardPaymentNotification.FraudControlDescription = fraudControlResult.message;
                                            foreignCreditCardPaymentNotification.IsTrustedCardNumber = fraudControlResult.isTrustedCardNumber;

                                            if (!fraudControlResult.success)
                                                foreignCreditCardPaymentNotification.IsCaughtInFraudControl = true;
                                        }
                                    }
                                }
                                else
                                {
                                    foreignCreditCardPaymentNotification.FraudControlDescription = "Kart güvenli olarak kayıtlı";
                                    foreignCreditCardPaymentNotification.IsTrustedCardNumber = true;
                                }
                            }
                            else
                            {
                                foreignCreditCardPaymentNotification.FraudControlDescription = "Üye işyeri yurt dışı kart fraud kontrolü devredışı olduğu için kontrol sağlanmadı";
                            }

                            var responseInsert = _foreignCreditCardPaymentNotificationManager.Insert(foreignCreditCardPaymentNotification);
                            if (responseInsert.Status.Equals("OK"))
                            {
                                var connection = _httpContext.HttpContext.Connection;
                                _foreignCreditCardPaymentNotificationManager.SetMemberIPAdress(responseInsert.Data.ToString(), connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());
                      
                                return RedirectToAction(model.CreditCardRedirectToActionGetThreeDView);
                            }

                            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
                        }
                        else
                        {
                            var creditCardPaymentNotification = new CreditCardPaymentNotification
                            {
                                TransactionID = model.entity.TransactionID,
                                ActionDate = model.entity.ActionDate,
                                ActionTime = model.entity.ActionTime,
                                Amount = decimal.Parse(model.entity.Amount.ToString()),
                                Description = model.CreditCardModel.Description,
                                IDMember = model.entity.IDMember,
                                Phone = model.CreditCardModel.PhoneNumber,
                                Status = model.entity.Status,
                                SenderName = model.CreditCardModel.SenderName,
                                SenderIdentityNr = model.entity.SenderIdentityNr,
                                ServiceID = model.entity.ServiceID,
                                CardNumber = model.CreditCardModel.CardNumber.Replace(" ", "")[..4] + "****" + "****" + model.CreditCardModel.CardNumber.Replace(" ", "")[12..],
                                ParamCommission = model.CreditCardPaymentMethodModel.Param ? decimal.Parse(model.CreditCardModel.InstallmentAmount) - model.entity.Amount : 0,
                                PayNKolayCommission = model.CreditCardPaymentMethodModel.PayNKolay ? decimal.Parse(model.CreditCardModel.InstallmentAmount) - model.entity.Amount : 0,
                                MUser = "00000000-0000-0000-0000-000000000000",
                                CreditCardPaymentMethodID = model.CreditCardPaymentMethodID,
                                CardTypeId = model.CreditCardModel.CardTypeId,
                                EncryptedCardNumber = encryptedCardNumber,
                                IsCaughtInBlocked = (bool)checkPaymentBlockSetting?.Data
                            };

                            if(companyFraudControl != null && companyFraudControl.IsCreditCardFraudControlActive)
                            {
                                var encryptedCardNumberData = _creditCardPaymentNotificationManager.GetEncryptedCardNumberData(encryptedCardNumber);

                                if (!encryptedCardNumberData.Any(x => x.IsTrustedCardNumber))
                                {
                                    if (encryptedCardNumberData.Any(x => x.IsCaughtInFraudControl && x.Status == (int)Enums.StatusType.Pending))
                                    {
                                        creditCardPaymentNotification.FraudControlDescription = "Daha önceki işlemi fraud kontrolüne takıldı, işlem durduruldu";
                                        creditCardPaymentNotification.IsCaughtInFraudControl = true;
                                    }
                                    else
                                    {
                                        if (!encryptedCardNumberData.Any(x => x.Status == (int)Enums.StatusType.Confirmed) && creditCardPaymentNotification.Amount >= companyFraudControl.CreditCardFirstTransactionLimit)
                                        {
                                            creditCardPaymentNotification.FraudControlDescription = $"İlk işlem {companyFraudControl.CreditCardFirstTransactionLimit:n2} veya üstü, işlem durduruldu";
                                            creditCardPaymentNotification.IsCaughtInFraudControl = true;
                                        }

                                        else
                                        {
                                            OkObjectResult fraudControl = FraudControl(false, encryptedCardNumber, creditCardPaymentNotification.Amount, companyFraudControl.CreditCardTimeSpanInRecentTransactionMinutesLimitAmount, companyFraudControl.CreditCardTimeSpanInRecentTransactionMinutes, companyFraudControl.CreditCardDailyTransactionCount, companyFraudControl.CreditCardDailyTransactionLimitAmount, companyFraudControl.BeStoppedCreditCardDailyTransactionCount) as OkObjectResult;

                                            var resultValue = fraudControl.Value;
                                            var fraudControlResult = resultValue as dynamic;

                                            creditCardPaymentNotification.FraudControlDescription = fraudControlResult.message;
                                            creditCardPaymentNotification.IsTrustedCardNumber = fraudControlResult.isTrustedCardNumber;

                                            if (!fraudControlResult.success)
                                                creditCardPaymentNotification.IsCaughtInFraudControl = true;
                                        }
                                    }
                                }
                                else
                                {
                                    creditCardPaymentNotification.FraudControlDescription = "Kart güvenli olarak kayıtlı";
                                    creditCardPaymentNotification.IsTrustedCardNumber = true;
                                }
                            }
                            else
                            {
                                creditCardPaymentNotification.FraudControlDescription = "Üye işyeri yurt içi kart fraud kontrolü devredışı olduğu için kontrol sağlanmadı";
                            }

                            var responseInsert = _creditCardPaymentNotificationManager.Insert(creditCardPaymentNotification);
                            if (responseInsert.Status.Equals("OK"))
                            {
                                var connection = _httpContext.HttpContext.Connection;
                                _creditCardPaymentNotificationManager.SetMemberIPAdress(responseInsert.Data.ToString(), connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                                return RedirectToAction(model.CreditCardRedirectToActionGetThreeDView);
                            }

                            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
                        }
                    }
                    else
                        return RedirectToAction("Finish", "PaymentNotification", new { area = "Panel" });

                }
            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }

        [HttpGet]
        public virtual IActionResult FraudControl(bool isForeignCard, string encryptedCardNumber,decimal amount, decimal timeSpanInRecentTransactionMinutesLimitAmount, int timeSpanInMinutes, int transactionLimitToday,
            decimal dailyTransactionLimitAmount, int beStoppedCardDailyTransactionCount)
        {
            try
            {
                var result = isForeignCard ? _foreignCreditCardPaymentNotificationManager.ForeignCreditCardTransactionCheckFraudControl(encryptedCardNumber, timeSpanInMinutes, transactionLimitToday) : _creditCardPaymentNotificationManager.CreditCardTransactionCheckFraudControl(encryptedCardNumber, timeSpanInMinutes, transactionLimitToday);

                if (result.RecentTransactionLimitExceeded && amount >= timeSpanInRecentTransactionMinutesLimitAmount)
                {
                    return Ok(new { success = false, isTrustedCardNumber = false, message = $"Aynı kart {timeSpanInMinutes} dakika içinde {timeSpanInRecentTransactionMinutesLimitAmount:n2} veya üstü olarak 2. kez geçildi, işlem durduruldu." });
                }

                if (result.DailyTransactionLimitExceeded && result.TransactionCountToday >= beStoppedCardDailyTransactionCount)
                {
                    return Ok(new { success = false, isTrustedCardNumber = false, message = $"Aynı kart 24 saat içinde {result.TransactionCountToday}. kez geçildi, üye işyeri sınırı {beStoppedCardDailyTransactionCount}, işlem durduruldu." });
                }

                if (result.DailyTransactionLimitExceeded && amount >= dailyTransactionLimitAmount)
                {
                    return Ok(new { success = false, isTrustedCardNumber = false, message = $"Aynı kart 24 saat içinde {result.TransactionCountToday}. kez ve {dailyTransactionLimitAmount} üstü geçildi, işlem durduruldu." });
                }

                if (result.TransactionWithin24Hours)
                {
                    return Ok(new { success = true, isTrustedCardNumber = true, message = "Aynı kart 24 saat sonra tekrar geçildi, kart güvenilir olarak kayıt edildi.",  });
                }

                return Ok(new { success = true, isTrustedCardNumber = false, message = "Fraud kontrolleri başarıyla tamamlandı." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "İşlem sırasında bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(Entities.Concrete.Member member)
        {
            try
            {
                // Kullanılmıyor. O yüzden Güncellenmedi

                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");
                member.CUser = member.MUser = "00000000-0000-0000-0000-000000000000";
                member.Phone = model.CreditCardModel.PhoneNumber ?? model.entity.Phone;
                member.IDMemberType = "00000000-0000-0000-0000-000000000000";
                member.StatusFlag = true;

                var response = _memberManager.Insert(member);
                if (response.Status.Equals("OK"))
                {
                    byte enumType = 0;
                    var redirectToActionStr = "";
                    switch (model.CreditCardPaymentMethodModel)
                    {
                        case { Param: true }:
                            enumType = (byte)Enums.CreditCardPaymentMethodType.Param;
                            redirectToActionStr = "ThreeDSecure";
                            break;

                        case { PayNKolay: true }:
                            enumType = (byte)Enums.CreditCardPaymentMethodType.PayNKolay;
                            redirectToActionStr = "PayNKolayThreeDSecure";
                            break;

                        //case { ForeignCCPayNKolay: true }:
                        //    enumType = (byte)Enums.ForeignCreditCardPaymentMethodType.PayNKolay;
                        //    break;

                        case { IsBankSanalPOS: true }:
                            enumType = (byte)Enums.CreditCardPaymentMethodType.IsBankSanalPOS;
                            redirectToActionStr = "IsBankThreeDSecure";
                            break;

                        case { Paybull: true }:
                            enumType = (byte)Enums.CreditCardPaymentMethodType.Paybull;
                            redirectToActionStr = "PaybullThreeDSecure";
                            break;

                        case { AKODE: true }:
                            enumType = (byte)Enums.CreditCardPaymentMethodType.AKODE;
                            redirectToActionStr = "AKODEThreeDSecure";
                            break;
                    }

                    model.entity.IDMember = response.Data.ToString();
                    model.LastTime = DateTime.Now;

                    if (model.CreditCardModel.CardNumber != null)
                    {
                        if (model.IsForeignCreditCard)
                        {
                            var foreignCreditCardPaymentNotification = new ForeignCreditCardPaymentNotification
                            {
                                TransactionID = model.entity.TransactionID,
                                ActionDate = model.entity.ActionDate,
                                ActionTime = model.entity.ActionTime,
                                Amount = decimal.Parse(model.entity.Amount.ToString()),
                                Description = model.CreditCardModel.Description,
                                IDMember = model.entity.IDMember,
                                Phone = model.CreditCardModel.PhoneNumber,
                                CountryCode = model.CreditCardModel.CountryCode,
                                Status = model.entity.Status,
                                SenderName = model.CreditCardModel.SenderName,
                                SenderIdentityNr = model.entity.SenderIdentityNr,
                                ServiceID = model.entity.ServiceID,
                                CardNumber = model.CreditCardModel.CardNumber.Replace(" ", "")[..4] + "****" + "****" + model.CreditCardModel.CardNumber.Replace(" ", "")[12..],
                                MUser = "00000000-0000-0000-0000-000000000000",
                            };

                            var responseInsert = _foreignCreditCardPaymentNotificationManager.Insert(foreignCreditCardPaymentNotification);
                            if (responseInsert.Status.Equals("OK"))
                            {
                                if (model.CreditCardPaymentMethodModel.ForeignCCPayNKolay)
                                    return RedirectToAction("PayNKolayThreeDSecure", "PaymentNotification", new { area = "Panel" });
                            }

                            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
                        }
                        else
                        {
                            var creditCardPaymentNotification = new CreditCardPaymentNotification
                            {
                                TransactionID = model.entity.TransactionID,
                                ActionDate = model.entity.ActionDate,
                                ActionTime = model.entity.ActionTime,
                                Amount = decimal.Parse(model.entity.Amount.ToString()),
                                Description = model.CreditCardModel.Description,
                                IDMember = model.entity.IDMember,
                                Phone = model.CreditCardModel.PhoneNumber,
                                Status = model.entity.Status,
                                SenderName = model.CreditCardModel.SenderName,
                                SenderIdentityNr = model.entity.SenderIdentityNr,
                                ServiceID = model.entity.ServiceID,
                                CardNumber = model.CreditCardModel.CardNumber.Replace(" ", "")[..4] + "****" + "****" + model.CreditCardModel.CardNumber.Replace(" ", "")[12..],
                                ParamCommission = model.CreditCardPaymentMethodModel.Param ? decimal.Parse(model.CreditCardModel.InstallmentAmount) - model.entity.Amount : 0,
                                PayNKolayCommission = model.CreditCardPaymentMethodModel.PayNKolay ? decimal.Parse(model.CreditCardModel.InstallmentAmount) - model.entity.Amount : 0,
                                MUser = "00000000-0000-0000-0000-000000000000",
                                CreditCardPaymentMethodID = enumType
                            };

                            var responseInsert = _creditCardPaymentNotificationManager.Insert(creditCardPaymentNotification);
                            if (responseInsert.Status.Equals("OK"))
                            {
                                return RedirectToAction(redirectToActionStr);
                            }

                            return Json(new GenericResponse { Status = "ERROR", Message = response.Message });
                        }
                    }
                    _httpContext.HttpContext.Session.Write<PaymentNotificationViewModel>("Payment_Notification", model);

                }
            }
            catch { }

            return Json(new GenericResponse { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." });
        }

        [HttpGet]
        public IActionResult Finish()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");

                if (model != null && model.entity != null && model.LastTime >= DateTime.Now.AddSeconds(-30))
                {
                    GenericResponse response = _manager.Insert(model.entity);
                    if (response.Status.Equals("OK"))
                    {
                        var connection = _httpContext.HttpContext.Connection;
                        _manager.SetMemberIPAdress(response.Data.ToString(), connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

                        return View(model);
                    }
                }
            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }

        [HttpGet]
        public IActionResult StatusControl()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");

                if (model != null && model.entity != null)
                {
                    var entity = _manager.GetSingleByTransactionID(model.entity.TransactionID);

                    if (entity != null && entity.Status == (byte)Enums.StatusType.Pending)
                        return Json(new GenericResponse { Status = "OK", Data = false });

                    if (entity != null && entity.Status == (byte)Enums.StatusType.Confirmed)
                        return Json(new GenericResponse { Status = "OK", Data = true });

                    if (entity != null && entity.Status == (byte)Enums.StatusType.Canceled)
                        return Json(new GenericResponse { Status = "ERROR", Message = entity.Description ?? "İPTAL EDİLDİ" });
                }
            }
            catch { }

            return RedirectToAction("Error", "PaymentNotification", new { area = "Panel" });
        }

        [HttpGet]
        public IActionResult StatusControlCreditCard()
        {
            try
            {
                var model = _httpContext.HttpContext.Session.Read<PaymentNotificationViewModel>("Payment_Notification");

                if (model != null && model.CreditCardModel.CardNumber != null)
                {
                    var entity = _creditCardPaymentNotificationManager.GetSingleByTransactionID(model.entity.TransactionID);

                    if (entity != null)
                    {
                        if (entity.Status == (byte)Enums.StatusType.Pending)
                            return Json(new GenericResponse { Status = "OK", Data = false });

                        if (entity.Status == (byte)Enums.StatusType.Confirmed)
                            return Json(new GenericResponse { Status = "OK", Data = true });

                        if (entity.Status == (byte)Enums.StatusType.Canceled)
                            return Json(new GenericResponse { Status = "ERROR", Message = entity.Description ?? "İPTAL EDİLDİ" });
                    }
                    else
                    {
                        var foreignCreditCardEntity = _foreignCreditCardPaymentNotificationManager.GetSingleByTransactionID(model.entity.TransactionID);

                        if (foreignCreditCardEntity != null)
                        {
                            if (foreignCreditCardEntity.Status == (byte)Enums.StatusType.Pending)
                                return Json(new GenericResponse { Status = "OK", Data = false });

                            if (foreignCreditCardEntity.Status == (byte)Enums.StatusType.Confirmed)
                                return Json(new GenericResponse { Status = "OK", Data = true });

                            if (foreignCreditCardEntity.Status == (byte)Enums.StatusType.Canceled)
                                return Json(new GenericResponse { Status = "ERROR", Message = entity.Description ?? "İPTAL EDİLDİ" });
                        }
                    }
                }
            }
            catch { }

            return RedirectToAction("Transfer", "PaymentNotification", new { area = "Panel" });
        }

        [HttpGet]
        public IActionResult NotAllowed()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }
	}
}
