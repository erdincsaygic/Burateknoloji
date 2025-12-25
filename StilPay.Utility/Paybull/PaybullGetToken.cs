using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.Paybull.PaybullToken;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.Paybull
{
    public class PaybullGetToken
    {
        public static GenericResponseDataModel<PaybullTokenResponseModel.Root> GetAccessToken(PaybullTokenRequestModel paybullTokenRequestModel)
        {
            try
            {
                var options = new RestClientOptions("https://app.paybull.com");
                var client = new RestClient(options);
                var request = new RestRequest("/ccpayment/api/token", Method.Post);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("app_id", paybullTokenRequestModel.app_id);
                request.AddParameter("app_secret", paybullTokenRequestModel.app_password);
                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<PaybullTokenResponseModel.Root>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<PaybullTokenResponseModel.Root>
                    {
                        Status = !string.IsNullOrEmpty(deserialize.data.token) ? "OK" : "ERROR",
                        Data = deserialize
                    };
                }
                else
                {
                    return new GenericResponseDataModel<PaybullTokenResponseModel.Root>
                    {
                        Status = "ERROR",
                        Message = "Token Alımında Bir Hata Meydana Geldi"
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<PaybullTokenResponseModel.Root>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
