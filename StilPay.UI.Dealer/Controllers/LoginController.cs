using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.UI.Dealer.Infrastructures;
using StilPay.UI.Dealer.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Claims;

namespace StilPay.UI.Dealer.Controllers
{
    [AllowAnonymous]
    public class LogInController : BaseController<CompanyUser>
    {
        private readonly ICompanyUserManager _manager;
        private readonly ICompanyApplicationManager _managerApplication;

        public LogInController(ICompanyUserManager manager, ICompanyApplicationManager managerApplication, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
            _managerApplication = managerApplication;
        }

        public override IBaseBLL<CompanyUser> Manager()
        {
            return _manager;
        }

        [HttpGet]
        public override IActionResult Index()
        {
            if (User.Identity.IsAuthenticated && User.Identity.Name.Equals("Dealer"))
                return RedirectToAction("Index", "Main");
            else if (User.Identity.IsAuthenticated && User.Identity.Name.Equals("Visitor"))
                return RedirectToAction("Index", "Application");
            else
            {
                _httpContext.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();

                return Redirect("https://stilpay.com/");
            }
        }

        [HttpGet]
        public IActionResult Frame()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Frame(LoginModel model)
        {
            var genericResponse = _httpContext.HttpContext.Session.ValidateCaptchaCode("Dealer_Login_CaptchaCode", model.CaptchaCode);
            if (genericResponse.Status == "ERROR")
                return Json(genericResponse);

            var companyUser = _manager.GetUser(model.Phone, model.Password);
            var application = _managerApplication.GetApplication(model.Phone, model.Password);

            var ipAddress = _httpContext.HttpContext.Connection.RemoteIpAddress?.ToString();

            if (companyUser != null && companyUser.IPAddress != null && !string.IsNullOrEmpty(companyUser.IPAddress.Trim()) && !companyUser.IPAddress.Contains(ipAddress))
                return Json(new GenericResponse() { Status = "ERROR", Message = "Yetkisiz IP.." });

            if (companyUser == null && application == null)
            {
                return Json(new GenericResponse() { Status = "ERROR", Message = "Kullanıcı adı veya şifre hatalı.." });
            }

            else
            {
                var id = companyUser == null ? application.ID : companyUser.ID;
                tSmsSender sender = new tSmsSender();
                var smsResponse = sender.SendConfirmCode(model.Phone, "Dealer_Login_ConfirmCode");
                if (smsResponse.Status.Equals("OK"))
                {
                    _httpContext.HttpContext.Session.SaveSms(id, "Dealer_Login_ConfirmCode", smsResponse.ConfirmCode);
                    return Json(new GenericResponse() { Status = "OK" });
                }
                else
                {
                    return Json(new GenericResponse() { Status = "ERROR", Message = smsResponse.Message });
                }
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Validate(int confirmCode)
        {
            GenericResponse genericResponse = null;

            genericResponse = _httpContext.HttpContext.Session.ValidateConfirmCode("Dealer_Login_ConfirmCode", confirmCode);

            if (genericResponse.Status == "ERROR")
                return Json(genericResponse);
            else
            {
                var companyUser = _manager.GetSingle(new List<FieldParameter>{
                    new FieldParameter("ID", Enums.FieldType.NVarChar, genericResponse.Data.ToString())
                });

                var application = _managerApplication.GetSingle(new List<FieldParameter>{
                    new FieldParameter("ID", Enums.FieldType.NVarChar, genericResponse.Data.ToString())
                });

                var claims = new List<Claim>();

                if (companyUser == null && application == null)
                {
                    return Json(new GenericResponse() { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." });
                }
                else if (companyUser != null && !string.IsNullOrEmpty(companyUser.ID))
                {
                    claims.Add(new Claim(ClaimTypes.Name, "Dealer"));
                    claims.Add(new Claim(ClaimTypes.Sid, companyUser.ID));
                    claims.Add(new Claim(ClaimTypes.GivenName, companyUser.Name));
                    claims.Add(new Claim(ClaimTypes.MobilePhone, companyUser.Phone));
                    claims.Add(new Claim(ClaimTypes.Email, companyUser.Email));
                    claims.Add(new Claim(ClaimTypes.GroupSid, companyUser.IDCompany));
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, companyUser.Company));
                    claims.Add(new Claim(ClaimTypes.Expiration, companyUser.LoginDate.HasValue ? companyUser.LoginDate.Value.ToString("dd.MM.yyyy HH:mm") : "-"));
                    claims.Add(new Claim(ClaimTypes.StreetAddress, companyUser.IPAddress ?? "-"));
                    claims.Add(new Claim(ClaimTypes.SerialNumber, companyUser.ServiceID));
                    claims.Add(new Claim(ClaimTypes.Role, "Dealer"));

                    foreach (var role in companyUser.CompanyUserRoles)
                        if (role.Authorized)
                            claims.Add(new Claim(ClaimTypes.Role, role.RoleKey));


                    //_manager.SaveLastLogin(companyUser.ID, ipAddress);
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Name, "Visitor"));
                    claims.Add(new Claim(ClaimTypes.Sid, "00000000-0000-0000-0000-000000000000"));
                    claims.Add(new Claim(ClaimTypes.GivenName, application.Name));
                    claims.Add(new Claim(ClaimTypes.MobilePhone, application.Phone));
                    claims.Add(new Claim(ClaimTypes.Email, application.Email));
                    claims.Add(new Claim(ClaimTypes.GroupSid, application.ID));
                    //claims.Add(new Claim(ClaimTypes.NameIdentifier, application.Title));
                    claims.Add(new Claim(ClaimTypes.SerialNumber, "0000"));
                    claims.Add(new Claim(ClaimTypes.Role, "Visitor"));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var expiresUtc = DateTime.UtcNow.AddMinutes(15);

                if (companyUser.IDCompany == "6B90574C-67D1-41EA-9EF8-6EB6B404BC86" || companyUser.IDCompany == "E8C8427C-FCB6-4467-AB1B-9A3B36942C52"
                    || companyUser.IDCompany == "23F9E630-F1C2-41CC-8552-66ED0B3C64EE" || companyUser.IDCompany == "07C54300-9BE7-4AB9-8C3F-F4772D9E49D4")
                {
                    expiresUtc = DateTime.UtcNow.AddHours(12);
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

                return Json(new GenericResponse() { Status = "OK" });
            }
        }


        [HttpGet]
        [Route("get-captcha-image")]
        public IActionResult GetCaptchaImage()
        {
            int width = 100;
            int height = 36;
            var result = tCaptchaManager.GenerateCaptchaImage(width, height);
            _httpContext.HttpContext.Session.SaveCaptchaCode("Dealer_Login_CaptchaCode", result.CaptchaCode);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }


        public IActionResult LogOut()
        {
            _httpContext.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RefreshCookie(bool value)
        {
            if (value)
            {
                var claim = _httpContext.HttpContext.User.FindFirst(f => f.Type == ClaimTypes.Sid);

                var companyUser = _manager.GetSingle(new List<FieldParameter>{
                    new FieldParameter("ID", Enums.FieldType.NVarChar, claim.Value)
                });

                var application = _managerApplication.GetSingle(new List<FieldParameter>{
                    new FieldParameter("ID", Enums.FieldType.NVarChar, claim.Value)
                });

                var claims = new List<Claim>();

                if (companyUser == null && application == null)
                {
                    return Json(new GenericResponse() { Status = "ERROR", Message = "Lütfen tekrar deneyiniz.." });
                }
                else if (companyUser != null && !string.IsNullOrEmpty(companyUser.ID))
                {
                    claims.Add(new Claim(ClaimTypes.Name, "Dealer"));
                    claims.Add(new Claim(ClaimTypes.Sid, companyUser.ID));
                    claims.Add(new Claim(ClaimTypes.GivenName, companyUser.Name));
                    claims.Add(new Claim(ClaimTypes.MobilePhone, companyUser.Phone));
                    claims.Add(new Claim(ClaimTypes.Email, companyUser.Email));
                    claims.Add(new Claim(ClaimTypes.GroupSid, companyUser.IDCompany));
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, companyUser.Company));
                    claims.Add(new Claim(ClaimTypes.Expiration, companyUser.LoginDate.HasValue ? companyUser.LoginDate.Value.ToString("dd.MM.yyyy HH:mm") : "-"));
                    claims.Add(new Claim(ClaimTypes.StreetAddress, companyUser.IPAddress ?? "-"));
                    claims.Add(new Claim(ClaimTypes.SerialNumber, companyUser.ServiceID));
                    claims.Add(new Claim(ClaimTypes.Role, "Dealer"));

                    foreach (var role in companyUser.CompanyUserRoles)
                        if (role.Authorized)
                            claims.Add(new Claim(ClaimTypes.Role, role.RoleKey));

                    var ipAddress = _httpContext.HttpContext.Connection.RemoteIpAddress?.ToString();
                    //_manager.SaveLastLogin(companyUser.ID, ipAddress);
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Name, "Visitor"));
                    claims.Add(new Claim(ClaimTypes.Sid, "00000000-0000-0000-0000-000000000000"));
                    claims.Add(new Claim(ClaimTypes.GivenName, application.Name));
                    claims.Add(new Claim(ClaimTypes.MobilePhone, application.Phone));
                    claims.Add(new Claim(ClaimTypes.Email, application.Email));
                    claims.Add(new Claim(ClaimTypes.GroupSid, application.ID));
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, application.Title));
                    claims.Add(new Claim(ClaimTypes.SerialNumber, "0000"));
                    claims.Add(new Claim(ClaimTypes.Role, "Visitor"));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    RedirectUri = "/Login/index",
                    AllowRefresh = false,
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(15)
                };

                _httpContext.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                ).Wait();

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
