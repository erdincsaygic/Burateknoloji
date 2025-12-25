using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class MemberTransactionManager : BaseBLL<MemberTransaction>, IMemberTransactionManager
    {
        public MemberTransactionManager(IMemberTransactionDAL dal) : base(dal)
        {
        }
    }
}
