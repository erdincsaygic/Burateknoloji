using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;
using System.Linq;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Member")]
    public class MemberMoneyTransferTransactionController : BaseController<MemberMoneyTransferRequest>
    {
        private readonly IMemberMoneyTransferRequestManager _manager;

        public MemberMoneyTransferTransactionController(IMemberMoneyTransferRequestManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<MemberMoneyTransferRequest> Manager()
        {
            return _manager;
        }

        [HttpPost]
        public override IActionResult Gets([FromBody] JObject jObj)
        {
            var list = GetData(
                new FieldParameter("Status", Enums.FieldType.Tinyint, null),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, string.IsNullOrEmpty(jObj["IDMember"].ToString()) ? null : jObj["IDMember"].ToString()),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, Convert.ToDateTime(jObj["StartDate"].ToString())),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, Convert.ToDateTime(jObj["EndDate"].ToString()))
            );

            return Json(list);
        }

        [HttpPost]
        public IActionResult GetData()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = GetData(
                new FieldParameter("Status", Enums.FieldType.Tinyint, null),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["IDMember"].ToString()) ? null : HttpContext.Request.Form["IDMember"].ToString()),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString())),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString())),
                new FieldParameter("PageLenght", Enums.FieldType.Int, length),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue)
            );


            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;

            var result = new
            {
                recordsFiltered = recordsTotal,
                data = list
            };

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetStatus(MemberMoneyTransferRequest entity)
        {

            return Json(_manager.SetStatus(entity));
        }
    }
}
