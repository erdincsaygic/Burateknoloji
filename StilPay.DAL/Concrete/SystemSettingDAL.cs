using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.DAL.Concrete
{
    public class SystemSettingDAL : BaseDAL<SystemSetting>, ISystemSettingDAL
    {
        public override string TableName
        {
            get { return "SystemSettings"; }
        }

        public string SetIframeUseSettings(string idCompany, bool defaultTransferBeUsed, bool defaultCreditCardBeUsed)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, idCompany),
                    new FieldParameter("DefaultTransferBeUsed", Enums.FieldType.Bit, defaultTransferBeUsed),
                    new FieldParameter("DefaultCreditCardBeUsed", Enums.FieldType.Bit, defaultCreditCardBeUsed),
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

        public override string Update(SystemSetting entity)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                new FieldParameter("DefaultCreditCardPaymentWithParam", Enums.FieldType.Bit, entity.DefaultCreditCardPaymentWithParam),
                new FieldParameter("DefaultCreditCardPaymentWithPayNKolay", Enums.FieldType.Bit, entity.DefaultCreditCardPaymentWithPayNKolay),
                new FieldParameter("DefaultTransferBeUsed", Enums.FieldType.Bit, entity.DefaultTransferBeUsed),
                new FieldParameter("DefaultCreditCardBeUsed", Enums.FieldType.Bit, entity.DefaultCreditCardBeUsed),
                new FieldParameter("DefaultMoneyTransferWithZiraatBank", Enums.FieldType.Bit, entity.DefaultMoneyTransferWithZiraatBank),
                new FieldParameter("DefaultMoneyTransferWithIsBank", Enums.FieldType.Bit, entity.DefaultMoneyTransferWithIsBank),
                new FieldParameter("DefaultAcceptPaymentWithZiraatBank", Enums.FieldType.Bit, entity.DefaultAcceptPaymentWithZiraatBank),
                new FieldParameter("DefaultAcceptPaymentWithIsBank", Enums.FieldType.Bit, entity.DefaultAcceptPaymentWithIsBank),
                new FieldParameter("DefaultForeignCreditCardBeUsed", Enums.FieldType.Bit, entity.DefaultForeignCreditCardBeUsed),
                new FieldParameter("DefaultForeignCreditCardPaymentWithPayNKolay", Enums.FieldType.Bit, entity.DefaultForeignCreditCardPaymentWithPayNKolay)
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(spUpdate, parameters);
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

        public string SetCreditCardPaymentMethod(string idCompany, bool defaultCreditCardPaymentWithParam, bool defaultCreditCardPaymentWithPayNKolay)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, idCompany),
                    new FieldParameter("DefaultCreditCardPaymentWithParam", Enums.FieldType.Bit, defaultCreditCardPaymentWithParam),
                    new FieldParameter("DefaultCreditCardPaymentWithPayNKolay", Enums.FieldType.Bit, defaultCreditCardPaymentWithPayNKolay),
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
