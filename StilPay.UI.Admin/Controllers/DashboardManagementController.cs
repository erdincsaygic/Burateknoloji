using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Dashboard")]
    public class DashboardManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
