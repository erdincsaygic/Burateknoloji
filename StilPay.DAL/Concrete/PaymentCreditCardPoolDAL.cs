using DocumentFormat.OpenXml.Wordprocessing;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace StilPay.DAL.Concrete
{
    public class PaymentCreditCardPoolDAL : BaseDAL<PaymentCreditCardPool>, IPaymentCreditCardPoolDAL
    {
        public override string TableName
        {
            get { return "PaymentCreditCardPools"; }
        }



        public bool CheckTransactionKey(string transactionKey)
        {

            try
            {
                var parameters = new List<FieldParameter> { new FieldParameter("TransactionKey", Enums.FieldType.NVarChar, transactionKey) };
                _connector = new tSQLConnector();
                _connector.BeginTransaction();

                var response = _connector.GetBoolean(TableName + "_CheckTransactionKey", parameters);

                if (response == null) return true;
                else return (bool)response;
                
            }
            catch { }

            return true;
        }


        public string CheckStatusAndUpdate(string transactionKey, byte status)
        {
            try
            {
                var parameters = new List<FieldParameter> { new FieldParameter("TransactionKey", Enums.FieldType.NVarChar, transactionKey), new FieldParameter("Status", Enums.FieldType.Tinyint, status) };
                _connector = new tSQLConnector();
                _connector.BeginTransaction();

                var response = _connector.RunSqlCommand(TableName + "_CheckStatusAndUpdate", parameters);
                _connector.CommitOrRollBackTransaction(Enums.TransactionType.Commit);

                return response;

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
