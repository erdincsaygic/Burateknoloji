using StilPay.Utility.Parasut;
using System;
using System.Collections.Generic;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace StilPay.Utility.EsnekPos.Models.EsnekPosPaymentRequest
{
    public class EsnekPosPaymentRequestModel
    {
        public Config Config { get; set; }
        public CreditCard CreditCard { get; set; }
        public Customer Customer { get; set; }
        public Product Product { get; set; }

    }
    public class Config
    {
        public string MERCHANT { get; set; }
        public string MERCHANT_KEY { get; set; }

        //public string BACK_URL = "http://localhost:63352/panel/paymentnotification/EsnekPosThreeDSecureResult";
        public string BACK_URL = "https://burateknoloji.com/panel/paymentnotification/EsnekPosThreeDSecureResult";
        public string PRICES_CURRENCY = "TRY";
        public string ORDER_REF_NUMBER { get; set; }
        public string ORDER_AMOUNT { get; set; }
    }

    public class CreditCard
    {
        public string CC_NUMBER { get; set; }
        public string EXP_MONTH { get; set; }
        public string EXP_YEAR { get; set; }
        public string CC_CVV { get; set; }
        public string CC_OWNER { get; set; }
        public string INSTALLMENT_NUMBER { get; set; }
    }

    public class Customer
    {
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string MAIL { get; set; }
        public string PHONE { get; set; }
        public string CITY { get; set; }
        public string STATE { get; set; }
        public string ADDRESS { get; set; }
        public string CLIENT_IP { get; set; }
    }

    public class Product
    {
        public string PRODUCT_ID { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string PRODUCT_CATEGORY { get; set; }
        public string PRODUCT_DESCRIPTION { get; set; }
        public string PRODUCT_AMOUNT { get; set; }
    }
}
