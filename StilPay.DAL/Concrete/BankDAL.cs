using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System.Collections.Generic;
using System.Data;

namespace StilPay.DAL.Concrete
{
    public class BankDAL : BaseDAL<Bank>, IBankDAL
    {
        public override string TableName
        {
            get { return "Banks"; }
        }

        public List<Bank> GetBanksForIframeSetting()
        {
            try
            {
                _connector = new tSQLConnector();

                DataTable dt = _connector.GetDataTable(TableName + "_GetBanksForIframeSetting", null);
                return CreateAndGetObjectFromDataTable(dt);
            }
            catch { }

            return new List<Bank>();
        }
    }
}
