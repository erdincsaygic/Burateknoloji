using StilPay.Entities.Concrete;
using System.Collections.Generic;

namespace StilPay.UI.Admin.Models
{
    public class AccountingSummaryEditViewModel
    {
        public List<BankAccountSumModel> CompanyBankAccounts { get; set; }
        public List<PaymentInstitution> PaymentInstitutions { get; set; }
        public List<CreditCardAccountSummaryReportDetail> CreditCardAccountSummaryReportDetails { get; set; }
        public List<BankTransferAccountSummaryReportDetail> BankTransferAccountSummaryReportDetails  { get; set; }
        public AccountSummary AccountSummary { get; set; }

        public AccountingSummaryEditViewModel()
        {
        }
    }
}
