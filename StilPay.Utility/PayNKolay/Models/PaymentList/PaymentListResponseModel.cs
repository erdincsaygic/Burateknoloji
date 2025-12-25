using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.PayNKolay.Models.PaymentList
{
    public class PaymentListResponseModel
    {
        public class PaymentListResponse
        {
            public string id { get; set; }
            public Result result { get; set; }

            public string error { get; set; }
        }

        public class Result
        {
            public int? RESPONSE_CODE { get; set; }
            public string CORE_TRX_ID_RESERVED { get; set; }
            public string RESPONSE_DATA { get; set; }
            public string sessionId { get; set; }
            public List<LIST> LIST { get; set; }
        }

        public class LIST
        {
            public string REFERENCE_CODE { get; set; }
            public string AUTH_CODE { get; set; }
            public string COMMISION { get; set; }
            public string AUTHORIZATION_AMOUNT { get; set; }
            public string OID { get; set; }
            public string TRANSACTION_TYPE { get; set; }
            public bool IS_3D { get; set; }
            public string MERCHANT_COMMISSION_AMOUNT { get; set; }
            public string TRANSACTION_AMOUNT { get; set; }
            public string CARD_HOLDER_NAME { get; set; }
            public string CLIENT_REFERENCE_CODE { get; set; }
            public string STATUS { get; set; }
            public string TRX_DATE { get; set; }
            public string DESCRIPTION { get; set; }
            public string INSTALLMENT_COUNT { get; set; }
            public string BANK_RESULT { get; set; }
            public string BANK_NAME { get; set; }
        }    
    }
}
