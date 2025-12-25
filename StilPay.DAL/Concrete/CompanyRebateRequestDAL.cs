using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System.Collections.Generic;
using System;
using System.Data;

namespace StilPay.DAL.Concrete
{
    public class CompanyRebateRequestDAL : BaseDAL<CompanyRebateRequest>, ICompanyRebateRequestDAL
    {
        public override string TableName
        {
            get { return "CompanyRebateRequests"; }
        }

        public string SetStatus(CompanyRebateRequest entity)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, entity.ID),
                    new FieldParameter("MUser", Enums.FieldType.NVarChar, entity.MUser),
                    new FieldParameter("MDate", Enums.FieldType.DateTime, entity.MDate),
                    new FieldParameter("Status", Enums.FieldType.Tinyint, entity.Status),
                    new FieldParameter("Description", Enums.FieldType.NVarChar, entity.Description),
                    new FieldParameter("IsBankQueryNr", Enums.FieldType.NVarChar, entity.IsBankQueryNr),
                    new FieldParameter("SIDBank", Enums.FieldType.NVarChar, entity.SIDBank),
                    new FieldParameter("CompanyBankAccountID", Enums.FieldType.NVarChar, entity.CompanyBankAccountID)
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

        public CompanyRebateRequest GetSingleByTransactionID(string transactionID)
        {

            try
            {
                var parameters = new List<FieldParameter> { new FieldParameter("TransactionID", Enums.FieldType.NVarChar, transactionID) };
                _connector = new tSQLConnector();
                DataRow dr = _connector.GetDataRow(TableName + "_GetSingleByTransactionID", parameters);
                return CreateAndGetObjectFromDataRow(dr);
            }
            catch { }

            return new CompanyRebateRequest();
        }

        public CompanyRebateRequest GetSingleByTransactionNr(string transactionNr)
        {

            try
            {
                var parameters = new List<FieldParameter> { new FieldParameter("TransactionNr", Enums.FieldType.NVarChar, transactionNr) };
                _connector = new tSQLConnector();
                DataRow dr = _connector.GetDataRow(TableName + "_GetSingleByTransactionNr", parameters);
                return CreateAndGetObjectFromDataRow(dr);
            }
            catch { }

            return new CompanyRebateRequest();
        }
    }
}
