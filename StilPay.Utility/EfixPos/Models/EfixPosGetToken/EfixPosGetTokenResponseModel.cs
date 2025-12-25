using System.Text.Json.Serialization;

namespace StilPay.Utility.EfixPos.Models.EfixPosGetToken
{
    public class EfixPosGetTokenResponseModel
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
