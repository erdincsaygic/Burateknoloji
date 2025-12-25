using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Dto
{
    public class CreditCardDetailedSearchDto
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

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CardNumber", FieldType = Enums.FieldType.NVarChar, Description = "CardNumber", Nullable = false)]
        public string CardNumber { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CardTypeId", FieldType = Enums.FieldType.Int, Description = "CardTypeId", Nullable = true)]
        public int CardTypeId { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "PaymentInstitutionName", FieldType = Enums.FieldType.NVarChar, Description = "PaymentInstitutionName", Nullable = false)]
        public string PaymentInstitutionName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Amount", FieldType = Enums.FieldType.Decimal, Description = "Amount", Nullable = false)]
        public decimal Amount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Status", FieldType = Enums.FieldType.Tinyint, Description = "Status", Nullable = false)]
        public byte Status { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "RebateID", FieldType = Enums.FieldType.NVarChar, Description = "RebateID", Nullable = false)]
        public string RebateID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "EntityID", FieldType = Enums.FieldType.NVarChar, Description = "EntityID", Nullable = false)]
        public string EntityID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsForeign", FieldType = Enums.FieldType.Bit, Description = "IsForeign", Nullable = false)]
        public bool IsForeign { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = false, Name = "TotalRecords", FieldType = Enums.FieldType.None, Description = "TotalRecords", Nullable = false)]
        public long TotalRecords { get; set; }
    }
}
