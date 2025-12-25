using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Job.KuveytTurk.Models
{
    internal class KuveytTurkTokenModel
    {
        public string transaction_url { get; set; }
        public string grant_type { get; set; }
        public string scope { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }
}
