using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.AKODESanalPOS.Models.AKODEGetTransactions;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.AKODESanalPOS
{
    public class AKODEGetTransactionRequest
    {
        public static GenericResponseDataModel<AKODEGetTransactionResponseModel.TransactionResponse> GetTransactionList(AKODEGetTransactionRequestModel akOdeGetTransactionRequestModel)
        {
            try
            {
                var options = new RestClientOptions("https://api.akodepos.com/api/Payment/")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("history", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = JsonConvert.SerializeObject(akOdeGetTransactionRequestModel);
                request.AddStringBody(body, DataFormat.Json);
                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<AKODEGetTransactionResponseModel.TransactionResponse>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<AKODEGetTransactionResponseModel.TransactionResponse>
                    {
                        Status = deserialize.Code == 0 ? "OK" : "ERROR",
                        Data = deserialize
                    };
                }
                else
                {
                    return new GenericResponseDataModel<AKODEGetTransactionResponseModel.TransactionResponse>
                    {
                        Status = "ERROR",
                        Message = deserialize.Message ?? "Hata!",
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<AKODEGetTransactionResponseModel.TransactionResponse>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }

    }
}
