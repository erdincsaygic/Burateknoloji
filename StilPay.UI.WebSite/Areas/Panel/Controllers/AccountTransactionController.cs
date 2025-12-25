using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL.Abstract;
using StilPay.BLL;
using StilPay.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using StilPay.Utility.Helper;
using System.Collections.Generic;

namespace StilPay.UI.WebSite.Areas.Panel.Controllers
{
    [Area("Panel")]
    [Authorize(Roles = "Member")]
    public class AccountTransactionController : BaseController<MemberTransaction>
    {
        private readonly IMemberTransactionManager _manager;

        public AccountTransactionController(IMemberTransactionManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<MemberTransaction> Manager()
        {
            return _manager;
        }

        [HttpGet]
        public override IActionResult Gets()
        {
            var list = Manager().GetList(new List<FieldParameter>
            {
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, IDMember),
                new FieldParameter("StartDate", Enums.FieldType.NVarChar, null),
                new FieldParameter("EndDate", Enums.FieldType.NVarChar, null)
            });

            return Json(list);
        }
    }
}
