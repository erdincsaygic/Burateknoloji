using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;

namespace StilPay.BLL.Abstract
{
    public interface ISystemSettingManager : IBaseBLL<SystemSetting>
    {
        GenericResponse SetIframeUseSettings(string idCompany, bool defaultTransferBeUsed, bool defaultCreditCardBeUsed);
        GenericResponse SetCreditCardPaymentMethod(string idCompany, bool defaultCreditCardPaymentWithParam, bool defaultCreditCardPaymentWithPayNKolay);
    }
}
