using System.Runtime.Serialization;

namespace StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPaymentValidation
{
    public class IsBankPaymentValidationRequestModel
    {
        public class IsBankPaymentValidationRequest
        {
            [IgnoreDataMember]
            public string apiUrl { get; set; }
            [IgnoreDataMember]
            public string isbank_client_id { get; set; }
            [IgnoreDataMember]
            public string isbank_client_secret { get; set; }
            [IgnoreDataMember]
            public string isbank_client_certificate { get; set; }

            [IgnoreDataMember]
            public string authorization { get; set; }

            [IgnoreDataMember]
            public string username { get; set; }

            [IgnoreDataMember]
            public string password { get; set; }
            public decimal amount { get; set; }
            public string currency = "TRY";
            public string debtor_account_id { get; set; }

            public bool debtor_use_of_overdraft = false;
            public string creditor_name { get; set; }
            public string creditor_iban { get; set; }
            public string description { get; set; }
            public string payment_reference_id { get; set; }
            public string customer_ip = "89.252.138.236";
            public IssuingParty issuing_party { get; set; }
        }

        public class IssuingParty
        {
            public string name { get; set; }
            public string address { get; set; }
            public string legal_id { get; set; }
            public int pisp_account_number { get; set; }
            public string country_city { get; set; }
            public string customer_Number { get; set; }
            public string place_of_birth { get; set; }
        }
    }
}
