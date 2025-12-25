using StilPay.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StilPay.UI.Admin.Models
{
    public class AccountReportModel
    {
        public decimal SumTotal { get; set; }
        public decimal SumCommission { get; set; }
        public decimal SumNetTotal { get; set; }
        public decimal SumProcessTotal { get; set; }
        public decimal SumEft { get; set; }
        public decimal SumFast { get; set; }
        public decimal SumProcess { get; set; }
        public decimal SumExpense { get; set; }
        public decimal sumCreditCardCommission { get; set; }
        public decimal sumTransferCommission { get; set; }
        public decimal sumTrPoolNotMatchTotal { get; set; }

        public decimal CountEft { get; set; }
        public decimal CountFast { get; set; }
        public decimal CountProcess { get; set; }

        public decimal TransferTotalAmount { get; set; }
        public decimal TransferCommissionProfitAmount { get; set; }
        public decimal TransferDealerPaidAmount { get; set; }
        public decimal TransferNotMatchedAmount { get; set; }

        public decimal CreditCardTotalAmount { get; set; }
        public decimal CreditCardCommissionProfitAmount { get; set; }
        public decimal CreditCardDealerPaidAmount { get; set; }
        public decimal CreditCardPaymentInstitutionPaidAmount { get; set; }

        public decimal ForeignCreditCardTotalAmount { get; set; }
        public decimal ForeignCreditCardCommissionProfitAmount { get; set; }
        public decimal ForeignCreditCardDealerPaidAmount { get; set; }
        public decimal ForeignCreditCardPaymentInstitutionPaidAmount { get; set; }

        public decimal WithdrawalTotalAmount { get; set; }
        public decimal WithdrawalTotalPaidAmount { get; set; }
        public decimal WithdrawalEftAmount { get; set; }
        public decimal WithdrawalFastAmount { get; set; }
        public decimal WithdrawalTotalTransactionFeeAmount { get; set; }

        public decimal RebateExpenseProfitAmount { get; set; }
        public decimal FraudExpenseProfitAmount { get; set; }
        public decimal TotalIncomeAmount { get; set; }
        public decimal TotalExpenseAmount { get; set; }
    }
}
