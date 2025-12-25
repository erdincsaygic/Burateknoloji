using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class MemberTypeManager : BaseBLL<MemberType>, IMemberTypeManager
    {
        public MemberTypeManager(IMemberTypeDAL dal) : base(dal)
        {
        }
    }
}
