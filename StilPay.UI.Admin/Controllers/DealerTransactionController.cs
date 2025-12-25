using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Dealer")]
    public class DealerTransactionController : BaseController<CompanyTransaction>
    {
        private readonly ICompanyTransactionManager _manager;

        public DealerTransactionController(ICompanyTransactionManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<CompanyTransaction> Manager()
        {
            return _manager;
        }
        
        public IActionResult Detail()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetListOld([FromBody] JObject jObj)
        {
            var list = _manager.GetListOld(string.IsNullOrEmpty(jObj["IDCompany"].ToString()) ? "0" : jObj["IDCompany"].ToString() == "all" ? null : jObj["IDCompany"].ToString(), Convert.ToDateTime(jObj["StartDate"].ToString()), Convert.ToDateTime(jObj["EndDate"].ToString()));

            return Json(list);
        }

        public IActionResult GetData()
        {

            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = Manager().GetList(new List<FieldParameter>
            {
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["IDCompany"].ToString()) ? "0" : HttpContext.Request.Form["IDCompany"].ToString() == "all" ? null : HttpContext.Request.Form["IDCompany"].ToString()),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString())),
                new FieldParameter("EndDate", Enums.FieldType.DateTime,  Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString())),
                new FieldParameter("PageLenght", Enums.FieldType.Int, length),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue)
            });

            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;

            var result = new
            {
                recordsFiltered = recordsTotal,
                total = list.Count == 0 ? 0 : list.FirstOrDefault().Balance,
                data = list,
            };

            return Json(result);
        }


        public IActionResult UnbilledTransactions()
        {
            return View();
        }


        [HttpPost]
        public IActionResult GetUnbilledTransactions([FromBody] JObject jObj)
        {
            var list = GetData(
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, string.IsNullOrEmpty(jObj["IDCompany"].ToString()) ? null : jObj["IDCompany"].ToString()),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, Convert.ToDateTime(jObj["StartDate"].ToString())),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, Convert.ToDateTime(jObj["EndDate"].ToString())),
                new FieldParameter("IDActionType", Enums.FieldType.NVarChar, string.IsNullOrEmpty(jObj["IDActionType"].ToString()) ? null : jObj["IDActionType"].ToString()),
                new FieldParameter("IDCompanyInvoice", Enums.FieldType.NVarChar, null),
                new FieldParameter("InvoiceStatus", Enums.FieldType.Bit, true)
            );

            return Json(list);
        }

        

        [HttpPost]
        public JsonResult ConvertToInvoice([FromBody]List<string> idList)
        {
            var result = _manager.ConvertToInvoice(idList);
            return Json(result);
        }


    }
}
