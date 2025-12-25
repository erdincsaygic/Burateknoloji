using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class CompanyCommission : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CreditCardRate", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal CreditCardRate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ForeignCreditCardRate", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal ForeignCreditCardRate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CreditCardStatus", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool CreditCardStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CreditCardBlocked", FieldType = Enums.FieldType.Tinyint, Description = "", Nullable = false)]
        public byte CreditCardBlocked { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ForeignCreditCardBlocked", FieldType = Enums.FieldType.Tinyint, Description = "", Nullable = false)]
        public byte ForeignCreditCardBlocked { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransferRate", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal TransferRate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransferStatus", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool TransferStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransferBlocked", FieldType = Enums.FieldType.Tinyint, Description = "", Nullable = false)]
        public byte TransferBlocked { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MobilePayRate", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal MobilePayRate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MobilePayStatus", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool MobilePayStatus { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MobilePayBlocked", FieldType = Enums.FieldType.Tinyint, Description = "", Nullable = false)]
        public byte MobilePayBlocked { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "WithdrawalTransferAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal WithdrawalTransferAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "WithdrawalEftAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal WithdrawalEftAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ToslaRate", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal ToslaRate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ToslaBlocked", FieldType = Enums.FieldType.Tinyint, Description = "", Nullable = false)]
        public byte ToslaBlocked { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "WithdrawalForeignCurrencySwiftAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal WithdrawalForeignCurrencySwiftAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SPWithdrawalTransferCostAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal SPWithdrawalTransferCostAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SPWithdrawalEftCostAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal SPWithdrawalEftCostAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "SPWithdrawalForeignCurrencySwiftCostAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal SPWithdrawalForeignCurrencySwiftCostAmount { get; set; }
    }
}
