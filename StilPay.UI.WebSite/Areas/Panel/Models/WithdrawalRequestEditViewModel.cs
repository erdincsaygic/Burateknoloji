using StilPay.Entities.Concrete;
using System.Collections.Generic;

namespace StilPay.UI.WebSite.Areas.Panel.Models
{
    public class WithdrawalRequestEditViewModel : EditViewModel<MemberWithdrawalRequest>
    {
        public List<Bank> Banks { get; set; }

        public WithdrawalRequestEditViewModel()
        {
            Banks = new List<Bank>();
        }

    }
}
