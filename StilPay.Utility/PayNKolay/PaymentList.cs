using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.PayNKolay.Models;
using StilPay.Utility.PayNKolay.Models.PaymentList;
using System;
using static StilPay.Utility.PayNKolay.Models.PaymentList.PaymentListResponseModel;

namespace StilPay.Utility.PayNKolay
{
    public class PayNKolayPaymentList
    {
        public static ResponseModel<PaymentListResponse> PaymentListRequest(PaymentListRequestModel paymentListRequestModel)
        {
            try
            {
                var client = new RestClient("https://paynkolay.nkolayislem.com.tr/Vpos/Payment/PaymentList");
                var request = new RestRequest
                {
                    Method = Method.Post,
                    AlwaysMultipartFormData = true
                };
                request.AddParameter("sx", paymentListRequestModel.Sx);
                request.AddParameter("startDate", paymentListRequestModel.StartDate);
                request.AddParameter("endDate", paymentListRequestModel.EndDate);
                request.AddParameter("hashData", paymentListRequestModel.HashData);

                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<PaymentListResponse>(JsonConvert.DeserializeObject<string>(response.Content));

                if (response.IsSuccessStatusCode)
                {
                    return new ResponseModel<PaymentListResponse>
                    {
                        Status = deserialize.result.RESPONSE_CODE == 2 ? "OK" : "ERROR",
                        Message = deserialize.result.RESPONSE_DATA,
                        Data = deserialize
                    };
                }
                else
                {
                    return new ResponseModel<PaymentListResponse>
                    {
                        Status = "ERROR",
                        Message = "A"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<PaymentListResponse>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
