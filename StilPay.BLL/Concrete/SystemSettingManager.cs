using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;

namespace StilPay.BLL.Concrete
{
    public class SystemSettingManager : BaseBLL<SystemSetting>, ISystemSettingManager
    {
        public SystemSettingManager(ISystemSettingDAL dal) : base(dal)
        {
        }

        public GenericResponse SetIframeUseSettings(string idCompany, bool defaultTransferBeUsed, bool defaultCreditCardBeUsed)
        {
            try
            {
                var response = ((ISystemSettingDAL)_dal).SetIframeUseSettings(idCompany, defaultTransferBeUsed, defaultCreditCardBeUsed);

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
        public GenericResponse SetCreditCardPaymentMethod(string idCompany, bool defaultCreditCardPaymentWithParam, bool defaultCreditCardPaymentWithPayNKolay)
        {
            try
            {
                var response = ((ISystemSettingDAL)_dal).SetCreditCardPaymentMethod(idCompany, defaultCreditCardPaymentWithParam, defaultCreditCardPaymentWithPayNKolay);

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
