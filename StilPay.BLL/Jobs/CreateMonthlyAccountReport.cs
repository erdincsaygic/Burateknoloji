using Coravel.Invocable;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.IsBankTransferService;
using StilPay.Utility.KuveytTurk;
using StilPay.Utility.Models;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Text;
using static StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPayment.IsBankPaymentRequestModel;
using static StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPaymentValidation.IsBankPaymentValidationRequestModel;
using System.Text.Json;
using ZiraatBankPaymentService;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.ExtendedProperties;
using StilPay.UI.Admin.Models;
using System.Linq;
using StilPay.BLL.Abstract;
using Microsoft.AspNetCore.Http;
using StilPay.BLL.Concrete;
using static StilPay.Utility.Helper.Enums;

namespace StilPay.BLL.Jobs
{
    public class CreateMonthlyAccountReport : IInvocable
    {
        private readonly IAccountSummaryManager _accountSummaryManager;
        private readonly ICompanyBankAccountManager _companyBankAccountManager;
        public readonly ICompanyManager _companyManager;
        public readonly ICreditCardAccountSummaryReportDetailManager _creditCardAccountSummaryReportDetailManager;
        public readonly IBankTransferAccountSummaryReportDetailManager _bankTransferAccountSummaryReportDetailManager;
        public readonly ICompanyRebateRequestManager _companyRebateRequestManager;
        public readonly ICompanyWithdrawalRequestManager _companyWithdrawalRequestManager;
        private readonly ICompanyTransactionManager _companyTransactionManager;
        private readonly IPaymentInstitutionManager _paymentInstitutionManager;
        private readonly ICreditCardPaymentNotificationManager _creditCardPaymentNotificationManager;
        private readonly IForeignCreditCardPaymentNotificationManager _foreignCreditCardPaymentNotificationManager;
        private readonly ICompanyCurrencyManager _companyCurrencyManager;

        public CreateMonthlyAccountReport(ICompanyBankAccountManager companyBankAccountManager,ICompanyTransactionManager companyTransactionManager, ICompanyManager companyManager, ICreditCardAccountSummaryReportDetailManager creditCardAccountSummaryReportDetailManager, IBankTransferAccountSummaryReportDetailManager bankTransferAccountSummaryReportDetailManager, ICompanyRebateRequestManager companyRebateRequestManager, ICompanyWithdrawalRequestManager companyWithdrawalRequestManager, IPaymentInstitutionManager paymentInstitutionManager, IAccountSummaryManager accountSummaryManager, ICreditCardPaymentNotificationManager creditCardPaymentNotificationManager, IForeignCreditCardPaymentNotificationManager foreignCreditCardPaymentNotificationManager, ICompanyCurrencyManager companyCurrencyManager)
        {
            _companyBankAccountManager = companyBankAccountManager;
            _companyTransactionManager = companyTransactionManager;
            _companyManager = companyManager;
            _creditCardAccountSummaryReportDetailManager = creditCardAccountSummaryReportDetailManager;
            _bankTransferAccountSummaryReportDetailManager = bankTransferAccountSummaryReportDetailManager;
            _companyRebateRequestManager = companyRebateRequestManager;
            _companyWithdrawalRequestManager = companyWithdrawalRequestManager;
            _paymentInstitutionManager = paymentInstitutionManager;
            _accountSummaryManager = accountSummaryManager;
            _creditCardPaymentNotificationManager = creditCardPaymentNotificationManager;
            _foreignCreditCardPaymentNotificationManager = foreignCreditCardPaymentNotificationManager;
            _companyCurrencyManager = companyCurrencyManager;
        }

