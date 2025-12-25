using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL.Abstract;
using StilPay.BLL;
using StilPay.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StilPay.UI.Dealer.Controllers
{
    [Authorize(Roles = "Process")]
    public class ProcessController : BaseController<CompanyTransaction>
    {
        private readonly ICompanyTransactionManager _manager;
        private readonly ICompanyPaymentRequestManager _managerPaymentRequest;
        private readonly IPaymentNotificationManager _managerPaymentNotification;
        private readonly ICreditCardPaymentNotificationManager _managerCreditCardPaymentNotification;
        private readonly ICompanyRebateRequestManager _managerRebate;
        private readonly IForeignCreditCardPaymentNotificationManager _managerForeignCreditCardPaymentNotification;
        private readonly ICompanyWithdrawalRequestManager _managerCompanyWithdrawalRequest;

        public ProcessController(ICompanyTransactionManager manager, ICompanyPaymentRequestManager managerPaymentRequest, IPaymentNotificationManager managerPaymentNotification, ICreditCardPaymentNotificationManager managerCreditCardPaymentNotification, ICompanyRebateRequestManager managerRebate, IForeignCreditCardPaymentNotificationManager managerForeignCreditCardPaymentNotification, IHttpContextAccessor httpContext, ICompanyWithdrawalRequestManager managerCompanyWithdrawalRequest) : base(httpContext)
        {
            _manager = manager;
            _managerPaymentNotification = managerPaymentNotification;
            _managerPaymentRequest = managerPaymentRequest;
            _managerCreditCardPaymentNotification = managerCreditCardPaymentNotification;
            _managerRebate = managerRebate;
            _managerForeignCreditCardPaymentNotification = managerForeignCreditCardPaymentNotification;
            _managerCompanyWithdrawalRequest = managerCompanyWithdrawalRequest;
        }
        public override IBaseBLL<CompanyTransaction> Manager()
        {
            return _manager;
        }

        public IActionResult GetData()
        {

            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = _manager.GetProcess(IDCompany, Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString()), Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString()), length, start, searchValue, false);

            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;

            var result = new
            {
                recordsFiltered = recordsTotal,
                data = list
            };

            return Json(result);
        }

        public IActionResult Detail(byte idActionType, string id)
        {
            if (idActionType == (byte)Enums.ActionType.PaymentAuto || idActionType == (byte)Enums.ActionType.PaymentNotify)
            {
                var model = _managerPaymentNotification.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                return View("DetailPaymentNotification", model);
            }
            else if (idActionType == (byte)Enums.ActionType.DealerPayment)
            {
                var model = _managerPaymentRequest.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                return View("DetailPaymentRequest", model);
            }
            else if (idActionType == (byte)Enums.ActionType.Rebate)
            {
                var model = _managerRebate.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                return View("DetailRebate", model);
            }
            else if (idActionType == (byte)Enums.ActionType.CreditCardPaymentNotify)
            {
                var model = _managerCreditCardPaymentNotification.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                return View("DetailCreditCardPaymentNotification", model);
            }
            else if (idActionType == (byte)Enums.ActionType.ForeignCreditCardPaymentNotify)
            {
                var model = _managerForeignCreditCardPaymentNotification.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                return View("DetailForeignCreditCardPaymentNotification", model);
            }
            else if (idActionType == (byte)Enums.ActionType.DealerWithdrawal)
            {
                var model = _managerCompanyWithdrawalRequest.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                return View("DetailWithdrawalRequest", model);
            }
            else
            {
                return View();
            }
        }
    }
}
