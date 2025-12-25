using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.Entities.Concrete
{
    public class Administrator : Entity
    {
        public Administrator()
        {
            AdministratorRoles = new List<AdministratorRole>();
        }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Name", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Name { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Phone", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string Phone { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Email", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string Email { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Password", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Password { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "StatusFlag", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool StatusFlag { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Roles", FieldType = Enums.FieldType.List, Description = "", Nullable = false)]
        public List<AdministratorRole> AdministratorRoles { get; set; }



        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "State", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public bool State { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IPAddress", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string IPAddress { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "LoginDate", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public DateTime LoginDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ExitDate", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public DateTime? ExitDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ShowRoles", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public bool ShowRoles { get; set; }
    }
}
