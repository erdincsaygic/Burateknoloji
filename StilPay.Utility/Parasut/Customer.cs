using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Parasut.Models;
using System;

namespace StilPay.Utility.Parasut
{
    public class Customer
    {
        public static ResponseModel<CustomerResponseModel> Create(CustomerModel customer, string companyID, string baseUrl, string token)
        {
            var result = new ResponseModel<CustomerResponseModel>();
            try
            {
                var client = new RestClient(baseUrl + "/v4/" + companyID + "/contacts");
                var request = new RestRequest();
                request.Method = Method.Post;
                request.AddHeader("Accept", "application/json;charset=utf-8");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Bearer " + token);

                CustomerPurchaseModel data = null;

                if (!customer.IsAbroad)
                {
                    data = new CustomerPurchaseModel
                    {
                        data = new CustomerPurchaseModel.CustomerPurchaseRoot
                        {
                            id = "",
                            type = "contacts",
                            attributes = new CustomerPurchaseModel.CustomerPurchaseAttributes
                            {
                                email = customer.Email,
                                name = customer.Name,
                                phone = customer.Phone,
                                address = customer.Address,
                                iban = customer.Iban,
                                tax_office = customer.TaxOffice,
                                tax_number = customer.TaxNumber,
                                city = customer.City,
                                district = customer.District,
                                contact_type = "company",
                                account_type = "customer"
                            }
                        }
                    };
                }
                else
                {
                    data = new CustomerPurchaseModel
                    {
                        data = new CustomerPurchaseModel.CustomerPurchaseRoot
                        {
                            id = "",
                            type = "contacts",
                            attributes = new CustomerPurchaseModel.CustomerPurchaseAttributes
                            {
                                name = customer.Name,
                                tax_number = customer.TaxNumber,
                                is_abroad = customer.IsAbroad,
                                contact_type = "company",
                                account_type = "customer"
                            }
                        }
                    };
                }


                var jsonString = JsonConvert.SerializeObject(data);

                request.AddStringBody(jsonString, DataFormat.Json);
                RestResponse response = client.ExecuteAsync(request).Result as RestResponse;

                if (response.IsSuccessStatusCode == true)
                {
                    CustomerResponseModel customerResponse = JsonConvert.DeserializeObject<CustomerResponseModel>(response.Content);

                    result.Status = true;
                    result.Data = customerResponse;
                }
                else
                {
                    result.Status = false;
                    result.Message = response.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
