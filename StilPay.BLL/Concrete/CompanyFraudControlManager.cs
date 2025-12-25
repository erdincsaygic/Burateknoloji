using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class CompanyFraudControlManager : BaseBLL<CompanyFraudControl>, ICompanyFraudControlManager
    {
        public CompanyFraudControlManager(ICompanyFraudControlDAL dal) : base(dal)
        {
        }
    }
}
