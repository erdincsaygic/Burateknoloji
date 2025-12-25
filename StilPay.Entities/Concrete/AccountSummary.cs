using StilPay.Utility.Helper;

namespace StilPay.Entities.Concrete
{
    public class AccountSummary : Entity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ReportNo", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public long ReportNo { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Profit", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Profit { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ExpenseAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal ExpenseAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IncomeAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal IncomeAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Difference", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal Difference { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "PaymentNotificationProfit", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal PaymentNotificationProfit { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CreditCardPaymentNotificationProfit", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal CreditCardPaymentNotificationProfit { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ForeignCreditCardPaymentNotificationProfit", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal ForeignCreditCardPaymentNotificationProfit { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "WithdrawalRequestProfit", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal WithdrawalRequestProfit { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "DealerTotalBalance", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal DealerTotalBalance { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "UnmatchedPaymentNotificationBalance", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal UnmatchedPaymentNotificationBalance { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "NetAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal NetAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "PreviousReportNetAmount", FieldType = Enums.FieldType.None, Description = "", Nullable = true)]
        public decimal PreviousReportNetAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "RebateExpenseProfitAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal RebateExpenseProfitAmount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsMonthly", FieldType = Enums.FieldType.Bit, Description = "", Nullable = true)]
        public bool IsMonthly { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Month", FieldType = Enums.FieldType.Int, Description = "", Nullable = true)]
        public int Month { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Year", FieldType = Enums.FieldType.Int, Description = "", Nullable = true)]
        public int Year { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CreditCardPoolBalance", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal CreditCardPoolBalance { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "FraudPoolBalance", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal FraudPoolBalance { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "FraudExpenseProfitAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        public decimal FraudExpenseProfitAmount { get; set; }

        //[FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "DailyTotalPaymentAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        //public decimal DailyTotalPaymentAmount { get; set; }

        //[FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "WeeklyTotalPaymentAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        //public decimal WeeklyTotalPaymentAmount { get; set; }

        //[FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MonthlyTotalPaymentAmount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = true)]
        //public decimal MonthlyTotalPaymentAmount { get; set; }

        //[FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Json", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        //public string Json { get; set; }
    }
}
