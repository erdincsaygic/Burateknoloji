using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Text;

namespace StilPay.DAL.Concrete
{
    public class CallbackResponseLogDAL : BaseDAL<CallbackResponseLog>, ICallbackResponseLogDAL
    {
        public override string TableName
        {
            get { return "CallbackResponseLogs"; }
        }

        public List<AutoCallbackService> AutoCallbackService()
        {
            try
            {
                _connector = new tSQLConnector();
                DataTable dt = _connector.GetDataTable("AutoCallbackService", null);

                List<AutoCallbackService> list = new List<AutoCallbackService>();

                for (int i = 0; i < dt.Rows.Count; i++)
                {                   
                    list.Add(new AutoCallbackService
                    {
                        TransactionID = dt.Rows[i]["TransactionID"].ToString(),
                        TransactionType = int.Parse(dt.Rows[i]["TransactionType"].ToString()),
                        CDate = Convert.ToDateTime(dt.Rows[i]["CDate"].ToString()),
                        ResponseStatus = int.Parse(dt.Rows[i]["ResponseStatus"].ToString()),
                        Callback = dt.Rows[i]["Callback"].ToString()
                    });
                }

                return list;
            }
            catch { }

            return new List<AutoCallbackService>();
        }

    }
}
