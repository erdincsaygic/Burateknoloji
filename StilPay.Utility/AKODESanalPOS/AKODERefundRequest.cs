using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.AKODESanalPOS.Models.AKODERefund;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.AKODESanalPOS
{
    public class AKODERefundRequest
    {
        public static GenericResponseDataModel<AKODERefundResponseModel> RefundRequest(AKODERefundRequestModel akOdeRefundRequestModel)
        {
            try
            {
                var options = new RestClientOptions("https://api.akodepos.com/api/Payment/")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("refund", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = JsonConvert.SerializeObject(akOdeRefundRequestModel);
                request.AddStringBody(body, DataFormat.Json);
                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<AKODERefundResponseModel>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<AKODERefundResponseModel>
                    {
                        Status = deserialize.Code == 0 ? "OK" : "ERROR",
                        Data = deserialize
                    };
                }
                else
                {
                    return new GenericResponseDataModel<AKODERefundResponseModel>
                    {
                        Status = "ERROR",
                        Message = deserialize.Message ?? "Hata!",
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<AKODERefundResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
