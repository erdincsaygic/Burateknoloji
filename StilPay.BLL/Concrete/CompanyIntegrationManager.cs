using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;

namespace StilPay.BLL.Concrete
{
    public class CompanyIntegrationManager : BaseBLL<CompanyIntegration>, ICompanyIntegrationManager
    {
        public CompanyIntegrationManager(ICompanyIntegrationDAL dal) : base(dal)
        {
        }

        public CompanyIntegration GetByServiceId(string serviceId)
        {
            return ((ICompanyIntegrationDAL)_dal).GetByServiceId(serviceId);
        }
        public GenericResponse SetIframeUseSettings(string idCompany, bool transferBeUsed, bool creditCardBeUsed, bool foreignCreditCardBeUsed, bool withdrawalApiBeUsed)
        {
            try
            {
                var response = ((ICompanyIntegrationDAL)_dal).SetIframeUseSettings(idCompany, transferBeUsed, creditCardBeUsed, foreignCreditCardBeUsed, withdrawalApiBeUsed);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse
                {
                    Status = "ERROR",
                    Message = ex.Message
                };
            }
        }
        public GenericResponse SetCreditCardPaymentMethod(string idCompany, bool creditCardPaymentWithParam, bool creditCardPaymentWithPayNKolay, bool foreignCreditCardPaymentWithPayNKolay)
        {
            try
            {
                var response = ((ICompanyIntegrationDAL)_dal).SetCreditCardPaymentMethod(idCompany, creditCardPaymentWithParam, creditCardPaymentWithPayNKolay, foreignCreditCardPaymentWithPayNKolay);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse
                {
                    Status = "ERROR",
                    Message = ex.Message
                };
            }
        }
    }
}
