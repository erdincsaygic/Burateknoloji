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
    public class CreditCardPaymentNotificationDAL : BaseDAL<CreditCardPaymentNotification>, ICreditCardPaymentNotificationDAL
    {
        public override string TableName
        {
            get { return "CreditCardPaymentNotifications"; }
        }

        public string SetStatus(CreditCardPaymentNotification entity)
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

        //public string SetDescriptions(CreditCardPaymentNotification entity)
        //{
        //    try
        //    {
        //        var parameters = new List<FieldParameter> {
        //            new FieldParameter("ID", Enums.FieldType.NVarChar, entity.ID),
        //            new FieldParameter("Description", Enums.FieldType.NVarChar, entity.Description),
        //            new FieldParameter("SmsVerificationDescription", Enums.FieldType.NVarChar, entity.SmsVerificationDescription)
        //        };

        //        _connector = new tSQLConnector();
        //        _connector.BeginTransaction();
        //        var IDMaster = _connector.RunSqlCommand(TableName + "_SetDescriptions", parameters);
        //        _connector.CommitOrRollBackTransaction(Enums.TransactionType.Commit);

        //        return IDMaster;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (_connector.SqlConn != null)
        //            _connector.CommitOrRollBackTransaction(Enums.TransactionType.RollBack);
        //        throw new Exception(ex.Message);
        //    }
        //}
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

        public List<CreditCardPaymentNotification> GetBlockeds(string IDCompany, int length, int start, string searchValue)
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

            return new List<CreditCardPaymentNotification>();
        }

        public List<CreditCardPaymentNotification> GetNotBlockeds(string IDCompany, int length, int start, string searchValue)
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

            return new List<CreditCardPaymentNotification>();
        }

        public List<CreditCardPaymentNotification> GetPendingList()
        {
            try
            {
                _connector = new tSQLConnector();
                DataTable dt = _connector.GetDataTable(TableName + "_GetPendingList", null);
                return CreateAndGetObjectFromDataTable(dt);
            }
            catch { }

            return new List<CreditCardPaymentNotification>();
        }
        public CreditCardPaymentNotification GetSingleByTransactionID(string transactionID)
        {

            try
            {
                var parameters = new List<FieldParameter> { new FieldParameter("TransactionID", Enums.FieldType.NVarChar, transactionID) };
                _connector = new tSQLConnector();
                DataRow dr = _connector.GetDataRow(TableName + "_GetSingleByTransactionID", parameters);
                return CreateAndGetObjectFromDataRow(dr);
            }
            catch { }

            return new CreditCardPaymentNotification();
        }

        public CreditCardPaymentNotification GetSingleByTransactionNr(string IDCompany, string transactionNr)
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

            return new CreditCardPaymentNotification();
        }

        public List<GetGetCreditCardTransactionsAPIModel> GetCreditCardTransactionsAPI(List<FieldParameter> parameters)
        {
            try
            {
                _connector = new tSQLConnector();

                var getCreditCardTransactionsAPIModelList = new List<GetGetCreditCardTransactionsAPIModel>();

                var dtList = _connector.GetDataTable(TableName + "_GetCreditCardTransactionsAPI", parameters);

                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    var getCreditCardTransactionsAPIModel = new GetGetCreditCardTransactionsAPIModel()
                    {
                        SenderName = dtList.Rows[i]["SenderName"].ToString(),
                        ActionDate = Convert.ToDateTime(dtList.Rows[i]["ActionDate"].ToString()),
                        Amount = Convert.ToDecimal(dtList.Rows[i]["Amount"].ToString()),
                        CDate = Convert.ToDateTime(dtList.Rows[i]["CDate"].ToString()),
                        Description = dtList.Rows[i]["Description"].ToString(),
                        Phone = dtList.Rows[i]["Phone"].ToString(),
                        Status = dtList.Rows[i]["Status"].ToString(),
                        TransactionID = dtList.Rows[i]["TransactionID"].ToString(),
                        TransactionNr = dtList.Rows[i]["TransactionNr"].ToString(),
                        CardNumber = dtList.Rows[i]["CardNumber"].ToString(),
                        MemberIPAddress = dtList.Rows[i]["MemberIPAddress"].ToString(),
                        MemberPort = dtList.Rows[i]["MemberPort"].ToString(),
                        RebateStatus = dtList.Rows[i]["RebateStatus"].ToString()
                    };

                    getCreditCardTransactionsAPIModelList.Add(getCreditCardTransactionsAPIModel);
                }

                return getCreditCardTransactionsAPIModelList;
            }
            catch { }

            return new List<GetGetCreditCardTransactionsAPIModel>();

        }

        public List<CreditCardPaymentNotification> GetEncryptedCardNumberData(string encryptedCardNumber)
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

            return new List<CreditCardPaymentNotification>();
        }

        public CreditCardTransactionCheckFraudControlDto CreditCardTransactionCheckFraudControl(string encryptedCardNumber, int timeSpanInMinutes, int transactionLimitToday)
        {
            try
            {
                _connector = new tSQLConnector();

                var parameters = new List<FieldParameter> {
                    new FieldParameter("EncryptedCardNumber", Enums.FieldType.NVarChar, encryptedCardNumber),
                    new FieldParameter("TimeSpanInMinutes", Enums.FieldType.Int, timeSpanInMinutes),
                    new FieldParameter("TransactionLimitToday", Enums.FieldType.Int, transactionLimitToday)
                };

                var dtList = _connector.GetDataTable(TableName + "_CreditCardTransactionCheckFraudControl", parameters);

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
