namespace StilPay.Utility.Models
{
    public class CompanyAutoNotificationSettingModel
    {
        public string IDCompany { get; set; }
        public string ServiceId { get; set; }
        public string ReferenceId { get; set; }
        public bool IsActive { get; set; }
        public string RequestUrl { get; set; }
        public string CallbackUrl { get; set; }
        public decimal AutoTransferLimit { get; set; }
    }
}
