using StilPay.Utility.Helper;
using System;

namespace StilPay.Entities.Concrete
{
    public class CompanyPaymentRequest : CompanyEntity
    {

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "TransactionNr", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string TransactionNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "SenderName", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string SenderName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDBank", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "Bank", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Bank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ActionDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime ActionDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ActionTime", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string ActionTime { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Amount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Amount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Description", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string Description { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Status", FieldType = Enums.FieldType.Tinyint, Description = "", Nullable = false)]
        public byte Status { get; set; }
    }
}
