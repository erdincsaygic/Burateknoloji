using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.EsnekPos.Models.EsnekPosPaymentRequest;
using StilPay.Utility.EsnekPos.Models.EsnekPosTransactionQuery;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StilPay.Utility.EsnekPos
{
    public class EsnekPosTransactionQueryRequest
    {
        public static GenericResponseDataModel<EsnekPosTransactionQueryRequestResponseModel> TransactionQueryRequest(EsnekPosTransactionQueryRequestModel esnekPosTransactionQueryRequestModel)
        {
            try
            {
                var systemSettingValues = tSQLBankManager.GetSystemSettingValues("EsnekPos");

                esnekPosTransactionQueryRequestModel.MERCHANT = systemSettingValues.FirstOrDefault(f => f.ParamDef == "merchant").ParamVal;
                esnekPosTransactionQueryRequestModel.MERCHANT_KEY = systemSettingValues.FirstOrDefault(f => f.ParamDef == "merchant_key").ParamVal;

                var options = new RestClientOptions("https://posservice.esnekpos.com");
                var client = new RestClient(options);
                var request = new RestRequest("/api/services/ProcessQuery", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = JsonConvert.SerializeObject(esnekPosTransactionQueryRequestModel);
                request.AddStringBody(body, DataFormat.Json);

                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<EsnekPosTransactionQueryRequestResponseModel>(response.Content);


                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<EsnekPosTransactionQueryRequestResponseModel>
                    {
                        Status = deserialize.STATUS == "SUCCESS" && deserialize.RETURN_CODE == "0" ? "OK" : "ERROR",
                        Data = deserialize,
                        Message = deserialize.RETURN_MESSAGE ?? ""
                    };
                }
                else
                {
                    return new GenericResponseDataModel<EsnekPosTransactionQueryRequestResponseModel>
                    {
                        Status = "ERROR",
                        Data = deserialize,
                        Message = deserialize.RETURN_MESSAGE ?? ""
                    };
                }

            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<EsnekPosTransactionQueryRequestResponseModel>
                {
                    Status = "ERROR",
                    Message = "Hata " + ex.Message,
                };
            }
        }
    }
}
