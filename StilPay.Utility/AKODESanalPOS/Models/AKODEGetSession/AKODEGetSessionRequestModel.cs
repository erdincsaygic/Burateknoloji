using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.AKODESanalPOS.Models.AKODEGetSession
{
    public class AKODEGetSessionRequestModel
    {
        public string clientId { get; set; }
        public string apiUser { get; set; }
        public string Rnd { get; set; }
        public string timeSpan { get; set; }
        public string Hash { get; set; }

        //public string callbackUrl = "http://localhost:63352/panel/paymentnotification/AKODEThreeDSecureResult";
        public string callbackUrl = "https://burateknoloji.com/panel/paymentnotification/AKODEThreeDSecureResult";

        public string orderId { get; set; }
        public long amount { get; set; }
        public int currency = 949;
        public int installmentCount = 0;

    }
}
