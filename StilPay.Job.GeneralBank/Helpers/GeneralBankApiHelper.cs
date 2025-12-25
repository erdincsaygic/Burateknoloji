namespace StilPay.Job.GeneralBank.Helpers
{
    internal class GeneralBankApiHelper
    {
        public string bank_id { get; set; }
        public int query_period_interval_second { get; set; }
        public double transaction_range_hour { get; set; }
        public double notification_range_minute { get; set; }
    }
}
