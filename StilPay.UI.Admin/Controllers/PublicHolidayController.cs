using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL.Abstract;
using StilPay.BLL;
using StilPay.Entities.Concrete;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;
using StilPay.BLL.Concrete;
using System.IO;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "SystemSetting")]
    public class PublicHolidayController : BaseController<PublicHoliday>
    {
        private readonly IPublicHolidayManager _manager;

        public PublicHolidayController(IPublicHolidayManager manager,IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<PublicHoliday> Manager()
        {
            return _manager;
        }

        public override IActionResult Gets()
        {
            var list = GetData();
            return Json(list);
        }

        public override EditViewModel<PublicHoliday> InitEditViewModel(string id = null)
        {
            var model = new EditViewModel<PublicHoliday>();

            if (!string.IsNullOrEmpty(id))
            {
                var entity = Manager().GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });
                model.entity = entity;

                model.entity.HolidayDate = entity.HolidayDate;
            }
            else
                model.entity.HolidayDate = DateTime.Today;
                
            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override IActionResult Save(PublicHoliday entity, IFormFile file)
        {
            if (!string.IsNullOrEmpty(entity.ID))

                return Json(Manager().Update(entity));
            else
                return Json(Manager().Insert(entity));
        }
    }
}
