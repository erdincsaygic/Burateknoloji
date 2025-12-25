using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.Paybull.PaybullGetTransactions
{
    public class PaybullGetTransactionResponseModel
    {
        public class Root
        {
            public int status_code { get; set; }
            public string status_description { get; set; }
            public Data data { get; set; }
        }

        public class Data
        {
            public List<Transaction> transactions { get; set; }
            public PaginateData paginate_data { get; set; }
        }

        public class Transaction
        {
            public int id { get; set; }
            public int user_id { get; set; }
            public int payment_source { get; set; }
            public string auth_code { get; set; }
            public int transaction_state_id { get; set; }
            public decimal gross { get; set; }
            public decimal net { get; set; }
            public string card_holder_bank { get; set; }
            public string @operator { get; set; }
            public string user_name { get; set; }
            public string card_issuer_name { get; set; }
            public string pos_name { get; set; }
            public int pos_id { get; set; }
            public string pos_bank { get; set; }
            public DateTime? settlement_date_bank { get; set; }
            public string payment_type_label { get; set; }
            public int merchant_id { get; set; }
            public string payment_id { get; set; }
            public decimal merchant_commission { get; set; }
            public int payment_type_id { get; set; }
            public decimal product_price { get; set; }
            public decimal total_refunded_amount { get; set; }
            public int dpl_id { get; set; }
            public int currency_id { get; set; }
            public string invoice_id { get; set; }
            public string credit_card_no { get; set; }
            public int installment { get; set; }
            public decimal rolling_amount { get; set; }
            public string gsm_number { get; set; }
            public string card_program { get; set; }
            public string result { get; set; }
            public DateTime? settlement_date_merchant { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public int recurring_id { get; set; }
            public string order_id { get; set; }
            public string ip { get; set; }
            public decimal refunded_chargeback_fee { get; set; }
            public string transaction_state_label { get; set; }
            public string currency_code { get; set; }
            public int created_at_int { get; set; }
            public int updated_at_int { get; set; }
            public object sale_recurring { get; set; }
            public SaleBilling sale_billing { get; set; }
            public List<object> refund_history { get; set; }
            public List<object> sale_recurring_history { get; set; }
            public List<object> rolling_balance { get; set; }
            [JsonConverter(typeof(MerchantSaleConverter))]
            public List<MerchantSale> merchant_sale { get; set; } // JSON converter kullanarak düzenlendi
            public UserData userdata { get; set; }
            public Merchant merchant { get; set; }
        }

        public class SaleBilling
        {
            public int id { get; set; }
            public object sale_id { get; set; }
            public string card_holder_name { get; set; }
        }

        public class UserData
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Merchant
        {
            public int user_id { get; set; }
            public string name { get; set; }
            public string mcc { get; set; }
        }

        public class MerchantSale
        {
            public int id { get; set; }
            public int sale_id { get; set; }
            public decimal merchant_commission_percentage { get; set; }
            public int merchant_commission_fixed { get; set; }
            public decimal end_user_commission_percentage { get; set; }
            public int end_user_commission_fixed { get; set; }
            public string merchant_rolling_percentage { get; set; }
        }

        public class PaginateData
        {
            public int current_page { get; set; }
            public int total { get; set; }
            public int per_page { get; set; }
            public int last_page { get; set; }
            public string next_page_url { get; set; }
            public string previous_page_url { get; set; }
            public string current_page_url { get; set; }
            public string last_page_url { get; set; }
        }

        public class MerchantSaleConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(List<MerchantSale>);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var token = JToken.Load(reader);
                if (token.Type == JTokenType.Array)
                {
                    return token.ToObject<List<MerchantSale>>();
                }
                else
                {
                    var singleValue = token.ToObject<MerchantSale>();
                    return new List<MerchantSale> { singleValue };
                }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }
    }

}
