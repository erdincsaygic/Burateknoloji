using StilPay.Utility.Helper;
using System;

namespace StilPay.Entities
{
    public abstract class Entity : BaseEntity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CDate", FieldType = Enums.FieldType.DateTime, Description = "CDate", Nullable = false)]
        public DateTime CDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CUser", FieldType = Enums.FieldType.NVarChar, Description = "CUser", Nullable = false)]
        public string CUser { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Creator", FieldType = Enums.FieldType.None, Description = "Creator", Nullable = false)]
        public string Creator { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MUser", FieldType = Enums.FieldType.NVarChar, Description = "MUser", Nullable = true)]
        public string MUser { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Modifier", FieldType = Enums.FieldType.None, Description = "Modifier", Nullable = false)]
        public string Modifier { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MDate", FieldType = Enums.FieldType.DateTime, Description = "MDate", Nullable = true)]
        public DateTime? MDate { get; set; }
    }
}
