using StilPay.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StilPay.UI.Admin.Models
{
    public class AccountReportCreditCardModel
    {

        public string ID { get; set; }
        public int PaymentMethod { get; set; }
        public string PaymentMethodName { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public byte TransactionType { get; set; }

        public string Description { get; set; }

        public string IDBankName { get; set; }
        public string IDBankName2 { get; set; }
        public string CUserName { get; set; }
        public string IncomeType { get; set; }
        public string ExpenseType { get; set; }
    }
}
