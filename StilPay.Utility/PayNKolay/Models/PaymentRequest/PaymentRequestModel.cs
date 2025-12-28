using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.PayNKolay.Models.PaymentRequest
{
    public class PaymentRequestModel
    {
        public string SenderName { get; set; }
        public string CardNumber { get; set; }
        public string ExpirationDateMonth { get; set; }
        public string ExpirationDateYear { get; set; }
        public string SecurityCode { get; set; }
        public string CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public string InstallmentMonth { get; set; }
        public decimal InstallmentAmount { get; set; }
        public string UCD_URL { get; set; }
        public string Description { get; set; }
        public string EncodedValue { get; set; }
        public string HashData { get; set; }
        public string Rnd { get; set; }
        public string ClientRefCode { get; set; }

        public string SuccessUrl = "https://burateknoloji.com/panel/paymentnotification/paynkolaythreedsecureresult";
        public string FailUrl = "https://burateknoloji.com/panel/paymentnotification/paynkolaythreedsecureresult";

        //public string SuccessUrl = "http://localhost:63352/panel/paymentnotification/paynkolaythreedsecureresult";
        //public string FailUrl = "http://localhost:63352/panel/paymentnotification/paynkolaythreedsecureresult";

        public string Use3D = "true";
        public string Sx { get; set; }
        public string MerchantSecretKey { get; set; }
    }
}
