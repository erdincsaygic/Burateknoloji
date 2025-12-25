using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class AdministratorRole : IEntity
    {

        [FieldAttribute(AutoIncrement = false, isMaster = true, PK = true, FK = false, Name = "IDAdministrator", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDAdministrator { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "RoleKey", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string RoleKey { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Name", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Name { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Authorized", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool Authorized { get; set; }

    }
}
