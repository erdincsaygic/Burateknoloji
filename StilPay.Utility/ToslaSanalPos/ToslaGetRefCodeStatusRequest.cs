using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.ToslaSanalPos.Models.ToslaGetRefCode;
using StilPay.Utility.ToslaSanalPos.Models.ToslaGetToken;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Text;
using StilPay.Utility.ToslaSanalPos.Models.ToslaGetRefCodeStatus;
using System.Linq;

namespace StilPay.Utility.ToslaSanalPos
{
    public class ToslaGetRefCodeStatusRequest
    {
        public static GenericResponseDataModel<ToslaGetRefCodeStatusResponseModel> GetRefCodeStatus(ToslaGetRefCodeStatusRequestModel toslaGetRefCodeStatusRequestModel)
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
                    toslaGetRefCodeStatusRequestModel.companyId = int.Parse(systemSettingValues.FirstOrDefault(f => f.ParamDef == "companyId").ParamVal);

                    var options = new RestClientOptions("https://api.tosla.com")
                    {
                        MaxTimeout = -1,
                    };
                    var client = new RestClient(options);
                    var request = new RestRequest("/api/gateway-third-party/ref-code/getRefCodeStatus", Method.Post);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Authorization", "Bearer " + toslaTokenRequestResponseModel.Data.access_token);
                    var body = JsonConvert.SerializeObject(toslaGetRefCodeStatusRequestModel);
                    request.AddStringBody(body, DataFormat.Json);
                    var response = client.Execute(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var deserialize = JsonConvert.DeserializeObject<ToslaGetRefCodeStatusResponseModel>(response.Content);

                        return new GenericResponseDataModel<ToslaGetRefCodeStatusResponseModel>
                        {
                            Status = deserialize.success ? "OK" : "ERROR",
                            Data = deserialize
                        };
                    }
                    else
                    {
                        JObject jsonObject = JObject.Parse(response.Content);

                        return new GenericResponseDataModel<ToslaGetRefCodeStatusResponseModel>
                        {
                            Status = "ERROR",
                            Message = jsonObject["error_description"].ToString(),
                        };
                    }
                }
                else
                {
                    return new GenericResponseDataModel<ToslaGetRefCodeStatusResponseModel>
                    {
                        Status = "ERROR",
                        Message = "Token Alımında Bir Hata Meydana Geldi"
                    };
                }

            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<ToslaGetRefCodeStatusResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
