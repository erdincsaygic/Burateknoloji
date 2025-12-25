using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.NomuPayPos.Models.NomuPayPaymentRequest
{
    public class NomuPayPosPaymentRequestModel
    {
        public string ServiceType = "CCProxy";
        public string OperationType = "Sale3DSEC";
        public string MPAY { get; set; }

        public string CurrencyCode = "TRY";
        public string Port { get; set; }
        public string IPAddress { get; set; }
        public string PaymentContent { get; set; }

        public int InstallmentCount = 0;
        public string Description { get; set; }
        public string ExtraParam { get; set; }

        public string SuccessURL = "http://localhost:63352/panel/paymentnotification/NomuPayPosThreeDSecure";

        public string ErrorURL = "http://localhost:63352/panel/paymentnotification/NomuPayPosThreeDSecure";
        public Token Token { get; set; }
        public CreditCardInfo CreditCardInfo { get; set; }
        public CardTokenization CardTokenization { get; set; }
        public NomuPayPosCustomerInfo CustomerInfo { get; set; }
        public string Language = "TR";
    }

    public class Token
    {
        public string UserCode { get; set; }
        public string Pin { get; set; }
    }

    public class CreditCardInfo
    {
        public string CreditCardNo { get; set; }
        public string OwnerName { get; set; }
        public int ExpireMonth { get; set; }
        public int ExpireYear { get; set; }
        public string Cvv { get; set; }
        public decimal Price { get; set; }
    }

    public class CardTokenization
    {
        public int RequestType { get; set; }
        public string CustomerId { get; set; }
        public int ValidityPeriod { get; set; }
        public Guid CCTokenId { get; set; }
    }

    public class NomuPayPosCustomerInfo
    {
        public string CustomerName { get; set; }
        public string CustomerSurname { get; set; }
        public string CustomerEmail { get; set; }
    }
}
