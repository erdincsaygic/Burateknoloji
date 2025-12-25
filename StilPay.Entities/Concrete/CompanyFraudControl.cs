using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class CompanyFraudControl : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsCreditCardFraudControlActive", FieldType = Enums.FieldType.Bit, Description = "", Nullable = true)]
        public bool IsCreditCardFraudControlActive { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CreditCardTimeSpanInRecentTransactionMinutes", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int CreditCardTimeSpanInRecentTransactionMinutes { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CreditCardDailyTransactionCount", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int CreditCardDailyTransactionCount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CreditCardFirstTransactionLimit", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal CreditCardFirstTransactionLimit { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CreditCardTimeSpanInRecentTransactionMinutesLimitAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal CreditCardTimeSpanInRecentTransactionMinutesLimitAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CreditCardDailyTransactionLimitAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal CreditCardDailyTransactionLimitAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "BeStoppedCreditCardDailyTransactionCount", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int BeStoppedCreditCardDailyTransactionCount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsForeignCreditCardFraudControlActive", FieldType = Enums.FieldType.Bit, Description = "", Nullable = true)]
        public bool IsForeignCreditCardFraudControlActive { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ForeignCreditCardTimeSpanInRecentTransactionMinutes", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int ForeignCreditCardTimeSpanInRecentTransactionMinutes { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ForeignCreditCardDailyTransactionCount", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int ForeignCreditCardDailyTransactionCount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ForeignCreditCardFirstTransactionLimit", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal ForeignCreditCardFirstTransactionLimit { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ForeignCreditCardTimeSpanInRecentTransactionMinutesLimitAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal ForeignCreditCardTimeSpanInRecentTransactionMinutesLimitAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ForeignCreditCardDailyTransactionLimitAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal ForeignCreditCardDailyTransactionLimitAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "BeStoppedForeignCreditCardDailyTransactionCount", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int BeStoppedForeignCreditCardDailyTransactionCount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsSmsConfirmationActiveTransfer", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsSmsConfirmationActiveTransfer { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsSmsConfirmationActiveCreditCard", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsSmsConfirmationActiveCreditCard { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsSmsConfirmationActiveForeingCreditCard", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsSmsConfirmationActiveForeingCreditCard { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsTransferFraudControlActive", FieldType = Enums.FieldType.Bit, Description = "", Nullable = true)]
        public bool IsTransferFraudControlActive { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransferTimeSpanInRecentTransactionMinutes", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int TransferTimeSpanInRecentTransactionMinutes { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransferDailyTransactionCount", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int TransferDailyTransactionCount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransferFirstTransactionLimit", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal TransferFirstTransactionLimit { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransferTimeSpanInRecentTransactionMinutesLimitAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal TransferTimeSpanInRecentTransactionMinutesLimitAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransferDailyTransactionLimitAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal TransferDailyTransactionLimitAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "BeStoppedTransferDailyTransactionCount", FieldType = Enums.FieldType.Int, Description = "", Nullable = false)]
        public int BeStoppedTransferDailyTransactionCount { get; set; }
    }
}
