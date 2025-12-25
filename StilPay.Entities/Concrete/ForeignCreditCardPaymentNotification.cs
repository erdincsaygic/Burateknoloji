using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace StilPay.Entities.Concrete
{
    public class ForeignCreditCardPaymentNotification : BaseEntity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CDate", FieldType = Enums.FieldType.None, Description = "CDate", Nullable = false)]
        public DateTime CDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MDate", FieldType = Enums.FieldType.None, Description = "MDate", Nullable = true)]
        public DateTime? MDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MUser", FieldType = Enums.FieldType.NVarChar, Description = "MUser", Nullable = true)]
        public string MUser { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Modifier", FieldType = Enums.FieldType.None, Description = "Modifier", Nullable = false)]
        public string Modifier { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDMember", FieldType = Enums.FieldType.NVarChar, Description = "IDMember", Nullable = false)]
        public string IDMember { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Member", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Member { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "ServiceID", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string ServiceID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "TransactionNr", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string TransactionNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "TransactionID", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string TransactionID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Phone", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Phone { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CountryCode", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string CountryCode { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Commission", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public decimal Commission { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ActionDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime ActionDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ActionTime", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string ActionTime { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Amount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Amount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SenderName", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string SenderName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SenderIdentityNr", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string SenderIdentityNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Description", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string Description { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Status", FieldType = Enums.FieldType.Tinyint, Description = "", Nullable = false)]
        public byte Status { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsAutoNotification", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public bool IsAutoNotification { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CardNumber", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string CardNumber { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionReferenceCode", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string TransactionReferenceCode { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MemberIPAddress", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string MemberIPAddress { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MemberPort", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string MemberPort { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Company", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Company { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CompanyPhone", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string CompanyPhone { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "NetTotal", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public decimal NetTotal { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "BlockedDate", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public DateTime BlockedDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "BlockedDays", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public int BlockedDays { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MemberType", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string MemberType { get; set; }


        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CommissionRate", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public decimal CommissionRate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "PaymentInstitutionCommissionRate", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal? PaymentInstitutionCommissionRate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "PaymentInstitutionNetAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal? PaymentInstitutionNetAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "PaymentInstitutionName", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string PaymentInstitutionName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CardTypeId", FieldType = Enums.FieldType.Int, Description = "", Nullable = true)]
        public int? CardTypeId { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CreditCardPaymentMethodID", FieldType = Enums.FieldType.Tinyint, Description = "", Nullable = true)]
        public byte CreditCardPaymentMethodID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CurrencyCode", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string CurrencyCode { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "EncryptedCardNumber", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string EncryptedCardNumber { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsTrustedCardNumber", FieldType = Enums.FieldType.Bit, Description = "", Nullable = true)]
        public bool IsTrustedCardNumber { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsCaughtInFraudControl", FieldType = Enums.FieldType.Bit, Description = "", Nullable = true)]
        public bool IsCaughtInFraudControl { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "FraudControlDescription", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string FraudControlDescription { get; set; }


        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CustomerName", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string CustomerName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CustomerEmail", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string CustomerEmail { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CustomerPhone", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string CustomerPhone { get; set; }
    }
}
