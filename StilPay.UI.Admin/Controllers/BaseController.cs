using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StilPay.BLL;
using StilPay.Entities;
using StilPay.UI.Admin.Infrastructures;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace StilPay.UI.Admin.Controllers
{
    public abstract class BaseController<T> : Controller where T : BaseEntity, new()
    {
        protected readonly IHttpContextAccessor _httpContext;

        protected string IDUser
        {
            get
            {
                var claim = _httpContext.HttpContext.User.FindFirst(f => f.Type == ClaimTypes.Sid);
                var id = claim.Value;

                return id;
            }
        }
        protected string IDUserName
        {
            get
            {
                var claim = _httpContext.HttpContext.User.FindFirst(f => f.Type == ClaimTypes.GivenName);
                var name = claim.Value;

                return name;
            }
        }

        protected string Phone = "05466113687";
        //{
        //    get
        //    {
        //        var claim = _httpContext.HttpContext.User.FindFirst(f => f.Type == ClaimTypes.MobilePhone);
        //        var phone = claim.Value;

        //        return phone;
        //    }
        //}


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
            var list = GetData();

            return Json(list);
        }

        [HttpPost]
        public virtual IActionResult Gets([FromBody] JObject jObj)
        {
            var list = GetData();

            return Json(list);
        }

        [NonAction]
        protected List<T> GetData(params FieldParameter[] prmtrs)
        {
            var list = Manager().GetList(prmtrs.ToList());

            return list;
        }


        [HttpGet]
        public virtual IActionResult Edit(string id)
        {
            var model = InitEditViewModel(id);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual IActionResult Save(T entity,IFormFile file)
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

        [HttpGet]
        public IActionResult GenerateRandomPassword()
        {
            return  Json(RandomPasswordGenerator.GenerateRandomPassword());
        }


        [HttpPost]
        public IActionResult SendConfirmSms(string operationType, string message = null, string phone = null)
        {
            if (!string.IsNullOrEmpty(phone))
                Phone = phone;

            var hasSent = _httpContext.HttpContext.Session.HasSentSms(Phone, operationType);
            if (hasSent)
                return Json(new GenericResponse() { Status = "OK" });
            else
            {
                tSmsSender sender = new tSmsSender();
                var smsResponse = sender.SendConfirmCode(Phone, operationType, message);
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
