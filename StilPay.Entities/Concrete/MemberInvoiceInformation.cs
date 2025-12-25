using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class MemberInvoiceInformation : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SendStatus", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool SendStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IDCity", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDCity { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IDTown", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDTown { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Address", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Address { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Member", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Member { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IdentityNr", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string IdentityNr { get; set; }
    }
}
