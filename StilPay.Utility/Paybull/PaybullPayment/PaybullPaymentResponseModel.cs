using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.Paybull.PaybullPayment
{
    public class PaybullPaymentResponseModel
    {
        public class Root
        {
            public long order_no { get; set; }
            public long order_id { get; set; }
            public string invoice_id { get; set; }
            public int status_code { get; set; }
            public string status_description { get; set; }
            public string credit_card_no { get; set; }
            public string transaction_type { get; set; }
            public int payment_status { get; set; }
            public int payment_method { get; set; }
            public int error_code { get; set; }
            public string error { get; set; }
            public int auth_code { get; set; }
            public int merchant_commission { get; set; }
            public int user_commission { get; set; }
            public int merchant_commission_percentage { get; set; }
            public int merchant_commission_fixed { get; set; }
            public int installment { get; set; }
            public int amount { get; set; }
            public string hash_key { get; set; }
            public int md_status { get; set; }
            public string original_bank_error_code { get; set; }
            public string original_bank_error_description { get; set; }
        }

    }
}
