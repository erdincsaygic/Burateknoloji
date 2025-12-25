using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Concrete
{
    public class CreditCardAccountSummaryReportDetail : BaseEntity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "AccountSummaryReportID", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public long AccountSummaryReportNo { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "PaymentInstitutionID", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string PaymentInstitutionID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "PaymentInstitutionName", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string PaymentInstitutionName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Balance", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Balance { get; set; }
    }
}
