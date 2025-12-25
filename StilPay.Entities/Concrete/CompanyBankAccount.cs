using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class CompanyBankAccount : CompanyEntity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Name", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Name { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Title", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Title { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IDBank", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Bank", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string Bank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IBAN", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IBAN { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "StatusFlag", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool StatusFlag { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Amount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Amount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CreditCardAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal CreditCardAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "EftAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal EftAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsExitAccount", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsExitAccount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsActiveByDefaultExpenseAccount", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsActiveByDefaultExpenseAccount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ShowInSystemSettings", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool ShowInSystemSettings { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsActiveForIFrame", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsActiveForIFrame { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "OrderNr", FieldType = Enums.FieldType.Tinyint, Description = "", Nullable = false)]
        public byte OrderNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "AccountNr", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string AccountNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IFrameWarnText", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IFrameWarnText { get; set; }
    }
}
