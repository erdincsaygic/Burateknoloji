using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.Paybull.PaybullRefund
{
    public class PaybullRefundResponseModel
    {
        public class Root
        {
            public int status_code { get; set; }
            public string status_description { get; set; }
            public string order_no { get; set; }
            public string invoice_id { get; set; }
            public string ref_no { get; set; }
        }


    }
}
