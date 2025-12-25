using StilPay.Utility.Helper;
using System;

namespace StilPay.Entities.Concrete
{
    public class CompanyApplication : Entity
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

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Password", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Password { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IdentityFrontSide", FieldType = Enums.FieldType.VarBinary, Description = "", Nullable = true)]
        public byte[] IdentityFrontSide { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IdentityBackSide", FieldType = Enums.FieldType.VarBinary, Description = "", Nullable = true)]
        public byte[] IdentityBackSide { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TaxPlate", FieldType = Enums.FieldType.VarBinary, Description = "", Nullable = true)]
        public byte[] TaxPlate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SignatureCirculars", FieldType = Enums.FieldType.VarBinary, Description = "", Nullable = true)]
        public byte[] SignatureCirculars { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TradeRegistryGazette", FieldType = Enums.FieldType.VarBinary, Description = "", Nullable = true)]
        public byte[] TradeRegistryGazette { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Agreement", FieldType = Enums.FieldType.VarBinary, Description = "", Nullable = true)]
        public byte[] Agreement { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IdentityFrontSideStatus", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public byte IdentityFrontSideStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IdentityBackSideStatus", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public byte IdentityBackSideStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TaxPlateStatus", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public byte TaxPlateStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SignatureCircularsStatus", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public byte SignatureCircularsStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TradeRegistryGazetteStatus", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public byte TradeRegistryGazetteStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "AgreementStatus", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public byte AgreementStatus { get; set; }



        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IdentityFrontSideColor", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string IdentityFrontSideColor { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IdentityBackSideColor", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string IdentityBackSideColor { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TaxPlateColor", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string TaxPlateColor { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SignatureCircularsColor", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string SignatureCircularsColor { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TradeRegistryGazetteColor", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string TradeRegistryGazetteColor { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "AgreementColor", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string AgreementColor { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Website", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string Website { get; set; }
    }
}
