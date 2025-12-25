using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Accounting")]
    public class CompanyProgressPaymentCalendarController : BaseController<CompanyProgressPaymentCalendar>
    {
        private readonly ICompanyProgressPaymentCalendarManager _manager;
        public CompanyProgressPaymentCalendarController(ICompanyProgressPaymentCalendarManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<CompanyProgressPaymentCalendar> Manager()
        {
            return _manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents(string idCompany)
        {
            var events = _manager.GetList(new List<FieldParameter>() { new FieldParameter("IDCompany", Enums.FieldType.NVarChar, idCompany)});
            return Json(events);
        }
    }
}
