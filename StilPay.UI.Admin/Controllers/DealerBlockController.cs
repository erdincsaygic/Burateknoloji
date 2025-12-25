using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL.Abstract;
using StilPay.BLL;
using StilPay.Entities.Concrete;
using Newtonsoft.Json.Linq;
using DocumentFormat.OpenXml.ExtendedProperties;
using System.Linq;
using StilPay.BLL.Concrete;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Dealer")]
    public class DealerBlockController : BaseController<PaymentNotification>
    {
        private readonly IPaymentNotificationManager _manager;
        private readonly ICreditCardPaymentNotificationManager _creditCardPaymentNotificationManager;
        private readonly IForeignCreditCardPaymentNotificationManager _foreignCreditCardPaymentNotificationManager;

        public DealerBlockController(IPaymentNotificationManager manager, ICreditCardPaymentNotificationManager creditCardPaymentNotificationManager, IForeignCreditCardPaymentNotificationManager foreignCreditCardPaymentNotificationManager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
            _creditCardPaymentNotificationManager = creditCardPaymentNotificationManager;
            _foreignCreditCardPaymentNotificationManager = foreignCreditCardPaymentNotificationManager;
        }

        public override IBaseBLL<PaymentNotification> Manager()
        {
            return _manager;
        }


        [HttpPost]
        public IActionResult GetBlockeds()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = _manager.GetBlockeds(HttpContext.Request.Form["IDCompany"].ToString(), length, start, searchValue);

            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;
            var result = new
            {
                recordsFiltered = recordsTotal,
                data = list
            };
            return Json(result);
        }

        [HttpPost]
        public IActionResult GetNotBlockeds()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = _manager.GetNotBlockeds(HttpContext.Request.Form["IDCompany"].ToString(), length, start, searchValue);

            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;
            var result = new
            {
                recordsFiltered = recordsTotal,
                data = list
            };
            return Json(result);
        }

        [HttpPost]
        public IActionResult GetCreditCardBlockeds()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = _creditCardPaymentNotificationManager.GetBlockeds(HttpContext.Request.Form["IDCompany"].ToString(), length, start, searchValue);

            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;
            var result = new
            {
                recordsFiltered = recordsTotal,
                data = list
            };
            return Json(result);
        }


        [HttpPost]
        public IActionResult GetCreditCardNotBlockeds()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = _creditCardPaymentNotificationManager.GetNotBlockeds(HttpContext.Request.Form["IDCompany"].ToString(), length, start, searchValue);

            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;
            var result = new
            {
                recordsFiltered = recordsTotal,
                data = list
            };
            return Json(result);
        }

        [HttpPost]
        public IActionResult GetForeignCreditCardBlockeds()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = _foreignCreditCardPaymentNotificationManager.GetBlockeds(HttpContext.Request.Form["IDCompany"].ToString(), length, start, searchValue);

            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;
            var result = new
            {
                recordsFiltered = recordsTotal,
                data = list
            };
            return Json(result);
        }


        [HttpPost]
        public IActionResult GetForeignCreditCardNotBlockeds()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = _foreignCreditCardPaymentNotificationManager.GetNotBlockeds(HttpContext.Request.Form["IDCompany"].ToString(), length, start, searchValue);

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
