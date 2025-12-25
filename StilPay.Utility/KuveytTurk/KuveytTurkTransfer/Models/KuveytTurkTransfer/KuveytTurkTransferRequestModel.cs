using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.KuveytTurk.KuveytTurkTransfer.Models.KuveytTurkTransfer
{
    public class KuveytTurkTransferRequestModel
    {

        public string CorporateWebUserName = "ovg";
        public decimal MoneyTransferAmount = 1;
        public string MoneyTransferDescription = "Test";
        public string ReceiverIBAN = "TR930006400000144005016347";
        public string ReceiverName = "Arda";
        public int SenderAccountSuffix = 5;
        public string TransactionGuid = "12344";
        public int TransferType = 4;
    }
    
}
