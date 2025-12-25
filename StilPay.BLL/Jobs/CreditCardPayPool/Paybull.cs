using Coravel.Invocable;
using StilPay.BLL.Abstract;
using StilPay.DAL.Concrete;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.PayNKolay.Models.PaymentList;
using StilPay.Utility.PayNKolay;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using static StilPay.Utility.Helper.Enums;
using System.Threading.Tasks;
using System.Linq;
using StilPay.Utility.Paybull.PaybullGetTransactions;
using StilPay.Utility.Worker;
using StilPay.Utility.Paybull;
using StilPay.BLL.Concrete;

namespace StilPay.BLL.Jobs.CreditCardPayPool
{
    public class Paybull : IInvocable
    {
        public readonly IPaymentCreditCardPoolManager _paymentCreditCardPoolManager;
        public readonly ICompanyManager _companyManager;
        public readonly ICompanyIntegrationManager _companyIntegrationManager;
        public readonly ICallbackResponseLogManager _callbackResponseLogManager;
        public readonly ISystemSettingManager _systemSettingManager;
        public readonly IPaymentInstitutionManager _paymentInstitutionManager;
        private readonly SettingDAL _settingDAL = new SettingDAL();

        public Paybull(IPaymentCreditCardPoolManager paymentCreditCardPoolManager, ICompanyManager companyManager, ISystemSettingManager systemSettingManager, ICompanyIntegrationManager companyIntegrationManager, ICallbackResponseLogManager callbackResponseLogManager, IPaymentInstitutionManager paymentInstitutionManager)
        {
            _paymentCreditCardPoolManager = paymentCreditCardPoolManager;
            _companyManager = companyManager;
            _companyIntegrationManager = companyIntegrationManager;
            _callbackResponseLogManager = callbackResponseLogManager;
            _systemSettingManager = systemSettingManager;
            _paymentInstitutionManager = paymentInstitutionManager;
        }

        public async Task Invoke()
        {
            var sendReq = _paymentInstitutionManager.GetList(null).FirstOrDefault(x => x.ID == ((byte)Enums.CreditCardPaymentMethodType.Paybull).ToString()).IsActive;

            if (sendReq)
            {
                string previousDay = null;
                DateTime dateNow = DateTime.Now;
                TimeSpan timeLimit = new TimeSpan(00, 5, 0);

                if (dateNow.TimeOfDay < timeLimit)
                    previousDay = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
                else
                    previousDay = null;

                var paybullIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "PaybullCreditCard") });

                if(previousDay != null)
                {
                    var paybullPreviousDayGetTransactionRequestModel = new PaybullGetTransactionRequestModel()
                    {
                        date = previousDay,
                        merchant_key = paybullIntegrationValues.FirstOrDefault(f => f.ParamDef == "secret_key").ParamVal
                    };

                    paybullPreviousDayGetTransactionRequestModel.hash_key = PaybullGenerateTransactionHashKey.GenerateTransactionHashKey(paybullPreviousDayGetTransactionRequestModel.merchant_key, paybullPreviousDayGetTransactionRequestModel.date, "", "", "", "", "", "");

                    var responsePreviousDay = PaybullGetTransaction.GetTransaction(paybullPreviousDayGetTransactionRequestModel);

                    if (responsePreviousDay.Status == "OK" && responsePreviousDay.Data != null && responsePreviousDay.Data.data != null)
                    {
                        foreach (var item in responsePreviousDay.Data.data.transactions.Take(100))
                        {
                            var formatStatus = item.transaction_state_label == "Completed" ? (byte)Enums.StatusType.Confirmed : item.transaction_state_label == "Failed" ? (byte)Enums.StatusType.Canceled : item.transaction_state_label == "Pending" ? (byte)Enums.StatusType.Pending : item.transaction_state_label == "Refunded" && item.result.Contains("Approved") ? (byte)Enums.StatusType.Confirmed : (byte)Enums.StatusType.Canceled;

                            var entity = _paymentCreditCardPoolManager.GetSingle(new List<FieldParameter>() { new FieldParameter("TransactionKey", FieldType.NVarChar, item.order_id) });

                            if (entity != null && entity.Status != formatStatus)
                            {
                                _paymentCreditCardPoolManager.CheckStatusAndUpdate(item.order_id, formatStatus);
                            }

                            if (!_paymentCreditCardPoolManager.CheckTransactionKey(item.order_id))
                            {
                                var paymentCreditCardPool = new PaymentCreditCardPool
                                {
                                    Amount = item.gross,
                                    BankName = item.card_holder_bank,
                                    Commission = item.merchant_commission,
                                    InstallmentCount = item.installment,
                                    PaymentMethodID = (int)CreditCardPaymentMethodType.Paybull,
                                    PaymentMethodName = "Paybull",
                                    SenderName = item.sale_billing != null ? item.sale_billing.card_holder_name : null,
                                    TransactionDate = item.created_at.AddHours(3),
                                    TransactionType = item.total_refunded_amount == 0 ? "SATIS" : "IADE",
                                    Status = formatStatus,
                                    TransactionKey = item.order_id,
                                    TransactionID = item.invoice_id,
                                    Description = item.result
                                };

                                _paymentCreditCardPoolManager.Insert(paymentCreditCardPool);
                            }
                        }
                    }
                }

