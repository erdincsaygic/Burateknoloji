using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.AKODESanalPOS.Models.AKODEGetTransactions
{
    public class AKODEGetTransactionRequestModel
    {
        public string ClientId { get; set; }
        public string ApiUser { get; set; }
        public string Rnd { get; set; }
        public string TimeSpan { get; set; }
        public string Hash { get; set; }
        public int TransactionDate { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string OrderId { get; set; }
    }
}
