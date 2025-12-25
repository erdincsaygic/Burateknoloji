using StilPay.Utility.Helper;
using System;

namespace StilPay.Entities.Concrete
{
    public class Announcement : Entity
    {

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "StartDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime StartDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "EndDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime EndDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Title", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string Title { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Body", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string Body { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "StatusFlag", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool StatusFlag { get; set; }
    }
}
