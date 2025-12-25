using System;
using System.Collections.Generic;


namespace StilPay.Utility.EsnekPos.Models.EsnekPosGetTransactions
{
    public class EsnekPosGetTransactionsRequestResponseModel
    {
        public List<Payment> PaymentList { get; set; }
        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public string STATUS { get; set; }
    }

    public class Payment
    {
        public int ID { get; set; }
        public string MERCHANT { get; set; }
        public int DEALERID { get; set; }
        public int DEALER_CODE { get; set; }
        public string INSERT_DATETIME { get; set; }
        public string CARD_TYPE { get; set; }
        public string CARD_NUMBER { get; set; }
        public string CARD_NAME { get; set; }
        public string CARD_BANK_NAME { get; set; }
        public string CARD_FAMILY { get; set; }
        public string CURRENCY { get; set; }
        public string VIRTUALPOS_NAME { get; set; }
        public string STATUS_NAME { get; set; }
        public int STATUS_ID { get; set; }
        public int INSTALLMENT { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal COMMISSION_AMOUNT { get; set; }
        public string DEALER_PAYMENT_REF_CODE { get; set; }
        public decimal COMMISSION_RATE { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string CUSTOMER_GSM { get; set; }
        public string USER_NAME { get; set; }
        public bool CANCEL_REQUEST { get; set; }
        public string DEALER_NAME { get; set; }
        public string JSONDATE { get; set; }
        public string JSONPAYMENT { get; set; }
        public string CC_HASH { get; set; }
        public string PAYMENT_BANK_CODE { get; set; }
        public List<Transaction> TRANSACTIONS { get; set; }
        public decimal? VIRTUAL_POS_COMMISSION_AMOUNT { get; set; } // Nullable decimal for optional fields
        public List<Product> PRODUCTS { get; set; }
    }

    public class Transaction
    {
        public int TRANSACTION_ID { get; set; }
        public string STATUS_NAME { get; set; }
        public int STATUS_ID { get; set; }
        public decimal AMOUNT { get; set; }
        public string DATE { get; set; }
        public MerchantAmountTransferDetail MERCHANT_AMOUNT_TRANSFER_DETAIL { get; set; }
        public List<SubMerchantDetail> SUB_MERCHANT_DETAILS { get; set; }
    }

    public class MerchantAmountTransferDetail
    {
        public int EXTRACT_ID { get; set; }
        public decimal SENDED_AMOUNT { get; set; }
        public string SENDED_DATE { get; set; }
    }

    public class SubMerchantDetail
    {
        public string EXTERNAL_ID { get; set; }
        public decimal AMOUNT { get; set; }
        public string DATE { get; set; }
        public SubMerchantAmountTransferDetail SUB_MERCHANT_AMOUNT_TRANSFER_DETAIL { get; set; }
    }

    public class SubMerchantAmountTransferDetail
    {
        public int EXTRACT_ID { get; set; }
        public decimal SENDED_AMOUNT { get; set; }
        public string SENDED_DATE { get; set; }
    }

    public class Product
    {
        public int PAYMENT_ID { get; set; }
        public int PRODUCT_ID { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string PRODUCT_CATEGORY { get; set; }
        public string PRODUCT_DESCRIPTION { get; set; }
        public decimal PRODUCT_AMOUNT { get; set; }
    }
}






