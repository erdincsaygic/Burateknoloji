using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.LidioPos.Models.LidioPosBinQuery;
using StilPay.Utility.Worker;
using System;
using System.Linq;


namespace StilPay.Utility.LidioPos
{
    public class LidioPosBinQueryRequest
    {
        public static GenericResponseDataModel<LidioPosBinQueryRequestResponseModel> BinQueryRequest(string cardBinNumber, bool IsForeignCard  = false)
        {
            try
            {
                var systemSettingValues = IsForeignCard ? tSQLBankManager.GetSystemSettingValues("LidioPosYD") : tSQLBankManager.GetSystemSettingValues("LidioPos");

                var options = new RestClientOptions("https://api.lidio.com")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/GetBankOfBINNumber", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", systemSettingValues.FirstOrDefault(f => f.ParamDef == "authorization").ParamVal);
                request.AddHeader("MerchantCode", systemSettingValues.FirstOrDefault(f => f.ParamDef == "merchant_code").ParamVal);
                var body = JsonConvert.SerializeObject(new { bin = cardBinNumber });
                request.AddStringBody(body, DataFormat.Json);
                var response = client.Execute(request);

                if (response.IsSuccessStatusCode)
                {
                    var deserialize = JsonConvert.DeserializeObject<LidioPosBinQueryRequestResponseModel>(response.Content);

                    return new GenericResponseDataModel<LidioPosBinQueryRequestResponseModel>
                    {
                        Status = "OK",
                        Data = deserialize
                    };
                }
                else
                {
                    return new GenericResponseDataModel<LidioPosBinQueryRequestResponseModel>
                    {
                        Status = "ERROR",
                        Message = response.ErrorMessage ?? "Hata",
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<LidioPosBinQueryRequestResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
