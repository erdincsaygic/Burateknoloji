using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.ToslaSanalPos.Models.ToslaGetRefCode
{
    public class ToslaGetRefCodeResponseModel
    {
        public string processId { get; set; }
        public string refCode { get; set; }
        public bool result { get; set; }
        public string errorMessage { get; set; }
        public int errorCode { get; set; }
    }
}
