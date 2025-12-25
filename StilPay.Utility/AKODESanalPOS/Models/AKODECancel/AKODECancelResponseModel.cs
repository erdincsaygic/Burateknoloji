using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.AKODESanalPOS.Models.AKODECancel
{
    public class AKODECancelResponseModel
    {
        public string OrderId { get; set; }
        public string BankResponseCode { get; set; }
        public string BankResponseMessage { get; set; }
        public string AuthCode { get; set; }
        public string HostReferenceNumber { get; set; }
        public string TransactionId { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
