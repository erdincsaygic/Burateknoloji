using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.EsnekPos.Models.EsnekPosCancelAndRefund;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Linq;

namespace StilPay.Utility.EsnekPos
{
    public class EsnekPosCancelAndRefundRequest
    {
        public static GenericResponseDataModel<EsnekPosCancelAndRefundRequestResponseModel> CancelAndRefundRequest(EsnekPosCancelAndRefundRequestModel esnekPosCancelAndRefundRequestModel)
        {
            try
            {
                var systemSettingValues = tSQLBankManager.GetSystemSettingValues("EsnekPos");

                esnekPosCancelAndRefundRequestModel.MERCHANT = systemSettingValues.FirstOrDefault(f => f.ParamDef == "merchant").ParamVal;
                esnekPosCancelAndRefundRequestModel.MERCHANT_KEY = systemSettingValues.FirstOrDefault(f => f.ParamDef == "merchant_key").ParamVal;

                var options = new RestClientOptions("https://posservice.esnekpos.com");
                var client = new RestClient(options);
                var request = new RestRequest("/api/services/OrderReturn", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = JsonConvert.SerializeObject(esnekPosCancelAndRefundRequestModel);
                request.AddStringBody(body, DataFormat.Json);

                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<EsnekPosCancelAndRefundRequestResponseModel>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<EsnekPosCancelAndRefundRequestResponseModel>
                    {
                        Status = deserialize.STATUS == "SUCCESS" && deserialize.RETURN_CODE == "0" ? "OK" : "ERROR",
                        Data = deserialize,
                        Message = deserialize.RETURN_MESSAGE ?? ""
                    };
                }
                else
                {
                    return new GenericResponseDataModel<EsnekPosCancelAndRefundRequestResponseModel>
                    {
                        Status = "ERROR",
                        Data = deserialize,
                        Message = deserialize.RETURN_MESSAGE ?? "",
                    };
                }

            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<EsnekPosCancelAndRefundRequestResponseModel>
                {
                    Status = "ERROR",
                    Message = "Hata " + ex.Message,
                };
            }
        }

    }
}
