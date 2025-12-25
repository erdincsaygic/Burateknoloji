using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Dto
{
    public class BankTransactionList
    {
        public DateTime TransactionDate { get; set; }
        public string IDBank { get; set; }
        public string SenderName { get; set; }
        public string BankName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime FinanceTransactionDate { get; set; }
        public decimal FinanceTransactionAmount { get; set; }
        public byte FinanceTransactionType { get; set; }
        public int TotalRecords { get; set; }
        public decimal TotalBalance { get; set; }
        public bool IsPositiveBalance { get; set; }

    }
}
