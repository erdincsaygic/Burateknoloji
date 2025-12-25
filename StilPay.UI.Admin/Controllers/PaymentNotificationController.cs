using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.Models;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "PendingProcess")]
    public class PaymentNotificationController : BaseController<PaymentNotification>
    {
        private readonly IPaymentNotificationManager _manager;
        private readonly IMemberTypeManager _managerMemberType;
        private readonly ICompanyIntegrationManager _companyIntegrationManager;
        private readonly IBankManager _bankManager;
        private readonly ICallbackResponseLogManager _callbackResponseLogManager;

        public PaymentNotificationController(IPaymentNotificationManager manager,ICallbackResponseLogManager callbackResponseLogManager,IMemberTypeManager managerMemberType,IBankManager bankManager, ICompanyIntegrationManager companyIntegration, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
            _managerMemberType = managerMemberType;
            _companyIntegrationManager = companyIntegration;
            _bankManager = bankManager; 
            _callbackResponseLogManager = callbackResponseLogManager;
        }

        public override IBaseBLL<PaymentNotification> Manager()
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

        public override EditViewModel<PaymentNotification> InitEditViewModel(string id = null)
        {
            var model = new PaymentNotificationEditViewModel();

            model.entity = Manager().GetSingle(new List<FieldParameter>()
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, id)
            });

            model.MemberTypes = _managerMemberType.GetActiveList(null);

            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetStatus(PaymentNotification entity)
        {
            var pyEntity = _manager.GetSingle(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, entity.ID) });

            if (pyEntity.Status != (byte)Enums.StatusType.Pending)
                return Json(new GenericResponse { Status = "OK", Message = "İşlem Yapılmak İstenilen Bildirim İçin Daha Önce İşlem Yapılmış." });

            pyEntity.MDate = DateTime.Now;
            pyEntity.MUser = IDUser;
            pyEntity.Status = entity.Status;
            pyEntity.Description = entity.Description ?? pyEntity.Description;
            var response = _manager.SetStatus(pyEntity);

            if (response.Status == "OK")
            {
                var companyIntegration = _companyIntegrationManager.GetByServiceId(pyEntity.ServiceID);

                var dataCallback = new
                {
                    status_code = "ERROR",
                    service_id = pyEntity.ServiceID,
                    status_type = 0,
                    ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                    data = new { transaction_id = pyEntity.TransactionID, sp_transactionNr = pyEntity.TransactionNr, amount = pyEntity.Amount, sp_id = pyEntity.ID, message = pyEntity.Description },
                    user_entered_data = new { member = pyEntity.Member, sender_name = pyEntity.SenderName, action_date = pyEntity.ActionDate, action_time = pyEntity.ActionTime, amount = pyEntity.Amount, user_ip = pyEntity.MemberIPAddress, user_port = pyEntity.MemberPort }
                };

                var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                var callbackEntity = new CallbackResponseLog();
                var opt = new JsonSerializerOptions() { WriteIndented = true };
                callbackEntity.TransactionID = entity.TransactionID;
                callbackEntity.ServiceType = "STILPAY";
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.TransactionType = "Ödeme Bildirimi Manuel İptal";
                callbackEntity.ResponseStatus = ((byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0));
                _callbackResponseLogManager.Insert(callbackEntity);

                return Json(new GenericResponse { Status = "OK", Message = response.Message });
            }
            else
                return Json(new GenericResponse { Status = "ERROR", Message = response.Message });
        }
    }
}
