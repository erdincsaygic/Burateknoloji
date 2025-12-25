using Newtonsoft.Json;
using RestSharp.Serializers;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.Paybull.PaybullRefund;
using StilPay.Utility.ToslaSanalPos.Models.ToslaGetToken;
using StilPay.Utility.ToslaSanalPos.Models.ToslaRefund;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.ToslaSanalPos
{
    public class ToslaRefundExecuteRequest
    {
        public static GenericResponseDataModel<ToslaRefundExecuteResponseModel> RefundExecuteRequest(string commandId, string token)
        {
            try
            {
                var options = new RestClientOptions("https://api.tosla.com");
                var client = new RestClient(options);

                var apiUrl = $"/api/gateway-third-party/ref-code/return/command/{commandId}/execute";

                var request = new RestRequest(apiUrl, Method.Post);
                request.AddHeader("Authorization", "Bearer " + token);
                var response = client.Execute(request);

                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<ToslaRefundExecuteResponseModel>
                    {
                        Status = "OK",
                        Data = null
                    };

                }
                else
                {
                    return new GenericResponseDataModel<ToslaRefundExecuteResponseModel>
                    {
                        Status = "ERROR",
                        Message = "error",
                    };
                }

            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<ToslaRefundExecuteResponseModel>
                {
                    Status = "ERROR",
                    Message = "Hata",
                };
            }
        }
    }
}
