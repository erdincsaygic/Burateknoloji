using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Concrete
{
    public class Slider : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Name", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Name { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Header", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Header { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Content", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Content { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ButtonRedirectUrl", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string ButtonRedirectUrl { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ShowButton", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool ShowButton { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsActive", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsActive { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "OrderNr", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int OrderNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ButtonName", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string ButtonName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ImageUrl", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string ImageUrl { get; set; }

    }
}
