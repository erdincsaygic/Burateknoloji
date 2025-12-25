using StilPay.Utility.NomuPayPos.Models.NomuPayPaymentRequest;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.NomuPayPos.Models.NomuPayBinQuery
{
    public class NomuPayBinQueryRequestModel
    {
        public string ServiceType = "MerchantQueries";
        public string OperationType = "BinQueryOperation";
        public string Bin { get; set; }
        public Token Token { get; set; }
    }
}
