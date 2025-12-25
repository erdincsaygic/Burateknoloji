using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.UI.Dealer.Controllers;
using StilPay.UI.Dealer.Models;
using StilPay.Utility.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Dealer")]
    public class MainController : BaseController<Main>
    {
        private readonly IMainManager _manager;
        private readonly ICompanyManager _companyManager;
        private readonly IPaymentNotificationManager _paymentNotificationManager;
        private readonly IAnnouncementManager _announcementManager;
        private readonly ICompanyCurrencyManager _companyCurrencyManager;

        public MainController(IMainManager manager, ICompanyManager companyManager, IPaymentNotificationManager paymentNotificationManager, IAnnouncementManager announcementManager, IHttpContextAccessor httpContext, ICompanyCurrencyManager companyCurrencyManager) : base(httpContext)
        {
            _manager = manager;
            _companyManager = companyManager;
            _paymentNotificationManager = paymentNotificationManager;
            _announcementManager = announcementManager;
            _companyCurrencyManager = companyCurrencyManager;
        }

        public override IBaseBLL<Main> Manager()
        {
            return _manager;
        }
        public override IActionResult Index()
        {
            var claims = _httpContext.HttpContext.User.Claims.ToList();

            var model = new MainModel()
            {
                Dealer = claims.FirstOrDefault(f => f.Type == ClaimTypes.NameIdentifier)?.Value,
                LoginDate = claims.FirstOrDefault(f => f.Type == ClaimTypes.Expiration)?.Value,
                LoginIP = claims.FirstOrDefault(f => f.Type == ClaimTypes.StreetAddress)?.Value,
            };

            string idCompany = claims.FirstOrDefault(f => f.Type == ClaimTypes.GroupSid)?.Value;
            if (!string.IsNullOrEmpty(idCompany))
            {
                var balances = _companyManager.GetBalance(idCompany);
                if (balances != null)
                {
                    model.UsingBalance = balances.UsingBalance;
                    model.BlockedBalance = balances.BlockedBalance;
                    model.TotalBalance = balances.TotalBalance;
                }
            }

            model.CompanyCurrencies = _companyCurrencyManager.GetList(new List<FieldParameter> 
            {
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, IDCompany),
            });

            model.PaymentNotifications = _paymentNotificationManager.GetList(new List<FieldParameter> {
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.Pending),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, IDCompany),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null)
            });

            model.entity = new Support { Name = Name, Phone = Phone };

            model.Announcements = _announcementManager.GetActiveList(null);

            return View(model);
        }
    }
}
