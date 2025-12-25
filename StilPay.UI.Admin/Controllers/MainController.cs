using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MainController : BaseController<Main>
    {
        private readonly IMainManager _manager;

        public MainController(IMainManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<Main> Manager()
        {
            return _manager;
        }


        [HttpGet]
        public virtual IActionResult GetNotifyCounts()
        {
            var model = _manager.GetNotifyCounts();

            return Json(model);
        }
    }
}
