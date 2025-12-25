using StilPay.Utility.Helper;
using System;

namespace StilPay.Entities.Concrete
{
    public class MemberTransaction : MemberEntity
    {

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDActionType", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDActionType { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ActionType", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string ActionType { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime TransactionDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionNr", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string TransactionNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransferDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime TransferDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDBank", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Bank", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Bank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Total", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Total { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Commission", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Commission { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CostTotal", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal CostTotal { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "NetTotal", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal NetTotal { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Balance", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public decimal Balance { get; set; }
    }
}
