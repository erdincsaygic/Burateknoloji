using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace StilPay.Utility.IsBankSanalPos.IsBankSanalPOSComplatePaymentXMLResponseModel
{
    public class IsBankSanalPOSComplatePaymentXMLResponseModel
    {
        [XmlRoot(ElementName = "Extra")]
        public class Extra
        {

            [XmlElement(ElementName = "SETTLEID")]
            public string SETTLEID { get; set; }

            [XmlElement(ElementName = "TRXDATE")]
            public string TRXDATE { get; set; }

            [XmlElement(ElementName = "ERRORCODE")]
            public string ERRORCODE { get; set; }

            [XmlElement(ElementName = "CARDBRAND")]
            public string CARDBRAND { get; set; }

            [XmlElement(ElementName = "CARDISSUER")]
            public string CARDISSUER { get; set; }

            [XmlElement(ElementName = "CARDHOLDERNAME")]
            public string CARDHOLDERNAME { get; set; }

            [XmlElement(ElementName = "HOSTDATE")]
            public string HOSTDATE { get; set; }

            [XmlElement(ElementName = "SECMELIKAMPANYASONUC")]
            public string SECMELIKAMPANYASONUC { get; set; }

            [XmlElement(ElementName = "NUMCODE")]
            public string NUMCODE { get; set; }
        }

        [XmlRoot(ElementName = "CC5Response")]
        public class CC5Response
        {

            [XmlElement(ElementName = "OrderId")]
            public string OrderId { get; set; }

            [XmlElement(ElementName = "GroupId")]
            public string GroupId { get; set; }

            [XmlElement(ElementName = "Response")]
            public string Response { get; set; }

            [XmlElement(ElementName = "AuthCode")]
            public string AuthCode { get; set; }

            [XmlElement(ElementName = "HostRefNum")]
            public string HostRefNum { get; set; }

            [XmlElement(ElementName = "ProcReturnCode")]
            public string ProcReturnCode { get; set; }

            [XmlElement(ElementName = "TransId")]
            public string TransId { get; set; }

            [XmlElement(ElementName = "ErrMsg")]
            public string ErrMsg { get; set; }

            [XmlElement(ElementName = "Extra")]
            public Extra Extra { get; set; }
        }


    }
}
