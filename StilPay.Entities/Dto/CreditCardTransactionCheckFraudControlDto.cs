namespace StilPay.Entities.Dto
{
    public class CreditCardTransactionCheckFraudControlDto
    {
        public bool RecentTransactionLimitExceeded { get; set; }
        public bool DailyTransactionLimitExceeded { get; set; }
        public bool TransactionWithin24Hours { get; set; }
        public int TransactionCountToday { get; set; }
    }
}
