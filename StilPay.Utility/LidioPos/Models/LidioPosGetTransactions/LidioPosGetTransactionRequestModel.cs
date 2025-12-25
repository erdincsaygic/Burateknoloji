using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.LidioPos.Models.LidioPosGetTransactions
{
    public class LidioPosGetTransactionRequestModel
    {
        public string paymentInstrument = "Card";
        public string startDate { get; set; }
        public string endDate { get; set; }

        public bool sortDesc = true;

        public string sortField = "TransactionDate";

        public InstrumentInfo instrumentInfo { get; set; } 
    }

    public class InstrumentInfo
    {
        public GetTransactionCard Card { get; set; }  
    }
    
    public class GetTransactionCard
    {
        public string processType {  get; set; }
    }
}
