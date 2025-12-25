using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.PayNKolay.Models.CancelRefundPayment
{
    public class CancelRefundPaymentRequestModel
    {
        public class CancelPaymentRequestModel
        {
            public string Sx { get; set; }

            public string ResultUrl = "json";
            public string Type = "cancel";
            public string ReferenceCode { get; set; }
            public decimal Amount { get; set; }
            public string HashData { get; set; }
            public string SecretKey { get; set; }
        }

        public class RefundRequestModel
        {
            public string Sx { get; set; }

            public string ResultUrl = "json";
            public string Type = "refund";
            public string ReferenceCode { get; set; }
            public decimal Amount { get; set; }
            public string HashData { get; set; }
            public string TrxDate { get; set; }
            public string SecretKey { get; set; }
        }
    }
}
