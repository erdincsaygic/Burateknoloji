using StilPay.Entities.Concrete;

namespace StilPay.BLL.Abstract
{
    public interface IPaymentCreditCardPoolManager : IBaseBLL<PaymentCreditCardPool>
    {
        bool CheckTransactionKey(string transactionKey);
        string CheckStatusAndUpdate(string transactionKey, byte status);
    }
}
