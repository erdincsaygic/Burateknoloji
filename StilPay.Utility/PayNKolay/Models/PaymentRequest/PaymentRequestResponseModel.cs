using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.PayNKolay.Models.PaymentRequest
{
    public class PaymentRequestResponseModel
    {
        public class PaymentRequestResponse
        {
            public string USE_3D { get; set; }
            public string BANK_REQUEST_MESSAGE { get; set; }
            public string REFERENCE_CODE { get; set; }
            public string AUTH_CODE { get; set; }
            public string COMMISION { get; set; }
            public string COMMISION_RATE { get; set; }
            public string INSTALLMENT { get; set; }
            public string AUTHORIZATION_AMOUNT { get; set; }
            public string BANK_CODE { get; set; }
            public string TRANSACTION_AMOUNT { get; set; }
            public string MERCHANT_NO { get; set; }
            public string CLIENT_REFERENCE_CODE { get; set; }
            public string BANK_RESULT { get; set; }
            public string BANK_MESSAGE { get; set; }
            public string HTML_STRING { get; set; }
            public int RESPONSE_CODE { get; set; }
            public string ERROR_CODE { get; set; }
            public string RESPONSE_DATA { get; set; }
            public string sessionId { get; set; }
            public string CORE_TRX_ID_RESERVED { get; set; }
            public string ERROR_MESSAGE { get; set; }
            public string TimeStamp { get; set; }
        }
    }
}
