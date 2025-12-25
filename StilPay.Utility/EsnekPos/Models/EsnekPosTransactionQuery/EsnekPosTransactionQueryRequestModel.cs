using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.EsnekPos.Models.EsnekPosTransactionQuery
{
    public class EsnekPosTransactionQueryRequestModel
    {
        public string MERCHANT { get; set; }
        public string MERCHANT_KEY { get; set; }
        public string ORDER_REF_NUMBER { get; set; }
    }
}
