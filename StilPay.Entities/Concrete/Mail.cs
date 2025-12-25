using DocumentFormat.OpenXml.Wordprocessing;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Concrete
{
    public class Mail:Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Body", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Body { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Name", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Name { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "StatusFlag", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool StatusFlag { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Category", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string Category { get; set; }


    }
}
