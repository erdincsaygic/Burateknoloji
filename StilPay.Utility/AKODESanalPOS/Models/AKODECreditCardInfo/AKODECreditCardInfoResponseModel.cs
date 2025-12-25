using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.AKODESanalPOS.Models.AKODECreditCardInfo
{
    public class AKODECreditCardInfoResponseModel
    {
        public class InstallmentInfo
        {
            public InstallmentDetail T2 { get; set; }
            public InstallmentDetail T3 { get; set; }
            public InstallmentDetail T4 { get; set; }
            public InstallmentDetail T5 { get; set; }
            public InstallmentDetail T6 { get; set; }
            public InstallmentDetail T7 { get; set; }
            public InstallmentDetail T8 { get; set; }
            public InstallmentDetail T9 { get; set; }
            public InstallmentDetail T10 { get; set; }
            public InstallmentDetail T11 { get; set; }
            public InstallmentDetail T12 { get; set; }
        }

        public class InstallmentDetail
        {
            public double Rate { get; set; }
            public int Constant { get; set; }
        }

        public class CardInfo
        {
            public int CardPrefix { get; set; }
            public int BankId { get; set; }
            public string BankCode { get; set; }
            public string BankName { get; set; }
            public string CardName { get; set; }
            public string CardClass { get; set; }
            public string CardType { get; set; }
            public string Country { get; set; }
            public double BankCommission { get; set; }
            public InstallmentInfo InstallmentInfo { get; set; }
            public int Code { get; set; }
            public string Message { get; set; }
        }
    }
}
