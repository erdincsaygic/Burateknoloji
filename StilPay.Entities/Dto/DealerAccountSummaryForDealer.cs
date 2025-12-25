using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Dto
{
    public class DealerAccountSummaryForDealer
    {
        public decimal PaymentNotificationTotalAmount { get; set; }
        public decimal PaymentNotificationSumNetTotal { get; set; }
        public decimal PaymentNotificationCount { get; set; }

        public decimal CreditCardSumTotal { get; set; }
        public decimal CreditCardSumNetTotal { get; set; }
        public decimal CreditCardCount { get; set; }

        public decimal WithdrawalRequestTotalAmount { get; set; }
        public decimal WithdrawalRequestCount { get; set; }

    }
}
