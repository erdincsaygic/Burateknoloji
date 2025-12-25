using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class Support : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDMember", FieldType = Enums.FieldType.NVarChar, Description = "IDMember", Nullable = true)]
        public string IDMember { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDCompany", FieldType = Enums.FieldType.NVarChar, Description = "IDCompany", Nullable = true)]
        public string IDCompany { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Name", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Name { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Email", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Email { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Phone", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Phone { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Question", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Question { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Answer", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string Answer { get; set; }
    }
}
