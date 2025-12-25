using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;


namespace StilPay.BLL.Concrete
{
    public class CompanyPaymentInstitutionManager : BaseBLL<CompanyPaymentInstitution>, ICompanyPaymentInstitutionManager
    {
        public CompanyPaymentInstitutionManager(ICompanyPaymentInstitutionDAL dal) : base(dal)
        {
        }
   
    }
}
