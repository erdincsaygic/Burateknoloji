using StilPay.Utility.Helper;
using System;

namespace StilPay.Entities.Concrete
{
    public class Member : Entity
    {

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Phone", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Phone { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "UsingBalance", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public decimal UsingBalance { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IdentityNr", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IdentityNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Name", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Name { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IDMemberType", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDMemberType { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MemberTypeName", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string MemberTypeName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Email", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Email { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "BirthYear", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string BirthYear { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "LoginDate", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public DateTime? LoginDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IPAddress", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string IPAddress { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "StatusFlag", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool StatusFlag { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "ServiceID", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string ServiceID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Company", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Company { get; set; }
    }
}
