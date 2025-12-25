using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.EsnekPos.Models.EsnekPosBinQuery;
using StilPay.Utility.Helper;
using System;

namespace StilPay.Utility.EsnekPos
{
    public class EsnekPosBinQueryRequest
    {
        public static GenericResponseDataModel<EsnekPosBinQueryRequestResponseModel> BinQueryRequest(string cardBinNumber)
        {
            try
            {
                var options = new RestClientOptions("https://posservice.esnekpos.com")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/api/services/EYVBinService", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = JsonConvert.SerializeObject(new { CardNumber = cardBinNumber });
                request.AddStringBody(body, DataFormat.Json);
                var response = client.Execute(request);

                if (response.IsSuccessStatusCode)
                {
                    var deserialize = JsonConvert.DeserializeObject<EsnekPosBinQueryRequestResponseModel>(response.Content);

                    return new GenericResponseDataModel<EsnekPosBinQueryRequestResponseModel>
                    {
                        Status = "OK",
                        Data = deserialize
                    };
                }
                else
                {
                    return new GenericResponseDataModel<EsnekPosBinQueryRequestResponseModel>
                    {
                        Status = "ERROR",
                        Message = response.ErrorMessage ?? "Hata",
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<EsnekPosBinQueryRequestResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
