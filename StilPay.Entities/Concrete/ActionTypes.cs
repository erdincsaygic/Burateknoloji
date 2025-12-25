using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class ActionTypes : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IDActionType", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDActionType { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ActionType", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string ActionType { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ActionName", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string ActionName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IDParasut", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDParasut { get; set; }
    }
}
