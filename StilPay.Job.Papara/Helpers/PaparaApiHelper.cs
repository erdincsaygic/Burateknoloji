using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Job.Papara.Helpers
{
    internal class PaparaApiHelper
    {
        public string ApiKey {  get; set; }
        public string transaction_url { get; set; }
        public string bank_id { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public int query_period_interval_second { get; set; }
        public double notification_range_minute { get; set; }
        public double transaction_range_hour { get; set; }

        public string companyBankAccountID { get; set; }
    }
}
