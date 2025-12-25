using StilPay.Utility.Helper;
using System;

namespace StilPay.Entities.Concrete
{
    public class CompanyRebateRequest : CompanyEntity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionNr", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string TransactionNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionDate", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public DateTime TransactionDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "TransactionID", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string TransactionID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDMember", FieldType = Enums.FieldType.None, Description = "IDMember", Nullable = false)]
        public string IDMember { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Member", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Member { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MemberPhone", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public virtual string MemberPhone { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDBank", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string IDBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "Iban", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string Iban { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "Bank", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Bank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ActionDate", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public DateTime ActionDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ActionTime", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string ActionTime { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Amount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Amount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SenderName", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string SenderName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SenderIdentityNr", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string SenderIdentityNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Description", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Description { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Status", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public byte Status { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsBankQueryNr", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string IsBankQueryNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SIDBank", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string SIDBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "PaymentRecordID", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string PaymentRecordID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "CompanyBankAccountID", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string CompanyBankAccountID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IsPartialRebate", FieldType = Enums.FieldType.Bit, Description = "", Nullable = true)]
        public bool IsPartialRebate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "PartialRebatePaymentInstitutionTotalAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal PartialRebatePaymentInstitutionTotalAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "PartialRebatePaymentInstitutionCommission", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal PartialRebatePaymentInstitutionCommission { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "PaymentedEntityID", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string PaymentedEntityID { get; set; }
    }
}
