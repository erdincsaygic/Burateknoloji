using StilPay.Entities.Concrete;
using System.Collections.Generic;

namespace StilPay.UI.WebSite.Areas.Panel.Models
{
    public class PaymentRequestEditViewModel : EditViewModel<MemberPaymentRequest>
    {
        public List<Bank> Banks { get; set; }

        public PaymentRequestEditViewModel()
        {
            Banks = new List<Bank>();
        }

    }
}
