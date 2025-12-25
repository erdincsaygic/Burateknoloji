using Coravel.Invocable;
using StilPay.BLL.Abstract;
using StilPay.DAL.Concrete;
using StilPay.Entities.Concrete;
using StilPay.Utility.EsnekPos.Models.EsnekPosGetTransactions;
using StilPay.Utility.EsnekPos;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using static StilPay.Utility.Helper.Enums;
using System.Threading.Tasks;
using System.Linq;
using StilPay.Utility.LidioPos.Models.LidioPosGetTransactions;
using StilPay.Utility.LidioPos;
using System.Text.Json;

namespace StilPay.BLL.Jobs.CreditCardPayPool
{
    public class LidioPos : IInvocable
    {
        public readonly IPaymentCreditCardPoolManager _paymentCreditCardPoolManager;
        public readonly IPaymentInstitutionManager _paymentInstitutionManager;
        private readonly SettingDAL _settingDAL = new SettingDAL();

        public LidioPos(IPaymentCreditCardPoolManager paymentCreditCardPoolManager, IPaymentInstitutionManager paymentInstitutionManager)
        {
            _paymentCreditCardPoolManager = paymentCreditCardPoolManager;
            _paymentInstitutionManager = paymentInstitutionManager;
        }

        public async Task Invoke()
        {
            var sendReq = _paymentInstitutionManager.GetList(null).FirstOrDefault(x => x.ID == ((byte)Enums.CreditCardPaymentMethodType.LidioPos).ToString()).IsActive;

            if (sendReq)
            {
                var lidioPosGetTransactionsRequestModel = new LidioPosGetTransactionRequestModel()
                {
                    startDate = DateTime.Now.AddHours(-3).ToString("yyyy-MM-dd HH:mm"),
                    endDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    instrumentInfo = new InstrumentInfo
                    {
                        Card = new GetTransactionCard
                        {
                            processType = "sales"
                        }
                    }
                };

                var lidioPosGetTransactionRequestResponseModel = LidioPosGetTransactions.GetTransactionRequest(lidioPosGetTransactionsRequestModel);

                if (lidioPosGetTransactionRequestResponseModel.Status == "OK" && lidioPosGetTransactionRequestResponseModel.Data != null && lidioPosGetTransactionRequestResponseModel.Data.TransactionList != null && lidioPosGetTransactionRequestResponseModel.Data.TransactionList.Count > 0)
                {
                    var filteredPayments = lidioPosGetTransactionRequestResponseModel.Data.TransactionList
                        .Where(payment => !payment.PaymentInfo.IsFullRefunded && !payment.PaymentInfo.IsCancelled && payment.ResultDetail != "SystemError");

                    foreach (var item in filteredPayments)
                    {
                        var formatStatus = item.IsSuccess ? (byte)Enums.StatusType.Confirmed : (byte)Enums.StatusType.Canceled;

                        var entity = _paymentCreditCardPoolManager.GetSingle(new List<FieldParameter>() { new FieldParameter("TransactionKey", FieldType.NVarChar, item.PaymentInfo.OrderId) });

                        if (entity != null && entity.Status != formatStatus)
                        {
                            _paymentCreditCardPoolManager.CheckStatusAndUpdate(item.PaymentInfo.OrderId, formatStatus);
                        }

                        if (!_paymentCreditCardPoolManager.CheckTransactionKey(item.PaymentInfo.OrderId))
                        {
                            var paymentCreditCardPool = new PaymentCreditCardPool
                            {
                                Amount = item.PaymentInfo.AmountRequested,
                                BankName = item.PaymentInfo.InstrumentDetail.Card.CardBankName,
                                Commission = formatStatus == (byte)Enums.StatusType.Confirmed ? item.PaymentInfo.PaybackTransactionList[0].BankTotalCommission : 0,
                                InstallmentCount = 0,
                                PaymentMethodID = (int)CreditCardPaymentMethodType.LidioPos,
                                PaymentMethodName = "LidioPos",
                                SenderName = item.PaymentInfo.InstrumentDetail.Card.CardHolderName,
                                TransactionDate = Convert.ToDateTime(item.PaymentInfo.TransactionDate),
                                TransactionType = "SATIS",
                                Status = formatStatus,
                                TransactionKey = item.PaymentInfo.OrderId,
                                TransactionID = item.PaymentInfo.OrderId,
                                Description = item.PaymentInfo.ResultCategory.RecommendedUIMessageTR,
                            };

                            var resp = _paymentCreditCardPoolManager.Insert(paymentCreditCardPool);
                        }
                    }
                }
            }
        }
    }
}
