using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL.Abstract;
using StilPay.BLL;
using StilPay.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using StilPay.Utility.Helper;
using System.Collections.Generic;
using System.Reflection;

namespace StilPay.UI.WebSite.Areas.Panel.Controllers
{
    [Area("Panel")]
    [Authorize(Roles = "Member")]
    public class MasterController : BaseController<MemberProcess>
    {
        private readonly IMemberProcessManager _manager;
        private readonly IMemberMoneyTransferRequestManager _managerMoneyTransfer;
        private readonly IMemberPaymentRequestManager _managerPaymentRequest;
        private readonly IMemberWithdrawalRequestManager _managerWithdrawalRequest;

        public MasterController(IMemberProcessManager manager, IMemberMoneyTransferRequestManager managerMoneyTransfer, IMemberPaymentRequestManager managerPaymentRequest, IMemberWithdrawalRequestManager managerWithdrawalRequest, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
            _managerMoneyTransfer = managerMoneyTransfer;
            _managerPaymentRequest = managerPaymentRequest;
            _managerWithdrawalRequest = managerWithdrawalRequest;
        }

        public override IBaseBLL<MemberProcess> Manager()
        {
            return _manager;
        }

        public IActionResult Detail(byte idActionType, string id)
        {
            if (idActionType == (byte)Enums.ActionType.MoneyTransferOut)
            {
                var model = _managerMoneyTransfer.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                return View("DetailMoneyTransfer", model);
            }
            else if (idActionType == (byte)Enums.ActionType.MemberPayment)
            {
                var model = _managerPaymentRequest.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                return View("DetailPayment", model);
            }
            else if (idActionType == (byte)Enums.ActionType.MemberWithdrawal)
            {
                var model = _managerWithdrawalRequest.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                return View("DetailWithdrawal", model);
            }
            else
            {
                return View();
            }
        }
    }
}
