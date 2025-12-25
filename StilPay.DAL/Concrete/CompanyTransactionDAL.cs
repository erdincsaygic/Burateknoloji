using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using StilPay.DAL.Extensions;
using Newtonsoft.Json;
using StilPay.Entities.Dto;

namespace StilPay.DAL.Concrete
{
    public class CompanyTransactionDAL : BaseDAL<CompanyTransaction>, ICompanyTransactionDAL
    {
        public override string TableName
        {
            get { return "CompanyTransactions"; }
        }

        public List<CompanyTransaction> GetCompanyInvoiceTransactions(string iDCompanyInvoice)
        {
            try
            {
                _connector = new tSQLConnector();
                List<FieldParameter> parameters = new List<FieldParameter>()
                {
                     new FieldParameter("IDCompanyInvoice", Enums.FieldType.NVarChar, iDCompanyInvoice),
                     new FieldParameter("InvoiceStatus", Enums.FieldType.Bit, true)
                };

                var dtList = _connector.GetDataTable(TableName + "_GetList", parameters);
                return CreateAndGetObjectFromDataTable(dtList);
            }
            catch { }

            return new List<CompanyTransaction>();
        }
        public List<CompanyTransaction> GetProcess(string idCompany, DateTime startDate, DateTime endDate, int pageLenght, int offsetValue, string searchValue, bool isMonthlyAccountSummary)
        {
            try
            {
                _connector = new tSQLConnector();
                List<FieldParameter> parameters = new List<FieldParameter>()
                {
                    new FieldParameter("IDCompany", Enums.FieldType.NVarChar, idCompany),
                    new FieldParameter("StartDate", Enums.FieldType.DateTime,startDate),
                    new FieldParameter("EndDate", Enums.FieldType.DateTime, endDate),
                    new FieldParameter("PageLenght", Enums.FieldType.Int, pageLenght),
                    new FieldParameter("OffsetValue", Enums.FieldType.Int, offsetValue),
                    new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue),
                    new FieldParameter("IsMonthlyAccountSummary", Enums.FieldType.Bit, isMonthlyAccountSummary)
                };

                var dtList = _connector.GetDataTable(TableName + "_GetProcess", parameters);
                return CreateAndGetObjectFromDataTable(dtList);
            }
            catch { }

            return new List<CompanyTransaction>();
        }

        public List<CompanyTransaction> GetListOld(string idCompany, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                _connector = new tSQLConnector();
                List<FieldParameter> parameters = new List<FieldParameter>()
                {
                    new FieldParameter("IDCompany", Enums.FieldType.NVarChar, idCompany),
                    new FieldParameter("StartDate", Enums.FieldType.DateTime,startDate),
                    new FieldParameter("EndDate", Enums.FieldType.DateTime, endDate)
                };

                var dtList = _connector.GetDataTable(TableName + "_GetListOld", parameters);
                return CreateAndGetObjectFromDataTable(dtList);
            }
            catch { }

