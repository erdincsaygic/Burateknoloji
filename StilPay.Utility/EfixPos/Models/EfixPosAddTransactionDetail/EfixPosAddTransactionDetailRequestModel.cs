using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.EfixPos.Models.EfixPosAddTransactionDetail
{
    public class EfixPosAddTransactionDetailRequestModel
    {
        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; }

        [JsonProperty("totalAmount")]
        public decimal TotalAmount { get; set; }

        [JsonProperty("successUrl")]
        public string SuccessUrl { get; set; }

        [JsonProperty("cancelUrl")]
        public string CancelUrl { get; set; }

        [JsonProperty("declineUrl")]
        public string DeclineUrl { get; set; }

        [JsonProperty("externalStatusCode")]
        public string ExternalStatusCode { get; set; }

        [JsonProperty("externalStatusDesc")]
        public string ExternalStatusDesc { get; set; }

        [JsonProperty("authCode")]
        public string AuthCode { get; set; }
    }
}
