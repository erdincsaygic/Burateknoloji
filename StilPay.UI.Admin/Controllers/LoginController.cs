using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.UI.Admin.Infrastructures;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace StilPay.UI.Admin.Controllers
{
    [AllowAnonymous]
    public class LogInController : BaseController<Administrator>
    {
        private readonly IAdministratorManager _manager;

        public LogInController(IAdministratorManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<Administrator> Manager()
        {
            return _manager;
        }

        [HttpGet]
        public override IActionResult Index()
        {
            if (User.Identity.IsAuthenticated && User.Identity.Name.Equals("Administrator"))
                return RedirectToAction("Index", "Main");
            else
            {
                
                _httpContext.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();

                return base.Index();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(LoginModel model)
        {
            var genericResponse = _httpContext.HttpContext.Session.ValidateCaptchaCode("Admin_Login_CaptchaCode", model.CaptchaCode);

            if (genericResponse.Status == "ERROR")
                return Json(genericResponse);
            else
            {
                var administrator = _manager.GetAdministrator(model.Phone, model.Password);

                if (administrator == null)
                {
                    return Json(new GenericResponse() { Status = "ERROR", Message = "Kullanıcı adı veya şifre hatalı.." });
                }
                else
                {
                    var hasSent = _httpContext.HttpContext.Session.HasSentSms(administrator.ID, "Admin_Login_ConfirmCode");
                    if (hasSent)
                        return Json(new GenericResponse() { Status = "OK" });
                    else
                    {
                        tSmsSender sender = new tSmsSender();
                        var smsResponse = sender.SendConfirmCode(model.Phone, "Admin_Login_ConfirmCode");
                        if (smsResponse.Status.Equals("OK"))
                        {
                            _httpContext.HttpContext.Session.SaveSms(administrator.ID, "Admin_Login_ConfirmCode", smsResponse.ConfirmCode);
                            return Json(new GenericResponse() { Status = "OK" });
                        }
                        else
                        {
                            return Json(new GenericResponse() { Status = "ERROR", Message = smsResponse.Message });
                        }
                    }
                }
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Validate(int confirmCode)
        {
            var genericResponse = _httpContext.HttpContext.Session.ValidateConfirmCode("Admin_Login_ConfirmCode", confirmCode);

            if (genericResponse.Status == "ERROR")
                return Json(genericResponse);

            var administrator = _manager.GetSingle(new List<FieldParameter>{
                new FieldParameter("ID", Enums.FieldType.NVarChar, genericResponse.Data.ToString())
            });

            if (administrator == null)
            {
                return Json(new GenericResponse() { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." });
            }
            else
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, "Administrator"),
                    new Claim(ClaimTypes.Sid,administrator.ID),
                    new Claim(ClaimTypes.GivenName, administrator.Name),
                    new Claim(ClaimTypes.MobilePhone, administrator.Phone),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                foreach (var role in administrator.AdministratorRoles)
                    if (role.Authorized)
                        claims.Add(new Claim(ClaimTypes.Role, role.RoleKey));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var expiresUtc = DateTime.UtcNow.AddHours(8);
                var enablecookietimeout = false;
                if (administrator.Phone == "05421230567")
                {
                    expiresUtc = DateTime.UtcNow.AddYears(1);
                    enablecookietimeout = false;
                }
                
                var authProperties = new AuthenticationProperties
                {
                    RedirectUri = "/Login/index",
                    AllowRefresh = false,
                    IsPersistent = true,
                    ExpiresUtc = expiresUtc
                };

                _httpContext.HttpContext.SignInAsync(
                   CookieAuthenticationDefaults.AuthenticationScheme,
                   new ClaimsPrincipal(claimsIdentity),
                   authProperties
               ).Wait();

                var ipAddress = _httpContext.HttpContext.Connection.RemoteIpAddress?.ToString();
                _manager.SaveLastLogin(administrator.ID, ipAddress);

                return Json(new GenericResponse() { Status = "OK", Data= enablecookietimeout });
            }
        }


        [HttpGet]
        [Route("get-captcha-image")]
        public IActionResult GetCaptchaImage()
        {
            int width = 100;
            int height = 36;
            var result = tCaptchaManager.GenerateCaptchaImage(width, height);
            _httpContext.HttpContext.Session.SaveCaptchaCode("Admin_Login_CaptchaCode", result.CaptchaCode);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }


        public IActionResult LogOut()
        {
            _httpContext.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RefreshExitDate()
        {
            var claim = _httpContext.HttpContext.User.FindFirst(f => f.Type == ClaimTypes.Sid);
            if (claim != null)
            {
                var idAdministrator = claim.Value;
                _manager.RefreshExitDate(idAdministrator);
            }

            return Json(new GenericResponse() { Status = "OK" });
        }

        [HttpPost]
        public IActionResult RefreshCookie(bool value)
        {
            if (value)
            {
                var claim = _httpContext.HttpContext.User.FindFirst(f => f.Type == ClaimTypes.Sid);
                _httpContext.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
                if (claim != null)
                {
                    var administrator = _manager.GetSingle(new List<FieldParameter>{
                        new FieldParameter("ID", Enums.FieldType.NVarChar, claim.Value)
                    });

                    if (administrator == null)
                    {
                        return Json(new GenericResponse() { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." });
                    }
                    else
                    {
                        _httpContext.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
                        var claims = new List<Claim> {
                        new Claim(ClaimTypes.Name, "Administrator"),
                        new Claim(ClaimTypes.Sid,administrator.ID),
                        new Claim(ClaimTypes.GivenName, administrator.Name),
                        new Claim(ClaimTypes.MobilePhone, administrator.Phone),
                        new Claim(ClaimTypes.Role, "Admin")
                        };

                        foreach (var role in administrator.AdministratorRoles)
                            if (role.Authorized)
                                claims.Add(new Claim(ClaimTypes.Role, role.RoleKey));

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            RedirectUri = "/Login/index",
                            AllowRefresh = false,
                            IsPersistent = true,
                            ExpiresUtc = DateTime.Now.AddMinutes(60),
                        };

                        _httpContext.HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties
                        ).Wait();

                        var ipAddress = _httpContext.HttpContext.Connection.RemoteIpAddress?.ToString();
                        _manager.SaveLastLogin(administrator.ID, ipAddress);
                    }
                }
                return Json(new GenericResponse() { Status = "OK" });
            }
            else
            {
                _httpContext.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();

                return Json(new GenericResponse() { Status = "ERROR" });
            }
        }
    }
}
