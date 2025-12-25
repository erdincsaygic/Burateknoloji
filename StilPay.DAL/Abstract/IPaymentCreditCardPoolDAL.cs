using StilPay.Entities.Concrete;

namespace StilPay.DAL.Abstract
{
    public interface IPaymentCreditCardPoolDAL : IBaseDAL<PaymentCreditCardPool>
    {
        string CheckStatusAndUpdate(string transactionKey, byte status);
        bool CheckTransactionKey(string transactionKey);
    }
}
