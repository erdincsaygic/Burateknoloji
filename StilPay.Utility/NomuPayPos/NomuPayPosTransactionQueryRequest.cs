using StilPay.Utility.Helper;
using StilPay.Utility.NomuPayPos.Models.NomuPayBinQuery;
using StilPay.Utility.NomuPayPos.Models.NomuPayPosTransactionQuery;
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
    public class NomuPayPosTransactionQueryRequest
    {
        public static GenericResponseDataModel<NomuPayPosTransactionQueryRequestResponseModel> TransactionQueryRequest(string transactionID)
        {
            try
            {
                var systemSettingValues = tSQLBankManager.GetSystemSettingValues("NomuPayPos");

                XDocument xmlDocument = new XDocument(
                    new XElement("token",
                        new XElement("UserCode", systemSettingValues.FirstOrDefault(f => f.ParamDef == "user_code").ParamVal),
                        new XElement("Pin", systemSettingValues.FirstOrDefault(f => f.ParamDef == "pin").ParamVal)
                    ),
                    new XElement("MPAY", transactionID)                
                );

                string xmlString = xmlDocument.ToString();

                string url = "https://www.nomupay.com.tr/SGate/Gate";

                using (WebClient client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/xml";
                    string response = client.UploadString(url, "POST", xmlString);

                    //var deserialize = JsonConvert.DeserializeObject<CC5Response>(response);
                    var deserialize = new NomuPayPosTransactionQueryRequestResponseModel();

                    XmlSerializer serializer = new XmlSerializer(typeof(NomuPayPosTransactionQueryRequestResponseModel));
                    using (StringReader reader = new StringReader(response))
                    {
                        deserialize = (NomuPayPosTransactionQueryRequestResponseModel)serializer.Deserialize(reader);
                    }


                    if (deserialize != null)
                    {
                        if (deserialize.State == 100)
                        {
                            return new GenericResponseDataModel<NomuPayPosTransactionQueryRequestResponseModel>
                            {
                                Status = "OK",
                                Data = deserialize,
                                Message = "İşlem Başarılı"
                            };
                        }
                        else
                        {
                            return new GenericResponseDataModel<NomuPayPosTransactionQueryRequestResponseModel>
                            {
                                Status = "ERROR",
                                Message = deserialize.ErrorMessage ?? "İşlem Gerçekleştirelemedi. Lütfen Daha Sonra Tekrar Deneyiniz."
                            };
                        }
                    }
                    else
                    {
                        return new GenericResponseDataModel<NomuPayPosTransactionQueryRequestResponseModel>
                        {
                            Status = "ERROR",
                            Message = "Teknik Bir Nedenden Dolayı İşlem Gerçekleştirelemedi. Lütfen Daha Sonra Tekrar Deneyiniz."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<NomuPayPosTransactionQueryRequestResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
