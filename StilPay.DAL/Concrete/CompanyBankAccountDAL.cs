using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Data;

namespace StilPay.DAL.Concrete
{
    public class CompanyBankAccountDAL : BaseDAL<CompanyBankAccount>, ICompanyBankAccountDAL
    {
        public override string TableName
        {
            get { return "CompanyBankAccounts"; }
        }

        public List<BankAccountSumModel> CompanyBankAccountSum(string idCompany, string IDBank, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                _connector = new tSQLConnector();

                var list = new List<BankAccountSumModel>();


                List<FieldParameter> parameters = new List<FieldParameter>()
                {
                     new FieldParameter("IDCompany", Enums.FieldType.NVarChar, idCompany),
                     new FieldParameter("IDBank", Enums.FieldType.NVarChar,IDBank),
                     new FieldParameter("StartDate", Enums.FieldType.DateTime,startDate),
                     new FieldParameter("EndDate", Enums.FieldType.DateTime,endDate)
                };

                DataTable dtList = _connector.GetDataTable(TableName + "_TotalSum", parameters);

                if (dtList != null)
                {
                    for (int i = 0; i < dtList.Rows.Count; i++)
                    {
                        BankAccountSumModel sumModel = new BankAccountSumModel();

                        sumModel.Amount = Math.Round(Convert.ToDecimal(dtList.Rows[i]["Amount"]), 2);
                        sumModel.CreditCardAmount = Math.Round(Convert.ToDecimal(dtList.Rows[i]["CreditCardAmount"]), 2);
                        sumModel.EftAmount = Math.Round(Convert.ToDecimal(dtList.Rows[i]["EftAmount"]), 2);
                        sumModel.ID = dtList.Rows[i]["ID"].ToString();
                        sumModel.Name = dtList.Rows[i]["Name"].ToString();
                        sumModel.IDBank = dtList.Rows[i]["IDBank"].ToString();
                        sumModel.IsExitAccount = Convert.ToBoolean(dtList.Rows[i]["IsExitAccount"]);

                        list.Add(sumModel);
                    }

                    return list;
                }
            }
            catch { }

            return new List<BankAccountSumModel>();
        }

        public List<BankAccountSumModel> CreditCardAccountSum(string idCompany, string IDBank, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                _connector = new tSQLConnector();

                List<BankAccountSumModel> list = new List<BankAccountSumModel>();
                

                List<FieldParameter> parameters = new List<FieldParameter>()
                {
                     new FieldParameter("IDCompany", Enums.FieldType.NVarChar, idCompany),
                     new FieldParameter("IDBank", Enums.FieldType.NVarChar,IDBank),
                     new FieldParameter("StartDate", Enums.FieldType.DateTime,startDate),
                     new FieldParameter("EndDate", Enums.FieldType.DateTime,endDate)
                };

                DataTable dtList = _connector.GetDataTable(TableName + "_TotalCreditCardSum", parameters);

                if (dtList != null)
                {
                    for (int i = 0; i < dtList.Rows.Count; i++)
                    {
                        BankAccountSumModel sumModel = new BankAccountSumModel();

                        sumModel.Amount = Math.Round(Convert.ToDecimal(dtList.Rows[i]["Amount"]), 2);
                        sumModel.CreditCardAmount = Math.Round(Convert.ToDecimal(dtList.Rows[i]["CreditCardAmount"]), 2);
                        sumModel.EftAmount = Math.Round(Convert.ToDecimal(dtList.Rows[i]["EftAmount"]), 2);
                        sumModel.NetAmount = Math.Round(Convert.ToDecimal(dtList.Rows[i]["NetAmount"]), 2);
                        sumModel.ID = dtList.Rows[i]["ID"].ToString();
                        sumModel.Name = dtList.Rows[i]["Name"].ToString();
                        sumModel.UseForForeignCard = Convert.ToBoolean(dtList.Rows[i]["UseForForeignCard"]);
                        sumModel.CountNetAmount = Math.Round(Convert.ToDecimal(dtList.Rows[i]["CountNetAmount"]), 0);
                        sumModel.EndOfDayTime = dtList.Rows[i]["EndOfDayTime"].ToString();

                        list.Add(sumModel);
                    }


                    return list;
                }
                
            }
            catch { }

            return new List<BankAccountSumModel>();
        }

        public string SetIsActiveByDefault(string id, bool IsActiveByDefaultExpenseAccount)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id),
                    new FieldParameter("IsActiveByDefaultExpenseAccount", Enums.FieldType.Bit, IsActiveByDefaultExpenseAccount),
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SetIsActiveByDefault", parameters);
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
