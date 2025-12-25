namespace StilPay.Utility.EsnekPos.Models.EsnekPosCancelAndRefund
{
    public class EsnekPosCancelAndRefundRequestModel
    {
        public string MERCHANT { get; set; }
        public string MERCHANT_KEY { get; set; }
        public string ORDER_REF_NUMBER { get; set; }
        public decimal AMOUNT { get; set; }
        public string SYNC_WITH_POS { get; set; }
    }
}
