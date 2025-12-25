using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.LidioPos.Models.LidioPosRefund
{
    public class LidioPosRefundRequestResponseModel
    {
        public string Result { get; set; }
        public string ResultDetail { get; set; }
        public string ResultMessage { get; set; }
        public ProcessInfo ProcessInfo { get; set; }
        public CustomerInfo CustomerInfo { get; set; }
        public List<BasketItem> BasketItems { get; set; }
        public PaymentInfo PaymentInfo { get; set; }
    }

    public class ProcessInfo
    {
        public string MerchantProcessId { get; set; }
        public string MerchantCustomField { get; set; }
        public string Channel { get; set; }
        public string PaymentDescription { get; set; }
        public string GroupCode { get; set; }
        public string MerchantSalesRepresentative { get; set; }
        public string MerchantReferralCode { get; set; }
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
        public int? Quantity { get; set; }
        public double? UnitPrice { get; set; }
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
        public double? Amount { get; set; }
    }

    public class MerchantPaymentItem
    {
        public string Name { get; set; }
        public double? Amount { get; set; }
    }

    public class CustomerDebtItemInfo
    {
        public string CustomerDebtItemId { get; set; }
        public DueDate DueDate { get; set; }
        public double? Amount { get; set; }
        public string DebtItemName { get; set; }
        public string DebtItemCustomField1 { get; set; }
        public string DebtItemCustomField2 { get; set; }
        public string Currency { get; set; }
    }

    public class Marketplace
    {
        public int? SubsellerId { get; set; }
        public double? ItemTotalPrice { get; set; }
        public double? SubsellerPayoutAmount { get; set; }
    }

    public class DueDate
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? Day { get; set; }
    }

    public class PaymentInfo
    {
        public string OrderId { get; set; }
        public string SystemTransId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public double? AmountRequested { get; set; }
        public double? AmountProcessed { get; set; }
        public double? UsedLoyaltyPoint { get; set; }
        public int? InstallmentCount { get; set; }
        public int? ExtraInstallment { get; set; }
        public string Currency { get; set; }
        public string InstrumentType { get; set; }
        public InstrumentDetail InstrumentDetail { get; set; }
        public string AcquirerType { get; set; }
        public AcquirerResultDetail AcquirerResultDetail { get; set; }
        public List<PaybackTransaction> PaybackTransactionList { get; set; }
        public bool IsCancelled { get; set; }
    }

    public class InstrumentDetail
    {
        public Card Card { get; set; }
        public DirectWiretransfer DirectWiretransfer { get; set; }
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
        public bool? IsDebitCard { get; set; }
        public bool? IsBusinessCard { get; set; }
        public bool? IsMoto { get; set; }
        public string CardNoMode { get; set; }
    }

    public class DirectWiretransfer
    {
    }

    public class InstantLoan
    {
    }

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
        public EmoneyAccount EmoneyAccount { get; set; }
        public SepaDetail Sepa { get; set; }
    }

    public class Pos
    {
        public int? PosId { get; set; }
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

    public class SepaDetail
    {
        public int? AccountId { get; set; }
        public string ReturnCode { get; set; }
        public string Message { get; set; }
        public DateTime? HostDate { get; set; }
        public string AuthCode { get; set; }
        public string TransId { get; set; }
        public string ReferenceNo { get; set; }
        public string CustomData { get; set; }
    }

    public class PaybackTransaction
    {
        public string TransType { get; set; }
        public string TransDate { get; set; }
        public double? TransAmount { get; set; }
        public string PaybackDate { get; set; }
        public int? InstallmentIndex { get; set; }
        public double? BankTotalCommissionRate { get; set; }
        public double? BankBaseCommissionRate { get; set; }
        public double? BankPointCommissionRate { get; set; }
        public double? BankInstCommissionRate { get; set; }
        public double? BankTotalCommission { get; set; }
        public double? BankBaseCommission { get; set; }
        public double? BankPointCommission { get; set; }
        public double? BankInstCommission { get; set; }
        public double? PaybackAmount { get; set; }
        public string CardProcessingType { get; set; }
        public int? CustReflectedValorDays { get; set; }
        public string DetailType { get; set; }
        public int? BasketItemId { get; set; }
        public string BasketItemIdGivenByMerchant { get; set; }
        public int? SubsellerId { get; set; }
        public string SubsellerIdGivenByMerchant { get; set; }
    }

}
