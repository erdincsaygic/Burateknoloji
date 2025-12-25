using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Concrete
{
    public class CompanyPaymentInstitution : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "PaymentInstitutionID", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string PaymentInstitutionID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "PaymentInstitutionName", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string PaymentInstitutionName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsActive", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsActive { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "RedirectToActionGetThreeDView", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string RedirectToActionGetThreeDView { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "RedirectToActionPaymentMethod", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string RedirectToActionPaymentMethod { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "UseForForeignCard", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool UseForForeignCard { get; set; }

    }
}
