using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.LidioPos.Models.LidioPosCancel
{
    public class LidioPosCancelRequestModel
    {
        public string orderId { get; set; }
        public string paymentInstrument { get; set; }
    }
}
