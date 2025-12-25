using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class MemberType : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Name", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Name { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsAuto", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsAuto { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MinAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal? MinAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MaxAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal? MaxAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Quantity", FieldType = Enums.FieldType.SmallInt, Description = "", Nullable = true)]
        public short? Quantity { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "StatusFlag", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool StatusFlag { get; set; }

    }
}
