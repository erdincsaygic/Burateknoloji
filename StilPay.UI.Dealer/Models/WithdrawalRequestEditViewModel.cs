using Microsoft.AspNetCore.Mvc.ViewComponents;
using StilPay.Entities.Concrete;
using StilPay.UI.Dealer.Infrastructures;
using System.Collections.Generic;

namespace StilPay.UI.Dealer.Models
{
    public class WithdrawalRequestEditViewModel : EditViewModel<CompanyWithdrawalRequest>
    {
        public List<Bank> Banks { get; set; }
        public List<CompanyBankAccount> BankAccounts { get; set; }
        public List<CompanyCurrency> CompanyCurrencies { get; set; }
        public decimal WithdrawalTransferAmount { get; set; }
        public decimal WithdrawalEftAmount { get; set; }
        public decimal WithdrawalForeignCurrencySwiftAmount { get; set; }
        public WithdrawalRequestEditViewModel()
        {
            Banks = new List<Bank>();
            BankAccounts = new List<CompanyBankAccount>();
            CompanyCurrencies = new List<CompanyCurrency>();
        }

    }
}
