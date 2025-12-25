using StilPay.Entities.Concrete;

namespace StilPay.UI.Dealer.Models
{
    public class RebateRequestEditViewModel : EditViewModel<CompanyRebateRequest>
    {
        public PaymentNotification PaymentNotification { get; set; }
        public CreditCardPaymentNotification CreditCardPaymentNotification { get; set; }
        public ForeignCreditCardPaymentNotification ForeignCreditCardPaymentNotification { get; set; }
        public string IDMember { get; set; }
        public bool hasProcess { get; set; }
        public bool isIbanValid { get; set; }
        public RebateRequestEditViewModel()
        {
        }

    }
}
