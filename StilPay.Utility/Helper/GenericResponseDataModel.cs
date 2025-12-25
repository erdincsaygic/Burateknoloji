namespace StilPay.Utility.Helper
{
    public class GenericResponseDataModel<T> where T : class
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
