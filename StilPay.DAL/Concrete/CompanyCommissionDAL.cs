using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class CompanyCommissionDAL : BaseDAL<CompanyCommission>, ICompanyCommissionDAL
    {
        public override string TableName
        {
            get { return "CompanyCommissions"; }
        }

    }
}
