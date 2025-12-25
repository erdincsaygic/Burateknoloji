using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.PayNKolay.Models;
using StilPay.Utility.PayNKolay.Models.ComplatePayment;
using System;

namespace StilPay.Utility.PayNKolay
{
    public class PayNKolayCompletePayment
    {
        public static ResponseModel<ComplatePaymentResponseModel> CompletePaymentRequest(ComplatePaymentRequestModel complatePaymentRequestModel)
        {
            try
            {
                var client = new RestClient("https://paynkolay.nkolayislem.com.tr/Vpos/v1/CompletePayment");
                var request = new RestRequest
                {
                    Method = Method.Post,
                    AlwaysMultipartFormData = true
                };
                request.AddParameter("sx", complatePaymentRequestModel.Sx);
                request.AddParameter("referenceCode", complatePaymentRequestModel.ReferenceCode);

                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<ComplatePaymentResponseModel>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new ResponseModel<ComplatePaymentResponseModel>
                    {
                        Status = deserialize.RESPONSE_CODE == 2 ? "OK" : "ERROR",
                        Message = deserialize.RESPONSE_DATA,
                        Data = deserialize
                    };

                }
                else
                {
                    return new ResponseModel<ComplatePaymentResponseModel>
                    {
                        Status = "ERROR",
                        Message = deserialize.RESPONSE_DATA,
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<ComplatePaymentResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
