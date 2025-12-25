using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPaymentQuery;
using System;
using static StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPaymentQuery.IsBankPaymentQueryResponseModel;

namespace StilPay.Utility.IsBankTransferService
{
    public class IsBankPaymentQuery
    {
        //public static GenericResponseDataModel<IsBankPaymentQueryResponse> IsBankPaymentValidationRequest(IsBankPaymentQueryRequestModel isBankPaymentQueryRequestModel)
        //{
        //    try
        //    {
        //        var accessToken = IsBankGetAccessToken.GetAccessToken();

        //        if (accessToken == null && accessToken.Status == "OK")
        //        {
        //            var options = new RestClientOptions("https://api.isbank.com.tr")
        //            {
        //                MaxTimeout = -1,
        //            };

        //            var client = new RestClient(options);
        //            var request = new RestRequest(isBankPaymentQueryRequestModel.apiUrl, Method.Post);
        //            request.AddHeader("X-Isbank-Client-Id", isBankPaymentQueryRequestModel.isbank_client_id);
        //            request.AddHeader("X-Isbank-Client-Secret", isBankPaymentQueryRequestModel.isbank_client_secret);
        //            request.AddHeader("X-Client-Certificate", isBankPaymentQueryRequestModel.isbank_client_certificate);
        //            request.AddHeader("Authorization", "Bearer " + accessToken);

        //            var body = JsonConvert.SerializeObject(isBankPaymentQueryRequestModel);
        //            request.AddParameter("application/json", body, ParameterType.RequestBody);
        //            var response = client.Execute(request);
        //            var deserialize = JsonConvert.DeserializeObject<IsBankPaymentQueryResponse>(response.Content);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                return new GenericResponseDataModel<IsBankPaymentQueryResponse>
        //                {
        //                    Status = deserialize.errors is null ? "OK" : "ERROR",
        //                    Message = deserialize.errors is null ? "Başarılı" : deserialize.errors.message ?? "Beklenmedik Bir Hata İle Karşılaşıldı",
        //                    Data = deserialize
        //                };

        //            }
        //            else
        //            {
        //                return new GenericResponseDataModel<IsBankPaymentQueryResponse>
        //                {
        //                    Status = "ERROR",
        //                    Message = deserialize.errors.message ?? $"{response.StatusCode} - Hata!"
        //                };
        //            }
        //        }
        //        else
        //        {
        //            return new GenericResponseDataModel<IsBankPaymentQueryResponse>
        //            {
        //                Status = "ERROR",
        //                Message = "Token Alımında Bir Hata Meydana Geldi"
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new GenericResponseDataModel<IsBankPaymentQueryResponse>
        //        {
        //            Status = "ERROR",
        //            Message = ex.Message,
        //        };
        //    }
        //}
    }
}
