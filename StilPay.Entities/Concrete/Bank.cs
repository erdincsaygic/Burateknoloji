using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class Bank : BaseEntity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Name", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Name { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Title", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Title { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Branch", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Branch { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "AccountNr", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string AccountNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IBAN", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IBAN { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Img", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Img { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "OrderNr", FieldType = Enums.FieldType.Tinyint, Description = "", Nullable = false)]
        public byte OrderNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "StatusFlag", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool StatusFlag { get; set; }

        //[FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ActivatedForGeneralUse", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        //public bool ActivatedForGeneralUse { get; set; }

        //[FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IFrameWarnText", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        //public string IFrameWarnText { get; set; }
    }
}
