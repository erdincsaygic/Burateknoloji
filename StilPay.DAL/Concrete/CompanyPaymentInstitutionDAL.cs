using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class CompanyPaymentInstitutionDAL : BaseDAL<CompanyPaymentInstitution>, ICompanyPaymentInstitutionDAL
    {
        public override string TableName
        {
            get { return "CompanyPaymentInstitutions"; }
        }   
    }
}
