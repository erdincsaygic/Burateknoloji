using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StilPay.BLL.Abstract;
using StilPay.BLL;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Linq;
using static StilPay.Utility.Helper.Enums;
using StilPay.BLL.Concrete;
using StilPay.Utility.AKODESanalPOS.Models.AKODEGetTransactions;
using StilPay.Utility.AKODESanalPOS;
using StilPay.DAL.Concrete;
using DocumentFormat.OpenXml.ExtendedProperties;
using RestSharp.Serializers;
using RestSharp;
using StilPay.Utility.Worker;
using StilPay.Utility.AKODESanalPOS.Models.AKODETransactionQuery;
using System.Text.Json;
using DocumentFormat.OpenXml.EMMA;
using StilPay.Entities.Dto;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Dealer")]
    public class DealerCreditCardTransactionController : BaseController<CreditCardPaymentNotification>
    {
        private readonly ICreditCardPaymentNotificationManager _manager;
        private readonly ICompanyIntegrationManager _companyIntegrationManager;
        private readonly ICompanyTransactionManager _companyTransactionManager;
        //private readonly IFraudTransactionManager _fraudTransactionManager;
        private readonly IPaymentInstitutionManager _paymentInstitutionManager;
        private readonly SettingDAL _settingDAL = new SettingDAL();

        public DealerCreditCardTransactionController(ICreditCardPaymentNotificationManager manager, ICompanyIntegrationManager companyIntegrationManager, ICompanyTransactionManager companyTransactionManager, IHttpContextAccessor httpContext, /*IFraudTransactionManager fraudTransactionManager ,*/ IPaymentInstitutionManager paymentInstitutionManager) : base(httpContext)
        {
            _manager = manager;
            _companyIntegrationManager = companyIntegrationManager;
            _companyTransactionManager = companyTransactionManager;
            //_fraudTransactionManager = fraudTransactionManager;
            _paymentInstitutionManager = paymentInstitutionManager;
        }

        public override IBaseBLL<CreditCardPaymentNotification> Manager()
        {
            return _manager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var paymentInstitutionList = _paymentInstitutionManager.GetList(null);
            ViewBag.PaymentInstitution = paymentInstitutionList;
            return View("Index");
        }

        [HttpPost]
        public IActionResult GetData()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = _manager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("Status", Enums.FieldType.Tinyint, string.IsNullOrEmpty(HttpContext.Request.Form["Status"].ToString()) ? (byte?)null : Convert.ToByte(HttpContext.Request.Form["Status"])),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["IDCompany"].ToString()) ? "0" : HttpContext.Request.Form["IDCompany"].ToString() == "all" ? null : HttpContext.Request.Form["IDCompany"].ToString()),                
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                new FieldParameter("IsAutoNotification", Enums.FieldType.Tinyint, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString())),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString())),
                new FieldParameter("PageLenght", Enums.FieldType.Int, length),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue),
                new FieldParameter("PaymentMethodID", Enums.FieldType.Int, int.Parse(HttpContext.Request.Form["PaymentMethodID"]) == 0 ? (int?)null : int.Parse(HttpContext.Request.Form["PaymentMethodID"])),
            });

            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;

            var result = new
            {
                recordsFiltered = recordsTotal,
                data = list
            };

            return Json(result);
        }

        [HttpPost]
        public IActionResult AKODETransactionQuery(string transactionId)
        {
            var entity = _manager.GetSingleByTransactionID(transactionId);

            if(entity != null)
            {
                var akOdeSanalPOSIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "AKODECreditCard") });

                var randomGenerator = new Random();
                var rnd = randomGenerator.Next(1, 1000000).ToString();
                var akOdeGetTransactionRequestModel = new AKODEGetTransactionRequestModel()
                {
                    ApiUser = akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_user").ParamVal,
                    ClientId = akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "client_id").ParamVal,
                    Rnd = rnd,
                    TimeSpan = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    TransactionDate = int.Parse(entity.ActionDate.ToString("yyyyMMdd")),
                    Page = 1,
                    PageSize = int.MaxValue,
                    OrderId = entity.TransactionID
                };

                akOdeGetTransactionRequestModel.Hash = AKODECreateHash.CreateHash(akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_pass").ParamVal, akOdeGetTransactionRequestModel.ClientId, akOdeGetTransactionRequestModel.ApiUser, akOdeGetTransactionRequestModel.Rnd, akOdeGetTransactionRequestModel.TimeSpan);

                var response = AKODEGetTransactionRequest.GetTransactionList(akOdeGetTransactionRequestModel);

                if (response.Status == "OK" && response.Data != null && response.Data.Transactions != null && response.Data.Transactions.Count > 0)
                {
                    var integration = _companyIntegrationManager.GetByServiceId(entity.ServiceID);
                    var opt = new JsonSerializerOptions() { WriteIndented = true };
                    var IDout = tSQLBankManager.AddCallbackResponseLog(transactionId, "AKODE" , System.Text.Json.JsonSerializer.Serialize(response.Data.Transactions.FirstOrDefault(), opt), integration.ID, "MANUAL KREDI KARTI ODEMESI QUERY RESPONSE", 0);

                    var data = response.Data.Transactions.Where(x => x.TransactionType == 1 && x.RequestStatus != 11).ToList().Select(s => new
                    {
                        status = s.RequestStatus == 1 ? "BAŞARILI" : s.RequestStatus == 0 ? "BAŞARISIZ" : s.RequestStatus == 2 ? "İPTAL EDİLDİ" : s.RequestStatus == 4 ? "İPTAL EDİLDİ / TAMAMI İADE EDİLDİ" : "BEKLEMEDE / BİLİNMİYOR",
                        message = s.BankResponseMessage ?? s.Message
                    });

                    return Json(new GenericResponse { Status = "OK", Data = data });

                    //foreach (var item in response.Data.Transactions.Where(x => x.TransactionType == 1 && x.RequestStatus != 11))
                    //{
                    //    var formatStatus = item.RequestStatus == 1 ? (byte)Enums.StatusType.Confirmed : item.RequestStatus == 0 ? (byte)Enums.StatusType.Canceled : (byte)Enums.StatusType.Pending;

                    //    var info = new
                    //    {
                    //        status = item.RequestStatus == 1 ? (byte)Enums.StatusType.Confirmed : item.RequestStatus == 0 ? (byte)Enums.StatusType.Canceled : (byte)Enums.StatusType.Pending,
                    //        message = item.BankResponseMessage ?? item.Message
                    //    };

                    //    return Json(new GenericResponse { Status = "OK", Data = info });
                    //}
                }
                else
                    return Json(new GenericResponse { Status = "ERROR", Message = "API den kayıt bulunamadı." });
            }
            else
                return Json(new GenericResponse { Status = "ERROR", Message = "Kayıt Bulunamadı." });

        }

        [HttpPost]
        public IActionResult ChangeStatusToFraud(string id, string description)
        {
            if (description == null || string.IsNullOrEmpty(description) || string.IsNullOrWhiteSpace(description))
                return Json(new GenericResponse { Status = "ERROR", Message = "Lütfen açıklama giriniz." });

            var entity = _manager.GetSingleByTransactionID(id);

            if (entity != null && entity.Status != (int)Enums.StatusType.FraudPool)
            {
                var companyIntegration = _companyIntegrationManager.GetByServiceId(entity.ServiceID);

                var setStatusToFraudDto = new SetStatusToFraudDto()
                {
                    EntityID = entity.ID,
                    MDate = DateTime.Now,
                    MUser = IDUser,
                    Description = description,
                    EntityActionType = "100,110",
                    IDCompany = companyIntegration.ID,
                    AdminAction = $"{IDUserName} Tarafından - Üye İşyeri = {entity.Company}, TransactionID = {entity.TransactionID}, TransactionNr = {entity.TransactionNr} - Kredi Kartı Ödemesi Fraud Havuzuna Gönderildi",
                };

                return Json(_companyTransactionManager.SetStatusToFraud(setStatusToFraudDto));
            }
            else
                return Json(new GenericResponse { Status = "ERROR", Message = "Entity Bulunamadı / İşlem Zaten Fraud Havuzunda" });
        }
    }
}
