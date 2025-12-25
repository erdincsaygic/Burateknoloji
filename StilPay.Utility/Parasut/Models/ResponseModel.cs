namespace StilPay.Utility.Parasut.Models
{
    public class ResponseModel<T> where T : class
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
