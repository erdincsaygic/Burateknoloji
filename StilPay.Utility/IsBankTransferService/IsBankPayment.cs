using ClosedXML;
using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.IsBankTransferService.IsBankTokenModel;
using StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPayment;
using StilPay.Utility.IsBankTransferService.Models.IsBankToken;
using StilPay.Utility.Parasut.Models;
using StilPay.Utility.PayNKolay.Models.ComplatePayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using static StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPayment.IsBankPaymentResponseModel;

namespace StilPay.Utility.IsBankTransferService
{
    public class IsBankPayment
    {
        public static GenericResponseDataModel<IsBankPaymentResponse> IsBankPaymentRequest(IsBankPaymentRequestModel.IsBankPaymentRequest isBankPaymentRequest)
        {
            try
            {
                var tokenModel = new IsBankTokenRequestModel
                {
                    Authorization = isBankPaymentRequest.authorization,
                    client_id = isBankPaymentRequest.isbank_client_id,
                    client_secret = isBankPaymentRequest.isbank_client_secret,
                    password = isBankPaymentRequest.password,
                    username = isBankPaymentRequest.username
                };

                var accessToken = IsBankGetAccessToken.GetAccessToken(tokenModel);

                if (accessToken != null && accessToken.Status == "OK")
                {
                    var options = new RestClientOptions("https://api.isbank.com.tr")
                    {
                        MaxTimeout = -1,
                    };

                    var client = new RestClient(options);
                    var request = new RestRequest(isBankPaymentRequest.apiUrl, Method.Post);
                    request.AddHeader("X-Isbank-Client-Id", isBankPaymentRequest.isbank_client_id);
                    request.AddHeader("X-Isbank-Client-Secret", isBankPaymentRequest.isbank_client_secret);
                    request.AddHeader("X-Client-Certificate", isBankPaymentRequest.isbank_client_certificate);
                    request.AddHeader("Authorization", "Bearer " + accessToken.Data.access_token);

                    var body = JsonConvert.SerializeObject(isBankPaymentRequest);
                    request.AddParameter("application/json", body, ParameterType.RequestBody);
                    var response = client.Execute(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var deserialize = JsonConvert.DeserializeObject<IsBankPaymentResponse>(response.Content);

                        return new GenericResponseDataModel<IsBankPaymentResponse>
                        {
                            Status = deserialize.errors is null ? "OK" : "ERROR",
                            Message = deserialize.errors is null ? "Başarılı" : deserialize.errors != null && deserialize.errors.Count > 0 ? deserialize.errors.FirstOrDefault().message : "Beklenmedik Bir Hata İle Karşılaşıldı",
                            Data = deserialize
                        };
                    }
                    else
                    {
                        return new GenericResponseDataModel<IsBankPaymentResponse>
                        {
                            Status = "ERROR",
                            Message = response.ErrorMessage ?? $"{response.StatusCode} - Hata!"
                        };
                    }
                }
                else
                {
                    return new GenericResponseDataModel<IsBankPaymentResponse>
                    {
                        Status = "ERROR",
                        Message = "Token Alımında Bir Hata Meydana Geldi"
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<IsBankPaymentResponse>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
