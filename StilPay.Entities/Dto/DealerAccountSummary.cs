using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Dto
{
    public class DealerAccountSummary
    {
        public string CreditCardPaymentMethodID { get; set; }
        public decimal CreditCardSumTotal { get; set; }
        public decimal CreditCardProfit { get; set; }

        public decimal ForeignCreditCardSumTotal { get; set; }
        public decimal ForeignCreditCardProfit { get; set; }

        public string PaymentTranferPoolBankID { get; set; }
        public decimal PaymentTranferPoolSumTotal { get; set; }

        public string WithdrawalRequestSIDBank { get; set; }
        public decimal WithdrawalRequestTotalAmount { get; set; }
        public decimal WithdrawalRequestProfit { get; set; }

        public string PaymentNotificationSIDBank { get; set; }
        public decimal PaymentNotificationTotalAmount { get; set; }
        public decimal PaymentNotificationTotalProfit { get; set; }


        public decimal WithdrawalTransactionTotalAmount { get; set; }

        public decimal WithdrawalRequestCount { get; set; }
        public decimal PaymentNotificationCount { get; set; }
        public decimal CreditCardCount { get; set; }
        public decimal ForeignCreditCardCount { get; set; }

        public decimal RebateNetAmount { get; set; }
        public decimal CreditCardFraudExpenseAmount { get; set; }
        
        public decimal BankCardTypePaymentNotificationTotalAmount { get; set; }
        public decimal BankCardTypePaymentNotificationProfit { get; set; }
        public decimal BankCardTypeCount { get; set; }


        //public decimal CreditCardRevenuePercentage { get; set; }
        //public decimal BankCardRevenuePercentage { get; set; }
        //public decimal CreditCardProfitPercentage { get; set; }
        //public decimal BankCardProfitPercentage { get; set; }

        public decimal AverageCommissionRate { get; set; }

        public List<ForeignCreditCardSummary> ForeignCreditCardSummaries { get; set; }
    }

    [JsonConverter(typeof(ForeignCreditCardSummaryConverter))]
    public class ForeignCreditCardSummary
    {
        public string Currency { get; set; }
        public decimal SumTotal { get; set; }
        public decimal Profit { get; set; }
        public int Count { get; set; }

        public WithdrawalRequestSummary WithdrawalRequestSummary { get; set; }
        public RebateRequestSummary RebateRequestSummary { get; set; }
        public FraudExpenseSummary FraudExpenseSummary { get; set; }
    }

    public class WithdrawalRequestSummary
    {
        public decimal WithdrawalRequestTotalAmount { get; set; }
        public decimal WithdrawalRequestProfit { get; set; }
        public int WithdrawalRequestCount { get; set; }
    }

    public class RebateRequestSummary
    {
        public decimal ForeignRebateNetAmount { get; set; }
    }
    public class FraudExpenseSummary
    {
        public decimal FraudExpenseAmount { get; set; }
    }

    public class ForeignCreditCardSummaryConverter : JsonConverter<ForeignCreditCardSummary>
    {
        public override ForeignCreditCardSummary ReadJson(JsonReader reader, Type objectType, ForeignCreditCardSummary existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var summary = new ForeignCreditCardSummary
            {
                Currency = (string)jsonObject["Currency"],
                SumTotal = (decimal)jsonObject["SumTotal"],
                Profit = (decimal)jsonObject["Profit"],
                Count = (int)jsonObject["Count"]
            };

            var withdrawalSummary = jsonObject["WithdrawalRequestSummary"];
            if (withdrawalSummary.Type == JTokenType.String)
            {
                summary.WithdrawalRequestSummary = JsonConvert.DeserializeObject<WithdrawalRequestSummary>(withdrawalSummary.ToString());
            }
            else
            {
                summary.WithdrawalRequestSummary = withdrawalSummary.ToObject<WithdrawalRequestSummary>();
            }

            var rebateSummary = jsonObject["RebateRequestSummary"];
            if (rebateSummary.Type == JTokenType.String)
            {
                summary.RebateRequestSummary = JsonConvert.DeserializeObject<RebateRequestSummary>(rebateSummary.ToString());
            }
            else
            {
                summary.RebateRequestSummary = rebateSummary.ToObject<RebateRequestSummary>();
            }

            var fraudSummary = jsonObject["FraudExpenseSummary"];
            if (fraudSummary.Type == JTokenType.String)
            {
                summary.FraudExpenseSummary = JsonConvert.DeserializeObject<FraudExpenseSummary>(fraudSummary.ToString());
            }
            else
            {
                summary.FraudExpenseSummary = fraudSummary.ToObject<FraudExpenseSummary>();
            }

            return summary;
        }

        public override void WriteJson(JsonWriter writer, ForeignCreditCardSummary value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("Currency");
            writer.WriteValue(value.Currency);

            writer.WritePropertyName("SumTotal");
            writer.WriteValue(value.SumTotal);

            writer.WritePropertyName("Profit");
            writer.WriteValue(value.Profit);

            writer.WritePropertyName("Count");
            writer.WriteValue(value.Count);

            writer.WritePropertyName("WithdrawalRequestSummary");
            serializer.Serialize(writer, value.WithdrawalRequestSummary);

            writer.WritePropertyName("RebateRequestSummary");
            serializer.Serialize(writer, value.RebateRequestSummary);

            writer.WritePropertyName("FraudExpenseSummary");
            serializer.Serialize(writer, value.FraudExpenseSummary);

            writer.WriteEndObject();
        }
    }

}
