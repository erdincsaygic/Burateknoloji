using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.Paybull.PaybullPayment
{
    public class PaybullPaymentRequestModel
    {
        public string cc_holder_name { get; set; }
        public string cc_no { get; set; }
        public string expiry_month { get; set; }
        public string expiry_year { get; set; }
        public string cvv { get; set; }
        public string currency_code { get; set; }
        public int installments_number { get; set; }
        public string invoice_id { get; set; }
        public string invoice_description { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public decimal total { get; set; }
        public string merchant_key { get; set; }
        public string items { get; set; }

        //public string return_url = "http://localhost:63352/panel/paymentnotification/PaybullThreeDSecureResult";

        //public string cancel_url = "http://localhost:63352/panel/paymentnotification/PaybullThreeDSecureResult";

        public string return_url = "https://stilpay.com/panel/paymentnotification/PaybullThreeDSecureResult";

        public string cancel_url = "https://stilpay.com/panel/paymentnotification/PaybullThreeDSecureResult";
        public string hash_key { get; set; }
    }
}
