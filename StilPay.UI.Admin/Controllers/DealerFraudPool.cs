using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.BLL.Concrete;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Dealer")]
    public class DealerFraudPoolController : Controller
    {
        private readonly ICompanyTransactionManager _companyTransactionManager;
        private readonly ICreditCardPaymentNotificationManager _creditCardPaymentNotificationManager;
        protected readonly IHttpContextAccessor _httpContext;

        public DealerFraudPoolController(ICompanyTransactionManager companyTransactionManager,ICreditCardPaymentNotificationManager creditCardPaymentNotificationManager, IHttpContextAccessor httpContext)
        {
            _companyTransactionManager = companyTransactionManager;
            _creditCardPaymentNotificationManager = creditCardPaymentNotificationManager;
            _httpContext = httpContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetData()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];


            var list = _companyTransactionManager.GetDealerFraudPool(new List<FieldParameter>() 
            {
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["IDCompany"].ToString()) ? "0" : HttpContext.Request.Form["IDCompany"].ToString() == "all" ? null : HttpContext.Request.Form["IDCompany"].ToString()),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString())),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString())),
                new FieldParameter("PageLength", Enums.FieldType.Int, length),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue)
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
        [ValidateAntiForgeryToken]
        public IActionResult ChangeStatusConfirm(string id)
        {
            var entity = _creditCardPaymentNotificationManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, id) });
            var claim = _httpContext.HttpContext.User.FindFirst(f => f.Type == ClaimTypes.Sid);

            if (entity != null)
            {
                entity.MDate = DateTime.Now;
                entity.MUser = claim.Value.ToString();
                entity.Status = (byte)Enums.StatusType.Confirmed;
                entity.Description = "İşlem Başarılı";

                return Json(_creditCardPaymentNotificationManager.SetStatus(entity));
            }
            else
                return Json(new GenericResponse { Status = "ERROR", Message = "İşlem Bulunamadı."});
        }
    }
}
