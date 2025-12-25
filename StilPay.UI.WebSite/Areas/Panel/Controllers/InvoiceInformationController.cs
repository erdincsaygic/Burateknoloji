using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL.Abstract;
using StilPay.BLL;
using StilPay.Entities.Concrete;
using StilPay.UI.WebSite.Areas.Panel.Models;
using StilPay.Utility.Helper;
using System.Collections.Generic;

namespace StilPay.UI.WebSite.Areas.Panel.Controllers
{
    [Area("Panel")]
    [Authorize(Roles = "Member")]
    public class InvoiceInformationController : BaseController<MemberInvoiceInformation>
    {
        private readonly IMemberInvoiceInformationManager _manager;
        private readonly ICityManager _cityManager;
        private readonly ITownManager _townManager;

        public InvoiceInformationController(IMemberInvoiceInformationManager manager, ICityManager cityManager, ITownManager townManager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
            _cityManager = cityManager;
            _townManager = townManager;
        }

        public override IBaseBLL<MemberInvoiceInformation> Manager()
        {
            return _manager;
        }

        public override IActionResult Index()
        {
            var model = InitEditViewModel();

            return View(model);
        }

        public override EditViewModel<MemberInvoiceInformation> InitEditViewModel(string id = null)
        {
            var model = new InvoiceInformationEditViewModel();

            model.entity = _manager.GetSingle(new List<FieldParameter>()
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, IDMember)
            });

            model.Cities = _cityManager.GetActiveList(null);
            model.Towns = _townManager.GetActiveList(null);

            return model;
        }
    }
}