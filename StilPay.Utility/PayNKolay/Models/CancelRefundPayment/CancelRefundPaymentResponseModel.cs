using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.PayNKolay.Models.CancelRefundPayment
{
    public class CancelRefundPaymentResponseModel
    {
        public string REFUND_DESCRIPTION { get; set; }
        public string CLIENT_REFERENCE_CODE { get; set; }
        public int RESPONSE_CODE { get; set; }
        public string ERROR_CODE { get; set; }
        public string RESPONSE_DATA { get; set; }
        public string sessionId { get; set; }
        public string CORE_TRX_ID_RESERVED { get; set; }
        public string ERROR_MESSAGE { get; set; }
        public string TimeStamp { get; set; }
    }
}
