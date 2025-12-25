using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using System;
using StilPay.Utility.KuveytTurk.KuveytTurkTransfer;
using StilPay.Utility.KuveytTurk.KuveytTurkToken;
using StilPay.Utility.Worker;
using System.Linq;
using StilPay.Utility.KuveytTurk.KuveytTurkAccountTransaction;
using RestSharp.Serializers;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace StilPay.Utility.KuveytTurk
{
    public class KuveytTurkMoneyTransfer
    {
        public static GenericResponseDataModel<KuveytTurkTransferResponseModel.Root> MoneyTranfer(KuveytTurkTransferRequestModel kuveytTurkTransferRequestModel)
        {
            try
            {
                var systemSettingValues = tSQLBankManager.GetSystemSettingValues("KuveytTurkClient");

                var tokenModel = new KuveytTurkTokenRequestModel()
                {
                    client_id = systemSettingValues.FirstOrDefault(f => f.ParamDef == "kuveytturk_client_id").ParamVal,
                    client_secret = systemSettingValues.FirstOrDefault(f => f.ParamDef == "kuveytturk_client_secret").ParamVal
                };

                var token = KuveytTurkGetToken.GetAccessToken(tokenModel);

                if(token.Status == "ERROR")
                    return new GenericResponseDataModel<KuveytTurkTransferResponseModel.Root>
                    {
                        Status = "ERROR",
                        Message = token.Message,
                    };

                var rsa = KuveytTurkRSAKeyGenerator.RSAKeyGenerator(systemSettingValues.FirstOrDefault(f => f.ParamDef == "kuveytturk_rsa_private_key").ParamVal, token.Data.access_token, "POST", JsonConvert.SerializeObject(kuveytTurkTransferRequestModel), null);

                var accClass = new KuveytTurkAccountTransactionRequestModel()
                {
                    Authorization = token.Data.access_token,
                    Signature = rsa
                };

                var options = new RestClientOptions("https://gateway.kuveytturk.com.tr");
                var client = new RestClient(options);
                var request = new RestRequest("/v1/moneytransfer/outgoingmoneytransfer", Method.Post);
                request.AddHeader("Signature", rsa);
                request.AddHeader("Authorization", "Bearer " + token.Data.access_token);
                request.AddHeader("Content-Type", "application/json");

                var body = JsonConvert.SerializeObject(kuveytTurkTransferRequestModel);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                var response = client.Execute(request);

                if (response.IsSuccessStatusCode)
                {
                    var deserialize = JsonConvert.DeserializeObject<KuveytTurkTransferResponseModel.Root>(response.Content);

                    return new GenericResponseDataModel<KuveytTurkTransferResponseModel.Root>
                    {
                        Status = deserialize.success ? "OK" : "ERROR",                     
                        Data = deserialize
                    };

                }
                else
                {
                    JObject json = JObject.Parse(response.Content);

                    string errorMessage = null;

                    // Check if "results" array is not null and has elements
                    if (json["results"] != null && json["results"].HasValues)
                    {
                        errorMessage = json["results"][0]["errorMessage"].ToString();
                    }
                    // If "results" array is null or empty, check "errors" array
                    else if (json["errors"] != null && json["errors"].HasValues)
                    {
                        errorMessage = json["errors"][0]["message"].ToString();
                    }

                    return new GenericResponseDataModel<KuveytTurkTransferResponseModel.Root>
                    {
                        Status = "ERROR",
                        Message = errorMessage ?? response.Content
                    };
                }
                
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<KuveytTurkTransferResponseModel.Root>
                {
                    Status = "ERROR",
                    Message = ex.Message
                };
            }
        }
    }
}
