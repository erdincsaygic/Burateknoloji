using StilPay.Entities.Concrete;
using System.Collections.Generic;

namespace StilPay.UI.Admin.Models
{
    public class BankAccountEditViewModel : EditViewModel<CompanyBankAccount>
    {
        public List<Bank> Banks { get; set; }

        public BankAccountEditViewModel()
        {
            Banks = new List<Bank>();
        }
    }
}
