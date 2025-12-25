namespace StilPay.Utility.JobBank
{
    public class CompanyFraudControl
    {
        public bool IsTransferFraudControlActive { get; set; }
        public int TransferTimeSpanInRecentTransactionMinutes { get; set; }
        public int TransferDailyTransactionCount { get; set; }
        public decimal TransferFirstTransactionLimit { get; set; }
        public decimal TransferTimeSpanInRecentTransactionMinutesLimitAmount { get; set; }
        public decimal TransferDailyTransactionLimitAmount { get; set; }
        public int BeStoppedTransferDailyTransactionCount { get; set; }
    }
}
