using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using static System.Net.Mime.MediaTypeNames;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using StilPay.Utility.Worker;
using System.Linq;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Dealer")]
    public class ApplicationController : BaseController<CompanyApplication>
    {
        private readonly ICompanyApplicationManager _manager;
        private readonly IMailManager _mailmanager;

        public ApplicationController(ICompanyApplicationManager manager, IMailManager mailmanager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
            _mailmanager = mailmanager;
        }

        public override IBaseBLL<CompanyApplication> Manager()
        {
            return _manager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Download(string formName, byte[] formFile)
        {
            return File(formFile, "application/octet-stream", formName);
        }

        [HttpPost]
        public IActionResult GetData()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = _manager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("PageLenght", Enums.FieldType.Int, length),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue)
            });

            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;

            var result = new
            {
                recordsFiltered = recordsTotal,
                data = list
            };

            return Json(result);
        }

        [HttpGet]
        public IActionResult ShowFile(string id, string name)
        {
            var application = _manager.GetSingle(new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar,id)
            });

            if (name.Equals("IdentityFrontSide"))
                return new FileContentResult(application.IdentityFrontSide, "application/pdf");
            else if (name.Equals("IdentityBackSide"))
                return new FileContentResult(application.IdentityBackSide, "application/pdf");
            else if (name.Equals("TaxPlate"))
                return new FileContentResult(application.TaxPlate, "application/pdf");
            else if (name.Equals("SignatureCirculars"))
                return new FileContentResult(application.SignatureCirculars, "application/pdf");
            else if (name.Equals("TradeRegistryGazette"))
                return new FileContentResult(application.TradeRegistryGazette, "application/pdf");
            else if (name.Equals("Agreement"))
                return new FileContentResult(application.Agreement, "application/pdf");
            else
                return new FileContentResult(null, "application/pdf");
        }

        [HttpPost]
        public IActionResult SetFileStatus([FromBody] JObject jObj)
        {
            var id = jObj["ID"].ToString();
            var file = jObj["File"].ToString();
            var status = Convert.ToByte(jObj["Status"]);

            GenericResponse response = _manager.SetFileStatus(id, file, status, IDUser);

            return Json(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetApplicationStatus(string id, bool status)
        {
            GenericResponse response = null;

            var applicaiton = _manager.GetSingle(new List<FieldParameter>
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, id)
            });

            response = _manager.SetApplicationStatus(id, IDUser, status);

            if (response.Status == "OK")
            {
                tSmsSender sender = new tSmsSender();
                string msg = string.Concat("Sayın ", applicaiton.Name, " başvurunuz onaylanmıştır.");
                sender.SendSms(applicaiton.Phone, msg);
                var mails = _mailmanager.GetList(null);
                foreach (var item in mails)
                {
                    if (item.Category=="Onay")
                    {
                        MailSender.SendEmail(applicaiton.Email, item.Name, item.Body);
                    }
                }
            }


            return Json(response);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetApplicationStatusCancel(string id, bool status)
        {
            GenericResponse response = null;

            var applicaiton = _manager.GetSingle(new List<FieldParameter>
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, id)
            });

            response = _manager.SetApplicationStatus(id, IDUser, status);

            if (response.Status == "OK")
            {
                tSmsSender sender = new tSmsSender();
                string msg = string.Concat("Sayın ", applicaiton.Name, " başvurunuz rededilmiştir.");
                sender.SendSms(applicaiton.Phone, msg);
                var mails = _mailmanager.GetList(null);
                foreach (var item in mails)
                {
                    if (item.Category == "RED")
                    {
                        MailSender.SendEmail(applicaiton.Email, item.Name, item.Body);
                    }
                }
            }


            return Json(response);
        }

        [ValidateAntiForgeryToken]
        public IActionResult SendSmsWithBody(string id, string messageBody)
        {
            var applicaiton = _manager.GetSingle(new List<FieldParameter>
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, id)
            });

            tSmsSender sender = new tSmsSender();
            string msg = string.Concat("Sayın ", applicaiton.Name, "\n", $"{messageBody}");
            var response = sender.SendSms(applicaiton.Phone, msg);
            

            return Json(response);
        }
    }
}
