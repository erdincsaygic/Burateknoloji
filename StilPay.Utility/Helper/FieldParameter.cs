namespace StilPay.Utility.Helper
{
    public class FieldParameter
    {
        public Enums.FieldType FielType;
        public string Name;
        public object Value;

        public FieldParameter(string name, Enums.FieldType fieldType, object value)
        {
            Name = name;
            FielType = fieldType;
            Value = value;
        }
    }
}
