using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.Paybull.PaybullCheckStatus;
using StilPay.Utility.Paybull.PaybullGetTransactions;
using StilPay.Utility.Paybull.PaybullToken;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StilPay.Utility.Paybull
{
    public class PaybullCheckStatusRequest
    {
        public static GenericResponseDataModel<PaybullCheckStatusResponseModel> CheckStatus(PaybullCheckStatusRequestModel paybullCheckStatusRequestModel)
        {
            try
            {
                var systemSettingValues = tSQLBankManager.GetSystemSettingValues("PaybullCreditCard");

                var tokenModel = new PaybullTokenRequestModel()
                {
                    app_id = systemSettingValues.FirstOrDefault(f => f.ParamDef == "app_id").ParamVal,
                    app_password = systemSettingValues.FirstOrDefault(f => f.ParamDef == "app_password").ParamVal
                };

                var token = PaybullGetToken.GetAccessToken(tokenModel);

                if (token != null && token.Data != null && token.Status == "OK" && token.Data.data.token != null)
                {
                    var options = new RestClientOptions("https://app.paybull.com");
                    var client = new RestClient(options);
                    var request = new RestRequest("/ccpayment/api/checkstatus", Method.Post);
                    request.AddHeader("Accept", "application/json");
                    request.AddHeader("Authorization", "Bearer " + token.Data.data.token);
                    request.AddParameter("invoice_id", paybullCheckStatusRequestModel.invoice_id);
                    request.AddParameter("merchant_key", paybullCheckStatusRequestModel.merchant_key);
                    request.AddParameter("include_pending_status", paybullCheckStatusRequestModel.include_pending_status);

                    var response = client.Execute(request);
                    var deserialize = JsonConvert.DeserializeObject<PaybullCheckStatusResponseModel>(response.Content);

                    if (response.IsSuccessStatusCode)
                    {
                        return new GenericResponseDataModel<PaybullCheckStatusResponseModel>
                        {
                            Status = deserialize.status_code == 100 ? "OK" : "ERROR",
                            Data = deserialize
                        };
                    }
                    else
                    {
                        return new GenericResponseDataModel<PaybullCheckStatusResponseModel>
                        {
                            Status = "ERROR",
                            Message = deserialize.reason ?? deserialize.bank_status_description,
                        };
                    }
                }
                else
                {
                    return new GenericResponseDataModel<PaybullCheckStatusResponseModel>
                    {
                        Status = "ERROR",
                        Message = "Token Alımında Bir Hata Meydana Geldi"
                    };
                }

            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<PaybullCheckStatusResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
