using Microsoft.AspNetCore.Mvc;
using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using System.Collections.Generic;

namespace StilPay.UI.Admin.Models
{
    public class DealerTransactionQueryEditViewModel 
    {
        public List<DealerTransactionQuery> DealerTransactionQuery { get; set; }
        public string Url { get; set; }
        public string SecondUrl { get; set; }
        public DealerTransactionQueryEditViewModel()
        {
        }
    }
}
