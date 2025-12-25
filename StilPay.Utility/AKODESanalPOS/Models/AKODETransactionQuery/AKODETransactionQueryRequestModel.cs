using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.AKODESanalPOS.Models.AKODETransactionQuery
{
    public class AKODETransactionQueryRequestModel
    {
        public string ClientId { get; set; }
        public string ApiUser { get; set; }
        public string Rnd { get; set; }
        public string TimeSpan { get; set; }
        public string Hash { get; set; }
        public string OrderId { get; set; }
    }
}
