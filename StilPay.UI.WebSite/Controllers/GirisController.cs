using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StilPay.UI.Controllers
{
    [AllowAnonymous]
    public class GirisController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
