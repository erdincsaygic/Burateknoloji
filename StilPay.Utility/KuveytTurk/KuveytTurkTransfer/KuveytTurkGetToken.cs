using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.KuveytTurk.KuveytTurkTransfer.Models.KuveytTurkToken;
using System;

namespace StilPay.Utility.KuveytTurk.KuveytTurkTransfer
{
    public class KuveytTurkGetToken
    {
        public static GenericResponseDataModel<KuveytTurkTokenResponseModel> GetAccessToken(KuveytTurkTokenRequestModel kuveytTurkTokenRequestModel)
        {
            try
            {
                var options = new RestClientOptions("https://prep-identity.kuveytturk.com.tr")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/connect/token", Method.Post);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("grant_type", "client_credentials");
                request.AddParameter("scope", "accounts transfers");
                request.AddParameter("client_id", kuveytTurkTokenRequestModel.client_id);
                request.AddParameter("client_secret", kuveytTurkTokenRequestModel.client_secret);
                var response = client.Execute(request);
 
                var deserialize = JsonConvert.DeserializeObject<KuveytTurkTokenResponseModel>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<KuveytTurkTokenResponseModel>
                    {
                        Status = !string.IsNullOrEmpty(deserialize.access_token) ? "OK" : "ERROR",
                        Data = deserialize
                    };
                }
                else
                {
                    return new GenericResponseDataModel<KuveytTurkTokenResponseModel>
                    {
                        Status = "ERROR",
                        Message = "Token Alımında Bir Hata Meydana Geldi"
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<KuveytTurkTokenResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
