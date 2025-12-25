using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.KuveytTurk.KuveytTurkAccountTransaction
{
    public class KuveytTurkAccountTransactionRequestModel
    {
        public string Authorization { get; set; }
        public string Signature { get; set; }
        public string url { get; set; }
    }
}
