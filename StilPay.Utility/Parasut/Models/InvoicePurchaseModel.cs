using System.Collections.Generic;

namespace StilPay.Utility.Parasut.Models
{
    public class InvoicePurchaseModel
    {
        public InvoicePurchaseRoot data { get; set; }

        public class InvoicePurchaseRoot
        {
            public string id { get; set; }
            public string type { get; set; }
            public InvoicePurchaseAttributes attributes { get; set; }
            public InvoicePurchaseRelationships relationships { get; set; }
        }

        public class InvoicePurchaseAttributes
        {
            public string item_type { get; set; }
            public string issue_date { get; set; }
            public string due_date { get; set; }
            public string currency { get; set; }
            public int quantity { get; set; }
            public double unit_price { get; set; }
            public double exchange_rate { get; set; }
            public int vat_rate { get; set; }
            public int payment_account_id { get; set; }
            public string payment_date { get; set; }
            public bool cash_sale { get; set; }
            public bool shipment_included { get; set; }
            
        }

        public class InvoicePurchaseRelationships
        {
            public InvoicePurchaseDetails details { get; set; }
            public InvoicePurchaseContact contact { get; set; }
            public InvoicePurchaseProduct product { get; set; }
        }

        public class InvoicePurchaseContact
        {
            public InvoicePurchaseData data { get; set; }
        }

        public class InvoicePurchaseDetails
        {
            public List<InvoicePurchaseRoot> data { get; set; }
        }

        public class InvoicePurchaseProduct
        {
            public InvoicePurchaseData data { get; set; }
        }

        public class InvoicePurchaseData
        {
            public string id { get; set; }
            public string type { get; set; }
        }
    }
}
