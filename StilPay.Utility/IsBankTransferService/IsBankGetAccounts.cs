using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.IsBankTransferService.IsBankTokenModel;
using StilPay.Utility.IsBankTransferService.Models.IsBankAccounts;
using StilPay.Utility.IsBankTransferService.Models.IsBankToken;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.IsBankTransferService
{
    public class IsBankGetAccounts
    {
        public static GenericResponseDataModel<IsBankAccountsResponseModel.Root> GetAccounts(IsBankAccountsRequestModel isBankAccountsRequestModel)
        {
            try
            {
                var options = new RestClientOptions("https://api.isbank.com.tr");
                var client = new RestClient(options);
                var request = new RestRequest("/api/isbank/v2/accounts", Method.Get);
                request.AddHeader("X-Isbank-Client-Id", isBankAccountsRequestModel.client_id);
                request.AddHeader("X-Isbank-Client-Secret", isBankAccountsRequestModel.client_secret);
                request.AddHeader("X-Client-Certificate", isBankAccountsRequestModel.certificate);
                request.AddHeader("Authorization", "Bearer " + isBankAccountsRequestModel.authorization); 
                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<IsBankAccountsResponseModel.Root>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponseDataModel<IsBankAccountsResponseModel.Root>
                    {
                        Status = deserialize.data != null && deserialize.httpCode == null ? "OK" : "ERROR",
                        Data = deserialize
                    };
                }
                else
                {
                    return new GenericResponseDataModel<IsBankAccountsResponseModel.Root>
                    {
                        Status = "ERROR",
                        Message = deserialize.httpMessage??deserialize.moreInformation
                    };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<IsBankAccountsResponseModel.Root>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
