using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Dto
{
    public class DealerTransactionQuery
    {
        public string ID { get; set; }  
        public byte Status { get; set; }
        public byte TableWithTheTransaction { get; set; }
    }
}
