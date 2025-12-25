using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Dto
{
    public class CreditCardTransactionList
    {
        public DateTime TransactionDate { get; set; }
        public string PaymentInstitutionID { get; set; }
        public string SenderName { get; set; }
        public string PaymentInstitutionName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalBalance { get; set; }
        public bool IsPositiveBalance { get; set; }
        public int TotalRecords { get; set; }
    }
}
