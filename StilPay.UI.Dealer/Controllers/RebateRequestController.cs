using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.BLL.Concrete;
using StilPay.Entities;
using StilPay.Entities.Concrete;
using StilPay.UI.Dealer.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static StilPay.Utility.Helper.Enums;

namespace StilPay.UI.Dealer.Controllers
{
    [Authorize(Roles = "RebateRequest")]
    public class RebateRequestController : BaseController<CompanyRebateRequest>
    {
        private readonly ICompanyRebateRequestManager _manager;
        private readonly IBankManager _bankManager;
        private readonly IPaymentNotificationManager _paymentNotificationManager;
        private readonly ICreditCardPaymentNotificationManager _creditCardPaymentNotificationManager;
        private readonly IForeignCreditCardPaymentNotificationManager _foreignCreditCardPaymentNotificationManager;
        private readonly ICompanyTransactionManager _companyTransactionManager;

        public RebateRequestController(ICompanyRebateRequestManager manager, IPaymentNotificationManager paymentNotificationManager, IBankManager bankManager,
            ICreditCardPaymentNotificationManager creditCardPaymentNotificationManager, IForeignCreditCardPaymentNotificationManager foreignCreditCardPaymentNotificationManager,ICompanyTransactionManager companyTransactionManager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
            _paymentNotificationManager = paymentNotificationManager;
            _creditCardPaymentNotificationManager = creditCardPaymentNotificationManager;
            _foreignCreditCardPaymentNotificationManager = foreignCreditCardPaymentNotificationManager;
            _bankManager = bankManager;
            _companyTransactionManager = companyTransactionManager;
        }   

        public override IBaseBLL<CompanyRebateRequest> Manager()
        {
            return _manager;
        }

