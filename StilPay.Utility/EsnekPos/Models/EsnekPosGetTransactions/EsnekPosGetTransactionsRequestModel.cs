using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.EsnekPos.Models.EsnekPosGetTransactions
{
    public class EsnekPosGetTransactionsRequestModel
    {
        public string MERCHANT { get; set; }
        public string MERCHANT_KEY { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
    }
}
