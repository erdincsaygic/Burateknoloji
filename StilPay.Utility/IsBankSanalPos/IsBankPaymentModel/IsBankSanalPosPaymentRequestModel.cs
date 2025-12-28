using DocumentFormat.OpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.IsBankSanalPos.IsBankPaymentModel
{
    public class IsBankSanalPosPaymentRequestModel
    {
        public string clientid { get; set; }

        public string storetype = "3d";
        public string hash { get; set; }

        public string islemtipi = "Auth";
        public string amount { get; set; }

        public string currency = "949";
        public string oid { get; set; }

        //public string okUrl = "http://localhost:63352/panel/paymentnotification/IsBankThreeDSecureResult";

        //public string failUrl = "http://localhost:63352/panel/paymentnotification/IsBankThreeDSecureResult";

        public string okUrl = "https://burateknoloji.com/panel/paymentnotification/IsBankThreeDSecureResult";

        public string failUrl = "https://burateknoloji.com/panel/paymentnotification/IsBankThreeDSecureResult";

        public string lang = "tr";
        public string pan { get; set; }
        public string Ecom_Payment_Card_ExpDate_Year { get; set; }
        public string Ecom_Payment_Card_ExpDate_Month { get; set; }

        public string hashAlgorithm = "ver3";
    }
}
