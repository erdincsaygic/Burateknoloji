using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.AutoNotificationCheckReferenceNr.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace StilPay.Utility.AutoNotificationCheckReferenceNr
{
    public class AutoNotificationCheckReferenceNrRequest
    {
        public static AutoNotificationCheckReferenceNrResponseModel CheckReferenceNrRequest(string referenceNr, string requestUrl)
        {
            try
            {
                var options = new RestClientOptions(requestUrl);
                var client = new RestClient(options);

                var request = new RestRequest(requestUrl, Method.Get);
                request.AddHeader("accept", "application/json");
                request.AddHeader("Content-Type", "application/json");

                request.AddQueryParameter("reference_number", referenceNr);

                var response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<AutoNotificationCheckReferenceNrResponseModel>(response.Content);
                }
                else
                    return new AutoNotificationCheckReferenceNrResponseModel()
                    {
                        status = "",
                        error = response.ErrorMessage
                    };
            }
            catch (Exception ex)
            {
                return new AutoNotificationCheckReferenceNrResponseModel()
                {
                    status = "",
                    error = ex.Message

                };
            }
        }
    }
}