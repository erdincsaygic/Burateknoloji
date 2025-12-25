using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;
using System.Collections.Generic;

namespace StilPay.BLL.Concrete
{
    public class CompanyAutoNotificationSettingManager : BaseBLL<CompanyAutoNotificationSetting>, ICompanyAutoNotificationSettingManager
    {
        public CompanyAutoNotificationSettingManager(ICompanyAutoNotificationSettingDAL dal) : base(dal)
        {
        }

        public CompanyAutoNotificationSetting GetSingleByIDCompany(List<FieldParameter> parameters)
        {
            return ((ICompanyAutoNotificationSettingDAL)_dal).GetSingleByIDCompany(parameters);
        }
    }
}
