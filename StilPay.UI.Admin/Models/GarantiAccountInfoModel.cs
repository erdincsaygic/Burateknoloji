using System.Collections.Generic;
using System;


namespace StilPay.UI.Admin.Models
{
    public class GarantiAccountInfoModel
    {
        public class Balance
        {
            public string Amount { get; set; }
            public string Type { get; set; }
        }

        public class Account
        {
            public int AccountNum { get; set; }
            public string IBAN { get; set; }
            public DateTime AccountInstanceId { get; set; }
            public string AccountSubType { get; set; }
            public string AccountType { get; set; }
            public int CustomerNum { get; set; }
            public int UnitNum { get; set; }
            public string AccountMainType { get; set; }
            public string ProductName { get; set; }
            public List<Balance> Balances { get; set; }
            public DateTime LastActivityDate { get; set; }
            public string CurrencyCode { get; set; }
            public string Status { get; set; }
        }

        public class Result
        {
            public int ReturnCode { get; set; }
            public int ReasonCode { get; set; }
            public string MessageText { get; set; }
        }

        public class GarantiAccountInfo
        {
            public Result Result { get; set; }
            public List<Account> Accounts { get; set; }
        }

        public class GarantiTokenModel
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string scope { get; set; }
        }
    }
}


