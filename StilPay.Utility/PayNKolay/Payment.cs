using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.PayNKolay.Models;
using StilPay.Utility.PayNKolay.Models.PaymentRequest;
using System;
using System.Globalization;
using static StilPay.Utility.PayNKolay.Models.PaymentRequest.PaymentRequestResponseModel;

namespace StilPay.Utility.PayNKolay
{
    public class PayNKolayPayment
    {
        public static ResponseModel<PaymentRequestResponse> PaymentRequest(PaymentRequestModel paymentRequestModel)
        {
            try
            {
                var client = new RestClient("https://paynkolay.nkolayislem.com.tr/Vpos/Payment/Payment");
                var request = new RestRequest
                {
                    Method = Method.Post,
                    AlwaysMultipartFormData = true
                };
                request.AddParameter("sx", paymentRequestModel.Sx);
                request.AddParameter("clientRefCode", paymentRequestModel.ClientRefCode);
                request.AddParameter("successUrl", paymentRequestModel.SuccessUrl);
                request.AddParameter("failUrl", paymentRequestModel.FailUrl);
                request.AddParameter("amount", paymentRequestModel.InstallmentAmount.ToString(CultureInfo.InvariantCulture));
                request.AddParameter("installmentNo", paymentRequestModel.InstallmentMonth);
                request.AddParameter("cardHolderName", paymentRequestModel.SenderName);
                request.AddParameter("month", paymentRequestModel.ExpirationDateMonth);
                request.AddParameter("year", paymentRequestModel.ExpirationDateYear);
                request.AddParameter("cvv", paymentRequestModel.SecurityCode);
                request.AddParameter("cardNumber", paymentRequestModel.CardNumber.Replace(" ", ""));
                request.AddParameter("EncodedValue", paymentRequestModel.EncodedValue);
                request.AddParameter("use3D", paymentRequestModel.Use3D);
                request.AddParameter("transactionType", "SALES");
                request.AddParameter("hosturl", "http://localhost:63352");
                request.AddParameter("rnd", paymentRequestModel.Rnd);
                request.AddParameter("hashData", paymentRequestModel.HashData);
                request.AddParameter("environment", "API");

                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<PaymentRequestResponse>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new ResponseModel<PaymentRequestResponse>
                    {
                        Status = deserialize.RESPONSE_CODE == 2 ? "OK" : "ERROR",
                        Message = deserialize.RESPONSE_DATA,
                        Data = deserialize
                    };

                }
                else
                {
                    return new ResponseModel<PaymentRequestResponse>
                    {
                        Status = "ERROR",
                        Message = deserialize.RESPONSE_DATA,
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<PaymentRequestResponse>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