        [HttpPost]
        public override IActionResult Gets([FromBody] JObject jObj)
        {
            var list = GetData(
                new FieldParameter("Status", Enums.FieldType.Tinyint, string.IsNullOrEmpty(jObj["Status"].ToString()) ? (byte?)null : Convert.ToByte(jObj["Status"])),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, IDCompany),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, string.IsNullOrEmpty(jObj["IDMember"].ToString()) ? null : jObj["IDMember"].ToString()),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, string.IsNullOrEmpty(jObj["StartDate"].ToString()) ? (DateTime?)null : Convert.ToDateTime(jObj["StartDate"].ToString())),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, string.IsNullOrEmpty(jObj["EndDate"].ToString()) ? (DateTime?)null : Convert.ToDateTime(jObj["EndDate"].ToString()))
            );

            return Json(list);
        }

        [HttpPost]
        public IActionResult GetData()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = GetData(
                new FieldParameter("Status", Enums.FieldType.Tinyint, string.IsNullOrEmpty(HttpContext.Request.Form["Status"].ToString()) ? (byte?)null : Convert.ToByte(HttpContext.Request.Form["Status"])),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, IDCompany),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, string.IsNullOrEmpty(HttpContext.Request.Form["StartDate"].ToString()) ? (DateTime?)null : Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString())),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, string.IsNullOrEmpty(HttpContext.Request.Form["StartDate"].ToString()) ? (DateTime?)null : Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString())),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["IDMember"].ToString()) ? null : HttpContext.Request.Form["IDMember"].ToString()),
                new FieldParameter("PageLenght", Enums.FieldType.Int, length),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue)

            );

            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;

            var result = new
            {
                recordsFiltered = recordsTotal,
                data = list
            };

            return Json(result);
        }
        
        public override EditViewModel<CompanyRebateRequest> InitEditViewModel(string id = null)
        {
            var model = new RebateRequestEditViewModel();

            if (!string.IsNullOrEmpty(id))
            {
                var hasReabateRequestForTransactionNr = _manager.GetSingleByTransactionNr(id);
                if (hasReabateRequestForTransactionNr != null)
                {
                    model.hasProcess = true;
                    model.entity = hasReabateRequestForTransactionNr;
                    model.IDMember = hasReabateRequestForTransactionNr.IDMember;

                    return model;
                }

                model.PaymentNotification = _paymentNotificationManager.GetSingleByTransactionNr(IDCompany, id);
                model.IDMember = model.PaymentNotification?.IDMember;

                if (model.PaymentNotification is null)
                {
                    model.CreditCardPaymentNotification = _creditCardPaymentNotificationManager.GetSingleByTransactionNr(IDCompany, id);
                    model.IDMember = model.CreditCardPaymentNotification?.IDMember;

                    if (model.CreditCardPaymentNotification is null)
                    {
                        model.ForeignCreditCardPaymentNotification = _foreignCreditCardPaymentNotificationManager.GetSingleByTransactionNr(IDCompany, id);
                        model.IDMember = model.ForeignCreditCardPaymentNotification?.IDMember;
                    }
                }
                else
                {
                    Regex regex = new Regex(@"^TR\d{7}[0-9]{17}$");

                    if (model.PaymentNotification.Iban != null && regex.IsMatch(model.PaymentNotification.Iban.Replace(" ", "")))
                        model.isIbanValid = true;
                }

                var hasReabateRequestForTransactionID = model.PaymentNotification?.TransactionID ?? model.CreditCardPaymentNotification?.TransactionID;

                if(hasReabateRequestForTransactionID != null)
                {
                    var check = _manager.GetSingleByTransactionID(hasReabateRequestForTransactionID);
                    if (check != null && (check.Status == (byte)Enums.StatusType.Pending || check.Status == (byte)Enums.StatusType.Confirmed ))
                    {
                        model.hasProcess = true;
                        model.entity = check;
                        model.IDMember = check.IDMember;
                    }
                }
            }

            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult SetIban(string pyID, string iban)
        {
            Regex regex = new Regex(@"^TR\d{7}[0-9]{17}$");

            var concatIban = string.Concat("TR", iban.Replace(" ", ""));

            if (!regex.IsMatch(concatIban))
                return Json(new GenericResponse { Status = "ERROR", Message = "Lütfen Iban Bilgisini Doğru Formatta Giriniz" });

            var banks = _bankManager.GetList(null);
            var bankCode = concatIban[5..9];
            var IDBank = banks.FirstOrDefault(x => x.Branch.Equals(bankCode))?.ID;

            if (IDBank == null)
                return Json(new GenericResponse { Status = "ERROR", Message = "Iban Bilgisi İle Eşleşen Banka Bulunamadı.Lütfen Bizimle İletişime Geçiniz" });

            var isSuccess = tSQLBankManager.SetPaymentNotificationIban(pyID, concatIban, IDBank);
            if (isSuccess)
                return Json(new GenericResponse { Status = "OK"});

            return Json(new GenericResponse { Status = "ERROR", Message = "Bir Hata İle Karşılaşıldı" });
        }

        public override IActionResult Save(CompanyRebateRequest entity)
        {
            var entityId = "";
            if (entity.PaymentRecordID == "1")
            {
                var checkEntity = _paymentNotificationManager.GetSingleByTransactionID(entity.TransactionID);

                if(checkEntity != null)
                {
                    var companyTransaction = _companyTransactionManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", FieldType.NVarChar, checkEntity.ID) });

                    var truncateCompanyTransactionNetTotal = Math.Truncate(companyTransaction.NetTotal * 100) / 100;
                    var truncateCompanyTransactionTotal = Math.Truncate(companyTransaction.Total * 100) / 100;
                    var truncateCompanyTransactionCommission = Math.Truncate(companyTransaction.Commission * 100) / 100;

                    entity.IsPartialRebate = entity.Amount != truncateCompanyTransactionNetTotal;

                    entity.PartialRebatePaymentInstitutionTotalAmount = entity.IsPartialRebate ? Math.Round(entity.Amount * 100 / (100 - companyTransaction.CommissionRate), 2) : truncateCompanyTransactionTotal;
                    entity.PartialRebatePaymentInstitutionCommission = entity.IsPartialRebate ? Math.Round(entity.PartialRebatePaymentInstitutionTotalAmount * companyTransaction.CommissionRate / 100, 2) : truncateCompanyTransactionCommission;
                    entity.PaymentedEntityID = checkEntity.ID;

                    if (checkEntity != null)
                    {
                        if (checkEntity.Status == (int)Enums.StatusType.Confirmed && entity.Amount > checkEntity.NetTotal)
                        {
                            return Json(new GenericResponse { Status = "ERROR", Message = "İade edilecek tutar hakediş tutarından fazla olamaz" });
                        }

                        entityId = checkEntity.ID;
                    }
                }           
            }
            else if (entity.PaymentRecordID == "2")
            {
                var checkEntity = _creditCardPaymentNotificationManager.GetSingleByTransactionID(entity.TransactionID);

                if (checkEntity != null)
                {
                    var companyTransaction = _companyTransactionManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", FieldType.NVarChar, checkEntity.ID) });
                    var truncateCompanyTransactionNetTotal = Math.Truncate(companyTransaction.NetTotal * 100) / 100;
                    var truncateCompanyTransactionTotal = Math.Truncate(companyTransaction.Total * 100) / 100;
                    var truncateCompanyTransactionCommission = Math.Truncate(companyTransaction.Commission * 100) / 100;

                    entity.IsPartialRebate = entity.Amount != truncateCompanyTransactionNetTotal;

                    entity.PartialRebatePaymentInstitutionTotalAmount = entity.IsPartialRebate ? Math.Round(entity.Amount * 100 / (100 - companyTransaction.CommissionRate),2) : truncateCompanyTransactionTotal;
                    entity.PartialRebatePaymentInstitutionCommission = entity.IsPartialRebate ? Math.Round(entity.PartialRebatePaymentInstitutionTotalAmount * companyTransaction.CompanyCommissionRate / 100, 2) : truncateCompanyTransactionCommission;

                    entity.PaymentedEntityID = checkEntity.ID;

                    if (checkEntity != null && checkEntity.Status == (int)Enums.StatusType.Confirmed && entity.Amount > checkEntity.NetTotal)
                    {
                        return Json(new GenericResponse { Status = "ERROR", Message = "İade edilecek tutar hakediş tutarından fazla olamaz"});
                    }

                    entityId = checkEntity.ID;
                }
            }
            else if (entity.PaymentRecordID == "3")
            {
                var checkEntity = _foreignCreditCardPaymentNotificationManager.GetSingleByTransactionID(entity.TransactionID);

                if (checkEntity != null)
                {
                    var companyTransaction = _companyTransactionManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", FieldType.NVarChar, checkEntity.ID) });
                    var truncateCompanyTransactionNetTotal = Math.Truncate(companyTransaction.NetTotal * 100) / 100;
                    var truncateCompanyTransactionTotal = Math.Truncate(companyTransaction.Total * 100) / 100;
                    var truncateCompanyTransactionCommission = Math.Truncate(companyTransaction.Commission * 100) / 100;

                    entity.IsPartialRebate = entity.Amount != truncateCompanyTransactionNetTotal;

                    entity.PartialRebatePaymentInstitutionTotalAmount = entity.IsPartialRebate ? Math.Round(entity.Amount * 100 / (100 - companyTransaction.CommissionRate), 2) : truncateCompanyTransactionTotal;
                    entity.PartialRebatePaymentInstitutionCommission = entity.IsPartialRebate ? Math.Round(entity.PartialRebatePaymentInstitutionTotalAmount * companyTransaction.CompanyCommissionRate / 100, 2) : truncateCompanyTransactionCommission;

                    entity.PaymentedEntityID = checkEntity.ID;

                    if (checkEntity != null && checkEntity.Status == (int)Enums.StatusType.Confirmed && entity.Amount > checkEntity.NetTotal)
                    {
                        return Json(new GenericResponse { Status = "ERROR", Message = "İade edilecek tutar hakediş tutarından fazla olamaz" });
                    }

                    entityId = checkEntity.ID;
                }
            }

            entity.Status = (byte)Enums.StatusType.Pending;

             return base.Save(entity);
        }
    }
}
