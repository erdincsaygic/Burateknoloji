using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.KuveytTurk.KuveytTurkTransfer
{
    public class KuveytTurkTransferRequestModel
    {
        public string CorporateWebUserName { get; set; }
        public decimal MoneyTransferAmount { get; set; }
        public string MoneyTransferDescription { get; set; }
        public string ReceiverIBAN { get; set; }
        public string ReceiverName { get; set; }
        public int SenderAccountSuffix { get; set; }
        public string TransactionGuid { get; set; }
        public int TransferType { get; set; }
    }
}
