using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Configuration;
using StilPay.Entities.Concrete;
using System.Text.Json;
using StilPay.BLL.Abstract;
using StilPay.Utility.Helper;
using StilPay.Utility.ToslaSanalPos.Models.ToslaGetRefCodeStatus;
using StilPay.Utility.ToslaSanalPos;
using System.Threading;
using StilPay.Utility.Worker;
using StilPay.Utility.Models;
using System.Collections.Generic;
using static StilPay.ApiService.Models.ExternalCallback;
using static StilPay.Utility.Helper.Enums;
using System.Linq;
using System.Security.Cryptography.Xml;
using RestSharp;

namespace StilPay.ApiService.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/external-callback")]
    public class ExternalCallbackController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ICreditCardPaymentNotificationManager _creditCardPaymentNotificationManager;
        private readonly ICompanyIntegrationManager _companyIntegrationManager;
        private readonly ICallbackResponseLogManager _callbackResponseLogManager;
        private readonly IPaymentInstitutionManager _paymentInstitutionManager;

        public ExternalCallbackController(IConfiguration configuration, ICreditCardPaymentNotificationManager creditCardPaymentNotificationManager, ICompanyIntegrationManager companyIntegrationManager, ICallbackResponseLogManager callbackResponseLogManager, IPaymentInstitutionManager paymentInstitutionManager)
        {
            _configuration = configuration;
            _creditCardPaymentNotificationManager = creditCardPaymentNotificationManager;
            _companyIntegrationManager = companyIntegrationManager;
            _callbackResponseLogManager = callbackResponseLogManager;
            _paymentInstitutionManager = paymentInstitutionManager;
        }

        [HttpPost("ToslaCallback")]
        public Models.ExternalCallback.ToslaCallbackResponseModel ToslaCallback(Models.ExternalCallback.ToslaCallbackModel toslaCallbackModel)
        {
            try
            {
                int responseStatus = 0;
                var callbackEntity = new CallbackResponseLog();
                var opt = new JsonSerializerOptions() { WriteIndented = true };
                var entity = _creditCardPaymentNotificationManager.GetSingleByTransactionID(toslaCallbackModel.processId);
                var integration = _companyIntegrationManager.GetByServiceId(entity.ServiceID);

                var commission = _paymentInstitutionManager.GetList(null).FirstOrDefault(x => x.ID == ((int)Enums.CreditCardPaymentMethodType.Tosla).ToString());

                callbackEntity.TransactionID = toslaCallbackModel.processId;
                callbackEntity.ServiceType = "TOSLA";
                callbackEntity.IDCompany = integration.ID;
                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(toslaCallbackModel, opt);
                callbackEntity.TransactionType = "TOSLA TRANSACTION RESPONSE";
                _callbackResponseLogManager.Insert(callbackEntity);

                if (entity != null && entity.Status == (byte)Enums.StatusType.Pending)
                {
                    entity.Status = entity.IsAutoNotification ? (byte)StatusType.Confirmed : (byte)StatusType.Pending;
                    entity.MUser = "00000000-0000-0000-0000-000000000000";
                    entity.MDate = DateTime.Now;
                    entity.Description = "İşlem Başarılı";
                    entity.PaymentInstitutionCommissionRate = commission.Rate;
                    entity.PaymentInstitutionNetAmount = entity.Amount - (entity.Amount * commission.Rate / 100);
                    entity.TransactionReferenceCode = toslaCallbackModel.transactionId;

                    var response =_creditCardPaymentNotificationManager.SetStatus(entity);

                    if(response != null && response.Status == "OK") 
                    {
                        var dataCallback = new
                        {
                            status_code = "OK",
                            status_type = 3,
                            service_id = entity.ServiceID,
                            ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                            data = new
                            {
                                transaction_id = entity.TransactionID,
                                transfer_date = entity.MDate,
                                amount = entity.Amount,
                                phoneNumber = entity.Phone
                            }
                        };


                        var responseCallback = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

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

                        tSQLBankManager.AddCallbackResponseLog(entity.TransactionID, "STILPAY", System.Text.Json.JsonSerializer.Serialize(dataCallback, opt), integration.ID, "Tosla Ödeme Bildirimi", responseStatus);

                        return new ToslaCallbackResponseModel()
                        {
                            isSuccess = true,
                            message = "success"
                        };
                    }
                }

                return new ToslaCallbackResponseModel()
                {
                    isSuccess = false,
                    message = "error"
                };
            }
            catch (Exception ex) 
            {
                return new ToslaCallbackResponseModel()
                {
                    isSuccess = false,
                    message = ex.Message
                };
            }
        }

    }
}
