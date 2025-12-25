using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.LidioPos.Models.LidioPosCancel;
using StilPay.Utility.LidioPos.Models.LidioPosRefund;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StilPay.Utility.LidioPos
{
    public class LidioPosRefundRequest
    {
        public static GenericResponseDataModel<LidioPosRefundRequestResponseModel> RefundRequest(LidioPosRefundRequestModel lidioPosRefundRequestModel, bool IsForeignCard = false)
        {
            try
            {
                var systemSettingValues = IsForeignCard ? tSQLBankManager.GetSystemSettingValues("LidioPosYD") : tSQLBankManager.GetSystemSettingValues("LidioPos");

                var options = new RestClientOptions("https://api.lidio.com");
                var client = new RestClient(options);
                var request = new RestRequest("/Refund", Method.Post);
                request.AddHeader("Authorization", systemSettingValues.FirstOrDefault(f => f.ParamDef == "authorization").ParamVal);
                request.AddHeader("MerchantCode", systemSettingValues.FirstOrDefault(f => f.ParamDef == "merchant_code").ParamVal);
                request.AddHeader("Content-Type", "application/json");
                var body = JsonConvert.SerializeObject(lidioPosRefundRequestModel);
                request.AddStringBody(body, DataFormat.Json);

                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<LidioPosRefundRequestResponseModel>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<LidioPosRefundRequestResponseModel>
                    {
                        Status = deserialize.Result == "Success" && deserialize.ResultDetail == "Success" ? "OK" : "ERROR",
                        Data = deserialize,
                        Message = deserialize.ResultMessage ?? "Hata!"
                    };
                }
                else
                {
                    return new GenericResponseDataModel<LidioPosRefundRequestResponseModel>
                    {
                        Status = "ERROR",
                        Data = deserialize,
                        Message = deserialize.ResultMessage ?? "Hata!",
                    };
                }

            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<LidioPosRefundRequestResponseModel>
                {
                    Status = "ERROR",
                    Message = "Hata " + ex.Message,
                };
            }
        }
    }
}
