using StilPay.Entities.Concrete;

namespace StilPay.DAL.Abstract
{
    public interface IMemberMoneyTransferRequestDAL : IBaseDAL<MemberMoneyTransferRequest>
    {
        string SetStatus(MemberMoneyTransferRequest entity);
    }
}
