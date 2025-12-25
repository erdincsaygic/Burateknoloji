using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System.Collections.Generic;
using System;
using System.Data;
using StilPay.Entities.Dto;

namespace StilPay.DAL.Concrete
{
    public class CompanyWithdrawalRequestDAL : BaseDAL<CompanyWithdrawalRequest>, ICompanyWithdrawalRequestDAL
    {
        public override string TableName
        {
            get { return "CompanyWithdrawalRequests"; }
        }

        public string SetStatus(CompanyWithdrawalRequest entity)
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
                    new FieldParameter("IsProcess", Enums.FieldType.Bit, entity.IsProcess),
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

        public CompanyWithdrawalRequest GetSingleByRequestNr(string requestNr)
        {

            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("RequestNr", Enums.FieldType.NVarChar, requestNr)
                };
                _connector = new tSQLConnector();
                DataRow dr = _connector.GetDataRow(TableName + "_GetSingleByRequestNr", parameters);
                return CreateAndGetObjectFromDataRow(dr);
            }
            catch { }

            return new CompanyWithdrawalRequest();
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
    }
}
