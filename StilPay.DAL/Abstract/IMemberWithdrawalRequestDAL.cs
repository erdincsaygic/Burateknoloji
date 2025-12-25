using StilPay.Entities.Concrete;

namespace StilPay.DAL.Abstract
{
    public interface IMemberWithdrawalRequestDAL : IBaseDAL<MemberWithdrawalRequest>
    {
        string SetStatus(MemberWithdrawalRequest entity);
    }
}
