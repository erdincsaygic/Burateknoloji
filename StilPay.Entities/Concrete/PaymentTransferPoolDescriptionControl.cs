using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class PaymentTransferPoolDescriptionControl : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Name", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Name { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Phone", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Phone { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CardNumber", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string CardNumber { get; set; }
    }
}
