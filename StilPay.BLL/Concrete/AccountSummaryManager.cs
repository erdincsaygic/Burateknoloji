using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class AccountSummaryManager : BaseBLL<AccountSummary>, IAccountSummaryManager
    {
        public AccountSummaryManager(IAccountSummaryDAL dal) : base(dal)
        {
        }
    }
}
