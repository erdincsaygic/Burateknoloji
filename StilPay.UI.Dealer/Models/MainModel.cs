using StilPay.Entities.Concrete;
using System.Collections.Generic;

namespace StilPay.UI.Dealer.Models
{
    public class MainModel
    {
        public string Dealer { get; set; }
        public string LoginDate { get; set; }
        public string LoginIP { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal UsingBalance { get; set; }
        public decimal BlockedBalance { get; set; }
        public List<Announcement> Announcements { get; set; }
        public List<PaymentNotification> PaymentNotifications { get; set; }
        public List<CompanyCurrency> CompanyCurrencies { get; set; }
        public Support entity { get; set; }
    }
}
