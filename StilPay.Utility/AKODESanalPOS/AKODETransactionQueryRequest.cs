using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.AKODESanalPOS.Models.AKODETransactionQuery;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.AKODESanalPOS
{
    public class AKODETransactionQueryRequest
    {
        public static GenericResponseDataModel<AKODETransactionQueryResponseModel> TransactionQueryRequest(AKODETransactionQueryRequestModel akOdeTransactionQueryRequestModel)
        {
            try
            {
                var options = new RestClientOptions("https://api.akodepos.com/api/Payment/")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("inquiry", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = JsonConvert.SerializeObject(akOdeTransactionQueryRequestModel);
                request.AddStringBody(body, DataFormat.Json);
                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<AKODETransactionQueryResponseModel>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<AKODETransactionQueryResponseModel>
                    {
                        Status = deserialize.Code == 0 ? "OK" : "ERROR",
                        Data = deserialize
                    };
                }
                else
                {
                    return new GenericResponseDataModel<AKODETransactionQueryResponseModel>
                    {
                        Status = "ERROR",
                        Message = deserialize.Message ?? "Hata!",
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<AKODETransactionQueryResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
