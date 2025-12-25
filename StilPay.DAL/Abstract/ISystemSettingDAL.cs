using StilPay.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.DAL.Abstract
{
    public interface ISystemSettingDAL : IBaseDAL<SystemSetting>
    {
        string SetIframeUseSettings(string idCompany, bool defaultTransferBeUsed, bool defaultCreditCardBeUsed);
        string SetCreditCardPaymentMethod(string idCompany, bool defaultCreditCardPaymentWithParam, bool defaultCreditCardPaymentWithPayNKolay);
    }
}
