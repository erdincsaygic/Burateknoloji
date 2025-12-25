using StilPay.Entities.Dto;
using System;
using System.Collections.Generic;

namespace StilPay.UI.Dealer.Models
{
    public class AccountSummaryEditViewModel
    {
        public DealerAccountSummary DealerAccountSummaries { get; set; }


        public AccountSummaryEditViewModel()
        {
        }

        public class DealerAccountSummary
        {
            public DateTime StartDate { get; set; }    
            public DateTime EndDate { get; set; }

            public decimal PaymentNotificationTotalAmount { get; set; }
            public decimal PaymentNotificationCount { get; set; }
            public decimal PaymentNotificationSumNetTotal { get; set; }

            public decimal CreditCardSumTotal { get; set; }
            public decimal CreditCardSumNetTotal { get; set; }
            public decimal CreditCardCount { get; set; }

            public decimal WithdrawalRequestTotalAmount { get; set; }
            public decimal WithdrawalRequestCount { get; set; }
        }
    }
}
