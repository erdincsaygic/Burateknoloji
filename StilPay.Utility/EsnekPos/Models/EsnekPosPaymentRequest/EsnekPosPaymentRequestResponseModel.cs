namespace StilPay.Utility.EsnekPos.Models.EsnekPosPaymentRequest
{
    public class EsnekPosPaymentRequestResponseModel
    {
        public string ORDER_REF_NUMBER { get; set; }
        public string STATUS { get; set; }
        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public string RETURN_MESSAGE_TR { get; set; }
        public string ERROR_CODE { get; set; }
        public string DATE { get; set; }
        public string URL_3DS { get; set; }
        public string REFNO { get; set; }
        public string HASH { get; set; }
        public string COMMISSION_RATE { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string CUSTOMER_MAIL { get; set; }
        public string CUSTOMER_PHONE { get; set; }
        public string CUSTOMER_ADDRESS { get; set; }
        public string CUSTOMER_CC_NUMBER { get; set; }
        public string CUSTOMER_CC_NAME { get; set; }
        public bool IS_NOT_3D_PAYMENT { get; set; }
        public string VIRTUAL_POS_VALUES { get; set; }
        public string RETURN_MESSAGE_3D { get; set; }
        public string BANK_AUTH_CODE { get; set; }
    }
}
