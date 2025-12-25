using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.AKODESanalPOS.Models.AKODEPaymentRequest
{
    public class AKODEPaymentRequestModel
    {
        public string ThreeDSessionId { get; set; }
        public string CardHolderName { get; set; }
        public string CardNo { get; set; }
        public string ExpireDate { get; set; }
        public string Cvv { get; set; }
    }
}
