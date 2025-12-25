using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class CompanyFraudControlDAL : BaseDAL<CompanyFraudControl>, ICompanyFraudControlDAL
    {
        public override string TableName
        {
            get { return "CompanyFraudControls"; }
        }
    }
}
