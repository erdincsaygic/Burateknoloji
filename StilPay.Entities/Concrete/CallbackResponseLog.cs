using DocumentFormat.OpenXml.Wordprocessing;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Concrete
{
    public class CallbackResponseLog : BaseEntity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CDate", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public DateTime CDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionID", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string TransactionID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ServiceType", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string ServiceType { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Callback", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Callback { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IDCompany", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDCompany { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionType", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string TransactionType { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CallbackStatus", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public byte CallbackStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Company", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string Company { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionNr", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string TransactionNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ResponseStatus", FieldType = Enums.FieldType.Tinyint, Description = "", Nullable = false)]
        public byte ResponseStatus { get; set; }


    }
}
