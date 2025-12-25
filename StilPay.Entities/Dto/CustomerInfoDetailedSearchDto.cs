using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Dto
{
    public class CustomerInfoDetailedSearchDto
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

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CustomerName", FieldType = Enums.FieldType.NVarChar, Description = "CustomerName", Nullable = false)]
        public string CustomerName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CustomerPhone", FieldType = Enums.FieldType.NVarChar, Description = "CustomerPhone", Nullable = false)]
        public string CustomerPhone { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CustomerEmail", FieldType = Enums.FieldType.NVarChar, Description = "CustomerEmail", Nullable = false)]
        public string CustomerEmail { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionType", FieldType = Enums.FieldType.NVarChar, Description = "TransactionType", Nullable = false)]
        public string TransactionType { get; set; }

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
