using StilPay.Utility.Helper;

namespace StilPay.Entities
{
    public class CompanyEntity : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDCompany", FieldType = Enums.FieldType.NVarChar, Description = "IDCompany", Nullable = false)]
        public string IDCompany { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Company", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Company { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Phone", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public virtual string Phone { get; set; }
    }
}
