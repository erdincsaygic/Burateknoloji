using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Concrete
{
    public class CompanyCurrency : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "IDCompany", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDCompany { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "CurrencyCode", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string CurrencyCode { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "IsActive", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsActive { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Balance", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public decimal Balance { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "BlockedBalance", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public decimal BlockedBalance { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CurrencyName", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string CurrencyName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CanCreateWithdrawalRequest", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool CanCreateWithdrawalRequest { get; set; }
    }
}
