using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL.Abstract;
using StilPay.BLL;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System.Xml.Linq;
using StilPay.UI.Admin.Models;
using Newtonsoft.Json.Linq;
using System;
using StilPay.UI.Admin.Infrastructures;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Support")]
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

        public IActionResult Pending()
        {
            return View();
        }

        public IActionResult Done()
        {
            return View();
        }

        [HttpPost]
        public override IActionResult Gets(JObject jObj)
        {
            var list = GetData(
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, (jObj["IDCompany"].IsNullOrEmpty() ? null : jObj["IDCompany"].ToString())),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, (jObj["IDMember"].IsNullOrEmpty() ? null : jObj["IDMember"].ToString())),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, (jObj["StartDate"].IsNullOrEmpty() ? (DateTime?)null : Convert.ToDateTime(jObj["StartDate"].ToString()))),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, (jObj["EndDate"].IsNullOrEmpty() ? (DateTime?)null : Convert.ToDateTime(jObj["EndDate"].ToString()))),
                new FieldParameter("Status", Enums.FieldType.Tinyint, (jObj["Status"].IsNullOrEmpty() ? (byte?)null : Convert.ToByte(jObj["Status"].ToString())))
            );

            return Json(list);
        }
    }
}
