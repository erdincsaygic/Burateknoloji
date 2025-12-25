using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System.Collections.Generic;
using System;
using StilPay.Utility.Models;
using System.Data;

namespace StilPay.DAL.Concrete
{
    public class CompanyAutoNotificationSettingDAL : BaseDAL<CompanyAutoNotificationSetting>, ICompanyAutoNotificationSettingDAL
    {
        public override string TableName
        {
            get { return "CompanyAutoNotificationSettings"; }
        }


        public CompanyAutoNotificationSetting GetSingleByIDCompany(List<FieldParameter> parameters)
        {
            try
            {
                _connector = new tSQLConnector();
                DataRow dr = _connector.GetDataRow(TableName + "_GetSingleByIDCompany", parameters);
                return CreateAndGetObjectFromDataRow(dr);
            }
            catch { }

            return new CompanyAutoNotificationSetting();

        }
    }
}
