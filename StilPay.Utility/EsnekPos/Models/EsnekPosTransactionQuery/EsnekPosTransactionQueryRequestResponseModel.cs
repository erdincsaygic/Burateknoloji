using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.EsnekPos.Models.EsnekPosTransactionQuery
{
    public class EsnekPosTransactionQueryRequestResponseModel
    {
        public string STATUS { get; set; }
        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public string DATE { get; set; }
        public string PAYMENT_DATE { get; set; }
        public string REFNO { get; set; }
        public string AMOUNT { get; set; }
        public string ORDER_REF_NO { get; set; }
        public string INSTALLMENT { get; set; }
        public string COMMISSION { get; set; }
        public List<Transaction> TRANSACTIONS { get; set; }
    }

    public class Transaction
    {
        public int TRANSACTION_ID { get; set; }
        public string STATUS_NAME { get; set; }
        public int STATUS_ID { get; set; }
        public string AMOUNT { get; set; }
        public string DATE { get; set; }
        public MerchantAmountTransferDetail MERCHANT_AMOUNT_TRANSFER_DETAIL { get; set; }
        public List<SubMerchantDetail> SUB_MERCHANT_DETAILS { get; set; }
    }

    public class MerchantAmountTransferDetail
    {
        public int EXTRACT_ID { get; set; }
        public string SENDED_AMOUNT { get; set; }
        public string SENDED_DATE { get; set; }
    }

    public class SubMerchantDetail
    {
        public string EXTERNAL_ID { get; set; }
        public string AMOUNT { get; set; }
        public string DATE { get; set; }
        public SubMerchantAmountTransferDetail SUB_MERCHANT_AMOUNT_TRANSFER_DETAIL { get; set; }
    }

    public class SubMerchantAmountTransferDetail
    {
        public int EXTRACT_ID { get; set; }
        public string SENDED_AMOUNT { get; set; }
        public string SENDED_DATE { get; set; }
    }
}
