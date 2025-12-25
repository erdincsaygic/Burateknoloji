using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using StilPay.BLL.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace StilPay.UI.WebSite.Areas.Panel.Infrastructures
{

    public class HeaderViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContext;

        public HeaderViewComponent(IMemberManager memberManager, IHttpContextAccessor httpContext)
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
                Roles = claims.Where(w => w.Type == ClaimTypes.Role).ToList().Select(s => s.Value).ToList()
            };

            return View(model);
        }
    }

    public class SideBarViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMemberManager _memberManager;

        public SideBarViewComponent(IMemberManager memberManager, IHttpContextAccessor httpContext)
        {
            _memberManager = memberManager;
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
                Roles = claims.Where(w => w.Type == ClaimTypes.Role).ToList().Select(s => s.Value).ToList()
            };

            string idMember = claims.FirstOrDefault(f => f.Type == ClaimTypes.Sid)?.Value;
            if (!string.IsNullOrEmpty(idMember))
                model.Balance = _memberManager.GetBalance(idMember);

            return View(model);
        }
    }

    public class MenuInformation
    {
        public string LoginType { get; set; }
        public string Name { get; set; }
        public decimal? Balance { get; set; }
        public string LoginDate { get; set; }
        public string IPAddress { get; set; }
        public List<string> Roles { get; set; }

        public MenuInformation()
        {
            Roles = new List<string>();
        }
    }

}
