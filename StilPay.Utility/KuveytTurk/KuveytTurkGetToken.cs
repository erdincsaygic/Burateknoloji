using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.KuveytTurk.KuveytTurkToken;
using System;
using System.Threading;

namespace StilPay.Utility.KuveytTurk
{
    public class KuveytTurkGetToken
    {
        public static GenericResponseDataModel<KuveytTurkTokenResponseModel> GetAccessToken(KuveytTurkTokenRequestModel kuveytTurkTokenRequestModel)
        {
            int maxRetryCount = 5;
            int currentRetry = 0;
            while (currentRetry < maxRetryCount)
            {
                try
                {
                    var options = new RestClientOptions("https://identity.kuveytturk.com.tr")
                    {
                        MaxTimeout = 10000 
                    };
                    var client = new RestClient(options);

                    var request = new RestRequest("/connect/token", Method.Post);
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddParameter("scope", "accounts transfers");
                    request.AddParameter("client_id", kuveytTurkTokenRequestModel.client_id);
                    request.AddParameter("client_secret", kuveytTurkTokenRequestModel.client_secret);
                    request.AddParameter("grant_type", "client_credentials");

                    var cancellationTokenSource = new CancellationTokenSource();
                    var token = cancellationTokenSource.Token;

                    var response = client.ExecuteAsync(request, token).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var deserialize = JsonConvert.DeserializeObject<KuveytTurkTokenResponseModel>(response.Content);
                        return new GenericResponseDataModel<KuveytTurkTokenResponseModel>
                        {
                            Status = !string.IsNullOrEmpty(deserialize.access_token) ? "OK" : "ERROR",
                            Data = deserialize
                        };
                    }
                    else
                    {
                        currentRetry++;
                        if (currentRetry == maxRetryCount)
                        {
                            return new GenericResponseDataModel<KuveytTurkTokenResponseModel>
                            {
                                Status = "ERROR",
                                Message = "Token alımında bir hata meydana geldi / " + response.ErrorMessage
                            };
                        }
                    }
                }
                catch (TimeoutException ex)
                {
                    currentRetry++;
                    if (currentRetry == maxRetryCount)
                    {
                        return new GenericResponseDataModel<KuveytTurkTokenResponseModel>
                        {
                            Status = "ERROR",
                            Message = "Token alımında zaman aşımı hatası meydana geldi / " + ex.Message
                        };
                    }
                }
                catch (Exception ex)
                {
                    currentRetry++;
                    if (currentRetry == maxRetryCount)
                    {
                        return new GenericResponseDataModel<KuveytTurkTokenResponseModel>
                        {
                            Status = "ERROR",
                            Message = "Token alımında bir hata meydana geldi / " + ex.Message
                        };
                    }
                }
            }

            return new GenericResponseDataModel<KuveytTurkTokenResponseModel>
            {
                Status = "ERROR",
                Message = "Token alımında bir hata meydana geldi / İstek başarısız oldu ve maksimum deneme sayısına ulaşıldı."
            };
        }
    }
}
