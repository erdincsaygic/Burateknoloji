using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System.Collections.Generic;

namespace StilPay.DAL.Abstract
{
    public interface ICompanyAutoNotificationSettingDAL : IBaseDAL<CompanyAutoNotificationSetting>
    {
        CompanyAutoNotificationSetting GetSingleByIDCompany(List<FieldParameter> parameters);
    }
}
