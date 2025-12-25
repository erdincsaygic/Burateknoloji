using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class MemberTypeDAL : BaseDAL<MemberType>, IMemberTypeDAL
    {
        public override string TableName
        {
            get { return "MemberTypes"; }
        }
    }
}
