using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.AKODESanalPOS.Models.AKODETransactionQuery
{
    public class AKODETransactionQueryResponseModel
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public int? TransactionType { get; set; }
        public string CreateDate { get; set; }
        public string OrderId { get; set; }
        public string BankResponseCode { get; set; }
        public string BankResponseMessage { get; set; }
        public string AuthCode { get; set; }
        public string HostReferenceNumber { get; set; }
        public long? Amount { get; set; }
        public int? Currency { get; set; }
        public int? InstallmentCount { get; set; }
        public long? ClientId { get; set; }
        public string CardNo { get; set; }
        public int? RequestStatus { get; set; }
        public long? RefundedAmount { get; set; }
        public long? PostAuthedAmount { get; set; }
        public long? TransactionId { get; set; }
        public int? CommissionStatus { get; set; }
        public long? NetAmount { get; set; }
        public long? MerchantCommissionAmount { get; set; }
        public decimal? MerchantCommissionRate { get; set; }
        public long? CardBankId { get; set; }
        public long? CardTypeId { get; set; }
        public int? ValorDate { get; set; }
        public int? TransactionDate { get; set; }
        public int? BankValorDate { get; set; }
        public string ExtraParameters { get; set; }
    }

}
