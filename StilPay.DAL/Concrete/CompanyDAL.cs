using StilPay.DAL.Abstract;
using StilPay.Entities;
using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace StilPay.DAL.Concrete
{
    public class CompanyDAL : BaseDAL<Company>, ICompanyDAL
    {
        public override string TableName
        {
            get { return "Companies"; }
        }

        public dynamic GetBalance(string idCompany)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, idCompany)
                };

                _connector = new tSQLConnector();
                var row = _connector.GetDataRow(TableName + "_GetBalance", parameters);

                if(row != null)
                {
                    dynamic dynamicObj = new ExpandoObject();
                    dynamicObj.UsingBalance = Convert.ToDecimal(row["UsingBalance"]);
                    dynamicObj.BlockedBalance = Convert.ToDecimal(row["BlockedBalance"]);
                    dynamicObj.TotalBalance = Convert.ToDecimal(row["TotalBalance"]);
                    dynamicObj.NegativeBalanceLimit = Convert.ToDecimal(row["NegativeBalanceLimit"]);

                    return dynamicObj;
                }

                return null;
            }
            catch { }

            return null;
        }

        public string SetBalance(string idCompany, decimal usingBalance, string idActionType, string paymentTransferPoolID)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, idCompany),
                    new FieldParameter("UsingBalance", Enums.FieldType.Decimal, usingBalance),
                    new FieldParameter("IDActionType", Enums.FieldType.NVarChar, idActionType),
                    new FieldParameter("PaymentTransferPoolID", Enums.FieldType.NVarChar, paymentTransferPoolID)
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SetBalance", parameters);
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

        public string BalanceTransfer(string idCompany, string receiverIdCompany, decimal amount)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, idCompany),
                    new FieldParameter("ReceiverIDCompany", Enums.FieldType.NVarChar, receiverIdCompany),
                    new FieldParameter("Amount", Enums.FieldType.Decimal, amount),
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_BalanceTransfer", parameters);
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

        public string SetNegativeBalanceLimit(string idCompany, decimal negativeBalanceLimit)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, idCompany),
                    new FieldParameter("NegativeBalanceLimit", Enums.FieldType.Decimal, negativeBalanceLimit)
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SetNegativeBalanceLimit", parameters);
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

        public string SetStatus(string idCompany, bool status)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, idCompany),
                    new FieldParameter("StatusFlag", Enums.FieldType.Bit, status)
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

        public string SetAutoWithdrawalLimit(string idCompany, decimal autoWithdrawalLimit)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, idCompany),
                    new FieldParameter("AutoWithdrawalLimit", Enums.FieldType.Decimal, autoWithdrawalLimit)
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SetAutoWithdrawalLimit", parameters);
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

        public string SetAutoTransferLimit(string idCompany, decimal autoTransferLimit)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, idCompany),
                    new FieldParameter("AutoTransferLimit", Enums.FieldType.Decimal, autoTransferLimit)
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SetAutoTransferLimit", parameters);
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

        public string SetAutoCreditCardLimit(string idCompany, decimal autoCreditCardLimit)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, idCompany),
                    new FieldParameter("AutoCreditCardLimit", Enums.FieldType.Decimal, autoCreditCardLimit)
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SetAutoCreditCardLimit", parameters);
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

        public string SetAutoForeignCreditCardLimit(string idCompany, decimal autoForeignCreditCardLimit)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, idCompany),
                    new FieldParameter("AutoForeignCreditCardLimit", Enums.FieldType.Decimal, autoForeignCreditCardLimit)
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SetAutoForeignCreditCardLimit", parameters);
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

        public string NewDealerInsert(NewDealerDto newDealerDto)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("CDate", Enums.FieldType.DateTime, DateTime.Now),
                    new FieldParameter("CUser", Enums.FieldType.NVarChar, newDealerDto.IDUser),
                    new FieldParameter("Name", Enums.FieldType.NVarChar, newDealerDto.Name),
                    new FieldParameter("Phone", Enums.FieldType.NVarChar, newDealerDto.Phone),
                    new FieldParameter("Password", Enums.FieldType.NVarChar, newDealerDto.Password),
                    new FieldParameter("Title", Enums.FieldType.NVarChar, newDealerDto.Title),
                    new FieldParameter("Email", Enums.FieldType.NVarChar, newDealerDto.Email),
                    new FieldParameter("TaxNr", Enums.FieldType.NVarChar, newDealerDto.TaxNr),
                    new FieldParameter("TaxOffice", Enums.FieldType.NVarChar, newDealerDto.TaxOffice),
                    new FieldParameter("MonthlyGiro", Enums.FieldType.NVarChar, newDealerDto.MonthlyGiro),
                    new FieldParameter("Address", Enums.FieldType.NVarChar, newDealerDto.Address),
                    new FieldParameter("AutoWithdrawalLimit", Enums.FieldType.Decimal, newDealerDto.AutoWithdrawalLimit),
                    new FieldParameter("AutoTransferLimit", Enums.FieldType.Decimal, newDealerDto.AutoTransferLimit),
                    new FieldParameter("AutoCreditCardLimit", Enums.FieldType.Decimal, newDealerDto.AutoCreditCardLimit),
                    new FieldParameter("AutoForeignCreditCardLimit", Enums.FieldType.Decimal, newDealerDto.AutoForeignCreditCardLimit),
                    new FieldParameter("CreditCardRate", Enums.FieldType.Decimal, newDealerDto.CreditCardRate),
                    new FieldParameter("TransferRate", Enums.FieldType.Decimal, newDealerDto.TransferRate),
                    new FieldParameter("MobilePayRate", Enums.FieldType.Decimal, newDealerDto.MobilePayRate),
                    new FieldParameter("WithdrawalTransferAmount", Enums.FieldType.Decimal, newDealerDto.WithdrawalTransferAmount),
                    new FieldParameter("WithdrawalEftAmount", Enums.FieldType.Decimal, newDealerDto.WithdrawalEftAmount),
                    new FieldParameter("ForeignCreditCardRate", Enums.FieldType.Decimal, newDealerDto.ForeignCreditCardRate),
                    new FieldParameter("SiteUrl", Enums.FieldType.NVarChar, newDealerDto.SiteUrl),
                    new FieldParameter("CallbackUrl", Enums.FieldType.NVarChar, newDealerDto.CallbackUrl),
                    new FieldParameter("RedirectUrl", Enums.FieldType.NVarChar, newDealerDto.RedirectUrl),
                    new FieldParameter("IPAddress", Enums.FieldType.NVarChar, newDealerDto.IPAddress),
                    new FieldParameter("WithdrawalRequestCallBack", Enums.FieldType.NVarChar, newDealerDto.WithdrawalRequestCallBack),
                    new FieldParameter("WithdrawalApiBeUsed", Enums.FieldType.Bit, newDealerDto.WithdrawalApiBeUsed),
                    new FieldParameter("ForeignCreditCardBeUsed", Enums.FieldType.Bit, newDealerDto.ForeignCreditCardBeUsed),
                    new FieldParameter("TransferBeUsed", Enums.FieldType.Bit, newDealerDto.TransferBeUsed),
                    new FieldParameter("CreditCardBeUsed", Enums.FieldType.Bit, newDealerDto.CreditCardBeUsed)
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_NewDealerInsert", parameters);
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
