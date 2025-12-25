using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.JobBank
{
    public class WithdrawalPoolModel
    {
        public string ID { get; set; }
        public DateTime CDate { get; set; }

        public DateTime? MDate { get; set; }

        public DateTime TransactionDate { get; set; }

        public string IDBank { get; set; }

        public string Bank { get; set; }

        public string ReceiverName { get; set; }

        public string ReceiverIban { get; set; }

        public string TransactionKey { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; }

        public string ResponseDescription { get; set; }

        public string ResponseTransactionNr { get; set; }

        public string ResponseRequestNr { get; set; }

        public byte Status { get; set; }

        public string Company { get; set; }

        public string CompanyBankAccountID { get; set; }

        public string IDCompany { get; set; }
    }
}
