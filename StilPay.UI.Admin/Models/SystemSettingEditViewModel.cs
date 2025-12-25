using StilPay.Entities.Concrete;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StilPay.UI.Admin.Models
{
    public class SystemSettingEditViewModel
    {
        public List<Bank> Banks { get; set; }
        public List<Setting> Settings { get; set; }
        public List<PaymentInstitution> PaymentInstitutions { get; set; }
        public List<CompanyBankAccount> CompanyBankAccounts { get; set; }

        public SystemSettingEditViewModel()
        {
        }
    }
}
