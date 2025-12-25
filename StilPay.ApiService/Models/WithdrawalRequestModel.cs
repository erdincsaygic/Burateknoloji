namespace StilPay.ApiService.Models
{
    public class WithdrawalRequestModel
    {
        public string request_nr { get; set; }
        public string iban { get; set; }
        public string title { get; set; }
        public decimal amount { get; set; }
        public bool is_eft { get; set; }
        public string description { get; set; }

    }
}
