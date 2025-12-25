using DocumentFormat.OpenXml.ExtendedProperties;
using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class PaymentCreditCardPoolManager : BaseBLL<PaymentCreditCardPool>, IPaymentCreditCardPoolManager
    {
        public PaymentCreditCardPoolManager(IPaymentCreditCardPoolDAL dal) : base(dal)
        {
        }

        public bool CheckTransactionKey(string transactionKey)
        {
            return ((IPaymentCreditCardPoolDAL)_dal).CheckTransactionKey(transactionKey);

        }

        public string CheckStatusAndUpdate(string transactionKey, byte status)
        {
            return ((IPaymentCreditCardPoolDAL)_dal).CheckStatusAndUpdate(transactionKey, status);

        }
    }
}
