using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.BLL.Concrete;
using StilPay.Entities.Concrete;
using StilPay.UI.WebSite.Areas.Panel.Infrastructures;
using StilPay.UI.WebSite.Areas.Panel.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Web;

namespace StilPay.UI.WebSite.Areas.Panel.Controllers
{
    [Area("Panel")]
    [Authorize(Roles = "Member")]
    public class PaymentRequestController : BaseController<MemberPaymentRequest>
    {
        private readonly IMemberPaymentRequestManager _manager;
        private readonly IBankManager _bankManager;

        public PaymentRequestController(IMemberPaymentRequestManager manager, IBankManager bankManager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
            _bankManager = bankManager;
        }

        public override IBaseBLL<MemberPaymentRequest> Manager()
        {
            return _manager;
        }

        public override EditViewModel<MemberPaymentRequest> InitEditViewModel(string id = null)
        {
            var model = new PaymentRequestEditViewModel();

            if (!string.IsNullOrEmpty(id))
            {
                var entity = _manager.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });
                model.entity = entity;
            }

            model.Banks = _bankManager.GetActiveList(new List<FieldParameter>() { new FieldParameter("IsAdminPanelRequest", Enums.FieldType.Bit, false) });

            return model;
        }

        public override IActionResult Save(MemberPaymentRequest entity)
        {
            entity.Status = (byte)Enums.StatusType.Pending;

            return base.Save(entity);
        }

    }
}