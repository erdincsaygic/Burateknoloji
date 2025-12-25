using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.EsnekPos.Models.EsnekPosPaymentRequest;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Linq;

namespace StilPay.Utility.EsnekPos
{
    public class EsnekPosPaymentRequest
    {
        public static GenericResponseDataModel<EsnekPosPaymentRequestResponseModel> PaymentRequest(EsnekPosPaymentRequestModel esnekPosPaymentRequestModel)
        {
            try
            {
                var systemSettingValues = tSQLBankManager.GetSystemSettingValues("EsnekPos");

                esnekPosPaymentRequestModel.Config.MERCHANT = systemSettingValues.FirstOrDefault(f => f.ParamDef == "merchant").ParamVal;
                esnekPosPaymentRequestModel.Config.MERCHANT_KEY = systemSettingValues.FirstOrDefault(f => f.ParamDef == "merchant_key").ParamVal;

                var options = new RestClientOptions("https://posservice.esnekpos.com");
                var client = new RestClient(options);
                var request = new RestRequest("/api/pay/EYV3DPay", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = JsonConvert.SerializeObject(esnekPosPaymentRequestModel);
                request.AddStringBody(body, DataFormat.Json);

                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<EsnekPosPaymentRequestResponseModel>(response.Content);


                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<EsnekPosPaymentRequestResponseModel>
                    {
                        Status = deserialize.STATUS == "SUCCESS" && deserialize.RETURN_CODE == "0" ? "OK" : "ERROR",
                        Data = deserialize,
                        Message = deserialize.RETURN_MESSAGE ?? ""
                    };
                }
                else
                {
                    return new GenericResponseDataModel<EsnekPosPaymentRequestResponseModel>
                    {
                        Status = "ERROR",
                        Data = deserialize,
                        Message = deserialize.RETURN_MESSAGE_TR ?? deserialize.RETURN_MESSAGE,
                    };
                }
                
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<EsnekPosPaymentRequestResponseModel>
                {
                    Status = "ERROR",
                    Message = "Hata " + ex.Message,
                };
            }
        }
    }
}
