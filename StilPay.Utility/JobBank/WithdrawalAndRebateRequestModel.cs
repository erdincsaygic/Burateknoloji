using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.JobBank
{
    public class WithdrawalAndRebateRequestModel
    {
        public string ID { get; set; }
        public DateTime MDate { get; set; }
        public string TransactionID { get; set; }
        public string TransactionNr { get; set; }
        public string RequestNr { get; set; }
        public string Title { get; set; }
        public bool IsRebate { get; set; }
        public decimal Amount { get; set; }
        public string Iban { get; set; }
        public string Description { get; set; }
        public string CompanyBankAccountID { get; set; }
        public string IDCompany { get; set; }
        public string SIDBank { get; set; }
    }
}
