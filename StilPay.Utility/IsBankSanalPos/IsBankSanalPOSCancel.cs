using StilPay.Utility.Helper;
using StilPay.Utility.IsBankSanalPos.IsBankSanalPOSCancelModel;
using System;
using System.IO;
using System.Net;
using System.Xml.Linq;
using System.Xml.Serialization;
using static StilPay.Utility.IsBankSanalPos.IsBankSanalPOSComplatePaymentXMLResponseModel.IsBankSanalPOSComplatePaymentXMLResponseModel;

namespace StilPay.Utility.IsBankSanalPos
{
    public class IsBankSanalPOSCancel
    {
        public static GenericResponseDataModel<CC5Response> CancelRequestXML(IsBankSanalPOSCancelRequestModel isBankSanalPOSCancelRequestModel)
        {
            try
            {
                XDocument xmlDocument = new XDocument(
                new XElement("CC5Request",
                new XElement("Name", isBankSanalPOSCancelRequestModel.apiUserName),
                new XElement("Password", isBankSanalPOSCancelRequestModel.apiUserPassword),
                    new XElement("ClientId", isBankSanalPOSCancelRequestModel.clientid),
                    new XElement("OrderId", isBankSanalPOSCancelRequestModel.oid),
                    new XElement("Type", isBankSanalPOSCancelRequestModel.type)
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
