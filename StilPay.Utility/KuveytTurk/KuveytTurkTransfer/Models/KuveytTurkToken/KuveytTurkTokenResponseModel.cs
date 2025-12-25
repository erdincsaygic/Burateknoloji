using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.KuveytTurk.KuveytTurkTransfer.Models.KuveytTurkToken
{
    public class KuveytTurkTokenResponseModel
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
    }
}
