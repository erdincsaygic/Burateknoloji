using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.IsBankSanalPos.IsBankPaymentModel;
using StilPay.Utility.PayNKolay.Models;
using StilPay.Utility.PayNKolay.Models.PaymentRequest;
using System;
using System.Globalization;
using System.Net.Http;
using static StilPay.Utility.PayNKolay.Models.PaymentRequest.PaymentRequestResponseModel;

namespace StilPay.Utility.IsBankSanalPos
{
    public class IsBankSanalPOS
    {
        public static string PaymentRequest(IsBankSanalPosPaymentRequestModel isBankPaymentRequestModel)
        {
            try
            {

                var client = new RestClient("https://sanalpos.isbank.com.tr/fim/est3Dgate");
                var request = new RestRequest
                {
                    Method = Method.Post,
                    AlwaysMultipartFormData = true
                };
                request.AddParameter("clientid", isBankPaymentRequestModel.clientid);
                request.AddParameter("storetype", isBankPaymentRequestModel.storetype);
                request.AddParameter("hash", isBankPaymentRequestModel.hash);
                request.AddParameter("islemtipi", isBankPaymentRequestModel.islemtipi);
                request.AddParameter("amount", isBankPaymentRequestModel.amount.ToString(CultureInfo.InvariantCulture));
                request.AddParameter("currency", isBankPaymentRequestModel.currency);
                request.AddParameter("oid", isBankPaymentRequestModel.oid);
                request.AddParameter("okUrl", isBankPaymentRequestModel.okUrl);
                request.AddParameter("failUrl", isBankPaymentRequestModel.failUrl);
                request.AddParameter("lang", isBankPaymentRequestModel.lang);
                request.AddParameter("pan", isBankPaymentRequestModel.pan.Replace(" ", ""));
                request.AddParameter("Ecom_Payment_Card_ExpDate_Year", isBankPaymentRequestModel.Ecom_Payment_Card_ExpDate_Year);
                request.AddParameter("Ecom_Payment_Card_ExpDate_Month", isBankPaymentRequestModel.Ecom_Payment_Card_ExpDate_Month);
                request.AddParameter("hashAlgorithm", isBankPaymentRequestModel.hashAlgorithm);

				var response = client.Execute(request);

                if (response.IsSuccessStatusCode)
                {
                    return response.Content;

                }
                else
                {
                    return "Şu anda İşleminiz Gerçekleştirilemiyor. Lütfen Bir Süre Sonra Tekrar Deneyiniz.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
