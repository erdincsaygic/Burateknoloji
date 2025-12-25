using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.UI.Dealer.Controllers
{
    [Authorize(Roles = "Account")]
    public class CommissionRatesController : BaseController<CompanyCommission>
    {
        private readonly ICompanyCommissionManager _manager;

        public CommissionRatesController(ICompanyCommissionManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<CompanyCommission> Manager()
        {
            return _manager;
        }

        public override IActionResult Index()
        {
            var model = InitEditViewModel(IDCompany);

            return View(model);
        }

    }
}