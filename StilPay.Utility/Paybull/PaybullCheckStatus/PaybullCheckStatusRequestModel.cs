using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.Paybull.PaybullCheckStatus
{
    public class PaybullCheckStatusRequestModel
    {
        public string merchant_key { get; set; }
        public string invoice_id { get; set; }
        public bool include_pending_status { get; set; }      
    }
}
