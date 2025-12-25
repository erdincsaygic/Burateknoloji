using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class MemberTransactionDAL : BaseDAL<MemberTransaction>, IMemberTransactionDAL
    {
        public override string TableName
        {
            get { return "MemberTransactions"; }
        }
    }
}
