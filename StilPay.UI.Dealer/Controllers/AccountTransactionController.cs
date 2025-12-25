using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StilPay.UI.Dealer.Controllers
{
    [Authorize(Roles = "AccountTransaction")]
    public class AccountTransactionController : BaseController<CompanyTransaction>
    {
        private readonly ICompanyTransactionManager _manager;

        public AccountTransactionController(ICompanyTransactionManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<CompanyTransaction> Manager()
        {
            return _manager;
        }

        [HttpGet]
        public IActionResult GetListOld()
        {
            var list = _manager.GetListOld(IDCompany, null, null);

            return Json(list);
        }


        public IActionResult GetData()
        {

            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = Manager().GetList(new List<FieldParameter>
            {
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, IDCompany),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("PageLenght", Enums.FieldType.Int, length),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue)
            });

            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;

            var result = new
            {
                recordsFiltered = recordsTotal,
                total = list.Count == 0 ? 0 : list.FirstOrDefault().Balance,
                data = list
            };

            return Json(result);
        }
    }
}
