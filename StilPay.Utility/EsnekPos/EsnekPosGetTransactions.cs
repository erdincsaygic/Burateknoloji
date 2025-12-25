using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.EsnekPos.Models.EsnekPosGetTransactions;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Linq;

namespace StilPay.Utility.EsnekPos
{
    public class EsnekPosGetTransactions
    {
        public static GenericResponseDataModel<EsnekPosGetTransactionsRequestResponseModel> GetTransactions(EsnekPosGetTransactionsRequestModel esnekPosGetTransactionsRequestModel)
        {
            try
            {
                var systemSettingValues = tSQLBankManager.GetSystemSettingValues("EsnekPos");

                esnekPosGetTransactionsRequestModel.MERCHANT = systemSettingValues.FirstOrDefault(f => f.ParamDef == "merchant").ParamVal;
                esnekPosGetTransactionsRequestModel.MERCHANT_KEY = systemSettingValues.FirstOrDefault(f => f.ParamDef == "merchant_key").ParamVal;

                var options = new RestClientOptions("https://posservice.esnekpos.com");
                var client = new RestClient(options);
                var request = new RestRequest("/api/services/GetPaymentList", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = JsonConvert.SerializeObject(esnekPosGetTransactionsRequestModel);
                request.AddStringBody(body, DataFormat.Json);

                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<EsnekPosGetTransactionsRequestResponseModel>(response.Content);


                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<EsnekPosGetTransactionsRequestResponseModel>
                    {
                        Status = deserialize.STATUS == "SUCCESS" && deserialize.RETURN_CODE == "0" ? "OK" : "ERROR",
                        Data = deserialize,
                        Message = deserialize.RETURN_MESSAGE ?? ""
                    };
                }
                else
                {
                    return new GenericResponseDataModel<EsnekPosGetTransactionsRequestResponseModel>
                    {
                        Status = "ERROR",
                        Data = deserialize,
                        Message = deserialize.RETURN_MESSAGE ?? ""
                    };
                }

            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<EsnekPosGetTransactionsRequestResponseModel>
                {
                    Status = "ERROR",
                    Message = "Hata " + ex.Message,
                };
            }
        }
    }
}
