using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.AKODESanalPOS.Models.AKODECreditCardInfo;
using StilPay.Utility.AKODESanalPOS.Models.AKODETransactionQuery;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.AKODESanalPOS
{
    public class AKODECreditCardInfoRequest
    {
        public static GenericResponseDataModel<AKODECreditCardInfoResponseModel.CardInfo> CreditCardInfoRequest(AKODECreditCardInfoRequestModel akOdeCreditCardInfoRequestModel)
        {
            try
            {
                var options = new RestClientOptions("https://api.akodepos.com/api/Payment/")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("GetCommissionAndInstallmentInfo", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = JsonConvert.SerializeObject(akOdeCreditCardInfoRequestModel);
                request.AddStringBody(body, DataFormat.Json);
                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<AKODECreditCardInfoResponseModel.CardInfo>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<AKODECreditCardInfoResponseModel.CardInfo>
                    {
                        Status = deserialize.Code == 0 ? "OK" : "ERROR",
                        Data = deserialize
                    };
                }
                else
                {
                    return new GenericResponseDataModel<AKODECreditCardInfoResponseModel.CardInfo>
                    {
                        Status = "ERROR",
                        Message = deserialize.Message ?? "Hata!",
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<AKODECreditCardInfoResponseModel.CardInfo>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
