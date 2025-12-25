using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class MemberProcessDAL : BaseDAL<MemberProcess>, IMemberProcessDAL
    {
        public override string TableName
        {
            get { return "MemberProcesses"; }
        }
    }
}
