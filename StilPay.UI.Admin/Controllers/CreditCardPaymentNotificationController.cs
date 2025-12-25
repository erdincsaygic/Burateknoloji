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
    public class CreditCardPaymentNotificationController : BaseController<CreditCardPaymentNotification>
    {
        private readonly ICreditCardPaymentNotificationManager _manager;
        private readonly IMemberTypeManager _managerMemberType;
        private readonly ICompanyIntegrationManager _companyIntegrationManager;
        private readonly IPaymentNotificationManager _paymentNotificationManager;
        private readonly ICreditCardPaymentNotificationManager _creditCardPaymentNotificationManager;
        private readonly ICallbackResponseLogManager _callbackResponseLogManager;

        public CreditCardPaymentNotificationController(ICreditCardPaymentNotificationManager manager, IPaymentNotificationManager paymentNotificationManager, ICreditCardPaymentNotificationManager creditCardPaymentNotificationManager, ICompanyIntegrationManager companyIntegrationManager, IMemberTypeManager managerMemberType, ICompanyIntegrationManager companyIntegration, ICallbackResponseLogManager callbackResponseLogManager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
            _managerMemberType = managerMemberType;
            _companyIntegrationManager = companyIntegration;
            _paymentNotificationManager = paymentNotificationManager;
            _creditCardPaymentNotificationManager = creditCardPaymentNotificationManager;
            _callbackResponseLogManager = callbackResponseLogManager;
        }

        public override IBaseBLL<CreditCardPaymentNotification> Manager()
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
        public override EditViewModel<CreditCardPaymentNotification> InitEditViewModel(string id = null)
        {
            var model = new CreditCardPaymentNotificationEditViewModel();

            model.entity = Manager().GetSingle(new List<FieldParameter>()
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, id)
            });

            model.MemberTypes = _managerMemberType.GetActiveList(null);

            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetStatus(CreditCardPaymentNotification entity)
        {
            int responseStatus = 0;
            var creditCardEntity = _creditCardPaymentNotificationManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, entity.ID) });
            creditCardEntity.MDate = DateTime.Now;
            creditCardEntity.MUser = IDUser;
            creditCardEntity.Status = entity.Status;
            creditCardEntity.Description = entity.Description ?? creditCardEntity.Description;
            var companyIntegration = _companyIntegrationManager.GetByServiceId(creditCardEntity.ServiceID);          
            var callbackEntity = new CallbackResponseLog();
            var opt = new JsonSerializerOptions() { WriteIndented = true };

            var response = _manager.SetStatus(creditCardEntity);

            if(response.Status == "OK" && entity.CreditCardPaymentMethodID == (byte)Enums.CreditCardPaymentMethodType.Tosla)
            {
                var dataCallback = new
                {
                    status_code = creditCardEntity.Status == (byte)Enums.StatusType.Confirmed ? "OK" : "ERROR",
                    status_type = 3,
                    service_id = creditCardEntity.ServiceID,
                    ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                    data = new
                    {
                        transaction_id = creditCardEntity.TransactionID,
                        transfer_date = creditCardEntity.MDate,
                        amount = creditCardEntity.Amount,
                        phoneNumber = creditCardEntity.Phone
                    }
                };

                var responseCallback = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                if (responseCallback != null && responseCallback.Result != null && !string.IsNullOrEmpty(responseCallback.Result.Status))
                {
                    responseStatus = responseCallback.Result.Status switch
                    {
                        "OK" => 1,
                        "RED" => 2,
                        "ERROR" => 3,
                        _ => 0,
                    };
                }

                callbackEntity.TransactionID = creditCardEntity.TransactionID;
                callbackEntity.ServiceType = "STILPAY";
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                callbackEntity.ResponseStatus = (byte)responseStatus;
                callbackEntity.TransactionType = creditCardEntity.Status == (byte)Enums.StatusType.Confirmed ? "TOSLA ODEMESI MANUEL ONAY" : "TOSLA ODEMESI MANUEL IPTAL";
                _callbackResponseLogManager.Insert(callbackEntity);
            }

            if (response.Status == "OK" && creditCardEntity.Status == (byte)Enums.StatusType.Confirmed)
            {
                var cardSenderName = "";

                string searchTerm = "Gönderici Adı:";
                int startIndex = creditCardEntity.Description.IndexOf(searchTerm);

                if (startIndex != -1)
                {
                    startIndex += searchTerm.Length;
                    cardSenderName = creditCardEntity.Description.Substring(startIndex).Trim();
                }

                var dataCallback = new
                {
                    status_code =  "OK",
                    status_type = 1,
                    service_id = creditCardEntity.ServiceID,
                    ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                    data = new { transaction_id = creditCardEntity.TransactionID, sp_transactionNr = creditCardEntity.TransactionNr, amount = creditCardEntity.Amount, sp_id = creditCardEntity.ID, message = creditCardEntity.Description },
                    user_entered_data = new { member = creditCardEntity.Member, sender_name = creditCardEntity.SenderName, action_date = creditCardEntity.ActionDate, action_time = creditCardEntity.ActionTime, creditCard = creditCardEntity.CardNumber, amount = creditCardEntity.Amount, user_ip = creditCardEntity.MemberIPAddress, user_port = creditCardEntity.MemberPort, cardSenderNameFromBank = cardSenderName }
                };

                var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                callbackEntity.TransactionID = creditCardEntity.TransactionID;
                callbackEntity.ServiceType = "STILPAY";
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                callbackEntity.TransactionType = "KREDI KARTI ODEMESI MANUEL ONAY";
                _callbackResponseLogManager.Insert(callbackEntity);
            }

            if (response.Status == "OK" && creditCardEntity.Status == (byte)Enums.StatusType.Canceled)
            {
                var dataCallback = new
                {
                    status_code = "ERROR",
                    status_type = 1,
                    service_id = creditCardEntity.ServiceID,
                    ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                    data = new { transaction_id = creditCardEntity.TransactionID, sp_transactionNr = creditCardEntity.TransactionNr, amount = creditCardEntity.Amount, sp_id = creditCardEntity.ID, message = creditCardEntity.Description },
                    user_entered_data = new { member = creditCardEntity.Member, sender_name = creditCardEntity.SenderName, action_date = creditCardEntity.ActionDate, action_time = creditCardEntity.ActionTime, creditCard = creditCardEntity.CardNumber, amount = creditCardEntity.Amount, user_ip = creditCardEntity.MemberIPAddress, user_port = creditCardEntity.MemberPort }
                };

                var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                callbackEntity.TransactionID = creditCardEntity.TransactionID;
                callbackEntity.ServiceType = "STILPAY";
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                callbackEntity.TransactionType = "KREDI KARTI ODEMESI MANUEL IPTAL";
                _callbackResponseLogManager.Insert(callbackEntity);
            }

            return Json(response);
        }
    }
}
