using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.Entities.Concrete
{
    public class CompanyUser : CompanyEntity
    {
        public CompanyUser()
        {
            CompanyUserRoles = new List<CompanyUserRole>();
        }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Phone", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public override string Phone { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Name", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Name { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Email", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Email { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Password", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Password { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "NewPassword", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string NewPassword { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "StatusFlag", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public bool StatusFlag { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "LoginDate", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public DateTime? LoginDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IPAddress", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string IPAddress { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ServiceID", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string ServiceID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Roles", FieldType = Enums.FieldType.List, Description = "", Nullable = false)]
        public List<CompanyUserRole> CompanyUserRoles { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsMainUser", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool IsMainUser { get; set; }
    }
}
