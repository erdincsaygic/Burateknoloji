using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Dealer")]
    public class DealerPaymentTransactionController : BaseController<PaymentNotification>
    {
        private readonly IPaymentNotificationManager _manager;
        private readonly ICompanyIntegrationManager _companyIntegrationManager;

        public DealerPaymentTransactionController(IPaymentNotificationManager manager, ICompanyIntegrationManager companyIntegrationManager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
            _companyIntegrationManager = companyIntegrationManager;
        }

        public override IBaseBLL<PaymentNotification> Manager()
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
                new FieldParameter("Status", Enums.FieldType.Tinyint, string.IsNullOrEmpty(HttpContext.Request.Form["Status"].ToString()) ? (byte?)null : Convert.ToByte(HttpContext.Request.Form["Status"])),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["IDCompany"].ToString()) ? "0" : HttpContext.Request.Form["IDCompany"].ToString() == "all" ? null : HttpContext.Request.Form["IDCompany"].ToString()),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString())),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString())),
                new FieldParameter("IsAutoNotification", Enums.FieldType.Tinyint, null),
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
    }
}