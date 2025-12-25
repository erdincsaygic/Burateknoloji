using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class CompanyFinanceTransactionManager : BaseBLL<CompanyFinanceTransaction>, ICompanyFinanceTransactionManager
    {
        public CompanyFinanceTransactionManager(ICompanyFinanceTransactionDAL dal) : base(dal)
        {
        }

    }
}
