using Coravel.Invocable;
using StilPay.BLL.Abstract;
using StilPay.DAL.Concrete;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Paybull.PaybullGetTransactions;
using StilPay.Utility.Paybull;
using System;
using System.Collections.Generic;
using System.Text;
using static StilPay.Utility.Helper.Enums;
using System.Threading.Tasks;
using System.Linq;
using StilPay.Utility.AKODESanalPOS.Models.AKODEGetTransactions;
using StilPay.Utility.AKODESanalPOS.Models.AKODECreditCardInfo;
using StilPay.Utility.AKODESanalPOS;
using StilPay.Utility.AKODESanalPOS.Models.AKODETransactionQuery;
using StilPay.Utility.EsnekPos.Models.EsnekPosGetTransactions;
using StilPay.Utility.EsnekPos;

namespace StilPay.BLL.Jobs.CreditCardPayPool
{
    public class EsnekPos : IInvocable
    {
        public readonly IPaymentCreditCardPoolManager _paymentCreditCardPoolManager;
        public readonly IPaymentInstitutionManager _paymentInstitutionManager;
        private readonly SettingDAL _settingDAL = new SettingDAL();

        public EsnekPos(IPaymentCreditCardPoolManager paymentCreditCardPoolManager, IPaymentInstitutionManager paymentInstitutionManager)
        {
            _paymentCreditCardPoolManager = paymentCreditCardPoolManager;
            _paymentInstitutionManager = paymentInstitutionManager;
        }

        public async Task Invoke()
        {
            var sendReq = _paymentInstitutionManager.GetList(null).FirstOrDefault(x => x.ID == ((byte)Enums.CreditCardPaymentMethodType.EsnekPos).ToString()).IsActive;

            if (sendReq)
            {
                string previousDay = null;
                DateTime dateNow = DateTime.Now;
                TimeSpan timeLimit = new TimeSpan(00, 5, 0);

                if (dateNow.TimeOfDay < timeLimit)
                    previousDay = dateNow.AddDays(-1).ToString("dd-MM-yy");
                else
                    previousDay = null;

                var esnekPosIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "EsnekPos") });

                if (previousDay != null)
                {
                    var esnekPosGetTransactionsRequestModel = new EsnekPosGetTransactionsRequestModel()
                    {
                        START_DATE = previousDay,
                        END_DATE = dateNow.ToString("dd-MM-yy"),
                    };

                    var responsePrevious = EsnekPosGetTransactions.GetTransactions(esnekPosGetTransactionsRequestModel);

                    if (responsePrevious.Status == "OK" && responsePrevious.Data != null && responsePrevious.Data.PaymentList.Count > 0)
                    {
                        var filteredPayments = responsePrevious.Data.PaymentList
                            .Where(payment =>
                                (payment.STATUS_ID == 3 || payment.STATUS_ID == 4) &&
                                payment.TRANSACTIONS.Any(transaction =>
                                    (transaction.STATUS_ID == 3 || transaction.STATUS_ID == 4) &&
                                    Convert.ToDateTime(transaction.DATE) >= DateTime.Now.AddHours(-1)));

                        foreach (var item in filteredPayments)
                        {
                            var formatStatus = item.STATUS_ID == 3 ? (byte)Enums.StatusType.Confirmed : (byte)Enums.StatusType.Canceled;

                            var entity = _paymentCreditCardPoolManager.GetSingle(new List<FieldParameter>() { new FieldParameter("TransactionKey", FieldType.NVarChar, item.DEALER_PAYMENT_REF_CODE) });

                            if (entity != null && entity.Status != formatStatus)
                            {
                                _paymentCreditCardPoolManager.CheckStatusAndUpdate(item.DEALER_PAYMENT_REF_CODE, formatStatus);
                            }

                            if (!_paymentCreditCardPoolManager.CheckTransactionKey(item.DEALER_PAYMENT_REF_CODE))
                            {
                                var paymentCreditCardPool = new PaymentCreditCardPool
                                {
                                    Amount = item.AMOUNT,
                                    BankName = item.CARD_BANK_NAME,
                                    Commission = item.COMMISSION_AMOUNT,
                                    InstallmentCount = item.INSTALLMENT,
                                    PaymentMethodID = (int)CreditCardPaymentMethodType.EsnekPos,
                                    PaymentMethodName = "EsnekPos",
                                    SenderName = item.CARD_NAME,
                                    TransactionDate = Convert.ToDateTime(item.TRANSACTIONS[0].DATE),
                                    TransactionType = "SATIS",
                                    Status = formatStatus,
                                    TransactionKey = item.DEALER_PAYMENT_REF_CODE,
                                    TransactionID = item.DEALER_PAYMENT_REF_CODE,
                                    Description = item.STATUS_NAME
                                };

                                var resp = _paymentCreditCardPoolManager.Insert(paymentCreditCardPool);
                            }
                        }
                    }
                }

                else
                {
                    var esnekPosGetTransactionsRequestModel = new EsnekPosGetTransactionsRequestModel()
                    {
                        START_DATE = dateNow.ToString("dd-MM-yy"),
                        END_DATE = dateNow.ToString("dd-MM-yy"),
                    };

                    var esnekPosGetTransactionsRequestResponseModel = EsnekPosGetTransactions.GetTransactions(esnekPosGetTransactionsRequestModel);

                    if (esnekPosGetTransactionsRequestResponseModel.Status == "OK" && esnekPosGetTransactionsRequestResponseModel.Data != null && esnekPosGetTransactionsRequestResponseModel.Data.PaymentList.Count > 0 )
                    {
                        var filteredPayments = esnekPosGetTransactionsRequestResponseModel.Data.PaymentList
                            .Where(payment =>
                                (payment.STATUS_ID == 3 || payment.STATUS_ID == 4));

                        foreach (var item in filteredPayments)
                        {
                            var formatStatus = item.STATUS_ID == 3 ? (byte)Enums.StatusType.Confirmed : (byte)Enums.StatusType.Canceled;

                            var entity = _paymentCreditCardPoolManager.GetSingle(new List<FieldParameter>() { new FieldParameter("TransactionKey", FieldType.NVarChar, item.DEALER_PAYMENT_REF_CODE) });

                            if (entity != null && entity.Status != formatStatus)
                            {
                                _paymentCreditCardPoolManager.CheckStatusAndUpdate(item.DEALER_PAYMENT_REF_CODE, formatStatus);
                            }

                            if (!_paymentCreditCardPoolManager.CheckTransactionKey(item.DEALER_PAYMENT_REF_CODE))
                            {
                                var paymentCreditCardPool = new PaymentCreditCardPool
                                {
                                    Amount = item.AMOUNT,
                                    BankName = item.CARD_BANK_NAME,
                                    Commission = item.COMMISSION_AMOUNT,
                                    InstallmentCount = item.INSTALLMENT,
                                    PaymentMethodID = (int)CreditCardPaymentMethodType.EsnekPos,
                                    PaymentMethodName = "EsnekPos",
                                    SenderName = item.CARD_NAME,
                                    TransactionDate = Convert.ToDateTime(item.TRANSACTIONS[0].DATE),
                                    TransactionType = "SATIS",
                                    Status = formatStatus,
                                    TransactionKey = item.DEALER_PAYMENT_REF_CODE,
                                    TransactionID = item.DEALER_PAYMENT_REF_CODE,
                                    Description = item.STATUS_NAME
                                };

                                var resp = _paymentCreditCardPoolManager.Insert(paymentCreditCardPool);
                            }
                        }
                    }
                }
            }
        }
    }
}
