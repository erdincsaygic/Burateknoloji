using StilPay.BLL.Concrete;
using StilPay.Entities.Concrete;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StilPay.UI.Admin.Models
{
    public class CompanyEditViewModel : EditViewModel<Company>
    {
        public CompanyIntegration Integration { get; set; }
        public List<CompanyBank> CompanyBanks { get; set; }
        public CompanyCommission Commission { get; internal set; }
        public CompanyBalance Balance { get; set; }
        public CompanyBalanceTransfer BalanceTransfer { get; set; }
        public CompanyIframeUseSetting IframeUseSetting { get; set; }
        public CreditCardPaymentMethod CreditCardPaymentMethods { get; set; }
        public decimal NegativeBalanceLimit { get; set; }
        public string CompanyStatus { get; set; }
        public List<CompanyUser> CompanyUsers { get; set; }
        public List<CompanyBank> PaymentsBanks { get; set; }
        public List<CompanyPaymentInstitution> CompanyPaymentInstitutions { get; set; }
        public List<PaymentInstitution> PaymentInstitutions { get; set; }
        public List<CompanyBankAccount> CompanyBankAccounts { get; set; }
        public List<Currency> Currencies { get; set; }
        public List<CompanyCurrency> CompanyCurrencies { get; set; }
        public CompanyFraudControl FraudControl { get; set; }
        public CompanyCreateWithdrawalRequestDto CompanyCreateWithdrawalRequest { get; set; }
        public CompanyAutoNotificationSetting CompanyAutoNotificationSettings { get; set; }

        public CompanyEditViewModel()
        {
            Integration = new CompanyIntegration();
            Commission = new CompanyCommission();
            Balance = new CompanyBalance();
            BalanceTransfer = new CompanyBalanceTransfer();
            IframeUseSetting = new CompanyIframeUseSetting();
            CreditCardPaymentMethods = new CreditCardPaymentMethod();
            CompanyUsers = new List<CompanyUser>();
            FraudControl = new CompanyFraudControl();
            CompanyAutoNotificationSettings= new CompanyAutoNotificationSetting();
        }

        public class CompanyBalance
        {
            public string Type { get; set; }
            public decimal Amount { get; set; }
            public decimal UsingBalance { get; set; }
            public decimal BlockedBalance { get; set; }
            public decimal TotalBalance { get; set; }
            public decimal NegativeBalance { get; set; }
        }

        public class CompanyBalanceTransfer
        {
            public string IDCompany { get; set; }
            public string ReceiverIDCompany { get; set; }
            public decimal Amount { get; set; }
            public string ConfirmCode { get; set; }
        }

        public class CompanyIframeUseSetting
        {
            public bool TransferBeUsed { get; set; }
            public bool CreditCardBeUsed { get; set; }
            public bool ForeignCreditCardBeUsed { get; set; }
            public bool WithdrawalApiBeUsed { get; set; }
        }
        public class CreditCardPaymentMethod
        {
            public bool CreditCardPaymentWithParam { get; set; }
            public bool CreditCardPaymentWithPayNKolay { get; set; }

            public bool ForeignCreditCardPaymentWithPayNKolay { get; set; }
        }

        public class PaymentsBank
        {
            public string IDBank { get; set; }
            public string Name { get; set; }
            public bool isActive { get; set; }
        }


        public class CompanyCreateWithdrawalRequestDto
        {
            public string ConfirmCode { get; set; }
            public decimal Amount { get; set; }
            public string IDCompany { get; set; }
            public string CurrencyCode { get; set; }
            public decimal CurrencyUsingBalance { get; set; }
            public decimal CurrencyCostTotal { get; set; }
            public string BankAccountID { get; set; }
        }
    }
}
