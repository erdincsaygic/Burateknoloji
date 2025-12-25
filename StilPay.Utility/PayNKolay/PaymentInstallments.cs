using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.PayNKolay.Models;
using System;
using System.ComponentModel;
using System.Globalization;
using static StilPay.Utility.PayNKolay.Models.PaymentInstallment.PaymentInstallmentsResponseModel;

namespace StilPay.Utility.PayNKolay
{
    public class PayNKolayPaymentInstallments
    {
        public static ResponseModel<PaymentInstallmentsResponse> PaymentInstallmentsRequest(PaymentInstallmentRequestModel paymentInstallmentRequestModel)
        {
            try
            {
                var client = new RestClient("https://paynkolay.nkolayislem.com.tr/Vpos/Payment/PaymentInstallments");
                var request = new RestRequest
                {
                    Method = Method.Post,
                    AlwaysMultipartFormData = true
                };
                request.AddParameter("sx", paymentInstallmentRequestModel.Sx);
                request.AddParameter("amount", paymentInstallmentRequestModel.Amount.ToString(CultureInfo.InvariantCulture));
                request.AddParameter("cardNumber", paymentInstallmentRequestModel.CardNumber);
                request.AddParameter("hosturl", "http://localhost:5000/");
                request.AddParameter("iscardvalid", paymentInstallmentRequestModel.Iscardvalid);

                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<PaymentInstallmentsResponse>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new ResponseModel<PaymentInstallmentsResponse>
                    {
                        Status = deserialize.RESPONSE_CODE == 2 ? "OK" : "ERROR",
                        Message = deserialize.RESPONSE_DATA,
                        Data = deserialize
                    };
                }
                else
                {
                    return new ResponseModel<PaymentInstallmentsResponse>
                    {
                        Status = "ERROR",
                        Message = deserialize.RESPONSE_DATA,
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<PaymentInstallmentsResponse>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
