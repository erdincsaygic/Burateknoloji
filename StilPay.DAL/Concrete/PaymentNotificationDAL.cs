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

namespace StilPay.DAL.Concrete
{
    public class PaymentNotificationDAL : BaseDAL<PaymentNotification>, IPaymentNotificationDAL
    {
        public override string TableName
        {
            get { return "PaymentNotifications"; }
        }

        public string SetStatus(PaymentNotification entity)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, entity.ID),
                    new FieldParameter("MUser", Enums.FieldType.NVarChar, entity.MUser),
                    new FieldParameter("MDate", Enums.FieldType.DateTime, entity.MDate),
                    new FieldParameter("Status", Enums.FieldType.Tinyint, entity.Status),
                    new FieldParameter("Description", Enums.FieldType.NVarChar, entity.Description),
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

        public List<PaymentNotification> GetBlockeds(string IDCompany, int length, int start, string searchValue)
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

            return new List<PaymentNotification>();
        }

        public List<PaymentNotification> GetNotBlockeds(string IDCompany, int length, int start, string searchValue)
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

            return new List<PaymentNotification>();
        }

        public PaymentNotification GetSingleByTransactionNr(string IDCompany, string transactionNr)
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

            return new PaymentNotification();
        }

        public PaymentNotification GetSingleByTransactionID(string transactionID)
        {

            try
            {
                var parameters = new List<FieldParameter> { new FieldParameter("TransactionID", Enums.FieldType.NVarChar, transactionID) };
                _connector = new tSQLConnector();
                DataRow dr = _connector.GetDataRow(TableName + "_GetSingleByTransactionID", parameters);
                return CreateAndGetObjectFromDataRow(dr);
            }
            catch { }

            return new PaymentNotification();
        }

        public PaymentNotification GetSingleByTransactionKey(string transactionKey)
        {

            try
            {
                var parameters = new List<FieldParameter> { new FieldParameter("TransactionKey", Enums.FieldType.NVarChar, transactionKey) };
                _connector = new tSQLConnector();
                DataRow dr = _connector.GetDataRow(TableName + "_GetSingleByTransactionKey", parameters);
                return CreateAndGetObjectFromDataRow(dr);
            }
            catch { }

            return new PaymentNotification();
        }

        public List<GetPaymentNotificationsAPIModel> GetPaymentNotificationsAPI(List<FieldParameter> parameters)
        {
            try
            {
                _connector = new tSQLConnector();

                var getPaymentNotificationsAPIModelList = new List<GetPaymentNotificationsAPIModel>();

                var dtList = _connector.GetDataTable(TableName + "_GetPaymentNotificationsAPI", parameters);

                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    var getPaymentNotificationsAPIModel = new GetPaymentNotificationsAPIModel()
                    {
                        SenderName = dtList.Rows[i]["SenderName"].ToString(),
                        ActionDate = Convert.ToDateTime(dtList.Rows[i]["ActionDate"].ToString()),
                        Amount = Convert.ToDecimal(dtList.Rows[i]["Amount"].ToString()),
                        CDate = Convert.ToDateTime(dtList.Rows[i]["CDate"].ToString()),
                        Description = dtList.Rows[i]["Description"].ToString(),
                        Phone = dtList.Rows[i]["Phone"].ToString(),
                        Status = dtList.Rows[i]["Status"].ToString(),
                        TransactionID = dtList.Rows[i]["TransactionID"].ToString(),
                    };

                    getPaymentNotificationsAPIModelList.Add(getPaymentNotificationsAPIModel);
                }

                return getPaymentNotificationsAPIModelList;
            }
            catch { }

            return new List<GetPaymentNotificationsAPIModel>();
         
        }

    }
}
