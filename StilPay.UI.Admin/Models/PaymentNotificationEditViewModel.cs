using StilPay.Entities.Concrete;
using System.Collections.Generic;

namespace StilPay.UI.Admin.Models
{
    public class PaymentNotificationEditViewModel : EditViewModel<PaymentNotification>
    {
        public List<MemberType> MemberTypes { get; set; }
        public PaymentNotificationEditViewModel()
        {
            MemberTypes = new List<MemberType>();
        }
    }
}
