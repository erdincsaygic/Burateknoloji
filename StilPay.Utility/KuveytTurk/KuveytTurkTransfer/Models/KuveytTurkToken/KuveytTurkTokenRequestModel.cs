using System;
using System.Collections.Generic;
using System.ServiceModel.Description;
using System.Text;

namespace StilPay.Utility.KuveytTurk.KuveytTurkTransfer.Models.KuveytTurkToken
{
    public class KuveytTurkTokenRequestModel
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }
}
