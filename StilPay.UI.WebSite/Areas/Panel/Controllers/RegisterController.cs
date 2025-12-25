using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StilPay.UI.WebSite.Areas.Panel.Controllers
{
    [Area("Panel")]
    [Authorize(Roles = "Register")]
    public class RegisterController : BaseController<Member>
    {
        private readonly IMemberManager _manager;

        public RegisterController(IMemberManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<Member> Manager()
        {
            return _manager;
        }

        public override IActionResult Index()
        {
            var model = InitEditViewModel();
            model.entity.Phone = Phone;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Member entity)
        {
            entity.Phone = Phone;
            entity.IDMemberType = "00000000-0000-0000-0000-000000000000";
            entity.StatusFlag = true;

            var response = Manager().Insert(entity);
            if (response != null && response.Status.Equals("OK"))
            {
                var member = _manager.GetSingle(new List<FieldParameter>
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, response.Data.ToString())
                });

                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, member!=null ? "Member" : "Register"),
                    new Claim(ClaimTypes.Sid, member!=null ? member.ID : "00000000-0000-0000-0000-000000000000"),
                    new Claim(ClaimTypes.GivenName, member!=null ? member.Name : "Yeni Üye"),
                    new Claim(ClaimTypes.MobilePhone, Phone),
                    new Claim(ClaimTypes.Role, member!=null ? "Member" : "Register")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    RedirectUri = "/Panel/Login/index",
                    AllowRefresh = true,
                    IsPersistent = false,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                };

                await _httpContext.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );
            }

            return Json(response);
        }
    }
}
