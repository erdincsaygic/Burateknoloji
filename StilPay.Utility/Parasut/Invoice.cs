using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Parasut.Models;
using System;
using System.Collections.Generic;

namespace StilPay.Utility.Parasut
{
    public class Invoice
    {
        public static ResponseModel<InvoiceResponseModel> Create(InvoiceModel invoice, string companyID, string baseUrl, string token)
        {
            var result = new ResponseModel<InvoiceResponseModel>();

            try
            {
                var client = new RestClient(baseUrl + "/v4/" + companyID + "/sales_invoices");
                var request = new RestRequest();
                request.Method = Method.Post;
                request.AddHeader("Accept", "application/json;charset=utf-8");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Bearer " + token);


                InvoicePurchaseModel data = new InvoicePurchaseModel
                {
                    data = new InvoicePurchaseModel.InvoicePurchaseRoot
                    {
                        id = "",
                        type = "sales_invoices",
                        attributes = new InvoicePurchaseModel.InvoicePurchaseAttributes
                        {
                            item_type = "invoice",
                            issue_date = invoice.InvoiceDate.ToString("yyyy-MM-dd"),
                            due_date = invoice.InvoiceDate.ToString("yyyy-MM-dd"),
                            currency = invoice.CurrencyCode,
                            payment_account_id = 1000108384,
                            payment_date = invoice.InvoiceDate.ToString("yyyy-MM-dd"),
                            cash_sale = true,
                            shipment_included = false,
                            exchange_rate = invoice.ExchangeRate
                        },
                        relationships = new InvoicePurchaseModel.InvoicePurchaseRelationships
                        {
                            details = new InvoicePurchaseModel.InvoicePurchaseDetails
                            {
                                data = new List<InvoicePurchaseModel.InvoicePurchaseRoot>
                                {
                                    new InvoicePurchaseModel.InvoicePurchaseRoot
                                    {
                                        id = "",
                                        type = "sales_invoice_details",
                                        attributes = new InvoicePurchaseModel.InvoicePurchaseAttributes
                                        {
                                            quantity = 1,
                                            unit_price = invoice.Amount,
                                            vat_rate = invoice.Tax,
                                        },
                                        relationships = new InvoicePurchaseModel.InvoicePurchaseRelationships
                                        {
                                            product = new InvoicePurchaseModel.InvoicePurchaseProduct
                                            {
                                                data = new InvoicePurchaseModel.InvoicePurchaseData
                                                {
                                                    id = invoice.ProductID,
                                                    type = "products"
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            contact = new InvoicePurchaseModel.InvoicePurchaseContact
                            {
                                data = new InvoicePurchaseModel.InvoicePurchaseData
                                {
                                    id = invoice.ContactID,
                                    type = "contacts"
                                }
                            }
                        }
                    }
                };

                var jsonString = JsonConvert.SerializeObject(data);

                request.AddStringBody(jsonString, DataFormat.Json);
                RestResponse response = client.ExecuteAsync(request).Result as RestResponse;

                if (response.IsSuccessStatusCode == true)
                {
                    InvoiceResponseModel invoiceResponse = JsonConvert.DeserializeObject<InvoiceResponseModel>(response.Content);

                    result.Status = true;
                    result.Data = invoiceResponse;
                }
                else
                {
                    result.Status = false;
                    result.Message = response.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = false;
            }

            return result;
        }
    }
}
