using DocumentFormat.OpenXml.Wordprocessing;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Concrete
{
    public class PublicHoliday : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Name", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Name { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "HolidayDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime HolidayDate { get; set; }
    }
}
