namespace StilPay.Utility.PayNKolay.Models
{
    public class ResponseModel<T> where T : class
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
