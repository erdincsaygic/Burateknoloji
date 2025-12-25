using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class CompanyCommissionManager : BaseBLL<CompanyCommission>, ICompanyCommissionManager
    {
        public CompanyCommissionManager(ICompanyCommissionDAL dal) : base(dal)
        {
        }
    }
}
