using DocumentFormat.OpenXml.Wordprocessing;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Concrete
{
    public class SmsLog : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Phone", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Phone { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SmsMessage", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string SmsMessage { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "OperationType", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string OperationType { get; set; }


        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IDCompany", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string IDCompany { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IDCompany", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string IDMember { get; set; }
    }
}
