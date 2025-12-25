using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Announcement")]
    public class AnnouncementController : BaseController<Announcement>
    {
        private readonly IAnnouncementManager _manager;

        public AnnouncementController(IAnnouncementManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<Announcement> Manager()
        {
            return _manager;
        }

        public override EditViewModel<Announcement> InitEditViewModel(string id = null)
        {
            var model = new AnnouncementEditViewModel();
            model.entity.StatusFlag = true;
            model.entity.StartDate = model.entity.EndDate = DateTime.Today;


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
    }
}
