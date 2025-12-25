using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class CustomerInfo : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionID", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string TransactionID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CustomerName", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string CustomerName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CustomerEmail", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string CustomerEmail { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CustomerPhone", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string CustomerPhone { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "ServiceID", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string ServiceID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Company", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public string Company { get; set; }
      
    }
}
