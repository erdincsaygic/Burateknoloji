using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.UI.Dealer.Models;
using StilPay.Utility.Helper;
using System.Collections.Generic;

namespace StilPay.UI.Dealer.Controllers
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

            model.Banks = _bankManager.GetActiveList(new List<FieldParameter>()
            {
                new FieldParameter("IsAdminPanelRequest", Enums.FieldType.Bit, false)
            });

            return model;
        }

        [HttpPost]
        public IActionResult SaveMyBank(CompanyBankAccount entity)
        {
            return base.Save(entity);
        }
    }
}
