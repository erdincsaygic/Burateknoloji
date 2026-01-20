using Newtonsoft.Json;

namespace StilPay.Utility.EfixPos.Models.EfixPosCheckout
{
    public class EfixPosCheckoutRequestModel
    {
        
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; } = "TR";

        [JsonProperty("successUrl")]
        public string SuccessUrl { get; set; } = "https://burateknoloji.com/panel/paymentnotification/EfixPosThreeDSecureResult";

        [JsonProperty("cancelUrl")]
        public string CancelUrl { get; set; } = "https://burateknoloji.com/panel/paymentnotification/EfixPosThreeDSecureResult";

        [JsonProperty("declineUrl")]
        public string DeclineUrl { get; set; } = "https://burateknoloji.com/panel/paymentnotification/EfixPosThreeDSecureResult";


        //[JsonProperty("successUrl")]
        //public string SuccessUrl { get; set; } = "http://localhost:63352/panel/paymentnotification/EfixPosThreeDSecureResult";

        //[JsonProperty("cancelUrl")]
        //public string CancelUrl { get; set; } = "http://localhost:63352/panel/paymentnotification/EfixPosThreeDSecureResult";

        //[JsonProperty("declineUrl")]
        //public string DeclineUrl { get; set; } = "http://localhost:63352/panel/paymentnotification/EfixPosThreeDSecureResult";


        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; } = "TRY";

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("memberId")]
        public string MemberId { get; set; }

        [JsonProperty("additionalInformation")]
        public string AdditionalInformation { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("clientName")]
        public string ClientName { get; set; }

        [JsonProperty("cardNumber")]
        public string CardNumber { get; set; }

        [JsonProperty("cvv")]
        public string Cvv { get; set; }

        [JsonProperty("cardExpireDate")]
        public string CardExpireDate { get; set; }

        [JsonProperty("cardOwner")]
        public string CardOwner { get; set; }

        [JsonProperty("merchantId")]
        public string MerchantId { get; set; }

        [JsonProperty("pan")]
        public string Pan { get; set; }

        [JsonProperty("expiryDate")]
        public string ExpiryDate { get; set; }

        [JsonProperty("sessionInfo")]
        public string SessionInfo { get; set; }

        [JsonProperty("installmentCount")]
        public int InstallmentCount { get; set; } = 1;

        [JsonProperty("subMerchantId")]
        public string SubMerchantId { get; set; }

        [JsonProperty("ad")]
        public string Ad { get; set; }

        [JsonProperty("soyad")]
        public string Soyad { get; set; }

        [JsonProperty("projeId")]
        public int ProjeId { get; set; }

        [JsonProperty("vergiKimlikNumarasi")]
        public string VergiKimlikNumarasi { get; set; }

        [JsonProperty("tuzelKisiUnvan")]
        public string TuzelKisiUnvan { get; set; }
    }
}
