using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.AKODESanalPOS.Models.AKODEGetSession
{
    public class AKODEGetSessionResponseModel
    {
        public int code { get; set; }
        public string message { get; set; }
        public string ThreeDSessionId { get; set; }
        public string transactionId { get; set; }
    }
}
