using System;

namespace StilPay.UI.Admin.Models
{
    public class SearchInBankModel
    {
        public DateTime TransactionDate { get; set; }
        public string SenderName { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Bank { get; set; }
        public string TransactionKey { get; set; }
    }
}
