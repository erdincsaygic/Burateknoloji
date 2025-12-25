using System;

namespace StilPay.Utility.Helper
{
    public class FieldAttribute : Attribute
    {
        public bool AutoIncrement { get; set; }
        public bool PK { get; set; }
        public bool FK { get; set; }
        public string Name { get; set; }
        public Enums.FieldType FieldType { get; set; }
        public string Description { get; set; }
        public bool Nullable { get; set; }
        public bool isMaster { get; set; }
    }
}
