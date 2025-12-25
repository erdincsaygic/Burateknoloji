using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.ToslaSanalPos.Models.ToslaRefund
{
    public class ToslaRefundRequestModel
    {
        public string phoneNumber { get; set; }
        public string processId { get; set; }
        public int companyId { get; set; }
        public string externalTransactionId { get; set; }
        public decimal amount { get; set; }
        public string description { get; set; }
    }
}
