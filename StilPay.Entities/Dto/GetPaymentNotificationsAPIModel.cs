using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Dto
{
    public class GetPaymentNotificationsAPIModel
    {
        public string SenderName { get; set; }
        public string Phone { get; set; }
        public decimal Amount { get; set; }
        public string TransactionID { get; set; }
        public DateTime CDate { get; set; }
        public DateTime ActionDate { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }

    public class GetGetCreditCardTransactionsAPIModel
    {
        public string SenderName { get; set; }
        public string Phone { get; set; }
        public string CardNumber { get; set; }
        public decimal Amount { get; set; }
        public string TransactionID { get; set; }
        public string TransactionNr { get; set; }
        public DateTime CDate { get; set; }
        public DateTime ActionDate { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string MemberIPAddress { get; set; }
        public string MemberPort { get; set; }
        public string RebateStatus { get; set; }
    }
}
