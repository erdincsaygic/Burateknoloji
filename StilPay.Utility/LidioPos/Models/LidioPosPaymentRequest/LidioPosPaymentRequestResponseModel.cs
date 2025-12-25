using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Text;

namespace StilPay.Utility.LidioPos.Models.LidioPosPaymentRequest
{
    public class LidioPosPaymentRequestResponseModel
    {
        public string result { get; set; }
        public string resultDetail { get; set; }
        public string resultMessage { get; set; }
        public string redirectForm { get; set; }
        public ProcessInfo processInfo { get; set; }
        public CustomerInfo customerInfo { get; set; }
        public List<BasketItem> basketItems { get; set; }
        public PaymentInfo paymentInfo { get; set; }

        public RedirectFromParams redirectFormParams {get;set;}
    }

    public class RedirectFromParams
    {
        public string actionLink { get; set; }
    }

    public class ProcessInfo
    {
        public string merchantProcessId { get; set; }
        public string merchantCustomField { get; set; }
        public string channel { get; set; }
        public string paymentDescription { get; set; }
        public string groupCode { get; set; }
        public string merchantSalesRepresentative { get; set; }
        public string merchantReferralCode { get; set; }
    }

    public class CustomerInfo
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

    public class ScheduledPaymentInfo
    {
        public string scheduledPaymentId { get; set; }
        public string scheduledPaymentLineId { get; set; }
        public Date dueDate { get; set; }
        public double amount { get; set; }
    }

    public class MerchantPaymentItem
    {
        public string name { get; set; }
        public double amount { get; set; }
    }

    public class CustomerDebtItemInfo
    {
        public string customerDebtItemId { get; set; }
        public Date dueDate { get; set; }
        public double amount { get; set; }
        public string debtItemName { get; set; }
        public string debtItemCustomField1 { get; set; }
        public string debtItemCustomField2 { get; set; }
        public string currency { get; set; }
    }

    public class BasketItem
    {
        public ScheduledPaymentInfo scheduledPaymentInfo { get; set; }
        public MerchantPaymentItem merchantPaymentItem { get; set; }
        public CustomerDebtItemInfo customerDebtItemInfo { get; set; }
        public int basketItemId { get; set; }
        public string name { get; set; }
        public string category1 { get; set; }
        public string category2 { get; set; }
        public string category3 { get; set; }
        public int quantity { get; set; }
        public double unitPrice { get; set; }
        public string criticalCategory { get; set; }
        public string itemIdGivenByMerchant { get; set; }
        public string itemType { get; set; }
        public string extendedItemInfo { get; set; }
    }

    public class Date
    {
        public int year { get; set; }
        public int month { get; set; }
        public int day { get; set; }
    }

    public class PaymentInfo
    {
        public string orderId { get; set; }
        public string systemTransId { get; set; }
        public DateTime? transactionDate { get; set; }
        public double? amountRequested { get; set; }
        public double? amountProcessed { get; set; }
        public double? usedLoyaltyPoint { get; set; }
        public int? installmentCount { get; set; }
        public int? extraInstallment { get; set; }
        public string currency { get; set; }
        public string instrumentType { get; set; }
        public InstrumentDetail instrumentDetail { get; set; }
        public string acquirerType { get; set; }
        public AcquirerResultDetail acquirerResultDetail { get; set; }
        public ResultCategory resultCategory { get; set; }
        public List<PaybackTransaction> paybackTransactionList { get; set; }
        public FraudControlInfo fraudControlInfo { get; set; }
        public bool? isCancelled { get; set; }
    }

    public class InstrumentDetail
    {
        public Card card { get; set; }
        public DirectWireTransfer directWiretransfer { get; set; }
        public InstantLoan instantLoan { get; set; }
        public MarketplaceBalance marketPlaceBalance { get; set; }
        public Emoney emoney { get; set; }
        public Ideal ideal { get; set; }
        public Sofort sofort { get; set; }
        public Sepa sepa { get; set; }
    }

    public class Card
    {
        public string processType { get; set; }
        public string refTransType { get; set; }
        public string maskedCardNumber { get; set; }
        public string cardNamebyUser { get; set; }
        public string cardBankCode { get; set; }
        public string cardBankName { get; set; }
        public string cardProgram { get; set; }
        public string cardHolderName { get; set; }
        public string cardToken { get; set; }
        public bool? isCardSaved { get; set; }
        public bool? is3DSecure { get; set; }
        public bool? isDebitCard { get; set; }
        public bool? isBusinessCard { get; set; }
        public bool? isMoto { get; set; }
        public string cardNoMode { get; set; }
    }

    public class AcquirerResultDetail
    {
        public Pos pos { get; set; }
        public EmoneyAccount emoneyAccount { get; set; }
        public Sepa sepa { get; set; }
    }

    public class Pos
    {
        public int posId { get; set; }
        public string posBankCode { get; set; }
        public string posBankName { get; set; }
        public string mdStatus { get; set; }
        public string cardHolderNameFromBank { get; set; }
        public string cardProcessingType { get; set; }
        public string returnCode { get; set; }
        public string message { get; set; }
        public DateTime hostDate { get; set; }
        public string authCode { get; set; }
        public string transId { get; set; }
        public string referenceNo { get; set; }
        public string customData { get; set; }
    }

    public class EmoneyAccount
    {
        public string returnCode { get; set; }
        public string message { get; set; }
        public DateTime hostDate { get; set; }
        public string authCode { get; set; }
        public string transId { get; set; }
        public string referenceNo { get; set; }
        public string customData { get; set; }
    }

    public class ResultCategory
    {
        public string categoryCode { get; set; }
        public string categoryName { get; set; }
        public string recommendedUIMessageTR { get; set; }
        public string recommendedUIMessageEN { get; set; }
    }

    public class PaybackTransaction
    {
        public string transType { get; set; }
        public string transDate { get; set; }
        public double transAmount { get; set; }
        public string paybackDate { get; set; }
        public int installmentIndex { get; set; }
        public double bankTotalCommissionRate { get; set; }
        public double bankBaseCommissionRate { get; set; }
        public double bankPointCommissionRate { get; set; }
        public double bankInstCommissionRate { get; set; }
        public double bankTotalCommission { get; set; }
        public double bankBaseCommission { get; set; }
        public double bankPointCommission { get; set; }
        public double bankInstCommission { get; set; }
        public double paybackAmount { get; set; }
        public string cardProcessingType { get; set; }
        public int custReflectedValorDays { get; set; }
        public string detailType { get; set; }
        public int basketItemId { get; set; }
        public string basketItemIdGivenByMerchant { get; set; }
        public int subsellerId { get; set; }
        public string subsellerIdGivenByMerchant { get; set; }
    }

    public class FraudControlInfo
    {
        public string fraudControlResult { get; set; }
        public bool isAutoCancelled { get; set; }
    }
}
