using StilPay.Utility.Helper;


namespace StilPay.Entities.Concrete
{
    public class Todo : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "Title", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Title { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "Body", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string Body { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "PriorityType", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int PriorityType { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "IsCompleted", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsCompleted { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = true, FK = true, Name = "ResponseMessage", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string ResponseMessage { get; set; }
    }
}
