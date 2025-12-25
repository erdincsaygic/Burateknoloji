using DocumentFormat.OpenXml.Wordprocessing;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Concrete
{
    public class PaymentCreditCardPool : BaseEntity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime? CDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime? MDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime TransactionDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "BankName", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string BankName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SenderName", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string SenderName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionType", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string TransactionType { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionKey", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string TransactionKey { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionID", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string TransactionID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Amount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Amount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Commission", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal Commission { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Status", FieldType = Enums.FieldType.Tinyint, Description = "", Nullable = false)]
        public byte Status { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Description", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Description { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "InstallmentCount", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int InstallmentCount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "PaymentMethodID", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int PaymentMethodID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "PaymentMethodName", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string PaymentMethodName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SPDiscardedCallbackStatus", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string SPDiscardedCallbackStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CBResponseStatus", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public int CBResponseStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CardNumber", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string CardNumber { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SPStatus", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public byte SPStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MatchesBalance", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public decimal MatchesBalance { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "NotMatchesBalance", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public decimal NotMatchesBalance { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MatchesCount", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public int MatchesCount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "NotMatchesCount", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public int NotMatchesCount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CardTypeId", FieldType = Enums.FieldType.Int, Description = "", Nullable = true)]
        public int? CardTypeId { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SPNetAmount", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public decimal SPNetAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "PaymentInstitutionNetAmount", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public decimal PaymentInstitutionNetAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "PaymentInstitutionTotalAmount", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public decimal PaymentInstitutionTotalAmount { get; set; }
    }
}
