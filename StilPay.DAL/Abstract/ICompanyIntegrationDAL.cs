using DocumentFormat.OpenXml.Spreadsheet;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Abstract
{
    public interface ICompanyIntegrationDAL : IBaseDAL<CompanyIntegration>
    {
        CompanyIntegration GetByServiceId(string serviceId);
        string SetIframeUseSettings(string idCompany, bool transferBeUsed, bool creditCardBeUsed, bool foreignCreditCardBeUsed, bool withdrawalApiBeUsed);
        string SetCreditCardPaymentMethod(string idCompany, bool creditCardPaymentWithParam, bool creditCardPaymentWithPayNKolay, bool foreignCreditCardPaymentWithPayNKolay);
    }
}
