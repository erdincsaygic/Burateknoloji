using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StilPay.UI.WebSite.Areas.Panel.Controllers
{
    [Area("Panel")]
    [Authorize(Roles = "Member")]
    public class InvoiceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
