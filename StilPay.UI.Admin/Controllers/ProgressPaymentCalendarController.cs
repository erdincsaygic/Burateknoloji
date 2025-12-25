using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using System.Threading.Tasks;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Accounting")]
    public class ProgressPaymentCalendarController : BaseController<ProgressPaymentCalendar>
    {
        private readonly IProgressPaymentCalendarManager _manager;
        public ProgressPaymentCalendarController(IProgressPaymentCalendarManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<ProgressPaymentCalendar> Manager()
        {
            return _manager;
        }

        public async Task<IActionResult> GetEvents()
        {
            var events = _manager.GetList(null);
            return Json(events);
        }
    }
}
    