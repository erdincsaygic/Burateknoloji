using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace StilPay.Utility.NomuPayPos.Models.NomuPayPaymentRequest
{
    [XmlRoot("Result")]
    public class NomuPayPosPaymentRequestResponseModel
    {
        [XmlElement("Item")]
        public List<Item> Items { get; set; }

        [XmlIgnore]
        public int StatusCode => int.Parse(GetItemValue("StatusCode"));

        [XmlIgnore]
        public string ResultCode => GetItemValue("ResultCode");

        [XmlIgnore]
        public string ResultMessage => GetItemValue("ResultMessage");

        [XmlIgnore]
        public string OrderObjectId => GetItemValue("OrderObjectId");

        [XmlIgnore]
        public string LastTransactionDate => GetItemValue("LastTransactionDate");

        [XmlIgnore]
        public string MaskedCreditCardNumber => GetItemValue("MaskedCreditCardNumber");

        [XmlIgnore]
        public string MPAY => GetItemValue("MPAY");

        [XmlIgnore]
        public string RedirectUrl => GetItemValue("RedirectUrl");     

        private string GetItemValue(string key)
        {
            var item = Items?.Find(i => i.Key == key);
            return item != null ? item.Value : null;
        }
    }

    public class Item
    {
        [XmlAttribute("Key")]
        public string Key { get; set; }

        [XmlAttribute("Value")]
        public string Value { get; set; }
    }


}
