using StilPay.Entities.Concrete;
using System.Collections.Generic;

namespace StilPay.UI.Dealer.Models
{
    public class CompanyEditViewModel : EditViewModel<Company>
    {
        public CompanyIntegration Integration { get; set; }
        public List<CompanyBank> CompanyBanks { get; set; }
        public List<CompanyPaymentInstitution> CompanyPaymentInstitutions { get; set; }
        

        public CompanyEditViewModel()
        {
            Integration = new CompanyIntegration();
            CompanyBanks = new List<CompanyBank>();
        }
    }
}
