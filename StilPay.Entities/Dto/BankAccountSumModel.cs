using StilPay.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StilPay.UI.Admin.Models
{
    public class BankAccountSumModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string IDBank { get; set; }
        public bool IsExitAccount { get; set; }
        public bool UseForForeignCard { get; set; }
        public decimal Amount { get; set; }
        public decimal CreditCardAmount { get; set; }
        public decimal EftAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal CountNetAmount { get; set; }
        public string EndOfDayTime { get; set; }
    }
}
