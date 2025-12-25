using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class CompanyBankDAL : BaseDAL<CompanyBank>, ICompanyBankDAL
    {
        public override string TableName
        {
            get { return "CompanyBanks"; }
        }
    }
}
