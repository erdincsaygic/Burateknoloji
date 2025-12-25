using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.ToslaSanalPos.Models.ToslaGetRefCodeStatus
{
    public class ToslaGetRefCodeStatusRequestModel
    {
        public string processId { get; set; }
        public string phoneNumber { get; set; }
        public int companyId { get; set; }
    }
}
