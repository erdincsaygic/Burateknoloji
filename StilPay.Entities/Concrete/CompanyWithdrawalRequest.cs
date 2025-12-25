using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class CompanyWithdrawalRequest : CompanyEntity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionNr", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string TransactionNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IDBank", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "Bank", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string Bank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IBAN", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IBAN { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Title", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Title { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Amount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Amount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CostTotal", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal CostTotal { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsEFT", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsEFT { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Description", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string Description { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "RequestNr", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string RequestNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Status", FieldType = Enums.FieldType.Tinyint, Description = "", Nullable = false)]
        public byte Status { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "DealerDescription", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string DealerDescription { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsBankQueryNr", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string IsBankQueryNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SIDBank", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string SIDBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsProcess", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsProcess { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SBankName", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string SBankName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "CompanyBankAccountID", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string CompanyBankAccountID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "CurrencyCode", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string CurrencyCode { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "SPCostAmount", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public decimal SPCostAmount { get; set; }
    }
}
