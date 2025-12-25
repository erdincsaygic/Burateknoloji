using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Linq;


namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class MailController : BaseController<Mail>
    {
        private readonly IMailManager _manager;
        private readonly ICompanyManager _companyManager;
        private readonly IMailLogManager _mailLogManager;

        public MailController(IMailManager manager, ICompanyManager companyManager, IMailLogManager mailLogManager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
            _companyManager = companyManager;
            _mailLogManager = mailLogManager;
        }

        public override IBaseBLL<Mail> Manager()
        {
            return _manager;
        }
        public override EditViewModel<Mail> InitEditViewModel(string id = null)
        {
            var model = new EditViewModel<Mail>();
            model.entity.StatusFlag = true;

            if (!string.IsNullOrEmpty(id))
            {
                var entity = Manager().GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                model.entity = entity;
            }

            return model;
        }

        [HttpGet]
        public IActionResult GetData()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetDataList()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = _mailLogManager.GetList( new List<FieldParameter>
            {
                new FieldParameter("PageLength", Enums.FieldType.Int, length),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue),
                new FieldParameter("IsSuccess", Enums.FieldType.Tinyint, string.IsNullOrEmpty(HttpContext.Request.Form["IsSuccess"].ToString()) ? (byte?)null : Convert.ToByte(HttpContext.Request.Form["IsSuccess"])),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["IDCompany"].ToString()) ? "0" : HttpContext.Request.Form["IDCompany"].ToString() == "all" ? null : HttpContext.Request.Form["IDCompany"].ToString())
            });

            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;

            var result = new
            {
                recordsFiltered = recordsTotal,
                data = list
            };

            return Json(result);
        }

        public IActionResult SendMail()
        {
            var model = new EditViewModelWithoutInterface<SendMailDto>();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendMail(EditViewModelWithoutInterface<SendMailDto> model)
        {
            var sendMailDto = model.entity;

            var failedSendMailCount = 0;
            var successSendMailCount = 0;

            foreach (var item in sendMailDto.IDCompanies)
            {
                var company = _companyManager.GetSingle(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, item) });
                var response = MailSender.SendEmail(company.Email, sendMailDto.Title, sendMailDto.Body);

                if (response == "OK")
                {
                    var mailLog = new MailLog()
                    {
                        Body = sendMailDto.Body,
                        Email = company.Email,
                        Title = sendMailDto.Title,
                        CUser = IDUser,
                        CDate = DateTime.Now,
                        IsSuccess = true,
                        IDCompany = item
                    };

                    _mailLogManager.Insert(mailLog);
                    successSendMailCount++;
                }
                else
                {
                    var mailLog = new MailLog()
                    {
                        Body = sendMailDto.Body,
                        Email = company.Email,
                        Title = sendMailDto.Title,
                        CUser = IDUser,
                        CDate = DateTime.Now,
                        IsSuccess = false,
                        IDCompany = item
                    };

                    _mailLogManager.Insert(mailLog);
                    failedSendMailCount++;
                }
            }

            if (failedSendMailCount > 0)
                return Json(new GenericResponse { Status = "ERROR", Message = $"{successSendMailCount} üye işyerine mail gönderildi. {failedSendMailCount} üye işyerine mail gönderilemedi. Mail loglarını kontrol ediniz." });

            return Json(new GenericResponse { Status = "OK", Message = "Tüm üye işyerlerine mail başarıyla gönderildi." });

        }
    }
}
