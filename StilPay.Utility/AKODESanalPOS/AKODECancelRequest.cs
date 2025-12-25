using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.AKODESanalPOS.Models.AKODECancel;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.AKODESanalPOS
{
    public class AKODECancelRequest
    {
        public static GenericResponseDataModel<AKODECancelResponseModel> CancelRequest(AKODECancelRequestModel akOdeCancelRequestModel)
        {
            try
            {
                var options = new RestClientOptions("https://api.akodepos.com/api/Payment/")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("void", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = JsonConvert.SerializeObject(akOdeCancelRequestModel);
                request.AddStringBody(body, DataFormat.Json);
                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<AKODECancelResponseModel>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<AKODECancelResponseModel>
                    {
                        Status = deserialize.Code == 0 ? "OK" : "ERROR",
                        Data = deserialize
                    };
                }
                else
                {
                    return new GenericResponseDataModel<AKODECancelResponseModel>
                    {
                        Status = "ERROR",
                        Message = deserialize.Message ?? "Hata!",
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<AKODECancelResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
