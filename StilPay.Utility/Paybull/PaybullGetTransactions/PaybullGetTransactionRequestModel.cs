using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.Paybull.PaybullGetTransactions
{
    public class PaybullGetTransactionRequestModel
    {
        public string merchant_key { get; set; }
        public string hash_key { get; set; }
        public string date { get; set; }
    }
}
