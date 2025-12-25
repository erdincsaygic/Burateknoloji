using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class MailLog : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IDCompany", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDCompany { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Email", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Email { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Title", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string Title { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Body", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string Body { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsSuccess", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public bool IsSuccess { get; set; }


        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Company", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Company { get; set; }
    }
}
