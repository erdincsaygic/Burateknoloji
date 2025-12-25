using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class AccountSummaryDAL : BaseDAL<AccountSummary>, IAccountSummaryDAL
    {
        public override string TableName
        {
            get { return "AccountSummaries"; }
        }
    }
}
