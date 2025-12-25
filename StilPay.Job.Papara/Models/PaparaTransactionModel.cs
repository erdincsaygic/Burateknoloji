using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Job.Papara.Models
{
    public class PaparaTransactionModel
    {
        public class CurrencyInfo
        {
            public int? CurrencyEnum { get; set; }
            public string Symbol { get; set; }
            public string Code { get; set; }
            public int? Number { get; set; }
            public string PreferredDisplayCode { get; set; }
            public string Name { get; set; }
            public bool IsCryptocurrency { get; set; }
            public bool IsInternationalMoneyTransferCurrency { get; set; }
            public int? Precision { get; set; }
            public string IconUrl { get; set; }
            public string FlagUrl { get; set; }
            public int? CurrencyEnumIso { get; set; }
            public bool IsMetalCurrency { get; set; }
        }

        public class Item
        {
            public int? Id { get; set; }
            public DateTime CreatedAt { get; set; }
            public int? EntryType { get; set; }
            public int? EntrySubType { get; set; }
            public string EntryTypeName { get; set; }
            public decimal Amount { get; set; }
            public bool IsCashout { get; set; }
            public decimal Fee { get; set; }
            public int? Currency { get; set; }
            public CurrencyInfo CurrencyInfo { get; set; }
            public decimal ResultingBalance { get; set; }
            public string Description { get; set; }
            public int? MassPaymentId { get; set; }
            public int? CheckoutPaymentId { get; set; }
            public int? CheckoutPaymentReferenceId { get; set; }
            public string OperatorUserId { get; set; }
            public decimal CommissionAmount { get; set; }
            public string UserId { get; set; }
            public bool IsCancellation { get; set; }
            public string ApiKey { get; set; }
            public bool IsLinkPayment { get; set; }
            public string ProductName { get; set; }
            public string EntryTypeTitle { get; set; }
            public string DescriptionTitle { get; set; }
            public string IconUrl { get; set; }
            public string TransactionUniqueId { get; set; }
            public string ReferenceNumber { get; set; }
            public string TransactionSource { get; set; }
            public int? PaparaCardTxType { get; set; }
            public string PaparaCardType { get; set; }
            public string PaparaCardId { get; set; }
            public decimal NetAmount { get; set; }
            public decimal ValueDatedAmount { get; set; }
            public DateTime? ValueDate { get; set; }
        }

        public class Data
        {
            public List<Item> Items { get; set; }
            public int? Page { get; set; }
            public int? PageItemCount { get; set; }
            public int? TotalItemCount { get; set; }
            public int? TotalPageCount { get; set; }
            public int? PageSkip { get; set; }
        }

        public class Root
        {
            public Data Data { get; set; }
            public bool Succeeded { get; set; }
        }
    }
}
