using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.Paybull.PaybullGetTransactions;
using StilPay.Utility.Paybull.PaybullPayment;
using StilPay.Utility.Paybull.PaybullRefund;
using StilPay.Utility.Paybull.PaybullToken;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace StilPay.Utility.Paybull
{
    public class PaybullGetTransaction
    {
        public static GenericResponseDataModel<PaybullGetTransactionResponseModel.Root> GetTransaction(PaybullGetTransactionRequestModel paybullGetTransactionRequestModel)
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
                    var request = new RestRequest("ccpayment/api/getTransactions", Method.Post);
                    request.AddHeader("Authorization", "Bearer " + token.Data.data.token);
                    request.AddHeader("Accept", "application/json");
                    request.AddParameter("merchant_key", paybullGetTransactionRequestModel.merchant_key);
                    request.AddParameter("hash_key", paybullGetTransactionRequestModel.hash_key);
                    request.AddParameter("date", paybullGetTransactionRequestModel.date);

                    var response = client.Execute(request);
                    var deserialize = JsonConvert.DeserializeObject<PaybullGetTransactionResponseModel.Root>(response.Content);

                    if (response.IsSuccessStatusCode)
                    {
                        return new GenericResponseDataModel<PaybullGetTransactionResponseModel.Root>
                        {
                            Status = deserialize.status_code == 100 ? "OK" : "ERROR",
                            Data = deserialize,
                            Message = deserialize.status_description ?? ""
                        };

                    }
                    else
                    {
                        return new GenericResponseDataModel<PaybullGetTransactionResponseModel.Root>
                        {
                            Status = "ERROR",
                            Message = deserialize.status_description,
                        };
                    }
                }
                else
                {
                    return new GenericResponseDataModel<PaybullGetTransactionResponseModel.Root>
                    {
                        Status = "ERROR",
                        Message = "Token Alımında Bir Hata Meydana Geldi"
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<PaybullGetTransactionResponseModel.Root>
                {
                    Status = "ERROR",
                    Message = "Hata",
                };
            }
        }
    }
}
