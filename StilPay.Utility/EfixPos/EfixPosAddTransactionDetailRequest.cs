using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.EfixPos.Models;
using StilPay.Utility.EfixPos.Models.EfixPosAddTransactionDetail;
using StilPay.Utility.Helper;
using System;

namespace StilPay.Utility.EfixPos
{
    public class EfixPosAddTransactionDetailRequest
    {
        public static GenericResponseDataModel<EfixPosAddTransactionDetailResponseModel> AddTransactionDetail(EfixPosAddTransactionDetailRequestModel model)
        {
            try
            {
                var tokenResult = EfixPosGetTokenRequest.GetAccessToken();
                var tokenString = tokenResult?.Data?.Token;
                if (string.IsNullOrEmpty(tokenString))
                {
                    return new GenericResponseDataModel<EfixPosAddTransactionDetailResponseModel>
                    {
                        Status = "ERROR",
                        Message = tokenResult?.Message ?? "Token alımında hata."
                    };
                }

                var client = new RestClient(new RestClientOptions("https://vpos.efixfatura.com.tr/api/")
                {
                    MaxTimeout = 10000
                });

                var request = new RestRequest("transactions/add-transactions-detail", Method.Post)
                    .AddHeader("Content-Type", "application/json")
                    .AddHeader("Authorization", $"Bearer {tokenString}")
                    .AddJsonBody(model);

                var response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    var resp = JsonConvert.DeserializeObject<bool>(response.Content);
                    return new GenericResponseDataModel<EfixPosAddTransactionDetailResponseModel>
                    {
                        Status = resp? "OK" : "ERROR",
                        Data =  new EfixPosAddTransactionDetailResponseModel { Value = resp}
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

                    return new GenericResponseDataModel<EfixPosAddTransactionDetailResponseModel>
                    {
                        Status = "ERROR",
                        Message = err?.Message
                                  ?? $"HTTP {(int)response.StatusCode} {response.StatusDescription}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<EfixPosAddTransactionDetailResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message
                };
            }
        }
    }
}
