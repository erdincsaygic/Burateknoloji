using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.PayNKolay.Models.PaymentInstallment
{
    public class PaymentInstallmentsResponseModel
    {
        public class PaymentInstallmentsResponse
        {
            public string TERMINAL_OID { get; set; }
            public string BANK_OID { get; set; }
            public List<PAYMENTBANKLIST> PAYMENT_BANK_LIST { get; set; }
            public string EncodedValue { get; set; }
            public string RESPONSE_DATA { get; set; }
            public int RESPONSE_CODE { get; set; }
            public string CARD_SCOPE { get; set; }
            public string BANK_OID_FOREIGN { get; set; }
            public string CURRENCY_CODE_LIST { get; set; }
            public string TERMINAL_OID_FOREIGN { get; set; }
            public string DCC_LIST { get; set; }
        }

        public class PAYMENTBANKLIST
        {
            public decimal INSTALLMENT_AMOUNT { get; set; }
            public int INSTALLMENT { get; set; }
            public string CARD_TRX_TYPE { get; set; }
            public string BANK_CODE { get; set; }
            public decimal COMMISION_AMOUNT { get; set; }
            public decimal COMMISION { get; set; }
            public string COMMISION_OID { get; set; }
            public decimal TRANSACTION_AMOUNT { get; set; }
            public decimal AUTHORIZATION_AMOUNT { get; set; }
            public string CALCULATION_DATA_OID { get; set; }
            public string EncodedValue { get; set; }
            public string CURRENCY_NUMBER { get; set; }
            public string CURRENCY_CODE { get; set; }
            public string PROGRAM { get; set; }
            public string CARD_BANK_NO { get; set; }
            public int PLUS_INSTALLMENT { get; set; }
        }
    }
}
