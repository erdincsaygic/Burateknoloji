using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.Paybull.PaybullRefund
{
    public class PaybullRefundRequestModel
    {
        public string invoice_id { get; set; }
        public decimal amount { get; set; }
        public string app_id { get; set; }
        public string app_secret { get; set; }
        public string merchant_key { get; set; }
        public string hash_key { get; set; }
    }
}
