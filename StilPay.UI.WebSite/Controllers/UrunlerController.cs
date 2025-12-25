using Microsoft.AspNetCore.Mvc;

namespace StilPay.UI.WebSite.Controllers
{
    public class UrunlerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreditCard()
        {
            return View("CreditCard");
        }
        public IActionResult Cashout()
        {
            return View("Cashout");
        }
    }
}
