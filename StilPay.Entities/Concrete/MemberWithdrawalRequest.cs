using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class MemberWithdrawalRequest : MemberEntity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionNr", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string TransactionNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IDBank", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "Bank", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string Bank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IBAN", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IBAN { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Title", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Title { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Amount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Amount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CostTotal", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal CostTotal { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsEFT", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsEFT { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Description", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string Description { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Status", FieldType = Enums.FieldType.Tinyint, Description = "", Nullable = false)]
        public byte Status { get; set; }


    }
}
