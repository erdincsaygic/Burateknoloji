using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.LidioPos.Models.LidioPosPaymentRequest
{
    public class ReqCustomerInfo
    {
        public string email { get; set; }
        public string customerId { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string customVal1 { get; set; }
        public string customVal2 { get; set; }
        public string customVal3 { get; set; }
        public string tcknVkn { get; set; }
        public string customerGroupName { get; set; }
    }

    public class VerificationInfo
    {
        public bool hostedVerification { get; set; }
        public List<string> verificationMethods { get; set; }
    }

    public class AmountDetail
    {
        public int baseAmount { get; set; }
        public int interestAmount { get; set; }
    }

    public class SubMerchant
    {
        public string subMerchantId { get; set; }
        public string vkn { get; set; }
        public string tckn { get; set; }
        public string terminalNo { get; set; }
    }

    public class PosAccount
    {
        public int id { get; set; }
        public SubMerchant subMerchant { get; set; }
    }

    public class InstallmentList
    {
        public int installmentCount { get; set; }
        public int interestRate { get; set; }
        public int extraInstallmentCount { get; set; }
        public string installmentAppliesTo { get; set; }
        public string extraInstallmentAppliesTo { get; set; }
    }

    public class PosList
    {
        public int id { get; set; }
        public SubMerchant subMerchant { get; set; }
        public List<InstallmentList> installmentList { get; set; }
    }

    public class PosConfiguration
    {
        public int defaultPOS { get; set; }
        public int defaultAmexPOS { get; set; }
        public int nonInstallmentPOS { get; set; }
        public int foreignPOS { get; set; }
        public int foreignAmexPOS { get; set; }
        public bool useCustomInstallment { get; set; }
        public int posGroupId { get; set; }
        public List<PosList> posList { get; set; }
    }

    public class StoredCard
    {
        public string processType { get; set; }
        public string cardToken { get; set; }
        public VerificationInfo verificationInfo { get; set; }
        public bool use3DSecure { get; set; }
        public string cvv { get; set; }
        public int installmentCount { get; set; }
        public int extraInstallment { get; set; }
        public AmountDetail amountDetail { get; set; }
        public string loyaltyPointUsage { get; set; }
        public int loyaltyPointAmount { get; set; }
        public PosAccount posAccount { get; set; }
        public PosConfiguration posConfiguration { get; set; }
    }

    public class CardInfo
    {
        public string cardHolderName { get; set; }
        public string cardNumber { get; set; }
        public string lastMonth { get; set; }
        public string lastYear { get; set; }
    }

    public class NewCard
    {
        public string processType { get; set; }
        public CardInfo cardInfo { get; set; }
        public bool saveCardTemporarily { get; set; }
        public bool use3DSecure { get; set; }
        public string cardNamebyUser { get; set; }
        public string cvv { get; set; }
        public int installmentCount { get; set; }
        public int extraInstallment { get; set; }
        public AmountDetail amountDetail { get; set; }
        public string loyaltyPointUsage { get; set; }
        public int loyaltyPointAmount { get; set; }
        public PosAccount posAccount { get; set; }
        public PosConfiguration posConfiguration { get; set; }
    }

    public class BkmExpress
    {
        public string processType { get; set; }
        public PosConfiguration posConfiguration { get; set; }
    }

    public class GarantiPay
    {
        public string processType { get; set; }
        public PosAccount posAccount { get; set; }
    }

    public class MaximumMobil
    {
        public string processType { get; set; }
        public PosAccount posAccount { get; set; }
    }

    public class Emoney
    {
        public string memberCode { get; set; }
        public string accountId { get; set; }
    }

    public class WireTransfer
    {
        public int bankAccountId { get; set; }
    }

    public class DirectWireTransfer
    {
        public int bankAccountId { get; set; }
    }

    public class CampaignCodeList
    {
        public string bankCode { get; set; }
        public string campaignCode { get; set; }
    }

    public class InstantLoan
    {
        public List<CampaignCodeList> campaignCodeList { get; set; }
        public int bankAccountId { get; set; }
    }

    public class Ideal
    {
        public int posId { get; set; }
        public string bankAccountCountry { get; set; }
        public string address { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string countryOfResidence { get; set; }
        public string language { get; set; }
    }

    public class Sofort
    {
        public int posId { get; set; }
        public string bankAccountCountry { get; set; }
        public string address { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string countryOfResidence { get; set; }
        public string language { get; set; }
    }

    public class Sepa
    {
        public int accountId { get; set; }
        public string iban { get; set; }
        public string bic { get; set; }
        public string accountHolder { get; set; }
        public string address { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string countryOfResidence { get; set; }
    }

    public class MarketplaceBalance
    {
        public string usageType { get; set; }
    }

    public class BasketItemReq
    {
        public string name { get; set; }
        public string category1 { get; set; }
        public string category2 { get; set; }
        public string category3 { get; set; }
        public int quantity { get; set; }
        public int unitPrice { get; set; }
        public string criticalCategory { get; set; }
        public bool isParticipationBankingCompatible { get; set; }
        public string acquirerCategoryCode { get; set; }
        public string itemIdGivenByMerchant { get; set; }
        public string itemType { get; set; }
        public string extendedItemInfo { get; set; }
    }

    public class InvoiceAddress
    {
        public string taxOffice { get; set; }
        public string taxNumber { get; set; }
        public string companyName { get; set; }
        public string contactName { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string town { get; set; }
        public string district { get; set; }
        public string address { get; set; }
        public string postalCode { get; set; }
        public string regionOrState { get; set; }
        public string contactPhone { get; set; }
        public string contactEmail { get; set; }
    }

    public class DeliveryAddress
    {
        public string contactName { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string town { get; set; }
        public string district { get; set; }
        public string address { get; set; }
        public string postalCode { get; set; }
        public string regionOrState { get; set; }
        public string contactPhone { get; set; }
        public string contactEmail { get; set; }
    }

    public class SubscriptionConfig
    {
        public string name { get; set; }
        public string groupName { get; set; }
        public string paymentItemSubscriptionType { get; set; }
        public DateTime periodStartDate { get; set; }
        public DateTime termStartDate { get; set; }
        public DateTime termEndDate { get; set; }
        public int periodDurationValue { get; set; }
        public string periodDurationUnit { get; set; }
        public int recurringAmount { get; set; }
        public bool isRecurringAmountEditable { get; set; }
        public bool useInstallment { get; set; }
        public int periodSwitchDay { get; set; }
        public bool isStartPeriodEditable { get; set; }
        public bool allowExpiredCurrentPeriodSelection { get; set; }
        public int maxTryCount { get; set; }
        public bool calculateFirstPeriodAmountPartially { get; set; }
        public int trialDurationValue { get; set; }
        public string trialDurationUnit { get; set; }
        public int recursionCount { get; set; }
        public bool isPostPaid { get; set; }
        public bool collectFirstPeriodAmountOnRegister { get; set; }
        public int recurrenceYear { get; set; }
        public int recurrenceMonth { get; set; }
        public int recurrenceDay { get; set; }
        public int entranceFee { get; set; }
        public bool entranceFeeEditable { get; set; }
        public int merchantPaymentItemRef { get; set; }
    }

    public class PaymentInstrumentInfo
    {
        public NewCard newCard { get; set; }
    }

    public class LidioPosPaymentRequestModel
    {
        public string orderId { get; set; }
        public string merchantProcessId { get; set; }
        public string merchantCustomField { get; set; }
        public double totalAmount { get; set; }
        public string currency { get; set; }
        public CustomerInfo customerInfo { get; set; }
        public string paymentInstrument { get; set; }
        public PaymentInstrumentInfo paymentInstrumentInfo { get; set; }
        public NewCard newCard { get; set; }
        public BkmExpress bkmExpress { get; set; }
        public GarantiPay garantiPay { get; set; }
        public MaximumMobil maximumMobil { get; set; }
        public Emoney emoney { get; set; }
        public WireTransfer wireTransfer { get; set; }
        public DirectWireTransfer directWireTransfer { get; set; }
        public InstantLoan instantLoan { get; set; }
        public Ideal ideal { get; set; }
        public Sofort sofort { get; set; }
        public Sepa sepa { get; set; }
        public MarketplaceBalance marketplaceBalance { get; set; }
        public bool dontDistributeSubsellerPayout { get; set; }
        public BasketItem basketItems { get; set; }
        public InvoiceAddress invoiceAddress { get; set; }
        public DeliveryAddress deliveryAddress { get; set; }
        public SubscriptionConfig subscriptionConfig { get; set; }

        public string returnUrl = "https://burateknoloji.com/panel/paymentnotification/LidioPosThreeDSecureResult";
        public string notificationUrl = "https://burateknoloji.com/panel/paymentnotification/LidioPosThreeDSecureResult";
        public string alternateNotificationUrl { get; set; }
        public string groupCode { get; set; }
        public bool useExternalFraudControl { get; set; }
        public string merchantSalesRepresentative { get; set; }
        public string merchantReferralCode { get; set; }
        public string customParameters { get; set; }
        public string clientType { get; set; }
        public string clientIp { get; set; }
        public int clientPort { get; set; }
        public string clientUserAgent { get; set; }
        public string clientInfo { get; set; }
    }
}
