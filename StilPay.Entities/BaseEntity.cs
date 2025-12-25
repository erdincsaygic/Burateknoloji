using StilPay.Utility.Helper;
using System;

namespace StilPay.Entities
{
    public abstract class BaseEntity : IEntity
    {
        [FieldAttribute(AutoIncrement = false, PK = true, FK = false, Name = "ID", FieldType = Enums.FieldType.NVarChar, Description = "ID", Nullable = false)]
        public string ID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = false, Name = "TotalRecords", FieldType = Enums.FieldType.None, Description = "TotalRecords", Nullable = false)]
        public long TotalRecords { get; set; }
    }

}
