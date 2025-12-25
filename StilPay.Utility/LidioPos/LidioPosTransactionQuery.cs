using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.LidioPos.Models.LidioPosPaymentRequest;
using StilPay.Utility.LidioPos.Models.LidioPosTransactionQuery;
using StilPay.Utility.Worker;
using System;
using System.Linq;

namespace StilPay.Utility.LidioPos
{
    public class LidioPosTransactionQuery
    {
        public static GenericResponseDataModel<LidioPosTransactionQueryRequestResponseModel> TransactionQueryRequest(LidioPosTransactionQueryRequestModel lidioPosTransactionQueryRequestModel, bool IsForeignCard = false)
        {
            try
            {
                var systemSettingValues = IsForeignCard ? tSQLBankManager.GetSystemSettingValues("LidioPosYD") : tSQLBankManager.GetSystemSettingValues("LidioPos");

                var options = new RestClientOptions("https://api.lidio.com");
                var client = new RestClient(options);
                var request = new RestRequest("/PaymentInquiry", Method.Post);
                request.AddHeader("Authorization", systemSettingValues.FirstOrDefault(f => f.ParamDef == "authorization").ParamVal);
                request.AddHeader("MerchantCode", systemSettingValues.FirstOrDefault(f => f.ParamDef == "merchant_code").ParamVal);
                request.AddHeader("Content-Type", "application/json");
                var body = JsonConvert.SerializeObject(lidioPosTransactionQueryRequestModel);
                request.AddStringBody(body, DataFormat.Json);

                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<LidioPosTransactionQueryRequestResponseModel>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<LidioPosTransactionQueryRequestResponseModel>
                    {
                        Status = deserialize.Result == "Success" && deserialize.ResultDetail == "Success" ? "OK" : "ERROR",
                        Data = deserialize,
                        Message = deserialize.ResultMessage ?? "Hata!"
                    };
                }
                else
                {
                    return new GenericResponseDataModel<LidioPosTransactionQueryRequestResponseModel>
                    {
                        Status = "ERROR",
                        Data = deserialize,
                        Message = deserialize.ResultMessage ?? "Hata!",
                    };
                }

            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<LidioPosTransactionQueryRequestResponseModel>
                {
                    Status = "ERROR",
                    Message = "Hata " + ex.Message,
                };
            }
        }
    }
}

