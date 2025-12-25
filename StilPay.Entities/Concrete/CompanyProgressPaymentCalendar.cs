using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Concrete
{
    public class CompanyProgressPaymentCalendar : BaseEntity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Date", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime Date { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ProgressPayment", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal ProgressPayment { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CompanyName", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string CompanyName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "RebateAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal RebateAmount { get; set; }
        
    }
}
