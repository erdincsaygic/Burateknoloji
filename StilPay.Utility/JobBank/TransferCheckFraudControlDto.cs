namespace StilPay.Utility.JobBank
{
    public class TransferCheckFraudControlDto
    {
        public bool RecentTransactionLimitExceeded { get; set; }
        public bool DailyTransactionLimitExceeded { get; set; }
        public bool TransactionWithin24Hours { get; set; }
        public bool IsCaughtInFraudControlPending { get; set; }
        public int TransactionCountToday { get; set; }
    }
}
