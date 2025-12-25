using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.ToslaSanalPos.Models.ToslaGetRefCode
{
    public class ToslaGetRefCodeRequestModel
    {
        public int companyId { get; set; }
        public string processId { get; set; }
        public string phoneNumber { get; set; }
        public decimal amount { get; set; }
    }
}
