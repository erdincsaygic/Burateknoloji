using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPaymentQuery
{
    public class IsBankPaymentQueryResponseModel
    {
        public class IsBankPaymentQueryResponse
        {
            public Data data { get; set; }
            public Error errors { get; set; }
        }

        public class Data
        {
            public double amount { get; set; }
            public string debtor_account_name { get; set; }
            public string creditor_account_name { get; set; }
            public string query_number { get; set; }
            public string state { get; set; }
        }

        public class Error
        {
            public string code { get; set; }
            public string message { get; set; }
        }
    }
}
