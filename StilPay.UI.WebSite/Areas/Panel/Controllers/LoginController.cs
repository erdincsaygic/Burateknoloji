using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.UI.WebSite.Areas.Panel.Helpers;
using StilPay.UI.WebSite.Areas.Panel.Infrastructures;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;

namespace StilPay.UI.WebSite.Areas.Panel.Controllers
{
    [Area("Panel")]
    [AllowAnonymous]
    public class LoginController : BaseController<Member>
    {
        private readonly IMemberManager _manager;

        public LoginController(IMemberManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<Member> Manager()
        {
            return _manager;
        }


        [HttpGet]
        public override IActionResult Index()
        {
            if (User.Identity.IsAuthenticated && User.Identity.Name.Equals("Member"))
                return RedirectToAction("Index", "Master", new { area = "panel" });
            else if (User.Identity.IsAuthenticated && User.Identity.Name.Equals("Register"))
                return RedirectToAction("Index", "Register", new { area = "panel" });
            else
            {
                _httpContext.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();

                return RedirectToAction("Index", "Giris", new { area = "" });
            }
        }


        [HttpGet]
        public IActionResult Frame()
        {
            return View();

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Frame(LoginHelper model)
        {
            return Json(new GenericResponse() { Status = "ERROR", Message = "Lütfen Daha Sonra Tekrar Deneyiniz." });
            //var genericResponse = _httpContext.HttpContext.Session.ValidateCaptchaCode("Member_Login_CaptchaCode", model.CaptchaCode);
            //if (genericResponse.Status == "ERROR")
            //    return Json(genericResponse);

            //tSmsSender sender = new tSmsSender();
            //var smsResponse = sender.SendConfirmCode(model.Phone, "Member_Login_ConfirmCode");
            //if (smsResponse.Status.Equals("OK"))
            //{
            //    _httpContext.HttpContext.Session.SaveSms(model.Phone, "Member_Login_ConfirmCode", smsResponse.ConfirmCode);
            //    return Json(new GenericResponse() { Status = "OK" });
            //}
            //else
            //{
            //    return Json(new GenericResponse() { Status = "ERROR", Message = smsResponse.Message });
            //}
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Validate(int confirmCode)
        {
            var genericResponse = _httpContext.HttpContext.Session.ValidateConfirmCode("Member_Login_ConfirmCode", confirmCode);
            if (genericResponse.Status == "ERROR")
                return Json(genericResponse);

            var phone = genericResponse.Data.ToString();

            var member = _manager.GetMember(phone);

            var claims = new List<Claim> {
                new Claim(ClaimTypes.MobilePhone, phone),
                new Claim(ClaimTypes.Name, member!=null ? "Member" : "Register"),
                new Claim(ClaimTypes.Sid, member!=null ? member.ID : "00000000-0000-0000-0000-000000000000"),
                new Claim(ClaimTypes.GivenName, member!=null ? member.Name : "Yeni Üye"),
                new Claim(ClaimTypes.Expiration, member.LoginDate.HasValue ? member.LoginDate.Value.ToString("dd.MM.yyyy") : "-"),
                new Claim(ClaimTypes.StreetAddress, member.IPAddress ?? "-"),
                new Claim(ClaimTypes.Role, member!=null ? "Member" : "Register")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                RedirectUri = "/Panel/Login/index",
                AllowRefresh = true,
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
            };

            _httpContext.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            ).Wait();

            var ipAddress = _httpContext.HttpContext.Connection.RemoteIpAddress?.ToString();
            _manager.SaveLastLogin(member.ID, ipAddress);

            return Json(new GenericResponse() { Status = "OK" });
        }


        [HttpGet]
        [Route("get-captcha-image")]
        public IActionResult GetCaptchaImage()
        {
            int width = 100;
            int height = 36;
            var result = tCaptchaManager.GenerateCaptchaImage(width, height);
            _httpContext.HttpContext.Session.SaveCaptchaCode("Member_Login_CaptchaCode", result.CaptchaCode);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }


        [HttpGet]
        public IActionResult LogOut()
        {
            _httpContext.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();

            return RedirectToAction("Index");
        }
    }
}
