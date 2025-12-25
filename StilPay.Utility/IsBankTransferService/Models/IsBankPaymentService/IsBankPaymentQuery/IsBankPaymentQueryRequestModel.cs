using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPaymentQuery
{
    public class IsBankPaymentQueryRequestModel
    {
        [IgnoreDataMember]
        public string apiUrl { get; set; }
        [IgnoreDataMember]
        public string isbank_client_id { get; set; }
        [IgnoreDataMember]
        public string isbank_client_secret { get; set; }
        [IgnoreDataMember]
        public string isbank_client_certificate { get; set; }

        public string query_number { get; set; }
        public DateTime date { get; set; }
    }
}
