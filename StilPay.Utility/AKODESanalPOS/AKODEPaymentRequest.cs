using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.AKODESanalPOS.Models.AKODEPaymentRequest;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.AKODESanalPOS
{
    public class AKODEPaymentRequest
    {
        public static string PaymentRequest(AKODEPaymentRequestModel akOdePaymentRequestModel)
        {
            try
            {
                var options = new RestClientOptions("https://api.akodepos.com/api/Payment/")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("ProcessCardForm", Method.Post);
                request.AlwaysMultipartFormData = true;
                request.AddParameter("ThreeDSessionId", akOdePaymentRequestModel.ThreeDSessionId);
                request.AddParameter("CardHolderName", akOdePaymentRequestModel.CardHolderName);
                request.AddParameter("CardNo", akOdePaymentRequestModel.CardNo);
                request.AddParameter("ExpireDate", akOdePaymentRequestModel.ExpireDate);
                request.AddParameter("Cvv", akOdePaymentRequestModel.Cvv);
                var response = client.Execute(request);

                if (response.IsSuccessStatusCode)
                {
                    return response.Content;

                }
                else
                {
                    return "1001 - Şu anda İşleminiz Gerçekleştirilemiyor. Lütfen Bir Süre Sonra Tekrar Deneyiniz.";
                }
            }
            catch (Exception ex)
            {
                return "1001 - Şu anda İşleminiz Gerçekleştirilemiyor. Lütfen Bir Süre Sonra Tekrar Deneyiniz.";
            }
        }
    }
}
