using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class CompanyAutoNotificationSetting : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IDCompany", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string IDCompany { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ReferenceId", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string ReferenceId { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsActive", FieldType = Enums.FieldType.Bit, Description = "", Nullable = true)]
        public bool IsActive { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "RequestUrl", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string RequestUrl { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CallbackUrl", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string CallbackUrl { get; set; }
    }
}
