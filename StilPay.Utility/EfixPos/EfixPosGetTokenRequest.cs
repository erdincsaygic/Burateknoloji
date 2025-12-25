using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.EfixPos.Models;
using StilPay.Utility.EfixPos.Models.EfixPosGetToken;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Linq;

namespace StilPay.Utility.EfixPos
{
    public class EfixPosGetTokenRequest
    {
        public static GenericResponseDataModel<EfixPosGetTokenResponseModel> GetAccessToken()
        {
            try
            {
                var systemSettingValues = tSQLBankManager.GetSystemSettingValues("EfixPos");

                var efixPosGetTokenRequestModel = new EfixPosGetTokenRequestModel()
                {
                    ApiKey = systemSettingValues.FirstOrDefault(f => f.ParamDef == "apiKey").ParamVal,
                    ApiSecret = systemSettingValues.FirstOrDefault(f => f.ParamDef == "apiSecret").ParamVal
                };

                var options = new RestClientOptions("https://vpos.efixfatura.com.tr/api/");
                var client = new RestClient(options);
                var request = new RestRequest("auth/merchant", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = JsonConvert.SerializeObject(efixPosGetTokenRequestModel);
                request.AddStringBody(body, DataFormat.Json);
                var response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    var tokenResp = JsonConvert
                        .DeserializeObject<EfixPosGetTokenResponseModel>(response.Content);

                    if (!string.IsNullOrEmpty(tokenResp?.Token))
                    {
                        return new GenericResponseDataModel<EfixPosGetTokenResponseModel>
                        {
                            Status = "OK",
                            Data = tokenResp
                        };
                    }

                    return new GenericResponseDataModel<EfixPosGetTokenResponseModel>
                    {
                        Status = "ERROR",
                        Message = "Beklenmedik bir yanıt aldık; token bulunamadı."
                    };
                }
                else
                {
                    EfixPosErrorResponseModel err = null;
                    try
                    {
                        err = JsonConvert
                              .DeserializeObject<EfixPosErrorResponseModel>(response.Content);
                    }
                    catch { }

                    return new GenericResponseDataModel<EfixPosGetTokenResponseModel>
                    {
                        Status = "ERROR",
                        Message = err?.Message
                                  ?? $"HTTP {(int)response.StatusCode} {response.StatusDescription}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<EfixPosGetTokenResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message
                };
            }
        }
    }

}
