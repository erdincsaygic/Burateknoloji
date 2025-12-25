namespace StilPay.Utility.Parasut.Models
{
    public class CustomerResponseModel
    {
        public CustomerResponseRoot data { get; set; }

        public class CustomerResponseRoot
        {
            public string id { get; set; }
            public string type { get; set; }
            public CustomerResponseAttributes attributes { get; set; }
        }

        public class CustomerResponseAttributes
        {
            public string email { get; set; }
            public string name { get; set; }
        }
    }
}
