using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Concrete
{
    public class BankTransferAccountSummaryReportDetail : BaseEntity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "AccountSummaryReportID", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public long AccountSummaryReportNo { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IDBank", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "BankName", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string BankName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsExitAccount", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsExitAccount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Balance", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Balance { get; set; }
    }
}
