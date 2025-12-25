using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;

namespace StilPay.BLL.Abstract
{
    public interface ICompanyPaymentRequestManager : IBaseBLL<CompanyPaymentRequest>
    {
        GenericResponse SetStatus(CompanyPaymentRequest entity);
    }
}