                var paybullGetTransactionRequestModel = new PaybullGetTransactionRequestModel()
                {
                    date = DateTime.Now.ToString("yyyy-MM-dd"),
                    merchant_key = paybullIntegrationValues.FirstOrDefault(f => f.ParamDef == "secret_key").ParamVal
                };

                paybullGetTransactionRequestModel.hash_key = PaybullGenerateTransactionHashKey.GenerateTransactionHashKey(paybullGetTransactionRequestModel.merchant_key, paybullGetTransactionRequestModel.date, "", "", "", "", "", "");

                var response = PaybullGetTransaction.GetTransaction(paybullGetTransactionRequestModel);

                if (response.Status == "OK" && response.Data != null && response.Data.data != null)
                {
                    foreach (var item in response.Data.data.transactions)
                    {
                        var formatStatus = item.transaction_state_label == "Completed" ? (byte)Enums.StatusType.Confirmed : item.transaction_state_label == "Failed" ? (byte)Enums.StatusType.Canceled : item.transaction_state_label == "Pending" ? (byte)Enums.StatusType.Pending : item.transaction_state_label == "Refunded" && item.result.Contains("Approved") ? (byte)Enums.StatusType.Confirmed : (byte)Enums.StatusType.Canceled;

                        var entity = _paymentCreditCardPoolManager.GetSingle(new List<FieldParameter>() { new FieldParameter("TransactionKey", FieldType.NVarChar, item.order_id) });

                        if(entity != null && entity.Status != formatStatus)
                        {
                            _paymentCreditCardPoolManager.CheckStatusAndUpdate(item.order_id, formatStatus);
                        }

                        if (!_paymentCreditCardPoolManager.CheckTransactionKey(item.order_id))
                        {
                            var paymentCreditCardPool = new PaymentCreditCardPool
                            {
                                Amount = item.gross,
                                BankName = item.card_holder_bank,
                                Commission = item.merchant_commission,
                                InstallmentCount = item.installment,
                                PaymentMethodID = (int)CreditCardPaymentMethodType.Paybull,
                                PaymentMethodName = "Paybull",
                                SenderName = item.sale_billing != null ? item.sale_billing.card_holder_name : null,
                                TransactionDate = item.created_at.AddHours(3),
                                TransactionType = item.total_refunded_amount == 0 ? "SATIS" : "IADE",
                                Status = formatStatus,
                                TransactionKey = item.order_id,
                                TransactionID = item.invoice_id,
                                Description = item.result
                            };

                            _paymentCreditCardPoolManager.Insert(paymentCreditCardPool);
                        }
                    }
                }
            }
        }
    }
}
