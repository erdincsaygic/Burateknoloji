using StilPay.Entities.Concrete;
using System.Collections.Generic;

namespace StilPay.UI.Admin.Models
{
    public class CreditCardPaymentNotificationEditViewModel : EditViewModel<CreditCardPaymentNotification>
    {
        public List<MemberType> MemberTypes { get; set; }
        public CreditCardPaymentNotificationEditViewModel()
        {
            MemberTypes = new List<MemberType>();
        }
    }
}
