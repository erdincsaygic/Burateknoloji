using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.LidioPos.Models.LidioPosBinQuery;
using StilPay.Utility.LidioPos.Models.LidioPosGetTransactions;
using StilPay.Utility.Worker;
using System;
using System.Linq;

namespace StilPay.Utility.LidioPos
{
    public class LidioPosGetTransactions
    {
        public static GenericResponseDataModel<LidioPosGetTransactionRequestResponseModel> GetTransactionRequest(LidioPosGetTransactionRequestModel lidioPosGetTransactionRequestModel, bool IsForeignCard = false)
        {
            try
            {
                var systemSettingValues = IsForeignCard ? tSQLBankManager.GetSystemSettingValues("LidioPosYD") : tSQLBankManager.GetSystemSettingValues("LidioPos");

                var options = new RestClientOptions("https://api.lidio.com")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/GetTransactionList", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", systemSettingValues.FirstOrDefault(f => f.ParamDef == "authorization").ParamVal);
                request.AddHeader("MerchantCode", systemSettingValues.FirstOrDefault(f => f.ParamDef == "merchant_code").ParamVal);
                var body = JsonConvert.SerializeObject(lidioPosGetTransactionRequestModel);
                request.AddStringBody(body, DataFormat.Json);
                var response = client.Execute(request);

                if (response.IsSuccessStatusCode)
                {
                    var deserialize = JsonConvert.DeserializeObject<LidioPosGetTransactionRequestResponseModel>(response.Content);

                    return new GenericResponseDataModel<LidioPosGetTransactionRequestResponseModel>
                    {
                        Status = "OK",
                        Data = deserialize
                    };
                }
                else
                {
                    return new GenericResponseDataModel<LidioPosGetTransactionRequestResponseModel>
                    {
                        Status = "ERROR",
                        Message = response.ErrorMessage ?? "Hata",
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<LidioPosGetTransactionRequestResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
