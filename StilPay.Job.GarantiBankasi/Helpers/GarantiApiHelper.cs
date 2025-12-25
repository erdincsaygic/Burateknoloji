namespace StilPay.Job.GarantiBankasi.Helpers
{
    internal class GarantiApiHelper
    {
        public string bank_id { get; set; }
        public string base_url { get; set; }
        public string token_url { get; set; }
        public string transaction_url { get; set; }
        public int query_period_interval_second { get; set; }
        public double transaction_range_hour { get; set; }
        public double notification_range_minute { get; set; }
        public string companyBankAccountID {get; set; }
    }
}
