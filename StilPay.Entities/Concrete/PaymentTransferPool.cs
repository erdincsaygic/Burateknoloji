using StilPay.Utility.Helper;
using System;

namespace StilPay.Entities.Concrete
{
    public class PaymentTransferPool : BaseEntity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime CDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime? MDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime TransactionDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDBank", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "Bank", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string Bank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SenderName", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string SenderName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SenderIban", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string SenderIban { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionKey", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string TransactionKey { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Amount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Amount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Description", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string Description { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ResponseDescription", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string ResponseDescription { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ResponseTransactionNr", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string ResponseTransactionNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ResponseTransactionId", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string ResponseTransactionId { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Status", FieldType = Enums.FieldType.Tinyint, Description = "", Nullable = false)]
        public byte Status { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MatchesBalance", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public decimal MatchesBalance { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "NotMatchesBalance", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public decimal NotMatchesBalance { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MatchesCount", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public int MatchesCount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "NotMatchesCount", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public int NotMatchesCount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Company", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string Company { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsHaveReferenceNr", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool IsHaveReferenceNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ResponseStatus", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public int ResponseStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "FraudControlDescription", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string FraudControlDescription { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsCaughtInFraudControl", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public bool IsCaughtInFraudControl { get; set; }
    }
}
