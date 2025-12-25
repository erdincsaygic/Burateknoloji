using StilPay.Utility.Helper;
using System;

namespace StilPay.Entities.Concrete
{
    public class CompanyFinanceTransaction : Entity
    {

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDCompany", FieldType = Enums.FieldType.NVarChar, Description = "IDCompany", Nullable = false)]
        public string IDCompany { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDBankAccount", FieldType = Enums.FieldType.NVarChar, Description = "IDBankAccount", Nullable = true)]
        public string IDBankAccount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDBankAccount2", FieldType = Enums.FieldType.NVarChar, Description = "IDBankAccount2", Nullable = true)]
        public string IDBankAccount2 { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Amount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Amount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Piece", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Piece { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Description", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string Description { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "PaymentMethod", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int PaymentMethod { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionType", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int TransactionType { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionDetailType", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int TransactionDetailType { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CompanyIntegrationType", FieldType = Enums.FieldType.Int, Description = "", Nullable = true)]
        public int CompanyIntegrationType { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime TransactionDate { get; set; }


        
    }

}
