namespace StilPay.Utility.PayNKolay.Models
{
    public class PaymentInstallmentRequestModel
    {
        public string Sx { get; set; }
        public decimal Amount { get; set; }
        public string CardNumber { get; set; }
        public string Iscardvalid = "true";
    }
}
