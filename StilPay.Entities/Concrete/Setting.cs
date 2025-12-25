using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class Setting : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ParamType", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string ParamType { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ParamDef", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string ParamDef { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ParamVal", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string ParamVal { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ActivatedForGeneralUse", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool ActivatedForGeneralUse { get; set; }
        
    }
}
