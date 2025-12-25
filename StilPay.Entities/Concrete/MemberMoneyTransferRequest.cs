using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class MemberMoneyTransferRequest : MemberEntity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionNr", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string TransactionNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ReceiverPhone", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string ReceiverPhone { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ReceiverName", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string ReceiverName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Amount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Amount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CostTotal", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal CostTotal { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Description", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string Description { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Status", FieldType = Enums.FieldType.Tinyint, Description = "", Nullable = false)]
        public byte Status { get; set; }


    }
}
