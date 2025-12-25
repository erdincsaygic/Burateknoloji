using Coravel.Invocable;
using DocumentFormat.OpenXml.Office.CustomUI;
using StilPay.BLL.Abstract;
using StilPay.BLL.Concrete;
using StilPay.Entities.Concrete;
using StilPay.Entities;
using StilPay.Utility.Helper;
using StilPay.Utility.Models;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text.Json;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Vml.Office;

namespace StilPay.BLL.Jobs
{
    public class CreditCardAutoCancel : IInvocable
    {
        public readonly ICreditCardPaymentNotificationManager _creditCardPaymentNotificationManager;
        public readonly IForeignCreditCardPaymentNotificationManager _foreignCreditCardPaymentNotificationManager;
        public readonly ICompanyIntegrationManager _companyIntegrationManager;
        public readonly ICallbackResponseLogManager _callbackResponseLogManager;
        public CreditCardAutoCancel(ICreditCardPaymentNotificationManager creditCardPaymentNotificationManager, IForeignCreditCardPaymentNotificationManager foreignCreditCardPaymentNotificationManager, ICompanyIntegrationManager companyIntegrationManager, ICallbackResponseLogManager callbackResponseLogManager) 
        {
            _creditCardPaymentNotificationManager = creditCardPaymentNotificationManager;
            _foreignCreditCardPaymentNotificationManager = foreignCreditCardPaymentNotificationManager;
            _companyIntegrationManager = companyIntegrationManager;
            _callbackResponseLogManager = callbackResponseLogManager;
        }

        public async Task Invoke()
        {
            var creditCardList = _creditCardPaymentNotificationManager.GetPendingList();
            var callbackEntity = new CallbackResponseLog();
            var opt = new JsonSerializerOptions() { WriteIndented = true };

            foreach (var item in creditCardList)
            {
                item.Status = (byte)Enums.StatusType.Canceled;
                item.Description = "Müşteri İşlemi Tamamlamadı.";
                var response = _creditCardPaymentNotificationManager.SetStatus(item);

                if (response.Status == "OK")
                {
                    var companyIntegration = _companyIntegrationManager.GetByServiceId(item.ServiceID);
                    var dataCallback = new
                    {
                        status_code = "ERROR",
                        status_type = 1,
                        service_id = item.ServiceID,
                        ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                        data = new { transaction_id = item.TransactionID, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = item.Description },
                        user_entered_data = new { member = item.Member, sender_name = item.SenderName, action_date = item.ActionDate, action_time = item.ActionTime, creditCard = item.CardNumber, amount = item.Amount, user_ip = item.MemberIPAddress, user_port = item.MemberPort }
                    };

                    var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                    callbackEntity.TransactionID = item.TransactionID;
                    callbackEntity.ServiceType = "STILPAY";
                    callbackEntity.IDCompany = companyIntegration.ID;
                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                    callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                    callbackEntity.TransactionType = "KREDİ KARTI ÖDEMESİ ZAMAN AŞIMI";
                    _callbackResponseLogManager.Insert(callbackEntity);
                }
            }

            var foreignCreditCardList = _foreignCreditCardPaymentNotificationManager.GetPendingList();
            foreach (var item in foreignCreditCardList)
            {
                item.Status = (byte)Enums.StatusType.Canceled;
                item.Description = "Müşteri İşlemi Tamamlamadı.";
                var response = _foreignCreditCardPaymentNotificationManager.SetStatus(item);

                if (response.Status == "OK")
                {
                    var companyIntegration = _companyIntegrationManager.GetByServiceId(item.ServiceID);
                    var dataCallback = new
                    {
                        status_code = "ERROR",
                        status_type = 1,
                        service_id = item.ServiceID,
                        ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                        data = new { transaction_id = item.TransactionID, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = item.Description },
                        user_entered_data = new { member = item.Member, sender_name = item.SenderName, action_date = item.ActionDate, action_time = item.ActionTime, creditCard = item.CardNumber, amount = item.Amount, user_ip = item.MemberIPAddress, user_port = item.MemberPort }
                    };

                    var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                    callbackEntity.TransactionID = item.TransactionID;
                    callbackEntity.ServiceType = "STILPAY";
                    callbackEntity.IDCompany = companyIntegration.ID;
                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                    callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                    callbackEntity.TransactionType = "YURT DIŞI KREDİ KARTI ÖDEMESİ ZAMAN AŞIMI";
                    _callbackResponseLogManager.Insert(callbackEntity);
                }
            }
        }
    }
}
