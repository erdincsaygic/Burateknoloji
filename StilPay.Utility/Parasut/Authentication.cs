using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Parasut.Models;
using System;

namespace StilPay.Utility.Parasut
{
    public class Authentication
    {
        public static ResponseModel<TokenResponse> GetAccessToken(AuthModel acT, string baseUrl)
        {
            var result = new ResponseModel<TokenResponse>();
            try
            {
                var client = new RestClient(baseUrl + "/oauth/token");
                var request = new RestRequest();
                request.Method = Method.Post;
                request.AddHeader("Accept", "application/json");

                var jsonString = JsonConvert.SerializeObject(acT);
                request.AddStringBody(jsonString, DataFormat.Json);

                RestResponse response = client.ExecuteAsync(request).Result as RestResponse;

                if (response.IsSuccessStatusCode == true)
                {
                    TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(response.Content);
                    
                    result.Status = true;
                    result.Data = tokenResponse;
                }
                else
                {
                    result.Status = false;
                    result.Message = response.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message;
            }
            
            return result;
        }
    }
}
