using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.EfixPos.Models.EfixPosCheckout;
using StilPay.Utility.EfixPos.Models;
using StilPay.Utility.Helper;
using System;
using StilPay.Utility.EfixPos.Models.EfixGetTransaction;

namespace StilPay.Utility.EfixPos
{
    public class EfixPosTransactionDetailRequest
    {
        public static GenericResponseDataModel<EfixPosGetTransactionDetailResponseModel> GetDetail(string clientOrderId)
        {
            try
            {
                var tokenResult = EfixPosGetTokenRequest.GetAccessToken();
                var tokenString = tokenResult?.Data?.Token;
                if (string.IsNullOrEmpty(tokenString))
                {
                    return new GenericResponseDataModel<EfixPosGetTransactionDetailResponseModel>
                    {
                        Status = "ERROR",
                        Message = tokenResult?.Message ?? "Token alımında hata."
                    };
                }

                var client = new RestClient(new RestClientOptions("https://vpos.efixfatura.com.tr/api/")
                {
                    MaxTimeout = 10000
                });

                var request = new RestRequest("transactions/details", Method.Get)
                    .AddHeader("Content-Type", "application/json")
                    .AddHeader("Authorization", $"Bearer {tokenString}")
                    .AddParameter("clientOrderId", clientOrderId);

                var response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    var resp = JsonConvert.DeserializeObject<EfixPosGetTransactionDetailResponseModel>(response.Content);

                    return new GenericResponseDataModel<EfixPosGetTransactionDetailResponseModel>
                    {
                        Status =  "OK",
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

                    return new GenericResponseDataModel<EfixPosGetTransactionDetailResponseModel>
                    {
                        Status = "ERROR",
                        Message = err?.Message
                                  ?? $"HTTP {(int)response.StatusCode} {response.StatusDescription}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<EfixPosGetTransactionDetailResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message
                };
            }
        }
    }
}
