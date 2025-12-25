using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.UI.WebSite.Areas.Panel.Controllers
{
    [Area("Panel")]
    [Authorize(Roles = "Member")]
    public class MyAccountController : BaseController<Member>
    {
        private readonly IMemberManager _manager;

        public MyAccountController(IMemberManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<Member> Manager()
        {
            return _manager;
        }

        public override IActionResult Index()
        {
            var model = InitEditViewModel(IDMember);

            return View(model);
        }
    }
}