        public async Task Invoke()
        {
            var reportStartDateTime = DateTime.Now;
            var companiesTotalBalance = 0.0M;
            var companies = _companyManager.GetActiveList(null);

            var totalBankExpenseAmount = 0.0M;
            var totalBankIncomeAmount = 0.0M;
            //var rebateNetTotal = 0.0M;

            var previosReportEntity = _accountSummaryManager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("PageLenght", Enums.FieldType.Int, 1),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null),
                new FieldParameter("IsMonthly", Enums.FieldType.Bit, false)
            }).FirstOrDefault();

            if (previosReportEntity == null)
            {
                var data2 = new AccountSummary()
                {
                    CDate = reportStartDateTime,
                    CUser = "00000000-0000-0000-0000-000000000000",
                    WithdrawalRequestProfit = 0,
                    PaymentNotificationProfit = 0,
                    UnmatchedPaymentNotificationBalance = 0,
                    CreditCardPaymentNotificationProfit = 0,
                    ExpenseAmount = 0,
                    IncomeAmount = 0,
                    DealerTotalBalance = 0,
                    Profit = 0,
                    NetAmount = 0,
                    Difference = 0,
                    IsMonthly = false,
                    RebateExpenseProfitAmount = 0
                };
            }

            foreach (var company in companies)
            {
                var companyBalance = _companyManager.GetBalance(company.ID);

                var companyWithdrawal = _companyWithdrawalRequestManager.GetList(new List<FieldParameter>
                    {
                        new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.Pending),
                        new FieldParameter("IDCompany", Enums.FieldType.NVarChar, company.ID),
                        new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                        new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                        new FieldParameter("PageLenght", Enums.FieldType.Int, 9999),
                        new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                        new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null),
                        new FieldParameter("WithProcess", Enums.FieldType.Bit, true)
                    });

                var companyRebate = _companyRebateRequestManager.GetList(new List<FieldParameter>
                    {
                        new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.Pending),
                        new FieldParameter("IDCompany", Enums.FieldType.NVarChar, company.ID),
                        new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                        new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                        new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                        new FieldParameter("PageLenght", Enums.FieldType.Int, 9999),
                        new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                        new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null),
                        new FieldParameter("WithProcess", Enums.FieldType.Bit, true)
                    });

                //if (companyRebate.Count > 0)
                //{
                //    foreach (var item in companyRebate)
                //    {
                //        rebateNetTotal += _companyTransactionManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, item.PaymentRecordID) }).NetTotal;
                //    }
                //}

                var getCompaniesCurrencyBalance = _companyCurrencyManager.GetList(new List<FieldParameter> { new FieldParameter("IDCompany", Enums.FieldType.NVarChar, company.ID) });

                companiesTotalBalance += Math.Round(companyBalance.TotalBalance, 2) + companyWithdrawal.Sum(s => s.Amount + s.CostTotal) + companyRebate.Sum(s => s.Amount);

                companiesTotalBalance += getCompaniesCurrencyBalance.Sum(s => s.Balance + s.BlockedBalance);
            }

            var model = new
            {
                PaymentInstitutions = _paymentInstitutionManager.GetList(null),
                CompanyBankAccounts = _companyBankAccountManager.CompanyBankAccountSum("1312E00F-E83E-45B4-85C6-892396D12331", null, null, null),
                CreditCardAccountSummaryReportDetails = new List<CreditCardAccountSummaryReportDetail>(),
                BankTransferAccountSummaryReportDetails = new List<BankTransferAccountSummaryReportDetail>()
            };

            foreach (var item in model.CompanyBankAccounts)
            {
                //var companyBankAccount = _companyBankAccountManager.CompanyBankAccountSum("1312E00F-E83E-45B4-85C6-892396D12331", item.ID,null,null);

                totalBankExpenseAmount += _companyTransactionManager.BankTransactionsDetailExpenseList(item.ID,
                   new DateTime(previosReportEntity.CDate.Year, previosReportEntity.CDate.Month, previosReportEntity.CDate.Day, previosReportEntity.CDate.Hour, previosReportEntity.CDate.Minute, 0),
                   new DateTime(reportStartDateTime.Year, reportStartDateTime.Month, reportStartDateTime.Day, reportStartDateTime.Hour, reportStartDateTime.Minute, 0)).Sum(s => s.Amount);

                totalBankIncomeAmount += _companyTransactionManager.BankTransactionsDetailIncomeList(item.ID,
                    new DateTime(previosReportEntity.CDate.Year, previosReportEntity.CDate.Month, previosReportEntity.CDate.Day, previosReportEntity.CDate.Hour, previosReportEntity.CDate.Minute, 0),
                    new DateTime(reportStartDateTime.Year, reportStartDateTime.Month, reportStartDateTime.Day, reportStartDateTime.Hour, reportStartDateTime.Minute, 0)).Sum(s => s.Amount);

                model.BankTransferAccountSummaryReportDetails.Add(new BankTransferAccountSummaryReportDetail()
                {
                    Balance = item.Amount,
                    BankName = item.Name,
                    IDBank = item.IDBank,
                    IsExitAccount = item.IsExitAccount
                });
            }

            List<BankAccountSumModel> ccSumModel = _companyBankAccountManager.CreditCardAccountSum("1312E00F-E83E-45B4-85C6-892396D12331", null, null, null);

            foreach (var item in model.PaymentInstitutions)
            {
                model.CreditCardAccountSummaryReportDetails.Add(new CreditCardAccountSummaryReportDetail()
                {
                    Balance = ccSumModel.FirstOrDefault(f => f.ID == item.ID).Amount,
                    PaymentInstitutionID = item.ID,
                    PaymentInstitutionName = item.Name
                });
            }

            var ccPool = _creditCardPaymentNotificationManager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.PayPool),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null),
                new FieldParameter("IsAutoNotification", Enums.FieldType.Tinyint, null),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("PageLenght", Enums.FieldType.Int, 9999),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar,null)
            });

            var creditCardPoolAmount = (decimal)(ccPool.Count > 0 ? ccPool.Sum(x => x.PaymentInstitutionNetAmount) : 0);

            var foreignCCPool = _foreignCreditCardPaymentNotificationManager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.PayPool),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null),
                new FieldParameter("IsAutoNotification", Enums.FieldType.Tinyint, null),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("PageLenght", Enums.FieldType.Int, 9999),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar,null),
            });

            var foreignCreditCardPoolAmount = (decimal)(foreignCCPool.Count > 0 ? foreignCCPool.Sum(x => x.PaymentInstitutionNetAmount) : 0);

            var dealerFraudPool = _companyTransactionManager.GetDealerFraudPool(new List<FieldParameter>()
            {
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime,null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("PageLength", Enums.FieldType.Int, 9999),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null)
            });

            creditCardPoolAmount += foreignCreditCardPoolAmount;

            var dealerFraudPoolAmount = dealerFraudPool.Count > 0 ? dealerFraudPool.Sum(x => x.PaymentInstitutionNetAmount) : 0;

            //var dealerFraudExpenseProfitAmount = dealerFraudPool.Where(x => (x.PaymentMethodType != (int)PaymentMethod.CreditCard || x.PaymentMethodType == (int)PaymentMethod.ForeignCreditCard) && x.MDate >= previosReportEntity.CDate).ToList().Count > 0 ? dealerFraudPool.Where(x => (x.PaymentMethodType == (int)PaymentMethod.CreditCard || x.PaymentMethodType == (int)PaymentMethod.ForeignCreditCard) && x.MDate >= previosReportEntity.CDate).Sum(x => x.PaymentInstitutionNetAmount - x.DealerCommission) : 0;

            var accountReport = _companyTransactionManager.GetAccountReport(null, previosReportEntity.CDate, reportStartDateTime.Date, previosReportEntity.CDate, reportStartDateTime);

            var positiveAmount = model.CreditCardAccountSummaryReportDetails.Sum(s => s.Balance) + model.BankTransferAccountSummaryReportDetails.Sum(s => s.Balance);

            var negativeAmount = companiesTotalBalance + accountReport.TransferNotMatchedAmount + creditCardPoolAmount + dealerFraudPoolAmount;

            var profit = accountReport.CreditCardCommissionProfitAmount + accountReport.TransferCommissionProfitAmount + accountReport.WithdrawalTotalTransactionFeeAmount + totalBankIncomeAmount;

            var netAmount = positiveAmount - negativeAmount;

            var data = new AccountSummary()
            {
                CDate = reportStartDateTime,
                CUser = "00000000-0000-0000-0000-000000000000",
                WithdrawalRequestProfit = accountReport.WithdrawalTotalTransactionFeeAmount,
                PaymentNotificationProfit = accountReport.TransferCommissionProfitAmount,
                UnmatchedPaymentNotificationBalance = accountReport.TransferNotMatchedAmount,
                CreditCardPaymentNotificationProfit = accountReport.CreditCardCommissionProfitAmount,
                ExpenseAmount = totalBankExpenseAmount,
                IncomeAmount = totalBankIncomeAmount,
                DealerTotalBalance = companiesTotalBalance,
                Profit = profit,
                NetAmount = netAmount,
                Difference = (netAmount + totalBankExpenseAmount + accountReport.RebateExpenseProfitAmount + accountReport.FraudExpenseProfitAmount) - (previosReportEntity.NetAmount + profit),
                IsMonthly = false,
                RebateExpenseProfitAmount = accountReport.RebateExpenseProfitAmount,
                CreditCardPoolBalance = creditCardPoolAmount,
                FraudPoolBalance = dealerFraudPoolAmount,
                FraudExpenseProfitAmount = accountReport.FraudExpenseProfitAmount
            };

            if(DateTime.Now.Day == 1)
            {
                data.IsMonthly = true;
                data.Month = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
                data.Year = DateTime.Now.Month == 1 ? DateTime.Now.Year - 1 : DateTime.Now.Year;
            }   

            var response = _accountSummaryManager.Insert(data);
            if (response != null && response.Status == "OK")
            {
                var insertedEntity = _accountSummaryManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, response.Data) });

                foreach (var item in model.CreditCardAccountSummaryReportDetails)
                {
                    item.AccountSummaryReportNo = insertedEntity.ReportNo;

                    _creditCardAccountSummaryReportDetailManager.Insert(item);
                }

                foreach (var item in model.BankTransferAccountSummaryReportDetails)
                {
                    item.AccountSummaryReportNo = insertedEntity.ReportNo;

                    _bankTransferAccountSummaryReportDetailManager.Insert(item);
                }
            }
        }
    }
}
