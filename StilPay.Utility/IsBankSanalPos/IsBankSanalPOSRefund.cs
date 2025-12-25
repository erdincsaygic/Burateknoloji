using StilPay.Utility.Helper;
using StilPay.Utility.IsBankSanalPos.IsBankSanalPOSRefundModel;
using System;
using System.IO;
using System.Net;
using System.Xml.Linq;
using System.Xml.Serialization;
using static StilPay.Utility.IsBankSanalPos.IsBankSanalPOSComplatePaymentXMLResponseModel.IsBankSanalPOSComplatePaymentXMLResponseModel;

namespace StilPay.Utility.IsBankSanalPos
{
    public class IsBankSanalPOSRefund
    {
        public static GenericResponseDataModel<CC5Response> RefundRequestXML(IsBankSanalPOSRefundRequestModel isBankSanalPOSRefundRequestModel)
        {
            try
            {
                XDocument xmlDocument = new XDocument(
                new XElement("CC5Request",
                new XElement("Name", isBankSanalPOSRefundRequestModel.apiUserName),
                new XElement("Password", isBankSanalPOSRefundRequestModel.apiUserPassword),
                    new XElement("ClientId", isBankSanalPOSRefundRequestModel.clientid),
                    new XElement("OrderId", isBankSanalPOSRefundRequestModel.oid),
                    new XElement("Type", isBankSanalPOSRefundRequestModel.type)
                    )
                );

                string xmlString = xmlDocument.ToString();

                string url = "https://sanalpos.isbank.com.tr/fim/api";

                using (WebClient client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/xml";
                    string response = client.UploadString(url, "POST", xmlString);

                    //var deserialize = JsonConvert.DeserializeObject<CC5Response>(response);
                    var deserialize = new CC5Response();

                    XmlSerializer serializer = new XmlSerializer(typeof(CC5Response));
                    using (StringReader reader = new StringReader(response))
                    {
                        deserialize = (CC5Response)serializer.Deserialize(reader);
                    }


                    if (deserialize != null)
                    {
                        if (deserialize.ProcReturnCode == "00" && deserialize.AuthCode != null && deserialize.Response == "Approved")
                        {
                            return new GenericResponseDataModel<CC5Response>
                            {
                                Status = "OK",
                                Data = deserialize,
                                Message = "İşlem Başarılı"
                            };
                        }
                        else
                        {
                            return new GenericResponseDataModel<CC5Response>
                            {
                                Status = "ERROR",
                                Message = deserialize.ErrMsg != null && deserialize.ErrMsg != "" ? deserialize.ErrMsg : "İşlem Gerçekleştirelemedi. Lütfen Daha Sonra Tekrar Deneyiniz."
                            };
                        }
                    }
                    else
                    {
                        return new GenericResponseDataModel<CC5Response>
                        {
                            Status = "ERROR",
                            Message = "Teknik Bir Nedenden Dolayı İşlem Gerçekleştirelemedi. Lütfen Daha Sonra Tekrar Deneyiniz."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<CC5Response>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
