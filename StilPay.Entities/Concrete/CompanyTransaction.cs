using StilPay.Utility.Helper;
using System;

namespace StilPay.Entities.Concrete
{
    public class CompanyTransaction : CompanyEntity
    {

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDActionType", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDActionType { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ActionType", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string ActionType { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime TransactionDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionNr", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string TransactionNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDBank", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Bank", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Bank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Total", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Total { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Commission", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Commission { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CostTotal", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal CostTotal { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "NetTotal", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal NetTotal { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionTotal", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal TransactionTotal { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDMember", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string IDMember { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "Member", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Member { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDCompanyInvoice", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string IDCompanyInvoice { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CommissionNetAmount", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public decimal CommissionNetAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CommissionTaxAmount", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public decimal CommissionTaxAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CommissionRate", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal CommissionRate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CompanyCommission", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal CompanyCommission { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CompanyCommissionRate", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal CompanyCommissionRate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Balance", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public decimal Balance { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "BalanceTotal", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public decimal BalanceTotal { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "BalanceCommission", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public decimal BalanceCommission { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "RebateTotal", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public decimal RebateTotal { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "RebateTotalCount", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public long RebateTotalCount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CommissionCount", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public long CommissionCount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "WithdrawalRequestSIDBank", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string WithdrawalRequestSIDBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CreditCardPaymentMethodID", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public byte CreditCardPaymentMethodID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SIDBank", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string SIDBank { get; set; }
    }
}
