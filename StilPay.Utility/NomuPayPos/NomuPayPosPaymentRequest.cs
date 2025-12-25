using DocumentFormat.OpenXml.InkML;
using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.LidioPos.Models.LidioPosPaymentRequest;
using StilPay.Utility.NomuPayPos.Models.NomuPayBinQuery;
using StilPay.Utility.NomuPayPos.Models.NomuPayPaymentRequest;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace StilPay.Utility.NomuPayPos
{
    public class NomuPayPosPaymentRequest
    {
        public static GenericResponseDataModel<NomuPayPosPaymentRequestResponseModel> PaymentRequest(NomuPayPosPaymentRequestModel nomuPayPosPaymentRequestModel)
        {
            try
            {
                XDocument xmlDocument = new XDocument(
                    new XElement("INPUT",
                        new XElement("ServiceType", nomuPayPosPaymentRequestModel.ServiceType),
                        new XElement("OperationType", nomuPayPosPaymentRequestModel.OperationType),
                    
                        new XElement("Token",
                            new XElement("UserCode", nomuPayPosPaymentRequestModel.Token.UserCode),               
                            new XElement("Pin", nomuPayPosPaymentRequestModel.Token.Pin)
                        ),
                        new XElement("CreditCardInfo",
                            new XElement("CreditCardNo", nomuPayPosPaymentRequestModel.CreditCardInfo.CreditCardNo),
                            new XElement("OwnerName", nomuPayPosPaymentRequestModel.CreditCardInfo.OwnerName),
                            new XElement("ExpireYear", nomuPayPosPaymentRequestModel.CreditCardInfo.ExpireYear),
                            new XElement("ExpireMonth", nomuPayPosPaymentRequestModel.CreditCardInfo.ExpireMonth),
                            new XElement("Cvv", nomuPayPosPaymentRequestModel.CreditCardInfo.Cvv),
                            new XElement("Price", nomuPayPosPaymentRequestModel.CreditCardInfo.Price)
                        ),
                        new XElement("Language", nomuPayPosPaymentRequestModel.Language),
                        new XElement("MPAY", nomuPayPosPaymentRequestModel.MPAY),
                        new XElement("CurrencyCode", nomuPayPosPaymentRequestModel.CurrencyCode),
                        new XElement("IPAddress", nomuPayPosPaymentRequestModel.IPAddress),
                        new XElement("PaymentContent", nomuPayPosPaymentRequestModel.PaymentContent),
                        new XElement("InstallmentCount", nomuPayPosPaymentRequestModel.InstallmentCount),
                        new XElement("SuccessURL", nomuPayPosPaymentRequestModel.SuccessURL),
                        new XElement("ErrorURL", nomuPayPosPaymentRequestModel.ErrorURL)
                    )
                );

                string xmlString = xmlDocument.ToString();

                string url = "https://www.nomupay.com.tr/SGate/Gate";

                using (WebClient client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/xml";
                    string response = client.UploadString(url, "POST", xmlString);

                    var deserialize = new NomuPayPosPaymentRequestResponseModel();

                    XmlSerializer serializer = new XmlSerializer(typeof(NomuPayPosPaymentRequestResponseModel));
                    using (StringReader reader = new StringReader(response))
                    {
                        deserialize = (NomuPayPosPaymentRequestResponseModel)serializer.Deserialize(reader);
                    }


                    if (deserialize != null)
                    {
                        if (deserialize.StatusCode == 0)
                        {
                            return new GenericResponseDataModel<NomuPayPosPaymentRequestResponseModel>
                            {
                                Status = "OK",
                                Data = deserialize,
                                Message = "İşlem Başarılı"
                            };
                        }
                        else
                        {
                            return new GenericResponseDataModel<NomuPayPosPaymentRequestResponseModel>
                            {
                                Status = "ERROR",
                                Message = deserialize.ResultMessage ?? "İşlem Gerçekleştirelemedi. Lütfen Daha Sonra Tekrar Deneyiniz."
                            };
                        }
                    }
                    else
                    {
                        return new GenericResponseDataModel<NomuPayPosPaymentRequestResponseModel>
                        {
                            Status = "ERROR",
                            Message = "Teknik Bir Nedenden Dolayı İşlem Gerçekleştirelemedi. Lütfen Daha Sonra Tekrar Deneyiniz."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<NomuPayPosPaymentRequestResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}

