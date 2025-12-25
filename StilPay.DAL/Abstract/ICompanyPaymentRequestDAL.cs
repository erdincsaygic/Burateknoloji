using StilPay.Entities.Concrete;

namespace StilPay.DAL.Abstract
{
    public interface ICompanyPaymentRequestDAL : IBaseDAL<CompanyPaymentRequest>
    {
        string SetStatus(CompanyPaymentRequest entity);
    }
}
