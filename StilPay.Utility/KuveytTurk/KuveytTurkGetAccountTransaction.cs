using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.KuveytTurk.KuveytTurkAccountTransaction;
using System;
using System.Collections.Generic;
using System.Text;
using static StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPayment.IsBankPaymentResponseModel;

namespace StilPay.Utility.KuveytTurk
{
    public class KuveytTurkGetAccountTransaction
    {
        public static GenericResponseDataModel<KuveytTurkAccountTransactionResponseModel.Root> GetAccountTransaction(KuveytTurkAccountTransactionRequestModel kuveytTurkAccountTransactionRequestModel)
        {
            try
            {
                var options = new RestClientOptions("https://gateway.kuveytturk.com.tr");
                var client = new RestClient(options);
                var request = new RestRequest(kuveytTurkAccountTransactionRequestModel.url, Method.Get);
                request.AddHeader("Signature", kuveytTurkAccountTransactionRequestModel.Signature);
                request.AddHeader("Authorization", "Bearer " + kuveytTurkAccountTransactionRequestModel.Authorization);
                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<KuveytTurkAccountTransactionResponseModel.Root>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<KuveytTurkAccountTransactionResponseModel.Root>
                    {
                        Status =  deserialize.success ? "OK" : "ERROR",
                        Data = deserialize
                    };
                }
                else
                {
                    return new GenericResponseDataModel<KuveytTurkAccountTransactionResponseModel.Root>
                    {
                        Status = "ERROR",
                        Message = deserialize.message??"Hata!",
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<KuveytTurkAccountTransactionResponseModel.Root>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
