using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.UI.WebSite.Areas.Panel.Models;
using StilPay.Utility.Helper;
using System.Collections.Generic;

namespace StilPay.UI.WebSite.Areas.Panel.Controllers
{
    [Area("Panel")]
    [Authorize(Roles = "Member")]
    public class WithdrawalRequestController : BaseController<MemberWithdrawalRequest>
    {
        private readonly IMemberWithdrawalRequestManager _manager;
        private readonly IBankManager _bankManager;

        public WithdrawalRequestController(IMemberWithdrawalRequestManager manager, IBankManager bankManager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
            _bankManager = bankManager;
        }

        public override IBaseBLL<MemberWithdrawalRequest> Manager()
        {
            return _manager;
        }

        public override EditViewModel<MemberWithdrawalRequest> InitEditViewModel(string id = null)
        {
            var model = new WithdrawalRequestEditViewModel();

            if (!string.IsNullOrEmpty(id))
            {
                var entity = Manager().GetSingle(new List<FieldParameter>()
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
        [ValidateAntiForgeryToken]
        public override IActionResult Save(MemberWithdrawalRequest entity)
        {
            entity.IBAN = "TR" + entity.IBAN;
            entity.CostTotal = 6;
            entity.Status = (byte)Enums.StatusType.Pending;

            return base.Save(entity);
        }
    }
}
