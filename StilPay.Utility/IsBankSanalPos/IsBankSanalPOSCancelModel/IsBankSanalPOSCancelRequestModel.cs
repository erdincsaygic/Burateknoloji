using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.IsBankSanalPos.IsBankSanalPOSCancelModel
{
    public class IsBankSanalPOSCancelRequestModel
    {
        public string apiUserName { get; set; }
        public string apiUserPassword  { get; set; }
        public string clientid { get; set; }

        public string type = "Void";
        public string oid { get; set; }
    }
}
