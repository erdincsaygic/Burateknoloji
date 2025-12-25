using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.IsBankTransferService.Models.IsBankAccounts
{
    public class IsBankAccountsResponseModel
    {
        public class Data
        {
            public string branch_code { get; set; }
            public string account_number { get; set; }
            public string account_id { get; set; }
            public string account_balance { get; set; }
            public string blockage_amount { get; set; }
            public string last_transactions_date { get; set; }
            public string available_amount { get; set; }
            public string iban { get; set; }
            public string currency_code { get; set; }
            public string product_code { get; set; }
            public bool overdraft_protection { get; set; }
            public string branch_name { get; set; }
        }

        public class Root
        {
            public List<Data> data { get; set; }
            public string httpCode { get; set; }
            public string httpMessage { get; set; }
            public string moreInformation { get; set; }
        }
    }
}
