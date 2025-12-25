using Newtonsoft.Json;

namespace StilPay.Utility.EfixPos.Models.EfixPosCheckout
{
    public class EfixPosCheckoutResponseModel
    {
        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("transactionType")]
        public string TransactionType { get; set; }

        [JsonProperty("transactionId")]
        public long TransactionId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("url3ds")]
        public string Url3ds { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("actionUrl")]
        public string ActionUrl { get; set; }
    }
}
