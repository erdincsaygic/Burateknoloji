using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Concrete
{
    public class Currency : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "CurrencyCode", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string CurrencyCode { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CurrencyName", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string CurrencyName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CurrencySymbol", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string CurrencySymbol { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "CompanyCurrencyBalance", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public decimal CompanyCurrencyBalance { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CompanyCurrencySymbol", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string CompanyCurrencySymbol { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CompanyCurrencyCode", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string CompanyCurrencyCode { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsActive", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool IsActive { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CanCreateWithdrawalRequest", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool CanCreateWithdrawalRequest { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Balance", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public decimal Balance { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "BlockedBalance", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public decimal BlockedBalance { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsAddedCurrency", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool IsAddedCurrency { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IDCompany", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string IDCompany { get; set; }
    }
}
