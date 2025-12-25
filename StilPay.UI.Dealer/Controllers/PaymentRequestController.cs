using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.UI.Dealer.Models;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StilPay.UI.Dealer.Controllers
{
    [Authorize(Roles = "PaymentRequest")]
    public class PaymentRequestController : BaseController<CompanyPaymentRequest>
    {
        private readonly ICompanyPaymentRequestManager _manager;
        private readonly ICompanyBankManager _companyBankManager;

        public PaymentRequestController(ICompanyPaymentRequestManager manager, ICompanyBankManager companyBankManager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
            _companyBankManager = companyBankManager;
        }

        public override IBaseBLL<CompanyPaymentRequest> Manager()
        {
            return _manager;
        }

        [HttpPost]
        public override IActionResult Gets([FromBody] JObject jObj)
        {
            var list = GetData(
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.All),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, IDCompany),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, Convert.ToDateTime(jObj["StartDate"].ToString())),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, Convert.ToDateTime(jObj["EndDate"].ToString()))
            );

            return Json(list);
        }


        public override EditViewModel<CompanyPaymentRequest> InitEditViewModel(string id = null)
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

            model.CompanyBanks = _companyBankManager.GetActiveList(new List<FieldParameter>
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, IDCompany)
            }).Where(f => f.IDBank == "07").ToList();

            return model;
        }

        public override IActionResult Save(CompanyPaymentRequest entity)
        {
            entity.Status = (byte)Enums.StatusType.Pending;

            return base.Save(entity);
        }
    }
}
