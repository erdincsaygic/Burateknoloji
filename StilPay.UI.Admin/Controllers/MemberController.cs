using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System.Collections.Generic;
using System.Linq;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Member")]
    public class MemberController : BaseController<Member>
    {
        private readonly IMemberManager _manager;

        public MemberController(IMemberManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<Member> Manager()
        {
            return _manager;
        }

        [HttpPost]
        public IActionResult GetActives()
        {   
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = Manager().GetActiveList(new List<FieldParameter>()
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

        public IActionResult GetData()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = Manager().GetList(new List<FieldParameter>()
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

        public IActionResult RebateTransactions()
        {
            return View();
        }

        public IActionResult RebateTransactionDetail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetMemberType(Member entity)
        {

            return Json(_manager.SetMemberType(entity));
        }

    }
}
