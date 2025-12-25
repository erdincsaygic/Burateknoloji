using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.LidioPos.Models.LidioPosFinishPayment;
using StilPay.Utility.LidioPos.Models.LidioPosFinishPaymentRequestModel;
using StilPay.Utility.LidioPos.Models.LidioPosPaymentRequest;
using StilPay.Utility.Worker;
using System;
using System.IO;
using System.Linq;

namespace StilPay.Utility.LidioPos
{
    public class LidioPosFinishPaymentRequest
    {
        public static GenericResponseDataModel<LidioPosFinishPaymentRequestResponseModel> FinishPaymentRequest(LidioPosFinishPaymentRequestModel lidioPosFinishPaymentRequestModel, bool IsForeignCard = false)
        {
            try
            {
                var systemSettingValues = IsForeignCard ? tSQLBankManager.GetSystemSettingValues("LidioPosYD") : tSQLBankManager.GetSystemSettingValues("LidioPos");

                var options = new RestClientOptions("https://api.lidio.com");
                var client = new RestClient(options);
                var request = new RestRequest("/FinishPaymentProcess", Method.Post);
                request.AddHeader("Authorization", systemSettingValues.FirstOrDefault(f => f.ParamDef == "authorization").ParamVal);
                request.AddHeader("MerchantCode", systemSettingValues.FirstOrDefault(f => f.ParamDef == "merchant_code").ParamVal);
                request.AddHeader("Content-Type", "application/json");
                var body = JsonConvert.SerializeObject(lidioPosFinishPaymentRequestModel);
                request.AddStringBody(body, DataFormat.Json);
                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<LidioPosFinishPaymentRequestResponseModel>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<LidioPosFinishPaymentRequestResponseModel>
                    {
                        Status = deserialize.result == "Success" && deserialize.resultDetail == "Success" ? "OK" : "ERROR",
                        Data = deserialize,
                        Message = deserialize.resultMessage ?? "Hata!"
                    };
                }
                else
                {
                    return new GenericResponseDataModel<LidioPosFinishPaymentRequestResponseModel>
                    {
                        Status = "ERROR",
                        Data = deserialize,
                        Message = deserialize.resultMessage ?? "Hata!",
                    };
                }

            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<LidioPosFinishPaymentRequestResponseModel>
                {
                    Status = "ERROR",
                    Message = "Hata " + ex.Message,
                };
            }
        }

    }
}
