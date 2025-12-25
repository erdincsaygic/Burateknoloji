using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;

namespace StilPay.BLL.Abstract
{
    public interface IMemberPaymentRequestManager : IBaseBLL<MemberPaymentRequest>
    {
        GenericResponse SetStatus(MemberPaymentRequest entity);
    }
}
