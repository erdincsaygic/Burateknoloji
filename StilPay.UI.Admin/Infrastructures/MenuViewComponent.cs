using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace StilPay.UI.Admin.Infrastructures
{
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
                Roles = claims.Where(w => w.Type == ClaimTypes.Role).ToList().Select(s => s.Value).ToList()
            };

            return View(model);
        }
    }

    public class HeaderViewComponent : ViewComponent
    {
        IHttpContextAccessor _httpContext;
        public HeaderViewComponent(IHttpContextAccessor httpContext)
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
                Roles = claims.Where(w => w.Type == ClaimTypes.Role).ToList().Select(s => s.Value).ToList()
            };

            return View(model);
        }
    }

    public class MenuInformation
    {
        public string LoginType { get; set; }
        public string Name { get; set; }
        public List<string> Roles { get; set; }

        public MenuInformation()
        {
            Roles = new List<string>();
        }
    }

}
