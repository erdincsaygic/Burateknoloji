using Newtonsoft.Json;
using System.Collections.Generic;

namespace StilPay.UI.Admin.Models
{
    public class GarantiTransactionModel
    {
        [JsonProperty("result")]
        public Result Result { get; set; }

        [JsonProperty("transactions")]
        public List<Transaction> Transactions { get; set; }
    }

    public class Result
    {
        [JsonProperty("returnCode")]
        public int ReturnCode { get; set; }

        [JsonProperty("reasonCode")]
        public int ReasonCode { get; set; }

        [JsonProperty("messageText")]
        public string MessageText { get; set; }
    }

    public class Transaction
    {
        [JsonProperty("corrTCKN")]
        public string SenderTCKN { get; set; }

        [JsonProperty("corrVKN")]
        public string SenderVKN { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("transactionInstanceId")]
        public string TransactionInstanceId { get; set; }

        [JsonProperty("clasificationCode")]
        public string ClasificationCode { get; set; }

        [JsonProperty("explanation")]
        public string Explanation { get; set; }

        [JsonProperty("transactionReferenceId")]
        public string TransactionReferenceId { get; set; }

        [JsonProperty("enrichmentInformation")]
        public List<EnrichmentInformation> EnrichmentInformation { get; set; }
    }

    public class EnrichmentInformation
    {
        [JsonProperty("enrichmentValue")]
        public EnrichmentValue EnrichmentValue { get; set; }

        [JsonProperty("enrichmentCode")]
        public string EnrichmentCode { get; set; }
    }

    public class EnrichmentValue
    {
        [JsonProperty("corrNameSurnameText")]
        public string SenderName { get; set; }

        [JsonProperty("corrIBAN")]
        public string SenderIban { get; set; }
    }
}
