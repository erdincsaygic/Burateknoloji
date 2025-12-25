namespace StilPay.Utility.IsBankTransferService.IsBankTokenModel
{
    public class IsBankTokenResponseModel
    {
        public string token_type { get; set; }
        public string access_token { get; set; }
        public string scope { get; set; }
        public string error { get; set; }
        public int expires_in { get; set; }
        public int consented_on { get; set; }
    }
}
