using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class CompanyIntegration : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ServiceID", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string ServiceID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SecretKey", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string SecretKey { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SiteUrl", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string SiteUrl { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CallbackUrl", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string CallbackUrl { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "WithdrawalRequestCallback", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string WithdrawalRequestCallback { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransferBeUsed", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool TransferBeUsed { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CreditCardBeUsed", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool CreditCardBeUsed { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CreditCardPaymentWithParam", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool CreditCardPaymentWithParam { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CreditCardPaymentWithPayNKolay", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool CreditCardPaymentWithPayNKolay { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "RedirectUrl", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string RedirectUrl { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IPAddress", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string IPAddress { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ForeignCreditCardBeUsed", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool ForeignCreditCardBeUsed { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ForeignCreditCardPaymentWithPayNKolay", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool ForeignCreditCardPaymentWithPayNKolay { get; set; }

        //[FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SIDBankForPayments", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        //public string SIDBankForPayments { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "AutoCallbackUrl", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string AutoCallbackUrl { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "AutoCallbackWithdrawalUrl", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string AutoCallbackWithdrawalUrl { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "WithdrawalApiBeUsed", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public bool WithdrawalApiBeUsed { get; set; }

    }
}
