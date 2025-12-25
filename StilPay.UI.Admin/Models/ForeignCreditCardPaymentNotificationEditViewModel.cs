using StilPay.Entities.Concrete;
using System.Collections.Generic;

namespace StilPay.UI.Admin.Models
{
    public class ForeignCreditCardPaymentNotificationEditViewModel : EditViewModel<ForeignCreditCardPaymentNotification>
    {
        public List<MemberType> MemberTypes { get; set; }
        public ForeignCreditCardPaymentNotificationEditViewModel()
        {
            MemberTypes = new List<MemberType>();
        }
    }
}
