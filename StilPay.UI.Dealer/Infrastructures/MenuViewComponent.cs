using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using StilPay.BLL.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace StilPay.UI.Dealer.Infrastructures
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly ICompanyManager _companyManager;

        public HeaderViewComponent(ICompanyManager companyManager, IHttpContextAccessor httpContext)
        {
            _companyManager = companyManager;
            _httpContext = httpContext;
        }

        public ViewViewComponentResult Invoke()
        {
            var claims = _httpContext.HttpContext.User.Claims.ToList();

            var model = new MenuInformation()
            {
                LoginType = claims.FirstOrDefault(f => f.Type == ClaimTypes.Name)?.Value,
                Name = claims.FirstOrDefault(f => f.Type == ClaimTypes.GivenName)?.Value,
                LoginDate = claims.FirstOrDefault(f => f.Type == ClaimTypes.Expiration)?.Value,
                IPAddress = claims.FirstOrDefault(f => f.Type == ClaimTypes.StreetAddress)?.Value,
                ServiceID = claims.FirstOrDefault(f => f.Type == ClaimTypes.SerialNumber)?.Value,
                Title = claims.FirstOrDefault(f => f.Type == ClaimTypes.NameIdentifier)?.Value,
                Roles = claims.Where(w => w.Type == ClaimTypes.Role).ToList().Select(s => s.Value).ToList()
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

            return View(model);
        }
    }

    public class NavBarViewComponent : ViewComponent
    {
        IHttpContextAccessor _httpContext;
        public NavBarViewComponent(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public ViewViewComponentResult Invoke()
        {
            var claims = _httpContext.HttpContext.User.Claims.ToList();

            var model = new MenuInformation()
            {
                LoginType = claims.FirstOrDefault(f => f.Type == ClaimTypes.Name)?.Value,
                Name = claims.FirstOrDefault(f => f.Type == ClaimTypes.GivenName)?.Value,
                LoginDate = claims.FirstOrDefault(f => f.Type == ClaimTypes.Expiration)?.Value,
                IPAddress = claims.FirstOrDefault(f => f.Type == ClaimTypes.StreetAddress)?.Value,
                ServiceID = claims.FirstOrDefault(f => f.Type == ClaimTypes.SerialNumber)?.Value,
                Title = claims.FirstOrDefault(f => f.Type == ClaimTypes.NameIdentifier)?.Value,
                Roles = claims.Where(w => w.Type == ClaimTypes.Role).ToList().Select(s => s.Value).ToList()
            };

            return View(model);
        }
    }

    public class SideBarViewComponent : ViewComponent
    {
        IHttpContextAccessor _httpContext;
        public SideBarViewComponent(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public ViewViewComponentResult Invoke()
        {
            var claims = _httpContext.HttpContext.User.Claims.ToList();

            var model = new MenuInformation()
            {
                LoginType = claims.FirstOrDefault(f => f.Type == ClaimTypes.Name)?.Value,
                Name = claims.FirstOrDefault(f => f.Type == ClaimTypes.GivenName)?.Value,
                LoginDate = claims.FirstOrDefault(f => f.Type == ClaimTypes.Expiration)?.Value,
                IPAddress = claims.FirstOrDefault(f => f.Type == ClaimTypes.StreetAddress)?.Value,
                ServiceID = claims.FirstOrDefault(f => f.Type == ClaimTypes.SerialNumber)?.Value,
                Title = claims.FirstOrDefault(f => f.Type == ClaimTypes.NameIdentifier)?.Value,
                Roles = claims.Where(w => w.Type == ClaimTypes.Role).ToList().Select(s => s.Value).ToList()
            };

            return View(model);
        }
    }

    public class MenuInformation
    {
        public string LoginType { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public decimal UsingBalance { get; set; }
        public decimal BlockedBalance { get; set; }
        public decimal TotalBalance { get; set; }
        public string LoginDate { get; set; }
        public string IPAddress { get; set; }
        public string ServiceID { get; set; }
        public List<string> Roles { get; set; }

        public MenuInformation()
        {
            Roles = new List<string>();
        }
    }

}
