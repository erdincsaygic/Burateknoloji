using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class BankBalancesLog : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IDBank", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Bank", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string Bank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Balance", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Balance { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Iban", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Iban { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "BankTitle", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string BankTitle { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "BankName", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string BankName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsExitAccount", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsExitAccount { get; set; }
    }
}
