using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class CompanyBank : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "IDBank", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Bank", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string Bank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Title", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string Title { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Branch", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string Branch { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "AccountNr", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string AccountNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IBAN", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string IBAN { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Img", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string Img { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsActive", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool IsActive { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsActiveForPaymentsBanks", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool IsActiveForPaymentsBanks { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IFrameWarnText", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string IFrameWarnText { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "CompanyBankAccountID", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string CompanyBankAccountID { get; set; }
    }
}
