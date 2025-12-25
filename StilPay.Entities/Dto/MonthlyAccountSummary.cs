using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Dto
{
    public class MonthlyAccountSummary
    {
        public string CreditCardPaymentMethodID { get; set; }
        public decimal CreditCardSumTotal { get; set; }
        public decimal CreditCardProfit { get; set; }

        public string ForeignCreditCardPaymentMethodID { get; set; }
        public decimal ForeignCreditCardSumTotal { get; set; }
        public decimal ForeignCreditCardProfit { get; set; }

        public string PaymentTranferPoolBankID { get; set; }
        public decimal PaymentTranferPoolSumTotal { get; set; }

        public string WithdrawalRequestSIDBank { get; set; }
        public decimal WithdrawalRequestTotalAmount { get; set; }
        public decimal WithdrawalRequestProfit { get; set; }

        public string PaymentNotificationSIDBank { get; set; }
        public decimal PaymentNotificationTotalAmount { get; set; }

        public string IDCompany { get; set; }
        public decimal CompanyTransactionTotalAmount { get; set; }
        public decimal CompanyTransactionProfit { get; set; }
        public decimal CompanyWithdrawalTransactionTotalAmount { get; set; }
        public decimal CompanyWithdrawalTransactionCount { get; set; }

        public decimal RebateNetAmount { get; set; }
        public decimal DailyTotalPaymentAmount { get; set; }
        public decimal WeeklyTotalPaymentAmount { get; set; }
        public decimal MonthlyTotalPaymentAmount { get; set; }
    }
}