            return new List<CompanyTransaction>();
        }

        public List<CompanyTransaction> GetProcess(string idCompany, DateTime startDate, DateTime endDate, string iDActionType, string iDCompanyInvoice, bool invoiceStatus)
        {
            try
            {
                _connector = new tSQLConnector();
                List<FieldParameter> parameters = new List<FieldParameter>()
                {
                     new FieldParameter("IDCompany", Enums.FieldType.NVarChar, idCompany),
                     new FieldParameter("StartDate", Enums.FieldType.DateTime,startDate),
                     new FieldParameter("EndDate", Enums.FieldType.DateTime, endDate),
                     new FieldParameter("IDActionType", Enums.FieldType.NVarChar, iDActionType),
                     new FieldParameter("IDCompanyInvoice", Enums.FieldType.NVarChar, iDCompanyInvoice),
                     new FieldParameter("InvoiceStatus", Enums.FieldType.Bit, invoiceStatus),
                };

                var dtList = _connector.GetDataTable(TableName + "_GetProcess", parameters);
                return CreateAndGetObjectFromDataTable(dtList);
            }
            catch { }

            return new List<CompanyTransaction>();
        }

        public AccountReportModel GetAccountReport(string idCompany, DateTime startDate, DateTime endDate, DateTime startDateTime, DateTime endDateTime)
        {
            try
            {
                _connector = new tSQLConnector();

                AccountReportModel list = new AccountReportModel();

                if (startDateTime.Hour == 0 && startDateTime.Minute == 0 && endDateTime.Hour == 0 && endDateTime.Minute == 0)
                {
                    endDate = endDate.AddDays(1);

                } else {
                    startDateTime = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDateTime.Hour, startDateTime.Minute, 0);
                    endDateTime = new DateTime(endDate.Year, endDate.Month, endDate.Day, endDateTime.Hour, endDateTime.Minute,0);
                }

                List<FieldParameter> parameters = new List<FieldParameter>()
                {
                     new FieldParameter("IDCompany", Enums.FieldType.NVarChar, idCompany),
                     new FieldParameter("StartDate", Enums.FieldType.DateTime, startDateTime.TimeOfDay == TimeSpan.Zero ? startDate : startDateTime),
                     new FieldParameter("EndDate", Enums.FieldType.DateTime,  endDateTime.TimeOfDay == TimeSpan.Zero ? endDate : endDateTime)
                };

                var dtList = _connector.GetDataTable(TableName + "_GetAccountReportv2", parameters);

                if (dtList != null)
                {
                    list.TransferTotalAmount = Convert.ToDecimal(dtList.Rows[0]["TransferTotalAmount"].ToString());
                    list.TransferCommissionProfitAmount = Convert.ToDecimal(dtList.Rows[0]["TransferCommissionProfitAmount"].ToString());
                    list.TransferDealerPaidAmount = Convert.ToDecimal(dtList.Rows[0]["TransferDealerPaidAmount"].ToString());
                    list.TransferNotMatchedAmount = Convert.ToDecimal(dtList.Rows[0]["TransferNotMatchedAmount"].ToString());

                    list.CreditCardTotalAmount = Convert.ToDecimal(dtList.Rows[0]["CreditCardTotalAmount"].ToString());
                    list.CreditCardCommissionProfitAmount = Convert.ToDecimal(dtList.Rows[0]["CreditCardCommissionProfitAmount"].ToString());
                    list.CreditCardPaymentInstitutionPaidAmount = Convert.ToDecimal(dtList.Rows[0]["CreditCardPaymentInstitutionPaidAmount"].ToString());
                    list.CreditCardDealerPaidAmount = Convert.ToDecimal(dtList.Rows[0]["CreditCardDealerPaidAmount"].ToString());

                    list.ForeignCreditCardTotalAmount = Convert.ToDecimal(dtList.Rows[0]["ForeignCreditCardTotalAmount"].ToString());
                    list.ForeignCreditCardCommissionProfitAmount = Convert.ToDecimal(dtList.Rows[0]["ForeignCreditCardCommissionProfitAmount"].ToString());
                    list.ForeignCreditCardPaymentInstitutionPaidAmount = Convert.ToDecimal(dtList.Rows[0]["ForeignCreditCardPaymentInstitutionPaidAmount"].ToString());
                    list.ForeignCreditCardDealerPaidAmount = Convert.ToDecimal(dtList.Rows[0]["ForeignCreditCardDealerPaidAmount"].ToString());

                    list.WithdrawalTotalAmount = Convert.ToDecimal(dtList.Rows[0]["WithdrawalTotalAmount"].ToString());
                    list.WithdrawalEftAmount = Convert.ToDecimal(dtList.Rows[0]["WithdrawalEftAmount"].ToString());
                    list.WithdrawalTotalPaidAmount = Convert.ToDecimal(dtList.Rows[0]["WithdrawalTotalPaidAmount"].ToString());
                    list.WithdrawalTotalTransactionFeeAmount = Convert.ToDecimal(dtList.Rows[0]["WithdrawalTotalTransactionFeeAmount"].ToString());
                    list.WithdrawalFastAmount = Convert.ToDecimal(dtList.Rows[0]["WithdrawalFastAmount"].ToString());

                    list.RebateExpenseProfitAmount = Convert.ToDecimal(dtList.Rows[0]["RebateExpenseProfitAmount"].ToString());

                    list.FraudExpenseProfitAmount = Convert.ToDecimal(dtList.Rows[0]["FraudExpenseProfitAmount"].ToString());

                    list.TotalIncomeAmount = Convert.ToDecimal(dtList.Rows[0]["TotalIncomeAmount"].ToString());
                    list.TotalExpenseAmount = Convert.ToDecimal(dtList.Rows[0]["TotalExpenseAmount"].ToString());

                    //list.SumTotal = Convert.ToDecimal(dtList.Rows[0]["sumTotal"].ToString());
                    //list.SumCommission = Convert.ToDecimal(dtList.Rows[0]["sumCommission"].ToString());
                    //list.SumNetTotal = Convert.ToDecimal(dtList.Rows[0]["sumNetTotal"].ToString());

                    //list.SumProcessTotal = Convert.ToDecimal(dtList.Rows[0]["sumProcessTotal"].ToString());
                    //list.SumEft = Convert.ToDecimal(dtList.Rows[0]["sumEft"].ToString());
                    //list.SumFast = Convert.ToDecimal(dtList.Rows[0]["sumFast"].ToString());
                    //list.SumProcess = Convert.ToDecimal(dtList.Rows[0]["sumProcess"].ToString());
                    //list.SumExpense = Convert.ToDecimal(dtList.Rows[0]["sumExpense"].ToString());
                    //list.sumCreditCardCommission = Convert.ToDecimal(dtList.Rows[0]["sumCreditCardCommission"].ToString());
                    //list.sumTransferCommission = Convert.ToDecimal(dtList.Rows[0]["sumTransferCommission"].ToString());
                    //list.sumTrPoolNotMatchTotal = Convert.ToDecimal(dtList.Rows[0]["sumTrPoolNotMatchTotal"].ToString());

                    //list.CountEft = Convert.ToDecimal(dtList.Rows[0]["countEft"].ToString());
                    //list.CountFast = Convert.ToDecimal(dtList.Rows[0]["countFast"].ToString());
                    //list.CountProcess = Convert.ToDecimal(dtList.Rows[0]["countProcess"].ToString());



                    return list;
                }
              }
            catch { }

            return new AccountReportModel();
        }

        public List<AccountReportCreditCardModel> GetAccountReportCreditCardDetail(string idCompany, DateTime startDate, DateTime endDate)
        {
            try
            {
                _connector = new tSQLConnector();

                List<AccountReportCreditCardModel> list = new List<AccountReportCreditCardModel>();


                List<FieldParameter> parameters = new List<FieldParameter>()
                {
                     new FieldParameter("IDCompany", Enums.FieldType.NVarChar, idCompany),
                     new FieldParameter("StartDate", Enums.FieldType.DateTime,startDate),
                     new FieldParameter("EndDate", Enums.FieldType.DateTime, endDate)
                };

                var dtList = _connector.GetDataTable(TableName + "_GetAccountReportCreditCardDetail", parameters);

                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    AccountReportCreditCardModel listDetail = new AccountReportCreditCardModel();
                    listDetail.ID = dtList.Rows[i]["ID"].ToString();
                    listDetail.Amount = Convert.ToDecimal(dtList.Rows[i]["Amount"].ToString());
                    listDetail.TransactionDate = Convert.ToDateTime(dtList.Rows[i]["TransactionDate"].ToString());
                    listDetail.PaymentMethod = Convert.ToInt32(dtList.Rows[i]["PaymentMethod"].ToString());
                    listDetail.PaymentMethodName = dtList.Rows[i]["PaymentMethodName"].ToString();
                    listDetail.IDBankName = dtList.Rows[i]["IDBankName"].ToString();
                    listDetail.IDBankName2 = dtList.Rows[i]["IDBankName2"].ToString();
                    listDetail.CUserName = dtList.Rows[i]["CUserName"].ToString();
                    listDetail.Description = dtList.Rows[i]["Description"].ToString();


                    list.Add(listDetail);
                }

                return list;
            }
            catch { }

            return new List<AccountReportCreditCardModel>();
        }

        public List<AccountReportCreditCardModel> BankTransactionsDetailIncomeList(string idBank, DateTime startDate, DateTime endDate)
        {
            try
            {
                _connector = new tSQLConnector();

                List<AccountReportCreditCardModel> list = new List<AccountReportCreditCardModel>();


                List<FieldParameter> parameters = new List<FieldParameter>()
                {
                     new FieldParameter("IDBank", Enums.FieldType.NVarChar, idBank),
                     new FieldParameter("StartDate", Enums.FieldType.DateTime,startDate),
                     new FieldParameter("EndDate", Enums.FieldType.DateTime, endDate)
                };

                var dtList = _connector.GetDataTable(TableName + "_GetAccountReportBankTransactionsDetailIncome", parameters);

                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    AccountReportCreditCardModel listDetail = new AccountReportCreditCardModel();
                    listDetail.ID = dtList.Rows[i]["ID"].ToString();
                    listDetail.Amount = Convert.ToDecimal(dtList.Rows[i]["Amount"].ToString());
                    listDetail.TransactionDate = Convert.ToDateTime(dtList.Rows[i]["TransactionDate"].ToString());
                    listDetail.PaymentMethod = Convert.ToInt32(dtList.Rows[i]["PaymentMethod"].ToString());
                    listDetail.PaymentMethodName = dtList.Rows[i]["PaymentMethodName"].ToString();
                    listDetail.IDBankName = dtList.Rows[i]["IDBankName"].ToString();
                    listDetail.IDBankName2 = dtList.Rows[i]["IDBankName2"].ToString();
                    listDetail.CUserName = dtList.Rows[i]["CUserName"].ToString();
                    listDetail.IncomeType = dtList.Rows[i]["IncomeType"].ToString();
                    listDetail.Description = dtList.Rows[i]["Description"].ToString();

                    list.Add(listDetail);
                }

                return list;
            }
            catch { }

            return new List<AccountReportCreditCardModel>();
        }

        public List<AccountReportCreditCardModel> BankTransactionsDetailExpenseList(string idBank, DateTime startDate, DateTime endDate)
        {
            try
            {
                _connector = new tSQLConnector();

                List<AccountReportCreditCardModel> list = new List<AccountReportCreditCardModel>();


                List<FieldParameter> parameters = new List<FieldParameter>()
                {
                     new FieldParameter("IDBank", Enums.FieldType.NVarChar, idBank),
                     new FieldParameter("StartDate", Enums.FieldType.DateTime,startDate),
                     new FieldParameter("EndDate", Enums.FieldType.DateTime, endDate)
                };

                var dtList = _connector.GetDataTable(TableName + "_GetAccountReportBankTransactionsDetailExpense", parameters);

                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    AccountReportCreditCardModel listDetail = new AccountReportCreditCardModel();
                    listDetail.ID = dtList.Rows[i]["ID"].ToString();
                    listDetail.Amount = Convert.ToDecimal(dtList.Rows[i]["Amount"].ToString());
                    listDetail.TransactionDate = Convert.ToDateTime(dtList.Rows[i]["TransactionDate"].ToString());
                    listDetail.PaymentMethod = Convert.ToInt32(dtList.Rows[i]["PaymentMethod"].ToString());
                    listDetail.PaymentMethodName = dtList.Rows[i]["PaymentMethodName"].ToString();
                    listDetail.IDBankName = dtList.Rows[i]["IDBankName"].ToString();
                    listDetail.IDBankName2 = dtList.Rows[i]["IDBankName2"].ToString();
                    listDetail.CUserName = dtList.Rows[i]["CUserName"].ToString();
                    listDetail.ExpenseType = dtList.Rows[i]["ExpenseType"].ToString();
                    listDetail.Description = dtList.Rows[i]["Description"].ToString();

                    list.Add(listDetail);
                }

                return list;
            }
            catch { }

            return new List<AccountReportCreditCardModel>();
        }

        public List<AccountReportCreditCardModel> BankTransactionsDetailTransferList(string idBank, DateTime startDate, DateTime endDate)
        {
            try
            {
                _connector = new tSQLConnector();

                List<AccountReportCreditCardModel> list = new List<AccountReportCreditCardModel>();


                List<FieldParameter> parameters = new List<FieldParameter>()
                {
                     new FieldParameter("IDBank", Enums.FieldType.NVarChar, idBank),
                     new FieldParameter("StartDate", Enums.FieldType.DateTime,startDate),
                     new FieldParameter("EndDate", Enums.FieldType.DateTime, endDate)
                };

                var dtList = _connector.GetDataTable(TableName + "_GetAccountReportBankTransactionsDetailTransfer", parameters);

                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    AccountReportCreditCardModel listDetail = new AccountReportCreditCardModel();
                    listDetail.ID = dtList.Rows[i]["ID"].ToString();
                    listDetail.Amount = Convert.ToDecimal(dtList.Rows[i]["Amount"].ToString());
                    listDetail.TransactionDate = Convert.ToDateTime(dtList.Rows[i]["TransactionDate"].ToString());
                    listDetail.PaymentMethod = Convert.ToInt32(dtList.Rows[i]["PaymentMethod"].ToString());
                    listDetail.PaymentMethodName = dtList.Rows[i]["PaymentMethodName"].ToString();
                    listDetail.IDBankName = dtList.Rows[i]["IDBankName"].ToString();
                    listDetail.IDBankName2 = dtList.Rows[i]["IDBankName2"].ToString();
                    listDetail.CUserName = dtList.Rows[i]["CUserName"].ToString();
                    listDetail.IncomeType = dtList.Rows[i]["IncomeType"].ToString();
                    listDetail.TransactionType = Convert.ToByte(dtList.Rows[i]["TransactionType"].ToString());
                    listDetail.Description = dtList.Rows[i]["Description"].ToString();
                    list.Add(listDetail);
                }

                return list;
            }
            catch { }

            return new List<AccountReportCreditCardModel>();
        }

        public List<DealerTransactionQuery> GetRecordsByQueryParameter(string queryParameter)
        {
            try
            {
                _connector = new tSQLConnector();

                var dealerTransactionQueryList = new List<DealerTransactionQuery>();

                List<FieldParameter> parameters = new List<FieldParameter>()
                {
                     new FieldParameter("QueryParameter", Enums.FieldType.NVarChar, queryParameter)
                };

                var dtList = _connector.GetDataTable(TableName + "_GetRecordsByQueryParameter", parameters);

                if (dtList != null)
                {
                    for (int i = 0; i < dtList.Rows.Count; i++)
                    {
                        var dealerTransactionQuery = new DealerTransactionQuery();
                        dealerTransactionQuery.ID = dtList.Rows[i]["ID"].ToString();
                        dealerTransactionQuery.TableWithTheTransaction = Convert.ToByte(dtList.Rows[i]["TableWithTheTransaction"].ToString());
                        dealerTransactionQuery.Status = Convert.ToByte(dtList.Rows[i]["Status"].ToString());
                        dealerTransactionQueryList.Add(dealerTransactionQuery);
                    }


                    return dealerTransactionQueryList;
                }
            }
            catch { }

            return new List<DealerTransactionQuery>();
        }

        public List<BankTransactionList> GetBankTransactionList(List<FieldParameter> parameters)
        {
            try
            {
                _connector = new tSQLConnector();

                var bankTransactionList = new List<BankTransactionList>();

                var dtList = _connector.GetDataTable(TableName + "_GetBankTransactions", parameters);

                if (dtList != null)
                {
                    for (int i = 0; i < dtList.Rows.Count; i++)
                    {
                        var bankTransaction = new BankTransactionList
                        {
                            IDBank = dtList.Rows[i]["IDBank"].ToString(),
                            BankName = dtList.Rows[i]["BankName"].ToString(),
                            TransactionDate = Convert.ToDateTime(dtList.Rows[i]["TransactionDate"].ToString()),
                            //FinanceTransactionDate = Convert.ToDateTime(dtList.Rows[i]["FinanceTransactionDate"].ToString()),
                            Amount = decimal.Parse(dtList.Rows[i]["Amount"].ToString()),
                            Description = dtList.Rows[i]["Description"].ToString(),
                            //FinanceTransactionAmount = decimal.Parse(dtList.Rows[i]["FinanceTransactionAmount"].ToString()),
                            SenderName = dtList.Rows[i]["SenderName"].ToString(),
                            //FinanceTransactionType = Convert.ToByte(dtList.Rows[i]["FinanceTransactionType"].ToString()),
                            TotalRecords = int.Parse(dtList.Rows[i]["TotalRecords"].ToString()),
                            TotalBalance = decimal.Parse(dtList.Rows[i]["TotalBalance"].ToString()),
                            IsPositiveBalance = Convert.ToBoolean(dtList.Rows[i]["IsPositiveBalance"])
                        };

                        bankTransactionList.Add(bankTransaction);
                    }


                    return bankTransactionList;
                }
            }
            catch { }

            return new List<BankTransactionList>();
        }

        public List<CreditCardTransactionList> GetCreditCardTransactionList(List<FieldParameter> parameters)
        {
            try
            {
                _connector = new tSQLConnector();

                var creditCardTransactionList = new List<CreditCardTransactionList>();

                var dtList = _connector.GetDataTable(TableName + "_GetCreditCardTransactions", parameters);

                if (dtList != null)
                {
                    for (int i = 0; i < dtList.Rows.Count; i++)
                    {
                        var creditCardTransaction = new CreditCardTransactionList
                        {
                            PaymentInstitutionID = dtList.Rows[i]["PaymentInstitutionID"].ToString(),
                            PaymentInstitutionName = dtList.Rows[i]["PaymentInstitutionName"].ToString(),
                            TransactionDate = Convert.ToDateTime(dtList.Rows[i]["TransactionDate"].ToString()),
                            //FinanceTransactionDate = Convert.ToDateTime(dtList.Rows[i]["FinanceTransactionDate"].ToString()),
                            Amount = decimal.Parse(dtList.Rows[i]["Amount"].ToString()),
                            TotalBalance = decimal.Parse(dtList.Rows[i]["TotalBalance"].ToString()),
                            Description = dtList.Rows[i]["Description"].ToString(),
                            //FinanceTransactionAmount = decimal.Parse(dtList.Rows[i]["FinanceTransactionAmount"].ToString()),
                            SenderName = dtList.Rows[i]["SenderName"].ToString(),
                            //FinanceTransactionType = Convert.ToByte(dtList.Rows[i]["FinanceTransactionType"].ToString()),
                            TotalRecords = int.Parse(dtList.Rows[i]["TotalRecords"].ToString()),
                            IsPositiveBalance = Convert.ToBoolean(dtList.Rows[i]["IsPositiveBalance"]) 
                        };

                        creditCardTransactionList.Add(creditCardTransaction);
                    }
                    return creditCardTransactionList;
                }
            }
            catch { }

            return new List<CreditCardTransactionList>();
        }

        public List<MonthlyAccountSummary> GetMonthlyAccountSummary(DateTime dailyReportStartDateTime, DateTime dailyReportEndDateTime, DateTime reportStartDateTime, DateTime reportEndDateTime, DateTime weeklyReportStartDateTime, DateTime weeklyReportEndDateTime)
        {
            try
            {
                _connector = new tSQLConnector();
                var monthlyAccountSummaryList = new List<MonthlyAccountSummary>();

                List<FieldParameter> parameters = new List<FieldParameter>()
                {
                     new FieldParameter("DailyReportStartDateTime", Enums.FieldType.DateTime,dailyReportStartDateTime),
                     new FieldParameter("DailyReportEndDateTime", Enums.FieldType.DateTime, dailyReportEndDateTime),
                     new FieldParameter("ReportStartDateTime", Enums.FieldType.DateTime,reportStartDateTime),
                     new FieldParameter("ReportEndDateTime", Enums.FieldType.DateTime, reportEndDateTime),
                     new FieldParameter("WeeklyReportStartDateTime", Enums.FieldType.DateTime,weeklyReportStartDateTime),
                     new FieldParameter("WeeklyReportEndDateTime", Enums.FieldType.DateTime, weeklyReportEndDateTime)
                };

                var dtList = _connector.GetDataTable(TableName + "_GetMonthlyAccountSummary", parameters);

                if (dtList != null)
                {
                    for (int i = 0; i < dtList.Rows.Count; i++)
                    {
                        var creditCardPaymentMethodID = dtList.Rows[i]["CreditCardPaymentMethodID"] == DBNull.Value
                            ? null
                            : dtList.Rows[i]["CreditCardPaymentMethodID"].ToString();

                        var creditCardSumTotal = dtList.Rows[i]["CreditCardSumTotal"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(dtList.Rows[i]["CreditCardSumTotal"]);

                        var creditCardProfit = dtList.Rows[i]["CreditCardProfit"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(dtList.Rows[i]["CreditCardProfit"]);

                        var foreignCreditCardPaymentMethodID = dtList.Rows[i]["ForeignCreditCardPaymentMethodID"] == DBNull.Value
                            ? null
                            : dtList.Rows[i]["ForeignCreditCardPaymentMethodID"].ToString();

                        var foreignCreditCardSumTotal = dtList.Rows[i]["ForeignCreditCardSumTotal"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(dtList.Rows[i]["ForeignCreditCardSumTotal"]);

                        var foreignCreditCardProfit = dtList.Rows[i]["ForeignCreditCardProfit"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(dtList.Rows[i]["ForeignCreditCardProfit"]);

                        var paymentTranferPoolBankID = dtList.Rows[i]["PaymentTranferPoolBankID"] == DBNull.Value
                            ? null
                            : dtList.Rows[i]["PaymentTranferPoolBankID"].ToString();

                        var paymentTranferPoolSumTotal = dtList.Rows[i]["PaymentTranferPoolSumTotal"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(dtList.Rows[i]["PaymentTranferPoolSumTotal"]);

                        var withdrawalRequestSIDBank = dtList.Rows[i]["WithdrawalRequestSIDBank"] == DBNull.Value
                            ? null
                            : dtList.Rows[i]["WithdrawalRequestSIDBank"].ToString();

                        var withdrawalRequestTotalAmount = dtList.Rows[i]["WithdrawalRequestTotalAmount"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(dtList.Rows[i]["WithdrawalRequestTotalAmount"]);

                        var withdrawalRequestProfit = dtList.Rows[i]["WithdrawalRequestProfit"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(dtList.Rows[i]["WithdrawalRequestProfit"]);

                        var paymentNotificationSIDBank = dtList.Rows[i]["PaymentNotificationSIDBank"] == DBNull.Value
                            ? null
                            : dtList.Rows[i]["PaymentNotificationSIDBank"].ToString();

                        var paymentNotificationTotalAmount = dtList.Rows[i]["PaymentNotificationTotalAmount"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(dtList.Rows[i]["PaymentNotificationTotalAmount"]);

                        var dailyTotalPaymentAmount = dtList.Rows[i]["DailyTotalPaymentAmount"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(dtList.Rows[i]["DailyTotalPaymentAmount"]);

                        var weeklyTotalPaymentAmount = dtList.Rows[i]["WeeklyTotalPaymentAmount"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(dtList.Rows[i]["WeeklyTotalPaymentAmount"]);

                        var monthlyTotalPaymentAmount = dtList.Rows[i]["MonthlyTotalPaymentAmount"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(dtList.Rows[i]["MonthlyTotalPaymentAmount"]);

                        var idCompany = dtList.Rows[i]["IDCompany"] == DBNull.Value
                            ? null
                            : dtList.Rows[i]["IDCompany"].ToString();

                        var companyTransactionTotalAmount = dtList.Rows[i]["CompanyTransactionTotalAmount"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(dtList.Rows[i]["CompanyTransactionTotalAmount"]);

                        var companyTransactionProfit = dtList.Rows[i]["CompanyTransactionProfit"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(dtList.Rows[i]["CompanyTransactionProfit"]);

                        var companyWithdrawalTransactionTotalAmount = dtList.Rows[i]["CompanyWithdrawalTransactionTotalAmount"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(dtList.Rows[i]["CompanyWithdrawalTransactionTotalAmount"]);

                        var companyWithdrawalTransactionCount = dtList.Rows[i]["CompanyWithdrawalTransactionCount"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(dtList.Rows[i]["CompanyWithdrawalTransactionCount"]);

                        var rebateNetAmount = dtList.Rows[i]["RebateNetAmount"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(dtList.Rows[i]["RebateNetAmount"]);

                        var monthlyAccountSummary = new MonthlyAccountSummary
                        {
                            CreditCardPaymentMethodID = creditCardPaymentMethodID,
                            CreditCardSumTotal = creditCardSumTotal,
                            CreditCardProfit = creditCardProfit,
                            ForeignCreditCardPaymentMethodID = foreignCreditCardPaymentMethodID,
                            ForeignCreditCardSumTotal = foreignCreditCardSumTotal,
                            ForeignCreditCardProfit = foreignCreditCardProfit,
                            PaymentTranferPoolBankID = paymentTranferPoolBankID,
                            PaymentTranferPoolSumTotal = paymentTranferPoolSumTotal,
                            WithdrawalRequestSIDBank = withdrawalRequestSIDBank,
                            WithdrawalRequestTotalAmount = withdrawalRequestTotalAmount,
                            WithdrawalRequestProfit = withdrawalRequestProfit,
                            PaymentNotificationSIDBank = paymentNotificationSIDBank,
                            PaymentNotificationTotalAmount = paymentNotificationTotalAmount,
                            DailyTotalPaymentAmount = dailyTotalPaymentAmount,
                            WeeklyTotalPaymentAmount = weeklyTotalPaymentAmount,
                            MonthlyTotalPaymentAmount = monthlyTotalPaymentAmount,
                            CompanyTransactionProfit = companyTransactionProfit,
                            CompanyWithdrawalTransactionTotalAmount = companyWithdrawalTransactionTotalAmount,
                            CompanyWithdrawalTransactionCount = companyWithdrawalTransactionCount,
                            IDCompany = idCompany,
                            CompanyTransactionTotalAmount = companyTransactionTotalAmount,
                            RebateNetAmount = rebateNetAmount,
                            
                        };

                        monthlyAccountSummaryList.Add(monthlyAccountSummary);
                    }

                    return monthlyAccountSummaryList;
                }
            }
            catch { }

            return new List<MonthlyAccountSummary>();
        }



        public List<DealerAccountSummary> GetDealerAccountSummary(string idCompany, DateTime reportStartDateTime, DateTime reportEndDateTime)
        {
            try
            {
                _connector = new tSQLConnector();
                var dealerAccountSummaryList = new List<DealerAccountSummary>();

                List<FieldParameter> parameters = new List<FieldParameter>()
                {
                     new FieldParameter("IDCompany", Enums.FieldType.NVarChar, idCompany),
                     new FieldParameter("ReportStartDateTime", Enums.FieldType.DateTime, reportStartDateTime),
                     new FieldParameter("ReportEndDateTime", Enums.FieldType.DateTime, reportEndDateTime),
                };

                var dtList = _connector.GetDataTable(TableName + "_GetDealerAccountSummary", parameters);

                if (dtList != null)
                {
                    for (int i = 0; i < dtList.Rows.Count; i++)
                    {

                        var foreignCreditCardSummaryJson = dtList.Rows[i]["ForeignCreditCardSummary"].ToString();

                        var foreignCreditCardSummaries = JsonConvert.DeserializeObject<List<ForeignCreditCardSummary>>(foreignCreditCardSummaryJson);

                        var dealerAccountSummary = new DealerAccountSummary
                        {
                            CreditCardSumTotal = Convert.ToDecimal(dtList.Rows[i]["CreditCardSumTotal"] ?? 0),
                            CreditCardProfit = Convert.ToDecimal(dtList.Rows[i]["CreditCardProfit"] ?? 0),
                            WithdrawalRequestTotalAmount = Convert.ToDecimal(dtList.Rows[i]["WithdrawalRequestTotalAmount"] ?? 0),
                            WithdrawalRequestProfit = Convert.ToDecimal(dtList.Rows[i]["WithdrawalRequestProfit"] ?? 0),
                            PaymentNotificationTotalAmount = Convert.ToDecimal(dtList.Rows[i]["PaymentNotificationTotalAmount"] ?? 0),
                            PaymentNotificationTotalProfit = Convert.ToDecimal(dtList.Rows[i]["PaymentNotificationProfit"] ?? 0),
                            CreditCardCount = Convert.ToDecimal(dtList.Rows[i]["CreditCardCount"] ?? 0),
                            PaymentNotificationCount = Convert.ToDecimal(dtList.Rows[i]["PaymentNotificationCount"] ?? 0),
                            WithdrawalRequestCount = Convert.ToDecimal(dtList.Rows[i]["WithdrawalRequestCount"] ?? 0),
                            RebateNetAmount = Convert.ToDecimal(dtList.Rows[i]["RebateNetAmount"] ?? 0),
                            BankCardTypeCount = Convert.ToDecimal(dtList.Rows[i]["BankCardCount"] ?? 0),
                            BankCardTypePaymentNotificationTotalAmount = Convert.ToDecimal(dtList.Rows[i]["BankCardSumTotal"] ?? 0),
                            BankCardTypePaymentNotificationProfit = Convert.ToDecimal(dtList.Rows[i]["BankCardProfit"] ?? 0),
                            AverageCommissionRate = Convert.ToDecimal(dtList.Rows[i]["AverageCommissionRate"] ?? 0),
                            ForeignCreditCardSummaries = foreignCreditCardSummaries,
                            CreditCardFraudExpenseAmount = Convert.ToDecimal(dtList.Rows[i]["CreditCardFraudExpenseAmount"] ?? 0)
                        };

                        dealerAccountSummaryList.Add(dealerAccountSummary);
                    }

                    return dealerAccountSummaryList;
                }
            }
            catch { }

            return new List<DealerAccountSummary>();
        }

        public List<DealerAccountSummaryForDealer> GetDealerAccountSummaryForDealer(string idCompany, DateTime reportStartDateTime, DateTime reportEndDateTime)
        {
            try
            {
                _connector = new tSQLConnector();
                var dealerAccountSummaryList = new List<DealerAccountSummaryForDealer>();

                List<FieldParameter> parameters = new List<FieldParameter>()
                {
                     new FieldParameter("IDCompany", Enums.FieldType.NVarChar, idCompany),
                     new FieldParameter("ReportStartDateTime", Enums.FieldType.DateTime, reportStartDateTime),
                     new FieldParameter("ReportEndDateTime", Enums.FieldType.DateTime, reportEndDateTime),
                };

                var dtList = _connector.GetDataTable(TableName + "_GetDealerAccountSummaryForDealer", parameters);

                if (dtList != null)
                {
                    for (int i = 0; i < dtList.Rows.Count; i++)
                    {

                        var dealerAccountSummary = new DealerAccountSummaryForDealer
                        {
                            CreditCardSumTotal = Convert.ToDecimal(dtList.Rows[i]["CreditCardSumTotal"] ?? 0),
                            CreditCardSumNetTotal = Convert.ToDecimal(dtList.Rows[i]["CreditCardSumNetTotal"] ?? 0),
                            CreditCardCount = Convert.ToDecimal(dtList.Rows[i]["CreditCardCount"] ?? 0),
                            PaymentNotificationTotalAmount = Convert.ToDecimal(dtList.Rows[i]["PaymentNotificationTotalAmount"] ?? 0),
                            PaymentNotificationSumNetTotal = Convert.ToDecimal(dtList.Rows[i]["PaymentNotificationSumNetTotal"] ?? 0),
                            PaymentNotificationCount = Convert.ToDecimal(dtList.Rows[i]["PaymentNotificationCount"] ?? 0),
                            WithdrawalRequestCount = Convert.ToDecimal(dtList.Rows[i]["WithdrawalRequestCount"] ?? 0),
                            WithdrawalRequestTotalAmount = Convert.ToDecimal(dtList.Rows[i]["WithdrawalRequestTotalAmount"] ?? 0),
                        };

                        dealerAccountSummaryList.Add(dealerAccountSummary);
                    }

                    return dealerAccountSummaryList;
                }
            }
            catch { }

            return new List<DealerAccountSummaryForDealer>();
        }


        public decimal GetDealerTransactionBalance(List<FieldParameter> parameters)
        {
            try
            {
                _connector = new tSQLConnector();

                var dtList = _connector.GetDataTable(TableName + "_GetDealerTransactionBalance", parameters);

                if (dtList != null)
                    return  Convert.ToDecimal(dtList.Rows[0]["Balance"].ToString());                 
            }
            catch { }

            return 0;
        }

        public string SetStatusToFraud(SetStatusToFraudDto setStatusToFraudDto)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("EntityID", Enums.FieldType.NVarChar, setStatusToFraudDto.EntityID),
                    new FieldParameter("MDate", Enums.FieldType.DateTime, setStatusToFraudDto.MDate),
                    new FieldParameter("MUser", Enums.FieldType.NVarChar, setStatusToFraudDto.MUser),
                    new FieldParameter("Description", Enums.FieldType.NVarChar, setStatusToFraudDto.Description),
                    new FieldParameter("EntityActionType", Enums.FieldType.NVarChar, setStatusToFraudDto.EntityActionType),
                    new FieldParameter("Status", Enums.FieldType.Tinyint, (int)Enums.StatusType.FraudPool),
                    new FieldParameter("AdminAction", Enums.FieldType.NVarChar, setStatusToFraudDto.AdminAction),
                    new FieldParameter("IDCompany", Enums.FieldType.NVarChar, setStatusToFraudDto.IDCompany)
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SetStatusToFraud", parameters);
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

        public List<DealerFraudPoolDto> GetDealerFraudPool(List<FieldParameter> parameters)
        {
            try
            {
                _connector = new tSQLConnector();

                var creditCardTransactionList = new List<DealerFraudPoolDto>();

                var dtList = _connector.GetDataTable(TableName + "_GetDealerFraudPool", parameters);

                if (dtList != null)
                {
                    for (int i = 0; i < dtList.Rows.Count; i++)
                    {
                        var dealerFraudPoolDto = new DealerFraudPoolDto
                        {
                            ID = dtList.Rows[i]["ID"].ToString(),
                            CDate = Convert.ToDateTime(dtList.Rows[i]["CDate"].ToString()),
                            MDate= Convert.ToDateTime(dtList.Rows[i]["MDate"].ToString()),
                            PaymentMethodType = int.Parse(dtList.Rows[i]["PaymentMethodType"].ToString()),
                            TransactionID = dtList.Rows[i]["TransactionID"].ToString(),
                            TransactionNr = dtList.Rows[i]["TransactionNr"].ToString(),
                            Amount = Convert.ToDecimal(dtList.Rows[i]["Amount"].ToString()),
                            Description = dtList.Rows[i]["Description"].ToString(),
                            PaymentInstitutionName = dtList.Rows[i]["PaymentInstitutionName"].ToString(),
                            IDCompany = dtList.Rows[i]["IDCompany"].ToString(),
                            SenderName = dtList.Rows[i]["SenderName"].ToString(),
                            MemberName = dtList.Rows[i]["MemberName"].ToString(),
                            MemberPhone = dtList.Rows[i]["MemberPhone"].ToString(),
                            CompanyPhone = dtList.Rows[i]["CompanyPhone"].ToString(),
                            CardNumber = dtList.Rows[i]["CardNumber"].ToString(),
                            Company = dtList.Rows[i]["Company"].ToString(),
                            MUserName = dtList.Rows[i]["MUserName"].ToString(),
                            MUser = dtList.Rows[i]["MUser"].ToString(),
                            PaymentInstitutionNetAmount = Convert.ToDecimal(dtList.Rows[i]["PaymentInstitutionNetAmount"].ToString()),
                            DealerCommission = Convert.ToDecimal(dtList.Rows[i]["DealerCommission"].ToString())
                        };

                        creditCardTransactionList.Add(dealerFraudPoolDto);
                    }
                    return creditCardTransactionList;
                }
            }
            catch { }

            return new List<DealerFraudPoolDto>();
        }

        public List<CreditCardDetailedSearchDto> CreditCardDetailedSearch(List<FieldParameter> parameters)
        {
            try
            {
                _connector = new tSQLConnector();

                var dtList = _connector.GetDataTable(TableName + "_CreditCardDetailedSearch", parameters);
                return dtList.ToList<CreditCardDetailedSearchDto>();
            }
            catch { }

            return new List<CreditCardDetailedSearchDto>();
        }

        public List<CustomerInfoDetailedSearchDto> CustomerInfoDetailedSearch(List<FieldParameter> parameters)
        {
            try
            {
                _connector = new tSQLConnector();

                var dtList = _connector.GetDataTable(TableName + "_CustomerInfoDetailedSearch", parameters);
                return dtList.ToList<CustomerInfoDetailedSearchDto>();
            }
            catch { }

            return new List<CustomerInfoDetailedSearchDto>();
        }

        public List<TransferDetailedSearchDto> TransferDetailedSearch(List<FieldParameter> parameters)
        {
            try
            {
                _connector = new tSQLConnector();

                var dtList = _connector.GetDataTable(TableName + "_TransferDetailedSearch", parameters);
                return dtList.ToList<TransferDetailedSearchDto>();
            }
            catch { }

            return new List<TransferDetailedSearchDto>();
        }
    }
}
