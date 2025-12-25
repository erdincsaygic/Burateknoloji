using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.AKODESanalPOS.Models.AKODEPaymentRequest
{
    public class AKODEPaymentResponseModel
    {
        public string ClientId { get; set; }
        public string OrderId { get; set; }
        public int MdStatus { get; set; }
        public string ThreeDSessionId { get; set; }
        public string BankResponseCode { get; set; }
        public string BankResponseMessage { get; set; }
        public int RequestStatus { get; set; }
        public string HashParameters { get; set; }
        public string Hash { get; set; }
    }
}
