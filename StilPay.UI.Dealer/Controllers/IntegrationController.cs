using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.UI.Dealer.Controllers
{
    [Authorize(Roles = "Integration")]
    public class IntegrationController : BaseController<CompanyIntegration>
    {
        private readonly ICompanyIntegrationManager _manager;

        public IntegrationController(ICompanyIntegrationManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<CompanyIntegration> Manager()
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