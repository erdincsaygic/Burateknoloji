using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Dto
{
    public class DealerFraudPoolDto
    {
        public string ID { get; set; }
        public DateTime CDate { get; set; }
        public DateTime MDate { get; set; }
        public int PaymentMethodType { get; set; }
        public string TransactionID { get; set; }
        public string TransactionNr{ get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string PaymentInstitutionName { get; set; }
        public decimal PaymentInstitutionNetAmount { get; set; }
        public decimal DealerCommission { get; set; }
        public string SenderName { get; set; }
        public string MemberName { get; set; }
        public string IDCompany { get; set; }
        public string Company { get; set; }
        public string CompanyPhone { get; set; }
        public string MemberPhone { get; set; }
        public string CardNumber { get; set; }
        public string MUser { get; set; }
        public string MUserName { get; set; }


        public int TotalRecords { get;set; }
    }
}