using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.NomuPayPos.Models.NomuPayBinQuery
{
    public class NomuPayPosBinQueryRequestResponseModel
    {
        public int StatusCode { get; set; }
        public string InstallmentEnabled { get; set; }
        public string ResultCode { get; set; }
        public string ResultMessage { get; set; }
        public string CardType { get; set; }
        public bool IsCorporate { get; set; }
        public bool IsVirtual { get; set; }
        public string Network { get; set; }
        public int BankCode { get; set; }
        public string BankName { get; set; }
    }
}
