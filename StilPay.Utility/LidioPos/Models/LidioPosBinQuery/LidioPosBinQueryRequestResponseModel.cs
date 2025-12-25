using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.LidioPos.Models.LidioPosBinQuery
{
    public class LidioPosBinQueryRequestResponseModel
    {
        public string result { get; set; }
        public string resultMessage { get; set; }
        public string bankCode { get; set; }
        public string cardType { get; set; }
        public bool isDebitCard { get; set; }
        public string cardProgramName { get; set; }
        public bool isBusinessCard { get; set; }
    }

}
