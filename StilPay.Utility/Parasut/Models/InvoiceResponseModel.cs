using System;
using static StilPay.Utility.Parasut.Models.InvoicePurchaseModel;

namespace StilPay.Utility.Parasut.Models
{
    public class InvoiceResponseModel
    {
        public InvoiceResponseRoot data { get; set; }

        public class InvoiceResponseRoot
        {
            public string id { get; set; }
            public string type { get; set; }
            public InvoiceResponseAttributes attributes { get; set; }
            public InvoiceResponseRelationships relationships { get; set; }
            public Meta meta { get; set; }
            public string contact_id { get; set; }
        }

        public class ActiveEDocument
        {
            public Meta meta { get; set; }
        }

        public class Activities
        {
            public Meta meta { get; set; }
        }

        public class InvoiceResponseAttributes
        {
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public object description { get; set; }
            public string issue_date { get; set; }
            public object invoice_id { get; set; }
            public object invoice_series { get; set; }
            public string invoice_no { get; set; }
            public string net_total { get; set; }
            public string currency { get; set; }
            public string gross_total { get; set; }
            public string withholding_rate { get; set; }
            public string withholding { get; set; }
            public string total_excise_duty { get; set; }
            public string total_communications_tax { get; set; }
            public string total_vat { get; set; }
            public string vat_withholding { get; set; }
            public string total_vat_withholding { get; set; }
            public string total_discount { get; set; }
            public string total_invoice_discount { get; set; }
            public string before_taxes_total { get; set; }
            public string total_accommodation_tax { get; set; }
            public string remaining { get; set; }
            public string total_paid { get; set; }
            public string remaining_in_trl { get; set; }
            public object due_date { get; set; }
            public string payment_status { get; set; }
            public bool archived { get; set; }
            public string item_type { get; set; }
            public object item_type_before_cancellation { get; set; }
            public bool is_recurred_item { get; set; }
            public int sharings_count { get; set; }
            public object printed_at { get; set; }
            public int days_overdue { get; set; }
            public object days_till_due_date { get; set; }
            public object city { get; set; }
            public object district { get; set; }
            public object tax_office { get; set; }
            public object tax_number { get; set; }
            public string invoice_discount { get; set; }
            public string invoice_discount_type { get; set; }
            public string net_total_in_trl { get; set; }
            public object shipment_document_no { get; set; }
            public object shipment_date { get; set; }
            public object shipment_address { get; set; }
            public string exchange_rate { get; set; }
            public object print_note { get; set; }
            public string print_url { get; set; }
            public object billing_address { get; set; }
            public object billing_phone { get; set; }
            public object billing_fax { get; set; }
            public object billing_postal_code { get; set; }
            public string contact_type { get; set; }
            public object order_no { get; set; }
            public object order_date { get; set; }
            public object accommodation_tax_exemption_reason_code { get; set; }
            public bool shipment_included { get; set; }
            public bool is_abroad { get; set; }
            public bool cash_sale { get; set; }
            public object country { get; set; }
            public object payer_tax_numbers { get; set; }
            public string vat_withholding_rate { get; set; }
            public string sharing_preview_url { get; set; }
            public string sharing_preview_path { get; set; }
        }

        public class Category
        {
            public Meta meta { get; set; }
        }

        public class Contact
        {
            public InvoicePurchaseData meta { get; set; }
        }

        public class Details
        {
            public Meta meta { get; set; }
        }

        public class FailedEInvoice
        {
            public Meta meta { get; set; }
        }

        public class Meta
        {
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
        }

        public class OperatedBy
        {
            public Meta meta { get; set; }
        }

        public class Payments
        {
            public Meta meta { get; set; }
        }

        public class PriceList
        {
            public Meta meta { get; set; }
        }

        public class RecurrenceOf
        {
            public Meta meta { get; set; }
        }

        public class RefundOf
        {
            public Meta meta { get; set; }
        }

        public class Refunds
        {
            public Meta meta { get; set; }
        }

        public class InvoiceResponseRelationships
        {
            public Category category { get; set; }
            public Contact contact { get; set; }
            public Details details { get; set; }
            public Payments payments { get; set; }
            public Tags tags { get; set; }
            public Activities activities { get; set; }
            public RefundOf refund_of { get; set; }
            public Refunds refunds { get; set; }
            public Sharings sharings { get; set; }
            public ActiveEDocument active_e_document { get; set; }
            public RecurrenceOf recurrence_of { get; set; }
            public ShipmentDocuments shipment_documents { get; set; }
            public SalesOffer sales_offer { get; set; }
            public PriceList price_list { get; set; }
            public OperatedBy operated_by { get; set; }
            public FailedEInvoice failed_e_invoice { get; set; }
        }

        public class SalesOffer
        {
            public Meta meta { get; set; }
        }

        public class Sharings
        {
            public Meta meta { get; set; }
        }

        public class ShipmentDocuments
        {
            public Meta meta { get; set; }
        }

        public class Tags
        {
            public Meta meta { get; set; }
        }
    }
}
