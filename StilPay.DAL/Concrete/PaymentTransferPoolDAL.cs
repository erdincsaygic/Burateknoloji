using DocumentFormat.OpenXml.ExtendedProperties;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StilPay.DAL.Concrete
{
    public class PaymentTransferPoolDAL : BaseDAL<PaymentTransferPool>, IPaymentTransferPoolDAL
    {
        public override string TableName
        {
            get { return "PaymentTransferPools"; }
        }

        public BankLastActivity GetBankLastActivity(string idBank)
        {
            try
            {
                _connector = new tSQLConnector();

                List<FieldParameter> parameters = new List<FieldParameter>()
                {
                     new FieldParameter("IDBank", Enums.FieldType.NVarChar, idBank)
                };

                var bankLastActivity = new BankLastActivity
                {
                    TransactionDate = _connector.GetDatetime(TableName + "_GetBankLastActivity", parameters)
                };

                return bankLastActivity;
            }
            catch { }

            return new BankLastActivity();
        }


        public string SetStatus(string id, byte status)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id),
                    new FieldParameter("Status", Enums.FieldType.Tinyint, status)
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SetStatus", parameters);
                _connector.CommitOrRollBackTransaction(Enums.TransactionType.Commit);

                return IDMaster;
            }
            catch (Exception ex)
            {
                if (_connector.SqlConn != null)
                    _connector.CommitOrRollBackTransaction(Enums.TransactionType.RollBack);
                throw new Exception(ex.Message);
            }
        }
    }
}
