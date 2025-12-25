using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.AKODESanalPOS.Models.AKODEGetSession;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace StilPay.Utility.AKODESanalPOS
{
    public class AKODEGetSessionRequest
    {
        public static GenericResponseDataModel<AKODEGetSessionResponseModel> GetSessionRequest(AKODEGetSessionRequestModel akODEGetSessionRequestModel)
        {
            try
            {
                var options = new RestClientOptions("https://api.akodepos.com/api/Payment/")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("threeDPayment", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = JsonConvert.SerializeObject(akODEGetSessionRequestModel);
                request.AddStringBody(body, DataFormat.Json);
                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<AKODEGetSessionResponseModel>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<AKODEGetSessionResponseModel>
                    {
                        Status = deserialize.code == 0 ? "OK" : "ERROR",
                        Data = deserialize
                    };
                }
                else
                {
                    return new GenericResponseDataModel<AKODEGetSessionResponseModel>
                    {
                        Status = "ERROR",
                        Message = deserialize.message ?? "Hata!",
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<AKODEGetSessionResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
