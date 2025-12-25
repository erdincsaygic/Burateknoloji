using StilPay.Utility.Helper;
using System;

namespace StilPay.Entities.Concrete
{
    public class NotificationTransaction : BaseEntity
    {

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "NotificationDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime NotificationDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime TransactionDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionNr", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string TransactionNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransferDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime? TransferDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDBank", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Bank", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string Bank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ServiceID", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string ServiceID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionID", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string TransactionID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionKey", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string TransactionKey { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Amount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Amount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Description", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string Description { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDMember", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string IDMember { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Member", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Member { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MemberPhone", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string MemberPhone { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SenderName", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string SenderName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SenderIdentityNr", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string SenderIdentityNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsAutomatic", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsAutomatic { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsOK", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsOK { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsAccepted", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsAccepted { get; set; }
    }
}
