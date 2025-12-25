using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.UI.Admin.Controllers;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Account")]
    public class BankAccountController : BaseController<CompanyBankAccount>
    {
        private readonly ICompanyBankAccountManager _manager;
        private readonly IBankManager _bankManager;

        public BankAccountController(ICompanyBankAccountManager manager, IBankManager bankManager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
            _bankManager = bankManager;
        }

        public override IBaseBLL<CompanyBankAccount> Manager()
        {
            return _manager;
        }


        [HttpPost]
        public override IActionResult Gets([FromBody] JObject jObj)
        {
            var list = _manager.GetList(null);
            return Json(list);
        }

        public override EditViewModel<CompanyBankAccount> InitEditViewModel(string id = null)
        {
            var model = new BankAccountEditViewModel();

            if (!string.IsNullOrEmpty(id))
            {
                var entity = _manager.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                model.entity = entity;
            }

            //model.Banks = _bankManager.GetList(null);

            return model;
        }
        //[HttpPost]
        //public IActionResult SaveMyBank(CompanyBankAccount entity)
        //{
        //    return base.Save(entity);
        //}
    }
}
