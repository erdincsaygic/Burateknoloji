using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.IsBankTransferService.Models.IsBankAccounts
{
    public class IsBankAccountsRequestModel
    {
        public string authorization { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string certificate { get; set; }
    }
}
