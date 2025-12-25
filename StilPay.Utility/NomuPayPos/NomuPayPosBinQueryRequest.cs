using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.IsBankSanalPos.IsBankPaymentModel;
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
using static StilPay.Utility.IsBankSanalPos.IsBankSanalPOSComplatePaymentXMLResponseModel.IsBankSanalPOSComplatePaymentXMLResponseModel;
using System.Xml.Serialization;

namespace StilPay.Utility.NomuPayPos
{
    public class NomuPayPosBinQueryRequest
    {
        public static GenericResponseDataModel<NomuPayPosBinQueryRequestResponseModel> BinQueryRequest(string bin)
        {
            try
            {
                var systemSettingValues = tSQLBankManager.GetSystemSettingValues("NomuPayPos");

                var nomuPayPosBinQueryRequestModel = new NomuPayBinQueryRequestModel()
                {
                    Bin = bin,
                    Token = new Token()
                    {
                        Pin = systemSettingValues.FirstOrDefault(f => f.ParamDef == "pin").ParamVal,
                        UserCode = systemSettingValues.FirstOrDefault(f => f.ParamDef == "user_code").ParamVal
                    }
                };

                //nomuPayPosBinQueryRequestModel.Token.Pin = systemSettingValues.FirstOrDefault(f => f.ParamDef == "pin").ParamVal;
                //nomuPayPosBinQueryRequestModel.Token.Pin = systemSettingValues.FirstOrDefault(f => f.ParamDef == "user_code").ParamVal;

                XDocument xmlDocument = new XDocument(
                    new XElement("INPUT",
                        new XElement("ServiceType", nomuPayPosBinQueryRequestModel.ServiceType),
                        new XElement("OperationType", nomuPayPosBinQueryRequestModel.OperationType),
                        new XElement("Token",
                            new XElement("UserCode", nomuPayPosBinQueryRequestModel.Token.UserCode),
                            new XElement("Pin", nomuPayPosBinQueryRequestModel.Token.Pin)
                        ),
                        new XElement("Bin", nomuPayPosBinQueryRequestModel.Bin)
                    )
                );

                string xmlString = xmlDocument.ToString();

                string url = "https://www.nomupay.com.tr/SGate/Gate";

                using (WebClient client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/xml";
                    string response = client.UploadString(url, "POST", xmlString);

                    //var deserialize = JsonConvert.DeserializeObject<CC5Response>(response);
                    var deserialize = new NomuPayPosBinQueryRequestResponseModel();

                    XmlSerializer serializer = new XmlSerializer(typeof(NomuPayPosBinQueryRequestResponseModel));
                    using (StringReader reader = new StringReader(response))
                    {
                        deserialize = (NomuPayPosBinQueryRequestResponseModel)serializer.Deserialize(reader);
                    }


                    if (deserialize != null)
                    {
                        if (deserialize.StatusCode == 0)
                        {
                            return new GenericResponseDataModel<NomuPayPosBinQueryRequestResponseModel>
                            {
                                Status = "OK",
                                Data = deserialize,
                                Message = "İşlem Başarılı"
                            };
                        }
                        else
                        {
                            return new GenericResponseDataModel<NomuPayPosBinQueryRequestResponseModel>
                            {
                                Status = "ERROR",
                                Message = deserialize.ResultMessage ?? "İşlem Gerçekleştirelemedi. Lütfen Daha Sonra Tekrar Deneyiniz."
                            };
                        }
                    }
                    else
                    {
                        return new GenericResponseDataModel<NomuPayPosBinQueryRequestResponseModel>
                        {
                            Status = "ERROR",
                            Message = "Teknik Bir Nedenden Dolayı İşlem Gerçekleştirelemedi. Lütfen Daha Sonra Tekrar Deneyiniz."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new GenericResponseDataModel<NomuPayPosBinQueryRequestResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}
