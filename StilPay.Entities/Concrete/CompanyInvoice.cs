using StilPay.Utility.Helper;
using StilPay.Utility.LidioPos.Models.LidioPosPaymentRequest;
using System;
using System.Collections.Generic;

namespace StilPay.Entities.Concrete
{
    public class CompanyInvoice : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IDCompany", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string IDCompany { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Company", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string Company { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SendStatus", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool SendStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "InvoiceNumber", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public long InvoiceNumber { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TaxAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal TaxAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "NetAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal NetAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TotalAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal TotalAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ParasutPrintUrl", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string ParasutPrintUrl { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "InvoiceStartDateTime", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime InvoiceStartDateTime { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "InvoiceEndDateTime", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime InvoiceEndDateTime { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CurrencyCode", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string CurrencyCode { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TaxRate", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int TaxRate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IntegratorInvoiceNumber", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IntegratorInvoiceNumber { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CompanyTransactions", FieldType = Enums.FieldType.List, Description = "", Nullable = false)]
        public List<CompanyTransaction> CompanyTransactions { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Status", FieldType = Enums.FieldType.Tinyint, Description = "", Nullable = false)]
        public byte Status { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ExchangeRate", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal ExchangeRate { get; set; }
    }
}
