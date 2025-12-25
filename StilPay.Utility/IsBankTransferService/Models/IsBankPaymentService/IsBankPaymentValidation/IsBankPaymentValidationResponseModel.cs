using System.Collections.Generic;

namespace StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPaymentValidation
{
    public class IsBankPaymentValidationResponseModel
    {
        public class IsBankPaymentValidationResponse
        {
            public Data data { get; set; }
            public List<Error> errors { get; set; }
            public List<Info> infos { get; set; }
        }

        public class Data
        {
            public bool result { get; set; }
            public double amount { get; set; }
            public double fee_amount { get; set; }
            public double fee_tax_amount { get; set; }
            public string debtor_account_name { get; set; }
            public string creditor_account_name { get; set; }
            public double source_account_amount { get; set; }
            public int overdraft_amount { get; set; }
            public int fund_801_amount { get; set; }
            public int fund_808_amount { get; set; }
            public int fund_818_amount { get; set; }
            public int fund_total_amount { get; set; }
            public int maximum_term_account_amount { get; set; }
            public int invesment_account_amount { get; set; }
            public int invesment_total_amount { get; set; }
            public string payment_reference_id { get; set; }
            public string query_number { get; set; }
        }

        public class Error
        {
            public string code { get; set; }
            public string message { get; set; }
        }

        public class Info
        {
            public string code { get; set; }
            public string message { get; set; }
        }
    }
}
