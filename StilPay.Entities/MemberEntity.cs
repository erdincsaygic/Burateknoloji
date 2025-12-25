using StilPay.Utility.Helper;

namespace StilPay.Entities
{
    public class MemberEntity : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDMember", FieldType = Enums.FieldType.NVarChar, Description = "IDMember", Nullable = false)]
        public string IDMember { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Member", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Member { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Phone", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Phone { get; set; }
    }
}
