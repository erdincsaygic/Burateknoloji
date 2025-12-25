using StilPay.Utility.Helper;
using System.Numerics;

namespace StilPay.Entities.Concrete
{
    public class Company : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Name", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Name { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Phone", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Phone { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Title", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Title { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TaxNr", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string TaxNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TaxOffice", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string TaxOffice { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Address", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Address { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MonthlyGiro", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string MonthlyGiro { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Email", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Email { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Password", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string Password { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IdentityFrontSide", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public byte[] IdentityFrontSide { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IdentityBackSide", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public byte[] IdentityBackSide { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TaxPlate", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public byte[] TaxPlate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SignatureCirculars", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public byte[] SignatureCirculars { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TradeRegistryGazette", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public byte[] TradeRegistryGazette { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Agreement", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public byte[] Agreement { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IdentityFrontSideStatus", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string IdentityFrontSideStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IdentityBackSideStatus", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string IdentityBackSideStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TaxPlateStatus", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string TaxPlateStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SignatureCircularsStatus", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string SignatureCircularsStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TradeRegistryGazetteStatus", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string TradeRegistryGazetteStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "AgreementStatus", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string AgreementStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "StatusFlag", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool StatusFlag { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IDParasut", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string IDParasut { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "NegativeBalanceLimit", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal NegativeBalanceLimit { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "AutoWithdrawalLimit", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal AutoWithdrawalLimit { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "AutoTransferLimit", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal AutoTransferLimit { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "AutoCreditCardLimit", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal AutoCreditCardLimit { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "AutoForeignCreditCardLimit", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal AutoForeignCreditCardLimit { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "DataStatus", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public int DataStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "District", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string District { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "City", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string City { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsAbroad", FieldType = Enums.FieldType.Bit, Description = "", Nullable = true)]
        public bool IsAbroad { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "InvoiceTitle", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string InvoiceTitle { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ProgressPaymentIban", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string ProgressPaymentIban { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ProgressPaymentAccountHolder", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string ProgressPaymentAccountHolder { get; set; }

        //[FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CurrencyCode", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        //public string CurrencyCode { get; set; }

        //[FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CurrencyName", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        //public string CurrencyName { get; set; }

        //[FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CurrencySymbol", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        //public string CurrencySymbol { get; set; }

        //[FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsExchangeCreditCardPanel", FieldType = Enums.FieldType.Bit, Description = "", Nullable = true)]
        //public bool IsExchangeCreditCardPanel { get; set; }
    }
}
