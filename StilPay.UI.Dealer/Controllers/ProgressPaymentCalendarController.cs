using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace StilPay.UI.Dealer.Controllers
{
    [Authorize(Roles = "Invoice")]
    public class ProgressPaymentCalendarController : BaseController<CompanyProgressPaymentCalendar>
    {
        private readonly ICompanyProgressPaymentCalendarManager _manager;
        public ProgressPaymentCalendarController(ICompanyProgressPaymentCalendarManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<CompanyProgressPaymentCalendar> Manager()
        {
            return _manager;
        }

        public async Task<IActionResult> GetEvents()
        {
            var events = _manager.GetList(new List<FieldParameter>() { new FieldParameter("IDCompany", Enums.FieldType.NVarChar, IDCompany) });
            return Json(events);
        }
    }
}