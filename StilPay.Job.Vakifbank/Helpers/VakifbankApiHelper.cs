using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Job.Vakifbank.Helpers
{
    internal class VakifbankApiHelper
    {
        public string bank_id { get; set; }
        public string transaction_url { get; set; }
        public int query_period_interval_second { get; set; }
        public double notification_range_minute { get; set; }
        public string companyBankAccountID { get; set; }
        public string kurumKullanici { get; set; }
        public string sifre { get; set; }
        public string musteriNo { get; set; }
        public string hesapNo { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
    }
}
