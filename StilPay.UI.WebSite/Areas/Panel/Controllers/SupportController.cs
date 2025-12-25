using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.UI.WebSite.Areas.Panel.Models;
using StilPay.Utility.Helper;
using System.Collections.Generic;

namespace StilPay.UI.WebSite.Areas.Panel.Controllers
{
    [Area("Panel")]
    [Authorize(Roles = "Member")]
    public class SupportController : BaseController<Support>
    {
        private readonly ISupportManager _manager;

        public SupportController(ISupportManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<Support> Manager()
        {
            return _manager;
        }


        public override IActionResult Index()
        {
            var model = InitEditViewModel();

            return View(model);
        }

        public override IActionResult Gets()
        {
            var list = Manager().GetList(new List<FieldParameter>
            {
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, IDMember),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("Status", Enums.FieldType.Tinyint, null)
            });

            return Json(list);
        }

        public override EditViewModel<Support> InitEditViewModel(string id = null)
        {
            var model = new EditViewModel<Support>()
            {
                entity = new Support { Name = Name, Phone = Phone }
            };

            return model;
        }

        public override IActionResult Save(Support entity)
        {
            entity.IDMember = IDMember;

            return base.Save(entity);
        }
    }
}