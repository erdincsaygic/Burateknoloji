using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Dto
{
    public class AutoCallbackService
    {
        public string TransactionID { get; set; }
        public int TransactionType { get; set; }
        public DateTime CDate { get; set; }
        public int ResponseStatus { get; set; }
        public string Callback { get; set; }
    }
}
