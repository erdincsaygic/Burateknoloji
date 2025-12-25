using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.KuveytTurk.KuveytTurkTransfer
{
    public class KuveytTurkTransferResponseModel
    {
        public class Error
        {
            public string message { get; set; }
            public string code { get; set; }
        }

        public class Result
        {
            public string errorMessage { get; set; }
            public string errorCode { get; set; }
        }

        public class Value
        {
            public int moneyTransferTransactionId { get; set; }
            public string transferType { get; set; }
            public string executionReferenceId { get; set; }
        }


        public class Root
        {
            public List<Result> results { get; set; }
            public List<Error> errors { get; set; }
            public Value value { get; set; }
            public bool success { get; set; }
            public int http { get; set; }
            public string messageCode { get; set; }
            public string text { get; set; }
            public string developerText { get; set; }
            public string moreInfo { get; set; }
            public string executionReferenceId { get; set; }
            public string message { get; set; }
            public string code { get; set; }
        }
    }
}
