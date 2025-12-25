using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Job.TangoKuveytturk.Models
{
    public class KuveytTurkTransactionModel
    {
        public class TokenExpiredError
        {
            public int code { get; set; }
            public string message { get; set; }
        }

        public class AccountActivity
        {
            public int suffix { get; set; }
            public DateTime date { get; set; }
            public string description { get; set; }
            public double amount { get; set; }
            public double balance { get; set; }
            public string fxCode { get; set; }
            public string transactionReference { get; set; }
            public string transactionCode { get; set; }
            public string senderIdentityNumber { get; set; }
            public string resourceCode { get; set; }
            public string iban { get; set; }
            public string businessKey { get; set; }
        }

        public class Root
        {
            public Value value { get; set; }
            public List<object> results { get; set; }
            public List<object> errors { get; set; }
            public bool success { get; set; }
            public string executionReferenceId { get; set; }
        }

        public class Value
        {
            public List<AccountActivity> accountActivities { get; set; }
        }
    }
}
