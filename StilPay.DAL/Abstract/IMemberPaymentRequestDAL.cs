using StilPay.Entities.Concrete;

namespace StilPay.DAL.Abstract
{
    public interface IMemberPaymentRequestDAL : IBaseDAL<MemberPaymentRequest>
    {
        string SetStatus(MemberPaymentRequest entity);
    }
}
