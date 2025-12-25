namespace StilPay.Utility.JobBank
{
    public class CompanyIntegrationModel
    {
        public string ID { get; set; }
        public string ServiceID { get; set; }
        public string SecretKey { get; set; }
        public string SiteUrl { get; set; }
        public string CallbackUrl { get; set; }
        public string WithdrawalRequestCallback { get; set; }
    }
}
