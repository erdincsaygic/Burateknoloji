using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System.Collections.Generic;

namespace StilPay.BLL.Abstract
{
    public interface ICompanyAutoNotificationSettingManager : IBaseBLL<CompanyAutoNotificationSetting>
    {
        CompanyAutoNotificationSetting GetSingleByIDCompany(List<FieldParameter> parameters);
    }
}
