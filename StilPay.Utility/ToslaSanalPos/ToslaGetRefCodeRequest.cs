using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.ToslaSanalPos.Models.ToslaGetToken;
using System;
using System.Collections.Generic;
using System.Text;
using StilPay.Utility.ToslaSanalPos.Models.ToslaGetRefCode;
using StilPay.Utility.AKODESanalPOS.Models.AKODECancel;
using StilPay.Utility.Worker;
using System.Linq;

namespace StilPay.Utility.ToslaSanalPos
{
    public class ToslaGetRefCodeRequest
    {
        public static GenericResponseDataModel<ToslaGetRefCodeResponseModel> GetRefCode(ToslaGetRefCodeRequestModel toslaGetRefCodeRequestModel)
        {
            try
            {
                var systemSettingValues = tSQLBankManager.GetSystemSettingValues("Tosla");

                var toslaGetTokenRequestModel = new ToslaGetTokenRequestModel
                {
                    BasicAuthBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(systemSettingValues.FirstOrDefault(f => f.ParamDef == "username_password").ParamVal)),
                };

                var toslaTokenRequestResponseModel = ToslaGetTokenRequest.GetToken(toslaGetTokenRequestModel);


                if (toslaTokenRequestResponseModel != null && toslaTokenRequestResponseModel.Status == "OK" && toslaTokenRequestResponseModel.Data != null && toslaTokenRequestResponseModel.Data.access_token != null)
                {
                    toslaGetRefCodeRequestModel.companyId = int.Parse(systemSettingValues.FirstOrDefault(f => f.ParamDef == "companyId").ParamVal);

                    var options = new RestClientOptions("https://api.tosla.com")
                    {
                        MaxTimeout = -1,
                    };
                    var client = new RestClient(options);
                    var request = new RestRequest("/api/gateway-third-party/ref-code/generateRefCode", Method.Post);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Authorization", "Bearer " + toslaTokenRequestResponseModel.Data.access_token);
                    var body = JsonConvert.SerializeObject(toslaGetRefCodeRequestModel);
                    request.AddStringBody(body, DataFormat.Json);
                    var response = client.Execute(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var deserialize = JsonConvert.DeserializeObject<ToslaGetRefCodeResponseModel>(response.Content);

                        return new GenericResponseDataModel<ToslaGetRefCodeResponseModel>
                        {
                            Status = deserialize.result ? "OK" : "ERROR",
                            Data = deserialize
                        };
                    }
                    else
                    {
                        JObject jsonObject = JObject.Parse(response.Content);

                        return new GenericResponseDataModel<ToslaGetRefCodeResponseModel>
                        {
                            Status = "ERROR",
                            Message = jsonObject["error_description"].ToString(),
                        };
                    }
                }
                else
                {
                    return new GenericResponseDataModel<ToslaGetRefCodeResponseModel>
                    {
                        Status = "ERROR",
                        Message = "Token Alımında Bir Hata Meydana Geldi"
                    };
                }

            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<ToslaGetRefCodeResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
