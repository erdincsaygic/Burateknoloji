using System;

namespace StilPay.Utility.Parasut.Models
{
    public class InvoiceModel
    {
        public double Amount { get; set; }
        public int Tax { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string Description { get; set; }
        public string ContactID { get; set; }
        public string ProductID { get; set; }
        public string CurrencyCode { get; set; }
        public double ExchangeRate { get; set; }
    }
}
