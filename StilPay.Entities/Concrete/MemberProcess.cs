using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class MemberProcess : MemberEntity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDActionType", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDActionType { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ActionType", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string ActionType { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionNr", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string TransactionNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Amount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Amount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Commission", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Commission { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CostTotal", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal CostTotal { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "NetTotal", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal NetTotal { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Status", FieldType = Enums.FieldType.Tinyint, Description = "", Nullable = false)]
        public byte Status { get; set; }
    }
}
