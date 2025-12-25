using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.ToslaSanalPos.Models.ToslaGetToken;
using System;

namespace StilPay.Utility.ToslaSanalPos
{
    public class ToslaGetTokenRequest
    {
        public static GenericResponseDataModel<ToslaGetTokenResponseModel> GetToken(ToslaGetTokenRequestModel toslaGetTokenRequest)
        {
            try
            {
                var options = new RestClientOptions("https://api.tosla.com")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/api/auth/oauth/token", Method.Post);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Authorization", "Basic " + toslaGetTokenRequest.BasicAuthBase64);
                request.AddParameter("grant_type", "client_credentials");
                var response = client.Execute(request);

                if (response.IsSuccessStatusCode)
                {
                    var deserialize = JsonConvert.DeserializeObject<ToslaGetTokenResponseModel>(response.Content);

                    return new GenericResponseDataModel<ToslaGetTokenResponseModel>
                    {
                        Status = !string.IsNullOrEmpty(deserialize.access_token) ? "OK" : "ERROR",
                        Data = deserialize
                    };
                }
                else
                {
                    JObject jsonObject = JObject.Parse(response.Content);

                    return new GenericResponseDataModel<ToslaGetTokenResponseModel>
                    {
                        Status = "ERROR",
                        Message = jsonObject["error_description"].ToString(),
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<ToslaGetTokenResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
