using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;

namespace StilPay.BLL.Abstract
{
    public interface IMemberMoneyTransferRequestManager : IBaseBLL<MemberMoneyTransferRequest>
    {
        GenericResponse SetStatus(MemberMoneyTransferRequest entity);
    }
}
