using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace StilPay.Utility.EfixPos.Models.EfixPosGetToken
{ 
    public class EfixPosGetTokenRequestModel
    {
        [JsonProperty("apiKey")]
        public string ApiKey { get; set; }

        [JsonProperty("apiSecret")]
        public string ApiSecret { get; set; }
    }
}
