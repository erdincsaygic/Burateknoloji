using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;

namespace StilPay.BLL.Abstract
{
    public interface IPaymentTransferPoolDescriptionControlManager : IBaseBLL<PaymentTransferPoolDescriptionControl>
    {
        GenericResponse PaymentWillBlocked(string senderName, string phone, string cardNumber);
    }
}
