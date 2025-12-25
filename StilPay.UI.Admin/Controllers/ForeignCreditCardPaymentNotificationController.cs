using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.BLL.Concrete;
using StilPay.Entities.Concrete;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.PayNKolay.Models.CancelRefundPayment;
using StilPay.Utility.PayNKolay;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Globalization;
using ParamPosLiveReference;
using StilPay.Utility.PayNKolay.Models.PaymentList;
using System.Linq;
using System.Text.Json;
using StilPay.Utility.Models;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "PendingProcess")]
    public class ForeignCreditCardPaymentNotificationController : BaseController<ForeignCreditCardPaymentNotification>
    {
        private readonly IForeignCreditCardPaymentNotificationManager _manager;
        private readonly IMemberTypeManager _managerMemberType;
        private readonly ICompanyIntegrationManager _companyIntegrationManager;
        private readonly ICallbackResponseLogManager _callbackResponseLogManager;

        public ForeignCreditCardPaymentNotificationController(IForeignCreditCardPaymentNotificationManager manager, ICompanyIntegrationManager companyIntegrationManager, IMemberTypeManager managerMemberType, ICompanyIntegrationManager companyIntegration, IHttpContextAccessor httpContext, ICallbackResponseLogManager callbackResponseLogManager) : base(httpContext)
        {
            _manager = manager;
            _managerMemberType = managerMemberType;
            _companyIntegrationManager = companyIntegration;
            _callbackResponseLogManager = callbackResponseLogManager;
        }

        public override IBaseBLL<ForeignCreditCardPaymentNotification> Manager()
        {
            return _manager;
        }

        public IActionResult GetDataAuto()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = GetData(
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.Pending),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null),
                new FieldParameter("IsAutoNotification", Enums.FieldType.Tinyint, 1),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("PageLenght", Enums.FieldType.Int, length),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue)
            );


            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;

            var result = new
            {
                recordsFiltered = recordsTotal,
                data = list
            };

            return Json(result);
        }

        public IActionResult GetDataManuel()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = GetData(
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.Pending),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null),
                new FieldParameter("IsAutoNotification", Enums.FieldType.Tinyint, 0),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("PageLenght", Enums.FieldType.Int, length),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue)
            );


            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;

            var result = new
            {
                recordsFiltered = recordsTotal,
                data = list
            };

            return Json(result);
        }
        public override EditViewModel<ForeignCreditCardPaymentNotification> InitEditViewModel(string id = null)
        {
            var model = new ForeignCreditCardPaymentNotificationEditViewModel();

            model.entity = Manager().GetSingle(new List<FieldParameter>()
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, id)
            });

            model.MemberTypes = _managerMemberType.GetActiveList(null);

            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetStatus(ForeignCreditCardPaymentNotification entity)
        {

            var creditCardEntity = _manager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, entity.ID) });
            creditCardEntity.MDate = DateTime.Now;
            creditCardEntity.MUser = IDUser;
            creditCardEntity.Status = entity.Status;
            creditCardEntity.Description = entity.Description ?? creditCardEntity.Description;
            var _companyIntegration = _companyIntegrationManager.GetByServiceId(creditCardEntity.ServiceID);
            var callbackEntity = new CallbackResponseLog();
            var opt = new JsonSerializerOptions() { WriteIndented = true };

            var response = _manager.SetStatus(creditCardEntity);
            if (response.Status == "OK" && creditCardEntity.Status == (byte)Enums.StatusType.Confirmed)
            {
                var dataCallback = new
                {
                    status_code =  "OK",
                    status_type = 1,
                    service_id = creditCardEntity.ServiceID,
                    ciphered = tMD5Manager.EncryptBasic(_companyIntegration.SecretKey),
                    data = new { transaction_id = creditCardEntity.TransactionID, sp_transactionNr = creditCardEntity.TransactionNr, amount = creditCardEntity.Amount, sp_id = creditCardEntity.ID, message = entity.Description, currencyCode = creditCardEntity.CurrencyCode },
                    user_entered_data = new { member = creditCardEntity.Member, sender_name = creditCardEntity.SenderName, action_date = creditCardEntity.ActionDate, action_time = creditCardEntity.ActionTime, creditCard = entity.CardNumber, amount = creditCardEntity.Amount, user_ip = creditCardEntity.MemberIPAddress, user_port = creditCardEntity.MemberPort }
                };

                var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(_companyIntegration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                callbackEntity.TransactionID = creditCardEntity.TransactionID;
                callbackEntity.ServiceType = "STILPAY";
                callbackEntity.IDCompany = _companyIntegration.ID;
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                callbackEntity.TransactionType = "YURT DISI KREDI KARTI ODEMESI MANUEL ONAY";
                _callbackResponseLogManager.Insert(callbackEntity);
            }

            if (response.Status == "OK" && creditCardEntity.Status == (byte)Enums.StatusType.Canceled)
            {
                var dataCallback = new
                {
                    status_code = "ERROR",
                    status_type = 1,
                    service_id = creditCardEntity.ServiceID,
                    ciphered = tMD5Manager.EncryptBasic(_companyIntegration.SecretKey),
                    data = new { transaction_id = creditCardEntity.TransactionID, sp_transactionNr = creditCardEntity.TransactionNr, amount = creditCardEntity.Amount, sp_id = creditCardEntity.ID, message = entity.Description },
                    user_entered_data = new { member = creditCardEntity.Member, sender_name = creditCardEntity.SenderName, action_date = creditCardEntity.ActionDate, action_time = creditCardEntity.ActionTime, creditCard = entity.CardNumber, amount = creditCardEntity.Amount, user_ip = creditCardEntity.MemberIPAddress, user_port = creditCardEntity.MemberPort }
                };

                var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(_companyIntegration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                callbackEntity.TransactionID = creditCardEntity.TransactionID;
                callbackEntity.ServiceType = "STILPAY";
                callbackEntity.IDCompany = _companyIntegration.ID;
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                callbackEntity.TransactionType = "YURT DISI KREDI KARTI ODEMESI MANUEL IPTAL";
                _callbackResponseLogManager.Insert(callbackEntity);
            }

            return Json(response);
        }
    }
}
