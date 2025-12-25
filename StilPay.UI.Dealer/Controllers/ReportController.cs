using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.UI.Dealer.Models;
using System;
using System.Linq;

namespace StilPay.UI.Dealer.Controllers
{
    [Authorize(Roles = "Report")]
    public class ReportController : BaseController<CompanyTransaction>
    {
        private readonly ICompanyTransactionManager _manager;
        private readonly IMailManager _mailmanager;
        public ReportController(ICompanyTransactionManager manager, IMailManager mailmanager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
            _mailmanager = mailmanager;
        }

        public override IBaseBLL<CompanyTransaction> Manager()
        {
            return _manager;
        }


        public override IActionResult Index()
        {
            var model = InitEditViewModel();

            return View("Index");
        }

        [HttpPost]
        public IActionResult AccountSummaryPartialView([FromBody] JObject jObj)
        {
            var model = new AccountSummaryEditViewModel();

            var startDate = jObj["StartDate"].ToString();
            var endDate = jObj["EndDate"].ToString();
            var startDateTime = jObj["StartDateTime"].ToString();
            var endDateTime = jObj["EndDateTime"].ToString();

            var startDateTimeParsed = DateTime.Parse($"{startDate} {startDateTime}");
            var endDateTimeParsed = DateTime.Parse($"{endDate} {endDateTime}");

            var dealerAccountSummary = _manager.GetDealerAccountSummaryForDealer(IDCompany, startDateTimeParsed, endDateTimeParsed);

            if (dealerAccountSummary.Count > 0)
            {
                var data = new AccountSummaryEditViewModel.DealerAccountSummary()
                {
                    WithdrawalRequestTotalAmount = dealerAccountSummary.FirstOrDefault().WithdrawalRequestTotalAmount,
                    PaymentNotificationTotalAmount = dealerAccountSummary.FirstOrDefault().PaymentNotificationTotalAmount,
                    PaymentNotificationSumNetTotal = dealerAccountSummary.FirstOrDefault().PaymentNotificationSumNetTotal,
                    CreditCardSumNetTotal = dealerAccountSummary.FirstOrDefault().CreditCardSumNetTotal,
                    CreditCardSumTotal = dealerAccountSummary.FirstOrDefault().CreditCardSumTotal,
                    CreditCardCount = dealerAccountSummary.FirstOrDefault().CreditCardCount,
                    PaymentNotificationCount = dealerAccountSummary.FirstOrDefault().PaymentNotificationCount,
                    WithdrawalRequestCount = dealerAccountSummary.FirstOrDefault().WithdrawalRequestCount,
                };

                model.DealerAccountSummaries = data;
                model.DealerAccountSummaries.StartDate = startDateTimeParsed;
                model.DealerAccountSummaries.EndDate = endDateTimeParsed;
            }
            else
            {
                var data = new AccountSummaryEditViewModel.DealerAccountSummary();
                model.DealerAccountSummaries = data;
            }

            return PartialView("AccountSummaryPartialView", model);
        }
    }
}
