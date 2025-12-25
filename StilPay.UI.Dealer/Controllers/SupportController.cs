using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL.Abstract;
using StilPay.BLL;
using StilPay.Entities.Concrete;
using System.Xml.Linq;
using StilPay.UI.Dealer.Models;
using StilPay.Utility.Helper;
using System;
using StilPay.Utility.Worker;

namespace StilPay.UI.Dealer.Controllers
{
    [Authorize(Roles = "Support")]
    public class SupportController : BaseController<Support>
    {
        private readonly ISupportManager _manager;
        private readonly IMailManager _mailmanager;
        public SupportController(ISupportManager manager, IMailManager mailmanager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
            _mailmanager = mailmanager;
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
            var list = GetData(
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, IDCompany),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("Status", Enums.FieldType.Tinyint, null)
            );

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
            entity.IDCompany = IDCompany;
            var mails = _mailmanager.GetList(null);
            foreach (var item in mails)
            {
                if (item.Category == "Talep")
                {
                    MailSender.SendEmail(entity.Email, item.Name, item.Body);
                }
            }
            return base.Save(entity);
        }
    }
}
