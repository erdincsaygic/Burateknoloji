using DocumentFormat.OpenXml.Wordprocessing;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Concrete
{
    public class SystemSetting : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "DefaultCreditCardPaymentWithParam", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool DefaultCreditCardPaymentWithParam { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "DefaultCreditCardPaymentWithPayNKolay", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool DefaultCreditCardPaymentWithPayNKolay { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "DefaultTransferBeUsed", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool DefaultTransferBeUsed { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "DefaultCreditCardBeUsed", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool DefaultCreditCardBeUsed { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "EftStartTime", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string EftStartTime { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "EftEndTime", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string EftEndTime { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "DefaultMoneyTransferWithZiraatBank", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool DefaultMoneyTransferWithZiraatBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "DefaultMoneyTransferWithIsBank", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool DefaultMoneyTransferWithIsBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "DefaultAcceptPaymentWithZiraatBank", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool DefaultAcceptPaymentWithZiraatBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "DefaultAcceptPaymentWithIsBank", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool DefaultAcceptPaymentWithIsBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "DefaultForeignCreditCardBeUsed", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool DefaultForeignCreditCardBeUsed { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "DefaultForeignCreditCardPaymentWithPayNKolay", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool DefaultForeignCreditCardPaymentWithPayNKolay { get; set; }
    }
}
