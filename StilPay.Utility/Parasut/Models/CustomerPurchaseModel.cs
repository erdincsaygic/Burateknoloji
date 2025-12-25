namespace StilPay.Utility.Parasut.Models
{
    public class CustomerPurchaseModel
    {
        public CustomerPurchaseRoot data { get; set; }

        public class CustomerPurchaseRoot
        {
            public string id { get; set; }
            public string type { get; set; }
            public CustomerPurchaseAttributes attributes { get; set; }
        }
        
        public class CustomerPurchaseAttributes
        {
            public string email { get; set; }
            public string name { get; set; }
            public string contact_type { get; set; }
            public string tax_office { get; set; }
            public string tax_number { get; set; }
            public string address { get; set; }
            public string phone { get; set; }
            public string iban { get; set; }
            public string account_type { get; set; }
            public string city { get; set; }
            public string district { get; set; }
            public bool is_abroad { get; set; }
        }
    }
}
