using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.IsBankTransferService.Models.IsBankToken
{
    public class IsBankTokenRequestModel
    {
        public string Authorization { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }
}
