using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.AKODESanalPOS.Models.AKODERefund
{
    public class AKODERefundRequestModel
    {
        public string OrderId { get; set; }
        public long Amount { get; set; }
        public string ClientId { get; set; }
        public string ApiUser { get; set; }
        public string Rnd { get; set; }
        public string TimeSpan { get; set; }
        public string Hash { get; set; }
    }
}
