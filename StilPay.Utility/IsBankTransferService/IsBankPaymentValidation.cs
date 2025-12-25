using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.IsBankTransferService.Models.IsBankToken;
using System;
using System.Linq;
using static StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPayment.IsBankPaymentRequestModel;
using static StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPaymentValidation.IsBankPaymentValidationRequestModel;
using static StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPaymentValidation.IsBankPaymentValidationResponseModel;

namespace StilPay.Utility.IsBankTransferService
{
    public class IsBankPaymentValidation
    {
        public static GenericResponseDataModel<IsBankPaymentValidationResponse> IsBankPaymentValidationRequest(IsBankPaymentValidationRequest isbankPaymentValidationRequest)
        {
            try
            {
                var tokenModel = new IsBankTokenRequestModel
                {
                    Authorization = isbankPaymentValidationRequest.authorization,
                    client_id = isbankPaymentValidationRequest.isbank_client_id,
                    client_secret = isbankPaymentValidationRequest.isbank_client_secret,
                    password = isbankPaymentValidationRequest.password,
                    username = isbankPaymentValidationRequest.username
                };

                var accessToken = IsBankGetAccessToken.GetAccessToken(tokenModel);

                if (accessToken != null && accessToken.Status == "OK")
                {
                    var options = new RestClientOptions("https://api.isbank.com.tr")
                    {
                        MaxTimeout = -1,
                    };

                    var client = new RestClient(options);
                    var request = new RestRequest(isbankPaymentValidationRequest.apiUrl, Method.Post);
                    request.AddHeader("X-Isbank-Client-Id", isbankPaymentValidationRequest.isbank_client_id);
                    request.AddHeader("X-Isbank-Client-Secret", isbankPaymentValidationRequest.isbank_client_secret);
                    request.AddHeader("X-Client-Certificate", isbankPaymentValidationRequest.isbank_client_certificate);
                    request.AddHeader("Authorization", "Bearer " + accessToken.Data.access_token);

                    var body = JsonConvert.SerializeObject(isbankPaymentValidationRequest);
                    request.AddParameter("application/json", body, ParameterType.RequestBody);
                    var response = client.Execute(request);
                    var deserialize = JsonConvert.DeserializeObject<IsBankPaymentValidationResponse>(response.Content);

                    if (response.IsSuccessStatusCode)
                    {
                        return new GenericResponseDataModel<IsBankPaymentValidationResponse>
                        {
                            Status = deserialize.data != null && deserialize.data.result ? "OK" : "ERROR",
                            Message = deserialize.data != null && deserialize.data.result ? "Başarılı" : deserialize.infos != null && deserialize.infos.Count > 0 ? deserialize.infos.FirstOrDefault().message : deserialize.errors != null && deserialize.errors.Count > 0 ? deserialize.errors.FirstOrDefault().message : "Beklenmedik Bir Hata İle Karşılaşıldı",
                            Data = deserialize
                        };

                    }
                    else
                    {
                        return new GenericResponseDataModel<IsBankPaymentValidationResponse>
                        {
                            Status = "ERROR",
                            Message = deserialize != null && deserialize.data != null && deserialize.data.result ? "Başarılı" : deserialize.infos != null && deserialize.infos.Count > 0 ? deserialize.infos.FirstOrDefault().message : deserialize.errors != null && deserialize.errors.Count > 0 ? deserialize.errors.FirstOrDefault().message : "Beklenmedik Bir Hata İle Karşılaşıldı",
                        };
                    }
                }
                else
                {
                    return new GenericResponseDataModel<IsBankPaymentValidationResponse>
                    {
                        Status = "ERROR",
                        Message = "Token Alımında Bir Hata Meydana Geldi"
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<IsBankPaymentValidationResponse>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
