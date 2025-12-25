using Coravel.Invocable;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Models;
using StilPay.Utility.Worker;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StilPay.BLL.Jobs
{
    public class AutoCallBack : IInvocable
    {
        public readonly ICallbackResponseLogManager _callbackResponseLogManager;
        public readonly ICompanyWithdrawalRequestManager _companyWithdrawalRequestManager;
        private readonly IPaymentNotificationManager _paymentNotificationManager;
        private readonly ICreditCardPaymentNotificationManager _creditCardPaymentNotificationManager;
        private readonly IForeignCreditCardPaymentNotificationManager _foreignCreditCardPaymentNotificationManager;
        private readonly ICompanyRebateRequestManager _companyRebateRequestManager;
        public readonly ICompanyManager _companyManager;
        public readonly ICompanyIntegrationManager _companyIntegrationManager;
        public AutoCallBack(ICallbackResponseLogManager callbackResponseLogManager,ICompanyRebateRequestManager companyRebateRequestManager, IPaymentNotificationManager paymentNotificationManager, ICreditCardPaymentNotificationManager creditCardPaymentNotificationManager, IForeignCreditCardPaymentNotificationManager foreignCreditCardPaymentNotificationManager, ICompanyManager companyManager, ICompanyIntegrationManager companyIntegrationManager, ICompanyWithdrawalRequestManager companyWithdrawalRequestManager) 
        {
            _callbackResponseLogManager = callbackResponseLogManager;
            _companyManager = companyManager;
            _companyIntegrationManager = companyIntegrationManager;
            _paymentNotificationManager = paymentNotificationManager; 
            _companyRebateRequestManager = companyRebateRequestManager;
            _creditCardPaymentNotificationManager = creditCardPaymentNotificationManager;
            _foreignCreditCardPaymentNotificationManager = foreignCreditCardPaymentNotificationManager;
            _companyWithdrawalRequestManager = companyWithdrawalRequestManager;
        }

        public async Task Invoke()
        {
            var transactions = _callbackResponseLogManager.AutoCallbackService();
            var callbackEntity = new CallbackResponseLog();
            var opt = new JsonSerializerOptions() { WriteIndented = true };

            foreach (var transaction in transactions)
            {
                switch (transaction.TransactionType)
                {
                    case (byte)Enums.CallbackTransacationType.Transfer:
                        {
                            var entity = _paymentNotificationManager.GetSingleByTransactionID(transaction.TransactionID);

                            var companyIntegration = _companyIntegrationManager.GetByServiceId(entity.ServiceID);

                            var cleanJson = Regex.Unescape(transaction.Callback);

                            var response = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.AutoCallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", cleanJson } });
                                                        
                            if (response != null && response.Result != null && response.Result.Status != null && !string.IsNullOrEmpty(response.Result.Status) && !string.IsNullOrWhiteSpace(response.Result.Status) && response.Result.Status == "OK")
                            {
                                callbackEntity.TransactionID = entity.TransactionID;
                                callbackEntity.ServiceType = "STILPAY AUTOCALLBACK";
                                callbackEntity.Callback = cleanJson;
                                callbackEntity.IDCompany = companyIntegration.ID;
                                callbackEntity.TransactionType = "STILPAY AUTOCALLBACK";
                                callbackEntity.ResponseStatus = 1;
                                _callbackResponseLogManager.Insert(callbackEntity);
                            }

                            break;
                        }


                    case (byte)Enums.CallbackTransacationType.CreditCard:
                        {
                            var entity = _creditCardPaymentNotificationManager.GetSingleByTransactionID(transaction.TransactionID);

                            var companyIntegration = _companyIntegrationManager.GetByServiceId(entity.ServiceID);

                            var cleanJson = Regex.Unescape(transaction.Callback);

                            var response = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.AutoCallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", cleanJson } });

                            if (response != null && response.Result != null && response.Result.Status != null && !string.IsNullOrEmpty(response.Result.Status) && !string.IsNullOrWhiteSpace(response.Result.Status) && response.Result.Status == "OK")
                            {
                                callbackEntity.TransactionID = entity.TransactionID;
                                callbackEntity.ServiceType = "STILPAY AUTOCALLBACK";
                                callbackEntity.Callback = cleanJson;
                                callbackEntity.IDCompany = companyIntegration.ID;
                                callbackEntity.TransactionType = "STILPAY AUTOCALLBACK";
                                callbackEntity.ResponseStatus = 1;
                                _callbackResponseLogManager.Insert(callbackEntity);
                            }

                            break;
                        }



                    case (byte)Enums.CallbackTransacationType.ForeignCreditCard:
                        {

                            var entity = _foreignCreditCardPaymentNotificationManager.GetSingleByTransactionID(transaction.TransactionID);

                            var companyIntegration = _companyIntegrationManager.GetByServiceId(entity.ServiceID);

                            var cleanJson = Regex.Unescape(transaction.Callback);

                            var response = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.AutoCallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", cleanJson } });

                            if (response != null && response.Result != null && response.Result.Status != null && !string.IsNullOrEmpty(response.Result.Status) && !string.IsNullOrWhiteSpace(response.Result.Status) && response.Result.Status == "OK")
                            {
                                callbackEntity.TransactionID = entity.TransactionID;
                                callbackEntity.ServiceType = "STILPAY AUTOCALLBACK";
                                callbackEntity.Callback = cleanJson;
                                callbackEntity.IDCompany = companyIntegration.ID;
                                callbackEntity.TransactionType = "STILPAY AUTOCALLBACK";
                                callbackEntity.ResponseStatus = 1;
                                _callbackResponseLogManager.Insert(callbackEntity);
                            }
                            break;

                        }

                    case (byte)Enums.CallbackTransacationType.Withdrawal:
                        {

                            var entity = _companyWithdrawalRequestManager.GetSingleByRequestNr(transaction.TransactionID);

                            var companyIntegration = _companyIntegrationManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, entity.IDCompany) });

                            var cleanJson = Regex.Unescape(transaction.Callback);

                            var response = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.AutoCallbackWithdrawalUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", cleanJson } });

                            if (response != null && response.Result != null && response.Result.Status != null && !string.IsNullOrEmpty(response.Result.Status) && !string.IsNullOrWhiteSpace(response.Result.Status) && response.Result.Status == "OK")
                            {
                                callbackEntity.TransactionID = entity.RequestNr;
                                callbackEntity.ServiceType = "STILPAY AUTOCALLBACK";
                                callbackEntity.Callback = cleanJson;
                                callbackEntity.IDCompany = companyIntegration.ID;
                                callbackEntity.TransactionType = "STILPAY AUTOCALLBACK";
                                callbackEntity.ResponseStatus = 1;
                                _callbackResponseLogManager.Insert(callbackEntity);
                            }
                            break;
                        }
                }
            }
        }
    }
}
