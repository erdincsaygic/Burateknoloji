using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System.Collections.Generic;

namespace StilPay.UI.Admin.Models
{
    public class AccountingSummaryMonthlyEditViewModel
    {
        public List<BankAccountSumModel> CompanyBankAccounts { get; set; }
        public List<PaymentInstitution> PaymentInstitutions { get; set; }
        public List<MonthlyCreditCardAccountSummaryReportDetail> MonthlyCreditCardAccountSummaryReportDetails { get; set; }
        public List<MonthlyForeignCreditCardAccountSummaryReportDetail> MonthlyForeignCreditCardAccountSummaryReportDetails { get; set; }
        public List<MonthlyBankTransferAccountSummaryReportDetail> MonthlyBankTransferAccountSummaryReportDetails { get; set; }
        public AccountSummary AccountSummary { get; set; }
        public MonthlyAccountSummary MonthlyAccountSummaries { get; set; }

        public List<DealerTransactionDetail> DealerTransactionDetails { get; set; }
        public int SelectedMonth { get; set; }
        public int SelectedYear { get; set; }

        public AccountingSummaryMonthlyEditViewModel()
        {
        }

        public class DealerTransactionDetail
        {
            public string IDCompany { get; set; }
            public string CompanyName { get; set; }
            public decimal TotalTransactionAmount { get; set; }
            public decimal TransactionProfitAmount { get; set; }
            public decimal TotalWithdrawalTransactionAmount { get; set; }
            public decimal WithdrawalTransactionCount { get; set; }
        }

        public class MonthlyAccountSummary
        {
            public decimal PreviousReportNetAmount { get; set; }

            public decimal Profit { get; set; }

            public decimal ExpenseAmount { get; set; }

            public decimal IncomeAmount { get; set; }

            public decimal Difference { get; set; }

            public decimal PaymentNotificationProfit { get; set; }

            public decimal CreditCardPaymentNotificationProfit { get; set; }
            public decimal ForeignCreditCardPaymentNotificationProfit { get; set; }

            public decimal WithdrawalRequestProfit { get; set; }

            public decimal MonthlyTotalPaymentAmount { get; set; }
            public decimal WeeklyTotalPaymentAmount { get; set; }
            public decimal DailyTotalPaymentAmount { get; set; }

            public decimal NetAmount { get; set; }

            public decimal RebateNetAmount { get; set; }

            public bool IsMonthly { get; set; }

            public decimal CreditCardPoolBalance { get; set; }
            public decimal FraudPoolBalance { get; set; }
            public decimal FraudExpenseProfitAmount { get; set; }
            
        }

        public class MonthlyCreditCardAccountSummaryReportDetail
        {
            public string PaymentInstitutionID { get; set; }
            public string PaymentInstitutionName { get; set; }
            public decimal Balance { get; set; }
            public decimal TotalProfit { get; set; }
            public decimal PaymentInstitutionProfit { get; set; }
            public decimal BalanceForNetCalculate { get; set; }
        }

        public class MonthlyForeignCreditCardAccountSummaryReportDetail
        {
            public string PaymentInstitutionID { get; set; }
            public string PaymentInstitutionName { get; set; }
            public decimal Balance { get; set; }
            public decimal TotalProfit { get; set; }
            public decimal PaymentInstitutionProfit { get; set; }
            public decimal BalanceForNetCalculate { get; set; }
        }

        public class MonthlyBankTransferAccountSummaryReportDetail
        {
            public string IDBank { get; set; }
            public string BankName { get; set; }
            public bool IsExitAccount { get; set; }
            public decimal Balance { get; set; }
            public decimal TotalProfit { get; set; }
            public decimal WithdrawalTotalProfit { get; set; }
            public decimal WithdrawalBankBalance{ get; set; }
            public decimal BalanceForNetCalculate{ get; set; }

        }
    }
}
