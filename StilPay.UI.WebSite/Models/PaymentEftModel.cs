using System;

namespace StilPay.UI.WebSite.Models
{
    public class PaymentEftModel
    {
        public string Name { get; set; }
        public double AmountSP { get; set; }
        public double AmountTRY { get; set; }
        public double Commission { get; set; }
        public int IdBank { get; set; }
        public string BankName { get; set; }
        public string IbanNo { get; set; }
        public string AccountNumber { get; set; }
        public double PaymentAmount { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime OperationDate { get; set; }
        public string OperationClock { get; set; }
        public string Description { get; set; }



        //public string Name { get; set; }
        //public string Name { get; set; }
        //public string Name { get; set; }
        //public string Name { get; set; }
        //public string Name { get; set; }

    }
}
