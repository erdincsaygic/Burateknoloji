using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace StilPay.Utility.NomuPayPos.Models.NomuPayPosTransactionQuery
{
    [XmlRoot("GetSaleResultMPAYResult")]
    public class NomuPayPosTransactionQueryRequestResponseModel
    {
        public Guid OrderObjectId { get; set; }
        public string Gsm { get; set; }
        public int GsmOperator { get; set; }
        public int GsmType { get; set; }
        public int State { get; set; }
        public int OrderChannelId { get; set; }
        public int PaymentCategoryId { get; set; }
        public DateTime LastTransactionDate { get; set; }
        public string MPAY { get; set; }
        public Guid SubscriberId { get; set; }
        public string PIN { get; set; }
        public MicroPaymentResults MicroPaymentResults { get; set; }
        public int StatusCode { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class MicroPaymentResults
    {
        [XmlElement("MMicroPaymentOutput")]
        public List<MMicroPaymentOutput> MMicroPaymentOutputs { get; set; }
    }

    public class MMicroPaymentOutput
    {
        public string PaymentObjectId { get; set; }
        public string StatusCode { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ExtraData { get; set; }
    }
    
}
