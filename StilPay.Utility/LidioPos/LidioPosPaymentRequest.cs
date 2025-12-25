using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.LidioPos.Models.LidioPosBinQuery;
using StilPay.Utility.LidioPos.Models.LidioPosPaymentRequest;
using StilPay.Utility.Worker;
using System;
using System.Linq;

namespace StilPay.Utility.LidioPos
{
    public class LidioPosPaymentRequest
    {
        public static GenericResponseDataModel<LidioPosPaymentRequestResponseModel> PaymentRequest(LidioPosPaymentRequestModel lidioPosPaymentRequestModel, bool IsForeignCard = false)
        {
            try
            {
                var systemSettingValues = IsForeignCard ? tSQLBankManager.GetSystemSettingValues("LidioPosYD") : tSQLBankManager.GetSystemSettingValues("LidioPos");

                var options = new RestClientOptions("https://api.lidio.com");
                var client = new RestClient(options);
                var request = new RestRequest("/ProcessPayment", Method.Post);
                request.AddHeader("Authorization", systemSettingValues.FirstOrDefault(f => f.ParamDef == "authorization").ParamVal);
                request.AddHeader("MerchantCode", systemSettingValues.FirstOrDefault(f => f.ParamDef == "merchant_code").ParamVal);
                request.AddHeader("Content-Type", "application/json");
                var body = JsonConvert.SerializeObject(lidioPosPaymentRequestModel);
                request.AddStringBody(body, DataFormat.Json);

                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<LidioPosPaymentRequestResponseModel>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<LidioPosPaymentRequestResponseModel>
                    {
                        Status = deserialize.result == "RedirectFormCreated" && deserialize.resultDetail == "ThreeDSRedirectFormCreated" ? "OK" : "ERROR",
                        Data = deserialize,
                        Message = deserialize.resultMessage ?? "Hata!"
                    };
                }
                else
                {
                    return new GenericResponseDataModel<LidioPosPaymentRequestResponseModel>
                    {
                        Status = "ERROR",
                        Data = deserialize,
                        Message = deserialize.resultMessage ?? "Hata!",
                    };
                }

            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<LidioPosPaymentRequestResponseModel>
                {
                    Status = "ERROR",
                    Message = "Hata " + ex.Message,
                };
            }
        }
    }
}
