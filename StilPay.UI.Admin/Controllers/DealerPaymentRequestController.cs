using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "PendingProcess")]
    public class DealerPaymentRequestController : BaseController<CompanyPaymentRequest>
    {
        private readonly ICompanyPaymentRequestManager _manager;

        public DealerPaymentRequestController(ICompanyPaymentRequestManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<CompanyPaymentRequest> Manager()
        {
            return _manager;
        }

        public override IActionResult Gets()
        {
            var list = GetData(
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.Pending),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null)
            );

            return Json(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetStatus(CompanyPaymentRequest entity)
        {
            return Json(_manager.SetStatus(entity));
        }

    }
}