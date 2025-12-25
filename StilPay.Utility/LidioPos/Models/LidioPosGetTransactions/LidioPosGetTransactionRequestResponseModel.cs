using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.LidioPos.Models.LidioPosGetTransactions
{
    public class LidioPosGetTransactionRequestResponseModel
    {
        public string Result { get; set; }
        public string ResultMessage { get; set; }
        public int TotalRecordCount { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageRecordCount { get; set; }
        public List<Transaction> TransactionList { get; set; }
    }

    public class Transaction
    {
        public bool IsSuccess { get; set; }
        public string ResultDetail { get; set; }
        public ProcessInfo ProcessInfo { get; set; }
        public CustomerInfo CustomerInfo { get; set; }
        public PaymentInfo PaymentInfo { get; set; }
        public List<BasketItem> BasketItems { get; set; }
    }

    public class ProcessInfo
    {
        public string MerchantProcessId { get; set; }
        public string MerchantCustomField { get; set; }
        public string Channel { get; set; }
        public bool? IsLoggedInProcess { get; set; }
        public PaymentConsents PaymentConsents { get; set; }
        public string PaymentDescription { get; set; }
        public string GroupCode { get; set; }
        public string MerchantSalesRepresentative { get; set; }
        public string MerchantReferralCode { get; set; }
    }

    public class PaymentConsents
    {
        public bool? PaymentExtraConsent1 { get; set; }
        public bool? PaymentExtraConsent2 { get; set; }
    }

    public class CustomerInfo
    {
        public string Email { get; set; }
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string CustomVal1 { get; set; }
        public string CustomVal2 { get; set; }
        public string CustomVal3 { get; set; }
        public string TcknVkn { get; set; }
        public string CustomerGroupName { get; set; }
    }

    public class PaymentInfo
    {
        public string OrderId { get; set; }
        public decimal EffectiveTotal { get; set; }
        public string SystemTransId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal AmountRequested { get; set; }
        public decimal AmountProcessed { get; set; }
        public decimal UsedLoyaltyPoint { get; set; }
        public int InstallmentCount { get; set; }
        public int ExtraInstallment { get; set; }
        public string Currency { get; set; }
        public string InstrumentType { get; set; }
        public InstrumentDetail InstrumentDetail { get; set; }
        public string AcquirerType { get; set; }
        public AcquirerResultDetail AcquirerResultDetail { get; set; }
        public ResultCategory ResultCategory { get; set; }
        public List<PaybackTransaction> PaybackTransactionList { get; set; }
        public FraudControlInfo FraudControlInfo { get; set; }
        public bool? IsPostauthed { get; set; }
        public bool? IsRefunded { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsFullRefunded { get; set; }
        public decimal TotalRefundedAmount { get; set; }
    }

    public class InstrumentDetail
    {
        public Card Card { get; set; }
        public WireTransfer WireTransfer { get; set; }
        public DirectWireTransfer DirectWireTransfer { get; set; }
        public InstantLoan InstantLoan { get; set; }
        public MarketPlaceBalance MarketPlaceBalance { get; set; }
        public Emoney Emoney { get; set; }
        public Ideal Ideal { get; set; }
        public Sofort Sofort { get; set; }
        public Sepa Sepa { get; set; }
    }

    public class Card
    {
        public string ProcessType { get; set; }
        public string RefTransType { get; set; }
        public string MaskedCardNumber { get; set; }
        public string CardNamebyUser { get; set; }
        public string CardBankCode { get; set; }
        public string CardBankName { get; set; }
        public string CardProgram { get; set; }
        public string CardHolderName { get; set; }
        public string CardToken { get; set; }
        public bool? IsCardSaved { get; set; }
        public bool? Is3DSecure { get; set; }
        public CardConsents CardConsents { get; set; }
        public bool? IsDebitCard { get; set; }
        public bool? IsBusinessCard { get; set; }
        public bool? IsMoto { get; set; }
        public string CardNoMode { get; set; }
    }

    public class CardConsents
    {
        public bool? CardSaveExtraConsent1 { get; set; }
    }

    public class WireTransfer
    {
        public string ProcessType { get; set; }
        public string RefTransType { get; set; }
        public string SenderIban { get; set; }
        public string SenderTckn { get; set; }
        public string SenderNameSurname { get; set; }
        public string SenderDescription { get; set; }
    }

    public class DirectWireTransfer { }
    public class InstantLoan { }

    public class MarketPlaceBalance
    {
        public string UsageType { get; set; }
    }

    public class Emoney
    {
        public string Phone { get; set; }
    }

    public class Ideal
    {
        public string BankAccountCountry { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string CountryOfResidence { get; set; }
        public string Language { get; set; }
    }

    public class Sofort
    {
        public string BankAccountCountry { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string CountryOfResidence { get; set; }
        public string Language { get; set; }
    }

    public class Sepa
    {
        public string Iban { get; set; }
        public string Bic { get; set; }
        public string AccountHolder { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string CountryOfResidence { get; set; }
    }

    public class AcquirerResultDetail
    {
        public Pos Pos { get; set; }
        public BankAccount BankAccount { get; set; }
        public EmoneyAccount EmoneyAccount { get; set; }
        public SepaAccount Sepa { get; set; }
    }

    public class Pos
    {
        public int PosId { get; set; }
        public string PosBankCode { get; set; }
        public string PosBankName { get; set; }
        public string MdStatus { get; set; }
        public string CardHolderNameFromBank { get; set; }
        public string CardProcessingType { get; set; }
        public string ReturnCode { get; set; }
        public string Message { get; set; }
        public DateTime? HostDate { get; set; }
        public string AuthCode { get; set; }
        public string TransId { get; set; }
        public string ReferenceNo { get; set; }
        public string CustomData { get; set; }
    }

    public class BankAccount
    {
        public int BankAccountId { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string InstantLoanStatus { get; set; }
        public decimal CommissionAmount { get; set; }
        public string ProvisionOrderId { get; set; }
        public string PaymentSourceType { get; set; }
        public string ReturnCode { get; set; }
        public string Message { get; set; }
        public DateTime? HostDate { get; set; }
        public string AuthCode { get; set; }
        public string TransId { get; set; }
        public string ReferenceNo { get; set; }
        public string CustomData { get; set; }
    }

    public class EmoneyAccount
    {
        public string ReturnCode { get; set; }
        public string Message { get; set; }
        public DateTime? HostDate { get; set; }
        public string AuthCode { get; set; }
        public string TransId { get; set; }
        public string ReferenceNo { get; set; }
        public string CustomData { get; set; }
    }

    public class SepaAccount
    {
        public int AccountId { get; set; }
        public string ReturnCode { get; set; }
        public string Message { get; set; }
        public DateTime? HostDate { get; set; }
        public string AuthCode { get; set; }
        public string TransId { get; set; }
        public string ReferenceNo { get; set; }
        public string CustomData { get; set; }
    }

    public class ResultCategory
    {
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string RecommendedUIMessageTR { get; set; }
        public string RecommendedUIMessageEN { get; set; }
    }

    public class PaybackTransaction
    {
        public string TransType { get; set; }
        public string TransDate { get; set; }
        public decimal TransAmount { get; set; }
        public string PaybackDate { get; set; }
        public int InstallmentIndex { get; set; }
        public decimal BankTotalCommissionRate { get; set; }
        public decimal BankBaseCommissionRate { get; set; }
        public decimal BankPointCommissionRate { get; set; }
        public decimal BankInstCommissionRate { get; set; }
        public decimal BankTotalCommission { get; set; }
        public decimal BankBaseCommission { get; set; }
        public decimal BankPointCommission { get; set; }
        public decimal BankInstCommission { get; set; }
        public decimal PaybackAmount { get; set; }
        public string CardProcessingType { get; set; }
        public int? CustReflectedValorDays { get; set; }
        public string DetailType { get; set; }
        public int? BasketItemId { get; set; }
        public string BasketItemIdGivenByMerchant { get; set; }
        public int SubsellerId { get; set; }
        public string SubsellerIdGivenByMerchant { get; set; }
    }

    public class FraudControlInfo
    {
        public string FraudControlResult { get; set; }
        public bool? IsAutoCancelled { get; set; }
    }

    public class BasketItem
    {
        public ScheduledPaymentInfo ScheduledPaymentInfo { get; set; }
        public MerchantPaymentItem MerchantPaymentItem { get; set; }
        public CustomerDebtItemInfo CustomerDebtItemInfo { get; set; }
        public int? BasketItemId { get; set; }
        public Marketplace Marketplace { get; set; }
        public string Name { get; set; }
        public string Category1 { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string CriticalCategory { get; set; }
        public string ItemIdGivenByMerchant { get; set; }
        public string ItemType { get; set; }
        public string ExtendedItemInfo { get; set; }
    }

    public class ScheduledPaymentInfo
    {
        public string ScheduledPaymentId { get; set; }
        public string ScheduledPaymentLineId { get; set; }
        public DueDate DueDate { get; set; }
        public decimal Amount { get; set; }
    }

    public class DueDate
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
    }

    public class MerchantPaymentItem
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }

    public class CustomerDebtItemInfo
    {
        public string CustomerDebtItemId { get; set; }
        public DueDate DueDate { get; set; }
        public decimal Amount { get; set; }
        public string DebtItemName { get; set; }
        public string DebtItemCustomField1 { get; set; }
        public string DebtItemCustomField2 { get; set; }
        public string Currency { get; set; }
    }

    public class Marketplace
    {
        public int SubsellerId { get; set; }
        public decimal ItemTotalPrice { get; set; }
        public decimal SubsellerPayoutAmount { get; set; }
    }
}
