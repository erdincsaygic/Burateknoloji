using StilPay.Utility.Helper;
using System;


namespace StilPay.Entities.Dto
{
    public class TransferDetailedSearchDto
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ID", FieldType = Enums.FieldType.NVarChar, Description = "ID", Nullable = false)]
        public string ID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CDate", FieldType = Enums.FieldType.DateTime, Description = "CDate", Nullable = false)]
        public DateTime CDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionID", FieldType = Enums.FieldType.NVarChar, Description = "TransactionID", Nullable = false)]
        public string TransactionID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionNr", FieldType = Enums.FieldType.NVarChar, Description = "TransactionNr", Nullable = false)]
        public string TransactionNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Company", FieldType = Enums.FieldType.NVarChar, Description = "Company", Nullable = false)]
        public string Company { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SenderName", FieldType = Enums.FieldType.NVarChar, Description = "SenderName", Nullable = false)]
        public string SenderName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SenderPhone", FieldType = Enums.FieldType.NVarChar, Description = "SenderPhone", Nullable = false)]
        public string SenderPhone { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SenderReferenceNr", FieldType = Enums.FieldType.NVarChar, Description = "SenderReferenceNr", Nullable = false)]
        public string SenderReferenceNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Amount", FieldType = Enums.FieldType.Decimal, Description = "Amount", Nullable = false)]
        public decimal Amount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Status", FieldType = Enums.FieldType.Tinyint, Description = "Status", Nullable = false)]
        public byte Status { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "RebateID", FieldType = Enums.FieldType.NVarChar, Description = "RebateID", Nullable = false)]
        public string RebateID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "EntityUrl", FieldType = Enums.FieldType.NVarChar, Description = "EntityUrl", Nullable = false)]
        public string EntityUrl { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = false, Name = "TotalRecords", FieldType = Enums.FieldType.None, Description = "TotalRecords", Nullable = false)]
        public long TotalRecords { get; set; }
    }
}
