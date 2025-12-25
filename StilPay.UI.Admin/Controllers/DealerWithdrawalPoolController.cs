using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL.Abstract;
using StilPay.BLL;
using StilPay.DAL.Concrete;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;
using System.Linq;
using StilPay.BLL.Concrete;
using System.Collections.Generic;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Dealer")]
    public class DealerWithdrawalPoolController : BaseController<WithdrawalPool>
    {
        private readonly IWithdrawalPoolManager _manager;
        private readonly ICompanyManager _companyManager;
        private readonly ICompanyBankAccountManager _companyBankAccountManager;

        public DealerWithdrawalPoolController(IWithdrawalPoolManager manager, ICompanyManager companyManager,ICompanyBankAccountManager companyBankAccountManager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
            _companyManager = companyManager;
            _companyBankAccountManager = companyBankAccountManager;
        }

        public override IBaseBLL<WithdrawalPool> Manager()
        {
            return _manager;
        }

        public override IActionResult Index()
        {
            ViewBag.Companies = _companyManager.GetActiveList(null);
            ViewBag.CompanyBankAccounts = _companyBankAccountManager.GetActiveList(new List<FieldParameter>() { new FieldParameter("IDCompany", Enums.FieldType.NVarChar, "1312E00F-E83E-45B4-85C6-892396D12331") }).Where(x => x.IsExitAccount).ToList();

            return View();
        }

        [HttpPost]
        public IActionResult GetData()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var startDate = Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString());
            var endDate = Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString());

            var list = GetData(
                new FieldParameter("Status", Enums.FieldType.Tinyint, string.IsNullOrEmpty(HttpContext.Request.Form["Status"].ToString()) ? (byte?)null : Convert.ToByte(HttpContext.Request.Form["Status"])),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, startDate),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, endDate),
                new FieldParameter("PageLength", Enums.FieldType.Int, length),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue),
                new FieldParameter("IDBank", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["IDBank"].ToString()) ? null : HttpContext.Request.Form["IDBank"].ToString())
            );

            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;

            var result = new
            {
                recordsFiltered = recordsTotal,
                data = list
            };

            return Json(result);
        }

    }
}
