namespace StilPay.ApiService.Models
{
    public class ExternalCallback
    {
        public class ToslaCallbackModel
        {
            public string phoneNumber { get; set; }
            public string processId { get; set; }
            public string transactionId { get; set; }
        }

        public class ToslaCallbackResponseModel
        {
            public bool isSuccess { get; set; }
            public string message { get; set; }
        }
    }
}
