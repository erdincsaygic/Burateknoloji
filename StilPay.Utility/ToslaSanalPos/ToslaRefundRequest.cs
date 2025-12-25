using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.ToslaSanalPos.Models.ToslaGetRefCode;
using StilPay.Utility.ToslaSanalPos.Models.ToslaGetRefCodeStatus;
using StilPay.Utility.ToslaSanalPos.Models.ToslaGetToken;
using StilPay.Utility.ToslaSanalPos.Models.ToslaRefund;
using StilPay.Utility.Worker;
using System;
using System.Linq;

namespace StilPay.Utility.ToslaSanalPos
{
    public class ToslaRefundRequest
    {
        public static GenericResponseDataModel<ToslaRefundResponseModel> RefundRequest(ToslaRefundRequestModel toslaRefundRequestModel)
        {
            try
            {
                var systemSettingValues = tSQLBankManager.GetSystemSettingValues("Tosla");

                var toslaGetTokenRequestModel = new ToslaGetTokenRequestModel
                {
                    BasicAuthBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(systemSettingValues.FirstOrDefault(f => f.ParamDef == "username_password").ParamVal)),
                };

                var toslaTokenRequestResponseModel = ToslaGetTokenRequest.GetToken(toslaGetTokenRequestModel);

                if (toslaTokenRequestResponseModel != null && toslaTokenRequestResponseModel.Data != null && toslaTokenRequestResponseModel.Status == "OK" && toslaTokenRequestResponseModel.Data.access_token != null)
                {
                    var options = new RestClientOptions("https://api.tosla.com");
                    var client = new RestClient(options);
                    var request = new RestRequest("/api/gateway-third-party/ref-code/return/command/create", Method.Post);
                    request.AddHeader("Authorization", "Bearer " + toslaTokenRequestResponseModel.Data.access_token);
                    var body = JsonConvert.SerializeObject(toslaRefundRequestModel);
                    request.AddStringBody(body, DataFormat.Json);
                    var response = client.Execute(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var deserialize = JsonConvert.DeserializeObject<ToslaRefundResponseModel>(response.Content);
                        deserialize.Token = toslaTokenRequestResponseModel.Data.access_token;
                        return new GenericResponseDataModel<ToslaRefundResponseModel>
                        {
                            Status = deserialize.CommandId != null && deserialize.CommandId != "" ? "OK" : "ERROR",
                            Data = deserialize,
                        };

                    }
                    else
                    {
                        return new GenericResponseDataModel<ToslaRefundResponseModel>
                        {
                            Status = "ERROR",
                            Message = "error",
                        };
                    }
                }
                else
                {
                    return new GenericResponseDataModel<ToslaRefundResponseModel>
                    {
                        Status = "ERROR",
                        Message = "Token Alımında Bir Hata Meydana Geldi"
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<ToslaRefundResponseModel>
                {
                    Status = "ERROR",
                    Message = "Hata",
                };
            }
        }
    }
}
