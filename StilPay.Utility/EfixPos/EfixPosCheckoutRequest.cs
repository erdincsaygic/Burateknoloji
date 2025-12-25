using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.EfixPos.Models;
using StilPay.Utility.Helper;
using System;
using StilPay.Utility.EfixPos.Models.EfixPosCheckout;


namespace StilPay.Utility.EfixPos
{
    public class EfixPosCheckoutRequest
    {
        public static GenericResponseDataModel<EfixPosCheckoutResponseModel> Checkout(EfixPosCheckoutRequestModel model)
        {
            try
            {
                var tokenResult = EfixPosGetTokenRequest.GetAccessToken();
                var tokenString = tokenResult?.Data?.Token;
                if (string.IsNullOrEmpty(tokenString))
                {
                    return new GenericResponseDataModel<EfixPosCheckoutResponseModel>
                    {
                        Status = "ERROR",
                        Message = tokenResult?.Message ?? "Token alımında hata."
                    };
                }

                var client = new RestClient(new RestClientOptions("https://vpos.efixfatura.com.tr/api/")
                {
                    MaxTimeout = 10000
                });

                var request = new RestRequest("transactions/checkout", Method.Post)
                    .AddHeader("Content-Type", "application/json")
                    .AddHeader("Authorization", $"Bearer {tokenString}")
                    .AddJsonBody(model);

                var response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    var resp = JsonConvert.DeserializeObject<EfixPosCheckoutResponseModel>(response.Content);

                    return new GenericResponseDataModel<EfixPosCheckoutResponseModel>
                    {
                        Status = string.IsNullOrEmpty(resp.ErrorMessage) && string.IsNullOrEmpty(resp.ErrorCode) && !string.IsNullOrEmpty(resp.Url3ds) ? "OK" : "ERROR",
                        Data = resp
                    };
                }
                else
                {
                    EfixPosErrorResponseModel err = null;
                    try
                    {
                        err = JsonConvert.DeserializeObject<EfixPosErrorResponseModel>(response.Content);
                    }
                    catch { }

                    return new GenericResponseDataModel<EfixPosCheckoutResponseModel>
                    {
                        Status = "ERROR",
                        Message = err?.Message
                                  ?? $"HTTP {(int)response.StatusCode} {response.StatusDescription}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<EfixPosCheckoutResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message
                };
            }
        }
    }
}
