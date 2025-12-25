using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class Town : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDCity", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDCity { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Name", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Name { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "StatusFlag", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool StatusFlag { get; set; }
    }
}
