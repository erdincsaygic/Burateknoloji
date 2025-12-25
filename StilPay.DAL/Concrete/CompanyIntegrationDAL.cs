using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Data;

namespace StilPay.DAL.Concrete
{
    public class CompanyIntegrationDAL : BaseDAL<CompanyIntegration>, ICompanyIntegrationDAL
    {
        public override string TableName
        {
            get { return "CompanyIntegrations"; }
        }

        public CompanyIntegration GetByServiceId(string serviceId)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ServiceId", Enums.FieldType.NVarChar, serviceId)
                };

                _connector = new tSQLConnector();
                DataRow dr = _connector.GetDataRow(TableName + "_GetByServiceId", parameters);

                return CreateAndGetObjectFromDataRow(dr);
            }
            catch { }

            return null;
        }

        public string SetIframeUseSettings(string idCompany, bool transferBeUsed, bool creditCardBeUsed, bool foreignCreditCardBeUsed, bool withdrawalApiBeUsed)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, idCompany),
                    new FieldParameter("TransferBeUsed", Enums.FieldType.Bit, transferBeUsed),
                    new FieldParameter("CreditCardBeUsed", Enums.FieldType.Bit, creditCardBeUsed),
                    new FieldParameter("ForeignCreditCardBeUsed", Enums.FieldType.Bit, foreignCreditCardBeUsed),
                    new FieldParameter("WithdrawalApiBeUsed", Enums.FieldType.Bit, withdrawalApiBeUsed),
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SetIframeUseSettings", parameters);
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

        public string SetCreditCardPaymentMethod(string idCompany, bool creditCardPaymentWithParam, bool creditCardPaymentWithPayNKolay, bool foreignCreditCardPaymentWithPayNKolay)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, idCompany),
                    new FieldParameter("CreditCardPaymentWithParam", Enums.FieldType.Bit, creditCardPaymentWithParam),
                    new FieldParameter("CreditCardPaymentWithPayNKolay", Enums.FieldType.Bit, creditCardPaymentWithPayNKolay),
                    new FieldParameter("ForeignCreditCardPaymentWithPayNKolay", Enums.FieldType.Bit, foreignCreditCardPaymentWithPayNKolay),
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SetCreditCardPaymentMethod", parameters);
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
