using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;

namespace StilPay.BLL.Abstract
{
    public interface ICompanyIntegrationManager : IBaseBLL<CompanyIntegration>
    {
        CompanyIntegration GetByServiceId(string serviceId);
        GenericResponse SetIframeUseSettings(string idCompany, bool transferBeUsed, bool creditCardBeUsed, bool foreignCreditCardBeUsed, bool withdrawalApiBeUsed);
        GenericResponse SetCreditCardPaymentMethod(string idCompany, bool creditCardPaymentWithParam, bool creditCardPaymentWithPayNKolay, bool foreignCreditCardPaymentWithPayNKolay);
    }
}
