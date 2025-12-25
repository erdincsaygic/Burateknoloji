using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Dto
{
    public class SetStatusToFraudDto
    {
        public string EntityID { get; set; }
        public string IDCompany { get; set; }     
        public DateTime MDate { get; set; }
        public string MUser { get; set; }
        public string EntityActionType { get; set; }
        public string Description { get; set; }
        public string AdminAction { get; set; }
    }
}
