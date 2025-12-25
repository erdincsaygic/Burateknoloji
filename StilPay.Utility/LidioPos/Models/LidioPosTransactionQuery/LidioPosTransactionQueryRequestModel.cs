using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.LidioPos.Models.LidioPosTransactionQuery
{
    public class LidioPosTransactionQueryRequestModel
    {
        public string orderId { get; set; }
        public decimal totalAmount   { get; set; }
        public string paymentInstrument { get; set; }
        public PaymentInquiryInstrumentInfo paymentInquiryInstrumentInfo { get; set; }
    }

    public class PaymentInquiryInstrumentInfo
    {
        public PaymentInquiryCard Card { get; set; }
    }

    public class PaymentInquiryCard
    {
        public string ProcessType { get; set; }
    }
}
