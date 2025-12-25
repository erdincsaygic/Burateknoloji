using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.Entities.Concrete
{
    public class Blog : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = true, FK = false, Name = "IDBlogCategory", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDBlogCategory { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = false, Name = "BlogCategoryName", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string BlogCategoryName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Title", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Title { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Body", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Body { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Image", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string Image { get; set; }
    
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CitationUrl", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string CitationUrl { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "StatusFlag", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool StatusFlag { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "OrderNr", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int OrderNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "BlogDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime BlogDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "BlogCategories", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public List<BlogCategory> BlogCategories { get; set; }
    }
}
