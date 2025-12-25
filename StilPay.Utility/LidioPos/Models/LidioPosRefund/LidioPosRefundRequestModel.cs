using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.LidioPos.Models.LidioPosRefund
{
    public class LidioPosRefundRequestModel
    {
        public string orderId { get; set; }
        public decimal totalAmount { get; set; }

        public string currency { get; set; }
    }
}
   
