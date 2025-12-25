using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.KuveytTurk.KuveytTurkTransfer.Models.KuveytTurkToken;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.KuveytTurk.KuveytTurkTransfer
{
    public class KuveytTurkTransfer
    {
        public static GenericResponseDataModel<KuveytTurkTokenResponseModel> Transfer()
        {
            try
            {
                var options = new RestClientOptions("https://prep-gateway.kuveytturk.com.tr")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/v1/moneytransfer/outgoingmoneytransfer", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Bearer 887bc5897f9f4345bde4af9caffc2899456aafbb475d492986efe93bd5d1c3671676995872");
                request.AddHeader("Signature", "fFQ99koClQBLbkYvSx50JDX3aOMVn09oBG9uxySH3Qz87wJxWMq5mE9SctibGOLlq+tz3ZJWIhby/TUBOnd5jPJYnAyRBKOpZ/ah5dV7JdxxgosQaThrCRSy4EDaTO7LSb38+uScr/BoSRJaUINRehJRgF7eVIMwS8nlwiVlmqc=");
                var body = @"{
" + "\n" +
                @"   ""CorporateWebUserName"":""gokcul"",
" + "\n" +
                @"   ""MoneyTransferAmount"":1,
" + "\n" +
                @"   ""MoneyTransferDescription"":""AndmeCuzdanOdemesi"",
" + "\n" +
                @"   ""ReceiverIBAN"":""TR470011100000000102561933"",
" + "\n" +
                @"   ""ReceiverName"":""Furkan"",
" + "\n" +
                @"   ""SenderAccountSuffix"":5,
" + "\n" +
                @"   ""TransactionGuid"":""954d5d41-a5ee-4a3a-ae7e-605a43574622"",
" + "\n" +
                @"   ""TransferType"":2
" + "\n" +
                @"}";
                request.AddStringBody(body, DataFormat.Json);
                var response = client.Execute(request);

                var deserialize = JsonConvert.DeserializeObject<KuveytTurkTokenResponseModel>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<KuveytTurkTokenResponseModel>
                    {
                        Status = !string.IsNullOrEmpty(deserialize.access_token) ? "OK" : "ERROR",
                        Data = deserialize
                    };
                }
                else
                {
                    return new GenericResponseDataModel<KuveytTurkTokenResponseModel>
                    {
                        Status = "ERROR",
                        Message = "Token Alımında Bir Hata Meydana Geldi"
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<KuveytTurkTokenResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
