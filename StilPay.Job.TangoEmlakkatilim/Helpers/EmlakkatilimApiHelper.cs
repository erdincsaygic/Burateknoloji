using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Job.Emlakkatilim.Helpers
{
    internal class EmlakkatilimApiHelper
    {
        public string bank_id { get; set; }
        public string transaction_url { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public int query_period_interval_second { get; set; }
        public double notification_range_minute { get; set; }
        public string companyBankAccountID { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string accountNumber { get; set; }
        public string accountSuffix { get; set; }
        public string serviceID { get; set; }
        
    }
}
