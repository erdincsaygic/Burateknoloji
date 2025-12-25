using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StilPay.BLL.Abstract;
using StilPay.BLL;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System.Collections.Generic;
using System;
using System.Linq;
using StilPay.DAL.Abstract;
using DocumentFormat.OpenXml.Wordprocessing;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Dealer")]
    public class DealerCreditCardPayPoolController : BaseController<CreditCardPaymentNotification>
    {
        private readonly ICreditCardPaymentNotificationManager _manager;
        private readonly IPaymentCreditCardPoolManager _paymentCreditCardPoolManager;
        private readonly IPaymentInstitutionManager _paymentInstitutionManager;

        public DealerCreditCardPayPoolController(ICreditCardPaymentNotificationManager manager, IPaymentCreditCardPoolManager paymentCreditCardPoolManager, IHttpContextAccessor httpContext, IPaymentInstitutionManager paymentInstitutionManager) : base(httpContext)
        {
            _manager = manager;
            _paymentCreditCardPoolManager = paymentCreditCardPoolManager;
            _paymentInstitutionManager = paymentInstitutionManager;
        }

        public override IBaseBLL<CreditCardPaymentNotification> Manager()
        {
            return _manager;
        }

        [HttpPost]
        public IActionResult GetData()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];


            var list = GetData(
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.PayPool),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["IDCompany"].ToString()) ? "0" : HttpContext.Request.Form["IDCompany"].ToString() == "all" ? null : HttpContext.Request.Form["IDCompany"].ToString()),
                new FieldParameter("IsAutoNotification", Enums.FieldType.Tinyint, null),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString())),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString())),
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

        [HttpGet]
        public IActionResult PayPool()
        {
            var paymentInstitutionList = _paymentInstitutionManager.GetList(null);
            ViewBag.PaymentInstitution = paymentInstitutionList;
            return View("PayPool");
        }

        [HttpPost]
        public IActionResult GetPayPool()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];
              
            var startDate = Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString());
            var endDate = Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString());

            var startDateTime = Convert.ToDateTime(HttpContext.Request.Form["StartDateTime"].ToString());
            var endDateTime = Convert.ToDateTime(HttpContext.Request.Form["EndDateTime"].ToString());

            if (startDateTime.Hour == 0 || startDateTime.Minute == 0 || startDateTime.Second == 0) 
            {
                startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDateTime.Hour, startDateTime.Minute, startDateTime.Second);
            }
            if (endDateTime.Hour == 0 && endDateTime.Minute == 0 && endDate.Second == 0)
            {
                endDate = endDate.AddDays(1);
            }
            else
            {
                endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, endDateTime.Hour, endDateTime.Minute, endDateTime.Second);
            }

            var list = _paymentCreditCardPoolManager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("Status", Enums.FieldType.Tinyint, int.Parse(HttpContext.Request.Form["Status"]) == 0 ? (byte?)null : Convert.ToByte(HttpContext.Request.Form["Status"])),
                new FieldParameter("PaymentMethodID", Enums.FieldType.Int, int.Parse(HttpContext.Request.Form["PaymentMethodID"]) == 0 ? (int?)null : int.Parse(HttpContext.Request.Form["PaymentMethodID"])),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, startDate),
                new FieldParameter("EndDate", Enums.FieldType.DateTime,  endDate ),
                new FieldParameter("PageLenght", Enums.FieldType.Int, length),
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
    }
}