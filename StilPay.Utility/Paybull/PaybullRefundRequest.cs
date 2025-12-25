using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.Paybull.PaybullPayment;
using StilPay.Utility.Paybull.PaybullRefund;
using StilPay.Utility.Paybull.PaybullToken;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using static StilPay.Utility.PayNKolay.Models.PaymentRequest.PaymentRequestResponseModel;

namespace StilPay.Utility.Paybull
{
    public class PaybullRefundRequest
    {
        public static GenericResponseDataModel<PaybullRefundResponseModel.Root> RefundRequest(PaybullRefundRequestModel paybullRefundRequest)
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
                    var request = new RestRequest("/ccpayment/api/refund", Method.Post);
                    request.AddHeader("Authorization", "Bearer " + token.Data.data.token);
                    request.AddHeader("Accept", "application/json");
                    request.AddParameter("invoice_id", paybullRefundRequest.invoice_id);
                    request.AddParameter("amount", string.Empty);
                    request.AddParameter("app_id", tokenModel.app_id);
                    request.AddParameter("app_secret", tokenModel.app_password);
                    request.AddParameter("merchant_key", systemSettingValues.FirstOrDefault(f => f.ParamDef == "secret_key").ParamVal);
                    request.AddParameter("hash_key", paybullRefundRequest.hash_key);
                    
                    var response = client.Execute(request);
                    var deserialize = JsonConvert.DeserializeObject<PaybullRefundResponseModel.Root>(response.Content);

                    if (response.IsSuccessStatusCode)
                    {
                        return new GenericResponseDataModel<PaybullRefundResponseModel.Root>
                        {
                            Status = deserialize.status_code == 100 ? "OK" : "ERROR",
                            Data = deserialize,
                            Message = deserialize.status_description??""
                        };

                    }
                    else
                    {
                        return new GenericResponseDataModel<PaybullRefundResponseModel.Root>
                        {
                            Status = "ERROR",
                            Message = deserialize.status_description,
                        };
                    }
                }
                else
                {
                    return new GenericResponseDataModel<PaybullRefundResponseModel.Root>
                    {
                        Status = "ERROR",
                        Message = "Token Alımında Bir Hata Meydana Geldi"
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<PaybullRefundResponseModel.Root>
                {
                    Status = "ERROR",
                    Message = "Hata",
                };
            }
        }
    }
}
