using ClosedXML.Excel;
using Microsoft.AspNetCore.Components.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StilPay.Utility.EfixPos.Models.EfixGetTransaction
{
    public class EfixPosGetTransactionDetailResponseModel
    {
        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("_id")]
        public int Id { get; set; }

        [JsonProperty("transactionStatus")]
        public string? TransactionStatus { get; set; }

        [JsonProperty("merchReqCurrency")]
        public string? MerchReqCurrency { get; set; }

        [JsonProperty("merchReqDescription")]
        public string? MerchReqDescription { get; set; }

        [JsonProperty("merchReqSuccessUrl")]
        public string? MerchReqSuccessUrl { get; set; }

        [JsonProperty("merchReqCancelUrl")]
        public string? MerchReqCancelUrl { get; set; }

        [JsonProperty("merchReqDeclineUrl")]
        public string? MerchReqDeclineUrl { get; set; }

        [JsonProperty("merchReqLanguage")]
        public string? MerchReqLanguage { get; set; }

        [JsonProperty("merchReqOrderId")]
        public string? MerchReqOrderId { get; set; }

        [JsonProperty("merchReqAmount")]
        public double MerchReqAmount { get; set; }

        [JsonProperty("merchantID")]
        public string? MerchantID { get; set; }

        [JsonProperty("merchReqType")]
        public string? MerchReqType { get; set; }

        [JsonProperty("merchReqOtherAttr")]
        public string? MerchReqOtherAttr { get; set; }

        [JsonProperty("merchReqBinNumber")]
        public string? MerchReqBinNumber { get; set; }

        [JsonProperty("bankReqAmount")]
        public string? BankReqAmount { get; set; }

        [JsonProperty("bankReqApproveUrl")]
        public string? BankReqApproveUrl { get; set; }

        [JsonProperty("bankReqCancelUrl")]
        public string? BankReqCancelUrl { get; set; }

        [JsonProperty("bankReqDeclineUrl")]
        public string? BankReqDeclineUrl { get; set; }

        [JsonProperty("bankReqIsoCurrencyCode")]
        public string? BankReqIsoCurrencyCode { get; set; }

        [JsonProperty("bankReqLanguage")]
        public string? BankReqLanguage { get; set; }

        [JsonProperty("bankReqExtreInfo")]
        public int? BankReqExtreInfo { get; set; }

        [JsonProperty("bankReqOperation")]
        public string? BankReqOperation { get; set; }

        [JsonProperty("bankReqOrderType")]
        public string? BankReqOrderType { get; set; }

        [JsonProperty("bankResOrderId")]
        public string? BankResOrderId { get; set; }

        [JsonProperty("bankResRedirectUrl")]
        public string? BankResRedirectUrl { get; set; }

        [JsonProperty("bankResSessionId")]
        public string? BankResSessionId { get; set; }

        [JsonProperty("bankResStatusCode")]
        public string? BankResStatusCode { get; set; }

        [JsonProperty("bankResRRN")]
        public string? BankResRRN { get; set; }

        [JsonProperty("merchResOwnStatus")]
        public string? MerchResOwnStatus { get; set; }

        [JsonProperty("merchResBankCardMask")]
        public string? MerchResBankCardMask { get; set; }

        [JsonProperty("merchResBankCardName")]
        public string? MerchResBankCardName { get; set; }

        [JsonProperty("createDate")]
        public DateTime? CreateDate { get; set; }

        [JsonProperty("updateDate")]
        public DateTime? UpdateDate { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("cardUID")]
        public string? CardUID { get; set; }

        [JsonProperty("originalTransactionId")]
        public int? OriginalTransactionId { get; set; }

        [JsonProperty("recurringPaymentTranID")]
        public string? RecurringPaymentTranID { get; set; }

        [JsonProperty("transactionCommissionAmount")]
        public double TransactionCommissionAmount { get; set; }

        [JsonProperty("transactionCommissionRate")]
        public double TransactionCommissionRate { get; set; }

        [JsonProperty("payzeeComissionAmount")]
        public double PayzeeComissionAmount { get; set; }

        [JsonProperty("payzeeCommissionRate")]
        public double PayzeeCommissionRate { get; set; }

        [JsonProperty("bankCommissionRate")]
        public double? BankCommissionRate { get; set; }

        [JsonProperty("bankCommissionAmount")]
        public double? BankCommissionAmount { get; set; }

        [JsonProperty("totalRefundCommissionAmount")]
        public double TotalRefundCommissionAmount { get; set; }

        [JsonProperty("totalAmount")]
        public decimal TotalAmount { get; set; }

        [JsonProperty("refundAmount")]
        public double RefundAmount { get; set; }

        [JsonProperty("settlementAmount")]
        public double SettlementAmount { get; set; }

        [JsonProperty("settlementDate")]
        public DateTime SettlementDate { get; set; }

        [JsonProperty("paymentAmount")]
        public double PaymentAmount { get; set; }

        [JsonProperty("paymentDate")]
        public DateTime PaymentDate { get; set; }

        [JsonProperty("isPaymentDone")]
        public bool IsPaymentDone { get; set; }

        [JsonProperty("isReversed")]
        public bool IsReversed { get; set; }

        [JsonProperty("isSettlementTransfer")]
        public bool IsSettlementTransfer { get; set; }

        [JsonProperty("transactionStatusDescription")]
        public string? TransactionStatusDescription { get; set; }

        [JsonProperty("externalStatusCode")]
        public string? ExternalStatusCode { get; set; }

        [JsonProperty("externalStatusDesc")]
        public string? ExternalStatusDesc { get; set; }

        [JsonProperty("paymentTransactionId")]
        public int? PaymentTransactionId { get; set; }

        [JsonProperty("preAuthCompletionAmount")]
        public double PreAuthCompletionAmount { get; set; }

        [JsonProperty("memberId")]
        public string? MemberId { get; set; }

        [JsonProperty("isWidgetPayment")]
        public bool IsWidgetPayment { get; set; }

        [JsonProperty("kioskTransactionId")]
        public string? KioskTransactionId { get; set; }

        [JsonProperty("kioskUpTxnCode")]
        public string? KioskUpTxnCode { get; set; }

        [JsonProperty("kioskCompany")]
        public string? KioskCompany { get; set; }

        [JsonProperty("transferRePaymentDone")]
        public bool? TransferRePaymentDone { get; set; }

        [JsonProperty("isTransactionStatusUpdated")]
        public bool IsTransactionStatusUpdated { get; set; }

        [JsonProperty("posTerminalNo")]
        public string? PosTerminalNo { get; set; }

        [JsonProperty("transactionGroup")]
        public string? TransactionGroup { get; set; }

        [JsonProperty("additionalInformation")]
        public string? AdditionalInformation { get; set; }

        [JsonProperty("companyWebSite")]
        public string? CompanyWebSite { get; set; }

        [JsonProperty("constantLinkId")]
        public int? ConstantLinkId { get; set; }

        [JsonProperty("constantLinkMerchantName")]
        public string? ConstantLinkMerchantName { get; set; }

        [JsonProperty("clientName")]
        public string? ClientName { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonProperty("projectId")]
        public int ProjectId { get; set; }

        [JsonProperty("responseStatusCodeId")]
        public int? ResponseStatusCodeId { get; set; }

        [JsonProperty("binCardId")]
        public int? BinCardId { get; set; }

        [JsonProperty("bankId")]
        public int? BankId { get; set; }

        [JsonProperty("bank")]
        public BankDto Bank { get; set; }

        [JsonProperty("paymentChannelId")]
        public int? PaymentChannelId { get; set; }

        [JsonProperty("mcc")]
        public string? Mcc { get; set; }

        [JsonProperty("payLinkId")]
        public int? PayLinkId { get; set; }

        [JsonProperty("paymentStage")]
        public int PaymentStage { get; set; }

        [JsonProperty("verifyEnrollmentRequestId")]
        public string? VerifyEnrollmentRequestId { get; set; }

        [JsonProperty("paReq")]
        public string? PaReq { get; set; }

        [JsonProperty("acsUrl")]
        public string? AcsUrl { get; set; }

        [JsonProperty("termUrl")]
        public string? TermUrl { get; set; }

        [JsonProperty("md")]
        public string? Md { get; set; }

        [JsonProperty("subMerchantId")]
        public string? SubMerchantId { get; set; }

        [JsonProperty("identity")]
        public string? Identity { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("brandName")]
        public string? BrandName { get; set; }

        [JsonProperty("ad")]
        public string? Ad { get; set; }

        [JsonProperty("soyad")]
        public string? Soyad { get; set; }

        [JsonProperty("vergiKimlikNumarasi")]
        public string? VergiKimlikNumarasi { get; set; }

        [JsonProperty("tuzelKisiUnvan")]
        public string? TuzelKisiUnvan { get; set; }

        [JsonProperty("installmentCount")]
        public int? InstallmentCount { get; set; }

        [JsonProperty("clientIp")]
        public string? ClientIp { get; set; }

        [JsonProperty("authCode")]
        public string? AuthCode { get; set; }

        [JsonProperty("bankResTransactionId")]
        public string? BankResTransactionId { get; set; }

        [JsonProperty("commissionDefinitionId")]
        public int? CommissionDefinitionId { get; set; }

        [JsonProperty("checkedDate")]
        public DateTime? CheckedDate { get; set; }

        [JsonProperty("checkedStatus")]
        public string? CheckedStatus { get; set; }
    }
    public class BankDto
    {
        [JsonProperty("createdAt")] public DateTime? CreatedAt { get; set; }
        [JsonProperty("updatedAt")] public DateTime? UpdatedAt { get; set; }
        [JsonProperty("_id")] public int Id { get; set; }
        [JsonProperty("channelNo")] public int ChannelNo { get; set; }
        [JsonProperty("bankName")] public string? BankName { get; set; }
        [JsonProperty("bkmId")] public int? BkmId { get; set; }
        [JsonProperty("isActive")] public bool IsActive { get; set; }

        [JsonProperty("bin")]
        public List<object>? Bin { get; set; }

        [JsonProperty("merchantIds")]
        public List<string>? MerchantIds { get; set; }
    }
}
