using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.LidioPos.Models.LidioPosFinishPaymentRequestModel
{
    public class LidioPosFinishPaymentRequestModel
    {
        public string orderId { get; set; }
        public string merchantProcessId { get; set; }
        public string merchantCustomField { get; set; }
        public double totalAmount { get; set; }

        public string currency { get; set; }
        public string systemTransId { get; set; }
        public string paymentInstrument { get; set; }
        public PaymentInstrumentInfo paymentInstrumentInfo { get; set; }
        public string customParameters { get; set; }
        public string clientType { get; set; }
        public string clientIp { get; set; }
        public int clientPort { get; set; }
        public string clientUserAgent { get; set; }
        public string clientInfo { get; set; }
    }

    public class PaymentInstrumentInfo
    {
        public StoredCardInfo storedCard { get; set; }
        public NewCardInfo newCard { get; set; }
        public BkmExpressInfo bkmExpress { get; set; }
        public GarantiPayInfo garantiPay { get; set; }
        public MaximumMobilInfo maximumMobil { get; set; }
        public EmoneyInfo emoney { get; set; }
        public object wireTransfer { get; set; } // Change object to specific type if needed
        public object directWireTransfer { get; set; } // Change object to specific type if needed
        public object instantLoan { get; set; } // Change object to specific type if needed
        public object ideal { get; set; } // Change object to specific type if needed
        public object sofort { get; set; } // Change object to specific type if needed
    }

    public class StoredCardInfo
    {
        public string otp { get; set; }
        public string cvv { get; set; }
        public PosAccountInfo posAccount { get; set; }
    }

    public class NewCardInfo
    {
        public PosAccountInfo posAccount { get; set; }
    }

    public class BkmExpressInfo
    {
        public PosAccountInfo posAccount { get; set; }
    }

    public class GarantiPayInfo
    {
        public PosAccountInfo posAccount { get; set; }
    }

    public class MaximumMobilInfo
    {
        public PosAccountInfo posAccount { get; set; }
    }

    public class EmoneyInfo
    {
        // No specific properties defined
    }

    public class PosAccountInfo
    {
        public int id { get; set; }
        public SubMerchantInfo subMerchant { get; set; }
    }

    public class SubMerchantInfo
    {
        public string subMerchantId { get; set; }
        public string vkn { get; set; }
        public string tckn { get; set; }
        public string terminalNo { get; set; }
    }

}
