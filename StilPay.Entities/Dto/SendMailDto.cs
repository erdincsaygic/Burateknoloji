using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Dto
{
    public class SendMailDto
    {
        public List<string> IDCompanies { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
