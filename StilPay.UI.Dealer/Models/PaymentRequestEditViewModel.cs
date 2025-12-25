using StilPay.Entities.Concrete;
using System.Collections.Generic;

namespace StilPay.UI.Dealer.Models
{
    public class PaymentRequestEditViewModel : EditViewModel<CompanyPaymentRequest>
    {
        public List<CompanyBank> CompanyBanks { get; set; }

        public PaymentRequestEditViewModel()
        {
            CompanyBanks = new List<CompanyBank>();
        }

    }
}
