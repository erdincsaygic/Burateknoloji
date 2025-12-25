using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Data;

namespace StilPay.DAL.Concrete
{
    public class ForeignCreditCardPaymentNotificationDAL : BaseDAL<ForeignCreditCardPaymentNotification>, IForeignCreditCardPaymentNotificationDAL
    {
        public override string TableName
        {
            get { return "ForeignCreditCardPaymentNotifications"; }
        }

        public string SetStatus(ForeignCreditCardPaymentNotification entity)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, entity.ID),
                    new FieldParameter("MUser", Enums.FieldType.NVarChar, entity.MUser),
                    new FieldParameter("MDate", Enums.FieldType.DateTime, entity.MDate),
                    new FieldParameter("Status", Enums.FieldType.Tinyint, entity.Status),
                    new FieldParameter("Description", Enums.FieldType.NVarChar, entity.Description),
                    new FieldParameter("TransactionReferenceCode", Enums.FieldType.NVarChar, entity.TransactionReferenceCode),
                    new FieldParameter("PaymentInstitutionCommissionRate", Enums.FieldType.Decimal, entity.PaymentInstitutionCommissionRate),
                    new FieldParameter("PaymentInstitutionNetAmount", Enums.FieldType.Decimal, entity.PaymentInstitutionNetAmount),
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
        public string SetMemberIPAdress(string IDEntity, string ipAddress, string port)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, IDEntity),
                    new FieldParameter("MemberIPAddress", Enums.FieldType.NVarChar, ipAddress),
                    new FieldParameter("MemberPort", Enums.FieldType.NVarChar, port),
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SetIPAddress", parameters);
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

        public List<ForeignCreditCardPaymentNotification> GetBlockeds(string IDCompany, int length, int start, string searchValue)
        {
            try
            {
                _connector = new tSQLConnector();
                var parameters = new List<FieldParameter> {
                    new FieldParameter("IDCompany", Enums.FieldType.NVarChar, IDCompany),
                    new FieldParameter("PageLenght", Enums.FieldType.Int, length),
                    new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                    new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue)
                };
                DataTable dt = _connector.GetDataTable(TableName + "_GetBlockeds", parameters);
                return CreateAndGetObjectFromDataTable(dt);
            }
            catch { }

            return new List<ForeignCreditCardPaymentNotification>();
        }

        public List<ForeignCreditCardPaymentNotification> GetNotBlockeds(string IDCompany, int length, int start, string searchValue)
        {
            try
            {
                _connector = new tSQLConnector();
                var parameters = new List<FieldParameter> {
                    new FieldParameter("IDCompany", Enums.FieldType.NVarChar, IDCompany),
                    new FieldParameter("PageLenght", Enums.FieldType.Int, length),
                    new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                    new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue)
                };
                DataTable dt = _connector.GetDataTable(TableName + "_GetNotBlockeds", parameters);
                return CreateAndGetObjectFromDataTable(dt);
            }
            catch { }

            return new List<ForeignCreditCardPaymentNotification>();
        }

        public ForeignCreditCardPaymentNotification GetSingleByTransactionID(string transactionID)
        {

            try
            {
                var parameters = new List<FieldParameter> { new FieldParameter("TransactionID", Enums.FieldType.NVarChar, transactionID) };
                _connector = new tSQLConnector();
                DataRow dr = _connector.GetDataRow(TableName + "_GetSingleByTransactionID", parameters);
                return CreateAndGetObjectFromDataRow(dr);
            }
            catch { }

            return new ForeignCreditCardPaymentNotification();
        }

        public ForeignCreditCardPaymentNotification GetSingleByTransactionNr(string IDCompany, string transactionNr)
        {

            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("IDCompany", Enums.FieldType.NVarChar, IDCompany),
                    new FieldParameter("TransactionNr", Enums.FieldType.NVarChar, transactionNr),
                };
                _connector = new tSQLConnector();
                DataRow dr = _connector.GetDataRow(TableName + "_GetSingleByTransactionNr", parameters);
                return CreateAndGetObjectFromDataRow(dr);
            }
            catch { }

            return new ForeignCreditCardPaymentNotification();
        }

        public List<ForeignCreditCardPaymentNotification> GetPendingList()
        {
            try
            {
                _connector = new tSQLConnector();
                DataTable dt = _connector.GetDataTable(TableName + "_GetPendingList", null);
                return CreateAndGetObjectFromDataTable(dt);
            }
            catch { }

            return new List<ForeignCreditCardPaymentNotification>();
        }

        public List<ForeignCreditCardPaymentNotification> GetEncryptedCardNumberData(string encryptedCardNumber)
        {
            try
            {
                _connector = new tSQLConnector();
                var parameters = new List<FieldParameter> {
                    new FieldParameter("EncryptedCardNumber", Enums.FieldType.NVarChar, encryptedCardNumber)
                };
                DataTable dt = _connector.GetDataTable(TableName + "_GetEncryptedCardNumberData", parameters);
                return CreateAndGetObjectFromDataTable(dt);
            }
            catch { }

            return new List<ForeignCreditCardPaymentNotification>();
        }

        public CreditCardTransactionCheckFraudControlDto ForeignCreditCardTransactionCheckFraudControl(string encryptedCardNumber, int timeSpanInMinutes, int transactionLimitToday)
        {
            try
            {
                _connector = new tSQLConnector();

                var parameters = new List<FieldParameter> {
                    new FieldParameter("EncryptedCardNumber", Enums.FieldType.NVarChar, encryptedCardNumber),
                    new FieldParameter("TimeSpanInMinutes", Enums.FieldType.Int, timeSpanInMinutes),
                    new FieldParameter("TransactionLimitToday", Enums.FieldType.Int, transactionLimitToday)
                };

                var dtList = _connector.GetDataTable(TableName + "_ForeignCreditCardTransactionCheckFraudControl", parameters);

                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    var creditCardTransactionCheckFraudControlDto = new CreditCardTransactionCheckFraudControlDto()
                    {
                        DailyTransactionLimitExceeded = Convert.ToBoolean(dtList.Rows[i]["DailyTransactionLimitExceeded"].ToString()),
                        RecentTransactionLimitExceeded = Convert.ToBoolean(dtList.Rows[i]["RecentTransactionLimitExceeded"].ToString()),
                        TransactionWithin24Hours = Convert.ToBoolean(dtList.Rows[i]["TransactionWithin24Hours"].ToString()),
                        TransactionCountToday = Convert.ToInt32(dtList.Rows[i]["TransactionCountToday"].ToString())
                    };

                    return creditCardTransactionCheckFraudControlDto;
                }
            }
            catch { }

            return new CreditCardTransactionCheckFraudControlDto();
        }

        public string SetAutoNotification(string entityId, string description, bool isAutoNotification)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, entityId),
                    new FieldParameter("Description", Enums.FieldType.NVarChar, description),
                    new FieldParameter("IsAutoNotification", Enums.FieldType.NVarChar, isAutoNotification),
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SetAutoNotification", parameters);
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
