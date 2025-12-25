using Microsoft.AspNetCore.Mvc;
using StilPay.Utility.Helper;
using System.Data;

namespace StilPay.UI.WebSite.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Store()
        {
            return View("Magaza");
        }

        public IActionResult ChoosingProduct()
        {
            return View("UrunSecimi");
        }

        [HttpGet]
        public IActionResult Payment(string amount)
        {
            ViewBag.Amount = amount;
            return View("Payment"); 
        }


        public IActionResult PaymentAccept()
        {
            return View("PaymentAccept");
        }
    }
}
