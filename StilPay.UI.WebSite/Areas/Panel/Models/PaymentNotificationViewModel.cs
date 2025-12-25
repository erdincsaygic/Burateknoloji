using StilPay.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace StilPay.UI.WebSite.Areas.Panel.Models
{
    public class PaymentNotificationViewModel : EditViewModel<PaymentNotification>
    {
        public List<CompanyBank> CompanyBanks { get; set; }
        public CompanyBankAccount SelectedBank { get; set; }
        public CreditCard CreditCardModel { get; set; }
        public ActivePaymentMethod ActivePaymentMethodModel { get; set; }
        public ThreeDSecureResult ThreeDSecureResultModel { get; set; }
        public PayNKolayThreeDSecureResult PayNKolayThreeDSecureResultModel { get; set; }
        public CreditCardPaymentMethod CreditCardPaymentMethodModel { get; set; }
        public DateTime LastTime { get; set; }
        // Param
        public int SanalPosId { get; set; }
        // PayNKolay 
        public List<PayNKolayEncodedValueList> PayNKolayEncodedValues { get; set; }

        public GenericCreditCardPaymentResponse GenericCreditCardPaymentResponseModel { get; set; }

        //PayNKolay
        public bool IsForeignCreditCard { get; set; }

        public string CurrencyCode = "TRY";
        public string CurrencyName = "Turkish Lira";
        public string CurrencySymbol = "₺";
        public string RedirectUrl { get; set; }
        public string CreditCardRedirectToActionPaymentMethod { get; set; }
        public string CreditCardRedirectToActionGetThreeDView { get; set; }
        public byte CreditCardPaymentMethodID { get; set; }

        public string PaymentMethodID { get; set; }


        public string IsAutoTransaction { get; set; }
        public bool HasSendSms { get; set; }
        public PaymentNotificationViewModel()
        {
            CompanyBanks = new List<CompanyBank>();
            SelectedBank = new CompanyBankAccount();
            CreditCardModel = new CreditCard();
            ActivePaymentMethodModel = new ActivePaymentMethod();
            ThreeDSecureResultModel = new ThreeDSecureResult();
            CreditCardPaymentMethodModel = new CreditCardPaymentMethod();
            PayNKolayEncodedValues = new List<PayNKolayEncodedValueList>();
            PayNKolayThreeDSecureResultModel = new PayNKolayThreeDSecureResult();
            GenericCreditCardPaymentResponseModel = new GenericCreditCardPaymentResponse();
        }

        public class CreditCard
        {
            public string SenderName { get; set; } 
            public string CardNumber { get; set; } 
            public string ExpirationDate { get; set; } 
            public string SecurityCode { get; set; } 
            public string CountryCode { get; set; }
            public string PhoneNumber { get; set; } 
            public string InstallmentMonth { get; set; }
            public string InstallmentAmount { get; set; }
            public string UCD_URL { get; set; }
            public string Description { get; set; }          
            //public string FraudDescription { get; set; }
            public int? CardTypeId { get; set; }
            public string CardBankId { get; set; }
        }

        public class ThreeDSecureResult
        {
            public string TURKPOS_RETVAL_Sonuc { get; set; }
            public string TURKPOS_RETVAL_Sonuc_Str { get; set; }
            public string TURKPOS_RETVAL_GUID { get; set; }
            public string TURKPOS_RETVAL_Islem_Tarih { get; set; }
            public string TURKPOS_RETVAL_Dekont_ID { get; set; }
            public string TURKPOS_RETVAL_Tahsilat_Tutari { get; set; }
            public string TURKPOS_RETVAL_Odeme_Tutari { get; set; }
            public string TURKPOS_RETVAL_Siparis_ID { get; set; }
            public string TURKPOS_RETVAL_Islem_ID { get; set; }
            public string TURKPOS_RETVAL_Ext_Data { get; set; }
            public string TURKPOS_RETVAL_Banka_Sonuc_Kod { get; set; }
            public string TURKPOS_RETVAL_Hash { get; set; }
            public string RedirectUrl { get; set; }
        }

        public class PayNKolayThreeDSecureResult
        {
            public string REFERENCE_CODE { get; set; }
            public string AUTH_CODE { get; set; }
            public string COMMISION { get; set; }
            public string COMMISION_RATE { get; set; }
            public string INSTALLMENT { get; set; }
            public string AUTHORIZATION_AMOUNT { get; set; }
            public string BANK_CODE { get; set; }
            public string TRANSACTION_AMOUNT { get; set; }
            public string CLIENT_REFERENCE_CODE { get; set; }
            public string BANK_RESULT { get; set; }
            public string TDSECURE_TYPE { get; set; }
            public string BANK_MESSAGE { get; set; }
            public string BANK_CARD_HOLDER_NAME { get; set; }
            public string USER_ID { get; set; }
            public string TRAN_ID { get; set; }
            public string AGENT_CODE { get; set; }
            public int RESPONSE_CODE { get; set; }
            public string ERROR_CODE { get; set; }
            public string RESPONSE_DATA { get; set; }
            public string sessionId { get; set; }
            public string CORE_TRX_ID_RESERVED { get; set; }
            public string ERROR_MESSAGE { get; set; }
            public string TimeStamp { get; set; }
        }

        public class CreditCardPaymentMethod
        {
            public bool Param { get;set; }
            public bool PayNKolay { get;set; }
            public bool ForeignCCPayNKolay { get; set; }
            public bool IsBankSanalPOS { get; set; }
            public bool Paybull { get; set; }
            public bool AKODE { get; set; }
            public bool Tosla { get; set; }
        }

        public class ActivePaymentMethod
        {
            public bool TransferBeUsed { get; set; }
            public bool CreditCardBeUsed { get; set; }
            public bool ForeignCreditCardBeUsed { get; set; }
        }

        public class PayNKolayEncodedValueList
        {
            public int InstallmentMonth { get; set; }
            public string EncodedValue { get; set; }
        }

        public class GenericCreditCardPaymentResponse
        {
            public bool Success { get; set; }
            public bool SmsVerification { get; set; }
            public string IsAutoTransaction { get; set; }
            public string Message { get; set; }
            public string TimeStamp { get; set; }
            public string RedirectUrl { get; set; }
        }
    }
}
