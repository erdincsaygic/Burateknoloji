using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.Entities;
using StilPay.UI.WebSite.Areas.Panel.Infrastructures;
using StilPay.UI.WebSite.Areas.Panel.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System.Collections.Generic;
using System.Security.Claims;

namespace StilPay.UI.WebSite.Areas.Panel.Controllers
{
    public abstract class BaseController<T> : Controller where T : BaseEntity, new()
    {
        protected readonly IHttpContextAccessor _httpContext;

        protected string IDMember
        {
            get
            {
                var claim = _httpContext.HttpContext.User.FindFirst(f => f.Type == ClaimTypes.Sid);
                var id = claim.Value;

                return id;
            }
        }

        protected string Name
        {
            get
            {
                var claim = _httpContext.HttpContext.User.FindFirst(f => f.Type == ClaimTypes.GivenName);
                var name = claim.Value;

                return name;
            }
        }

        protected string Phone
        {
            get
            {
                var claim = _httpContext.HttpContext.User.FindFirst(f => f.Type == ClaimTypes.MobilePhone);
                var phone = claim.Value;

                return phone;
            }
        }


        public BaseController(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }


        public abstract IBaseBLL<T> Manager();


        public virtual IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public virtual IActionResult Gets()
        {
            var list = Manager().GetList(new List<FieldParameter>
            {
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, IDMember)
            });

            return Json(list);
        }


        [HttpGet]
        public virtual IActionResult Edit(string id)
        {
            var model = InitEditViewModel(id);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual IActionResult Save(T entity)
        {
            if (!string.IsNullOrEmpty(entity.ID))
                return Json(Manager().Update(entity));
            else
                return Json(Manager().Insert(entity));
        }


        [HttpPost]
        public virtual IActionResult Drop(T entity)
        {
            return Json(Manager().Delete(entity));
        }


        [NonAction]
        public virtual EditViewModel<T> InitEditViewModel(string id = null)
        {
            var model = new EditViewModel<T>();

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


        [HttpPost]
        public IActionResult SendConfirmSms(string operationType)
        {
            var hasSent = _httpContext.HttpContext.Session.HasSentSms(Phone, operationType);
            if (hasSent)
                return Json(new GenericResponse() { Status = "OK" });
            else
            {
                tSmsSender sender = new tSmsSender();
                var smsResponse = sender.SendConfirmCode(Phone, operationType);
                if (smsResponse.Status.Equals("OK"))
                {
                    _httpContext.HttpContext.Session.SaveSms(Phone, operationType, smsResponse.ConfirmCode);
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
