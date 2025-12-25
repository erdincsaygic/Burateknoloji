using Microsoft.AspNetCore.Mvc;

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

        public IActionResult ChoosingPaymentMethod(string amount)
        {
            ViewBag.Amount = amount;
            return View("OdemeYontemi");
        }

        [HttpGet]
        public IActionResult PaymentMethodTransfer(string amount)
        {
            ViewBag.Amount = amount;
            return View("PaymentMethodTransfer");
        }

        [HttpGet]
        public IActionResult PaymentMethodMobile(string amount)
        {
            ViewBag.Amount = amount;
            return View("PaymentMethodMobile");
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

        public IActionResult PaymentAcceptTransfer()
        {
            return View("PaymentAcceptTransfer");
        }
    }
}
