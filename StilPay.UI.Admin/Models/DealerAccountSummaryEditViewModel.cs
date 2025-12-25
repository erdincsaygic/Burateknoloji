using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.UI.Admin.Models
{
    public class DealerAccountSummaryEditViewModel
    {
        public List<BankAccountSumModel> CompanyBankAccounts { get; set; }
        public List<PaymentInstitution> PaymentInstitutions { get; set; }
        public List<DealerCreditCardAccountSummaryReportDetail> MonthlyCreditCardAccountSummaryReportDetails { get; set; }
        public List<DealerBankTransferAccountSummaryReportDetail> MonthlyBankTransferAccountSummaryReportDetails { get; set; }
        public AccountSummary AccountSummary { get; set; }
        public DealerAccountSummary DealerAccountSummaries { get; set; }



        public DealerAccountSummaryEditViewModel()
        {
        }


        public class DealerAccountSummary
        {
            public DateTime StartDate { get; set; }    
            public DateTime EndDate { get; set; }
            public string IDCompany { get; set; }
            public decimal PreviousReportNetAmount { get; set; }

            public decimal Profit { get; set; }

            public decimal ExpenseAmount { get; set; }

            public decimal IncomeAmount { get; set; }

            public decimal Difference { get; set; }

            public decimal PaymentNotificationProfit { get; set; }
            public decimal PaymentNotificationTotalAmount { get; set; }
            public decimal PaymentNotificationCount { get; set; }

            public decimal CreditCardPaymentNotificationProfit { get; set; }
            public decimal CreditCardPaymentNotificationTotalAmount { get; set; }
            public decimal CreditCardCount { get; set; }

            public decimal ForeignCreditCardPaymentNotificationProfit { get; set; }
            public decimal ForeignCreditCardPaymentNotificationTotalAmount { get; set; }
            public decimal ForeignCreditCardCount { get; set; }

            public decimal WithdrawalRequestProfit { get; set; }
            public decimal WithdrawalRequestTotalAmount { get; set; }
            public decimal WithdrawalRequestCount { get; set; }

            public decimal NetAmount { get; set; }

            public decimal RebateNetAmount { get; set; }

            public bool IsMonthly { get; set; }

            public decimal CreditCardPoolBalance { get; set; }
            public decimal FraudPoolBalance { get; set; }
            public decimal FraudExpenseProfitAmount { get; set; }

            public decimal BankCardTypePaymentNotificationTotalAmount { get; set; }
            public decimal BankCardTypePaymentNotificationProfit { get; set; }
            public decimal BankCardTypeCount { get; set; }
            public decimal AverageCommissionRate { get; set; }
            public List<ForeignCreditCardSummary> ForeignCreditCardSummaries { get; set; }
        }

        public class DealerCreditCardAccountSummaryReportDetail
        {
            public string PaymentInstitutionID { get; set; }
            public string PaymentInstitutionName { get; set; }
            public decimal Balance { get; set; }
            public decimal TotalProfit { get; set; }
            public decimal PaymentInstitutionProfit { get; set; }
            public decimal BalanceForNetCalculate { get; set; }
        }

        public class DealerBankTransferAccountSummaryReportDetail
        {
            public string IDBank { get; set; }
            public string BankName { get; set; }
            public bool IsExitAccount { get; set; }
            public decimal Balance { get; set; }
            public decimal TotalProfit { get; set; }
            public decimal WithdrawalTotalProfit { get; set; }
            public decimal WithdrawalBankBalance { get; set; }
            public decimal BalanceForNetCalculate { get; set; }
        }
    }
}
