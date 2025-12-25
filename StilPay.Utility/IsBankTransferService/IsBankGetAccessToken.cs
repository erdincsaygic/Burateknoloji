using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.IsBankTransferService.IsBankTokenModel;
using StilPay.Utility.IsBankTransferService.Models.IsBankToken;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.IsBankTransferService
{
    public class IsBankGetAccessToken
    {
        public static GenericResponseDataModel<IsBankTokenResponseModel> GetAccessToken(IsBankTokenRequestModel isBankTokenRequestModel)
        {
            try
            {
                var options = new RestClientOptions("https://api.isbank.com.tr");
                var client = new RestClient(options);
                var request = new RestRequest("/api/isbank/v1/stos-identity-provider/oauth2/token", Method.Post);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("Authorization", isBankTokenRequestModel.Authorization);
                request.AddParameter("scope", "read:accounts transfer:eft transfer:remittance transfer:fast");
                request.AddParameter("username", isBankTokenRequestModel.username);
                request.AddParameter("password", isBankTokenRequestModel.password);
                request.AddParameter("client_id", isBankTokenRequestModel.client_id);
                request.AddParameter("client_secret", isBankTokenRequestModel.client_secret);
                request.AddParameter("grant_type", "password");
                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<IsBankTokenResponseModel>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<IsBankTokenResponseModel>
                    {
                        Status = !string.IsNullOrEmpty(deserialize.access_token) ? "OK" : "ERROR",
                        Data = deserialize
                    };
                }
                else
                {
                    return new GenericResponseDataModel<IsBankTokenResponseModel>
                    {
                        Status = "ERROR",
                        Message = "Token Alımında Bir Hata Meydana Geldi"
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<IsBankTokenResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
