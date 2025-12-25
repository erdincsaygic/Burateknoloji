using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Concrete
{
    public class PaymentInstitution : BaseEntity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Name", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Name { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Show", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool Show { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Rate", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Rate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "DebitCardRate", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal DebitCardRate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsActive", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsActive { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "UseForForeignCard", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool UseForForeignCard { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsVisibleCompanyPanel", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool IsVisibleCompanyPanel { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ForSpecialCreditCard", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool ForSpecialCreditCard { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "RedirectToActionGetThreeDView", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string RedirectToActionGetThreeDView { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "RedirectToActionPaymentMethod", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string RedirectToActionPaymentMethod { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "RedirectToActionRebateMethod", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string RedirectToActionRebateMethod { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "EndOfDayTime", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string EndOfDayTime { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "ConsecutiveTransactionLimit", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int ConsecutiveTransactionLimit { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "CurrentTransactionCount", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int CurrentTransactionCount { get; set; }

    }
}
