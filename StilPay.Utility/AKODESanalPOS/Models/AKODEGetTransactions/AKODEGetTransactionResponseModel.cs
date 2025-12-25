using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.AKODESanalPOS.Models.AKODEGetTransactions
{
    public class AKODEGetTransactionResponseModel
    {
        public class TransactionResponse
        {
            public int Code { get; set; }
            public string Message { get; set; }
            public int Count { get; set; }
            public List<Transaction> Transactions { get; set; }
        }

        public class Transaction
        {
            public int TransactionType { get; set; }
            public string CreateDate { get; set; }
            public string OrderId { get; set; }
            public string BankResponseCode { get; set; }
            public string BankResponseMessage { get; set; }
            public string AuthCode { get; set; }
            public string HostReferenceNumber { get; set; }
            public decimal Amount { get; set; }
            public int Currency { get; set; }
            public int InstallmentCount { get; set; }
            public int ClientId { get; set; }
            public string CardNo { get; set; }
            public int RequestStatus { get; set; }
            public decimal RefundedAmount { get; set; }
            public decimal PostAuthedAmount { get; set; }
            public long TransactionId { get; set; }
            public string CommissionStatus { get; set; }
            public decimal NetAmount { get; set; }
            public decimal MerchantCommissionAmount { get; set; }
            public decimal? MerchantCommissionRate { get; set; }
            public int CardBankId { get; set; }
            public int CardTypeId { get; set; }
            public int Code { get; set; }
            public string Message { get; set; }
        }
    }
}
