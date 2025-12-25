using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;

namespace StilPay.DAL.Abstract
{
    public interface IPaymentTransferPoolDescriptionControlDAL : IBaseDAL<PaymentTransferPoolDescriptionControl>
    {
        bool PaymentWillBlocked(string senderName, string phone, string cardNumber);

    }
}
