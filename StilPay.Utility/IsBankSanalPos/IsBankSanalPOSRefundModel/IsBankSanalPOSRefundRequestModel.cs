using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.IsBankSanalPos.IsBankSanalPOSRefundModel
{
    public class IsBankSanalPOSRefundRequestModel
    {
        public string apiUserName { get; set; }
        public string apiUserPassword  { get; set; }
        public string clientid { get; set; }

        public string type = "Credit";
        public string oid { get; set; }
    }
}
