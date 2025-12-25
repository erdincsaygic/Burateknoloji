namespace StilPay.Entities.Concrete
{
    public class Main : BaseEntity
    {
        public short PaymentNotifications { get; set; }
        public short CreditCardPaymentNotifications { get; set; }
        public short ForeignCreditCardPaymentNotifications { get; set; }
        public short CompanyPaymentRequests { get; set; }
        public short MemberPaymentRequests { get; set; }
        public short CompanyWithdrawalRequests { get; set; }
        public short MemberWithdrawalRequests { get; set; }
        public short CompanyRebateRequests { get; set; }
        public short MemberMoneyTransferRequests { get; set; }
        public short CompanyApplications { get; set; }
        public short TotalPending { get; set; }
        public short Supports { get; set; }
    }
}
