using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Dto
{
    public class NewDealerDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string TaxNr { get; set; }
        public string TaxOffice { get; set; }
        public string MonthlyGiro { get; set; }
        public string Address { get; set; }
        public decimal AutoWithdrawalLimit { get; set; }
        public decimal AutoTransferLimit { get; set; }
        public decimal AutoCreditCardLimit { get; set; }
        public decimal AutoForeignCreditCardLimit { get; set; }
        public decimal CreditCardRate { get; set; }
        public decimal TransferRate { get; set; }
        public decimal MobilePayRate { get; set; }
        public decimal WithdrawalTransferAmount { get; set; }
        public decimal WithdrawalEftAmount { get; set; }
        public decimal ForeignCreditCardRate { get; set; }
        public string SiteUrl { get; set; }
        public string CallbackUrl { get; set; }
        public string RedirectUrl { get; set; }
        public string IPAddress { get; set; }
        public string WithdrawalRequestCallBack { get; set; }

        public bool WithdrawalApiBeUsed { get; set; }
        public bool ForeignCreditCardBeUsed { get; set; }
        public bool TransferBeUsed { get; set; }
        public bool CreditCardBeUsed { get; set; }

        public string IDUser { get; set; }
    }
}
