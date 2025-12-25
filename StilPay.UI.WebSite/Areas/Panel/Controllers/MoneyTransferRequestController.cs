using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;

namespace StilPay.UI.WebSite.Areas.Panel.Controllers
{
    [Area("Panel")]
    [Authorize(Roles = "Member")]
    public class MoneyTransferRequestController : BaseController<MemberMoneyTransferRequest>
    {
        private readonly IMemberMoneyTransferRequestManager _manager;

        public MoneyTransferRequestController(IMemberMoneyTransferRequestManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<MemberMoneyTransferRequest> Manager()
        {
            return _manager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override IActionResult Save(MemberMoneyTransferRequest entity)
        {
            entity.ReceiverPhone = "0" + entity.ReceiverPhone;
            entity.CostTotal = 6;
            entity.Status = (byte)Enums.StatusType.Pending;

            return base.Save(entity);
        }
    }
}
