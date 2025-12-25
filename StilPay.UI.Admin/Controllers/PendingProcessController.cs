using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "PendingProcess")]
    public class PendingProcessController : Controller
    {
        public IActionResult PaymentDeclarations()
        {
            return View();
        }
        public IActionResult PaymentDeclarationDetail()
        {
            return View();
        }

        public IActionResult DealerWithdrawalRequests()
        {
            return View();
        }
        public IActionResult DealerWithdrawalRequestDetail()
        {
            return View();
        }
        public IActionResult MemberWithdrawalRequests()
        {
            return View();
        }
        public IActionResult MemberWithdrawalRequestDetail()
        {
            return View();
        }
        public IActionResult DealerRebateRequests()
        {
            return View();
        }
        public IActionResult DealerRebateRequestDetail()
        {
            return View();
        }
        public IActionResult DealerApplications()
        {
            return View();
        }
        public IActionResult DealerApplicationDetail()
        {
            return View();
        }
    }
}
