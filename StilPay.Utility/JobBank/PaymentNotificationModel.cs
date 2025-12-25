using StilPay.Utility.Helper;
using System;

namespace StilPay.Utility.JobBank
{
    public class PaymentNotificationModel
    {
        public string ID { get; set; }
        public DateTime CDate { get; set; }
        public string ServiceID { get; set; }
        public string TransactionID { get; set; }
        public string TransactionNr { get; set; }
        public string IDMember { get; set; }
        public DateTime ActionDateTime { get; set; }
        public decimal Amount { get; set; }
        public string SenderName { get; set; }
        public string SenderIdentityNr { get; set; }
        public string Iban { get; set; }
        public string Member { get; set; }
        public DateTime ActionDate { get; set; }
        public string ActionTime { get; set; }
        public string MemberIPAddress { get; set; }
        public string MemberPort { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public bool IsCaughtInFraudControl { get; set; }
        public string FraudControlDescription { get; set; }
    }

    public class PaymentTransferPoolModel
    {
        public string ID { get; set; }
        public DateTime CDate { get; set; }
        public DateTime TransactionDate { get; set; }
        public string IDBank { get; set; }
        public string SenderName { get; set; }
        public string SenderIban { get; set; }
        public decimal Amount { get; set; }
        public string TransactionKey { get; set; }
        public string Description { get; set; }
        public bool IsCaughtInFraudControl { get; set; }
        public string FraudControlDescription { get; set; }
    }
}
