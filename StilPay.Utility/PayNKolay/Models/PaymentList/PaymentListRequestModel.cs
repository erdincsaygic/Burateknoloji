using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.PayNKolay.Models.PaymentList
{
    public class PaymentListRequestModel
    {
        public string Sx { get; set; }
        public string SecretKey  { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string HashData { get; set; }
        
    }
}
