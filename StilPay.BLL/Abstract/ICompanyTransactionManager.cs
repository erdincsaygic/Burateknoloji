using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.BLL.Abstract
{
    public interface ICompanyTransactionManager : IBaseBLL<CompanyTransaction>
    {
        List<CompanyTransaction> GetProcess(string idCompany, DateTime startDate, DateTime endDate, int pageLenght, int offsetValue, string searchValue, bool isMonthlyAccountSummary);

        List<CompanyTransaction> GetListOld(string idCompany, DateTime? startDate, DateTime? endDate);
        List<CompanyTransaction> GetProcess(string idCompany, DateTime startDate, DateTime endDate, string iDActionType, string iDCompanyInvoice, bool invoiceStatus);
        List<MonthlyAccountSummary> GetMonthlyAccountSummary(DateTime dailyReportStartDateTime, DateTime dailyReportEndDateTime, DateTime reportStartDateTime, DateTime reportEndDateTime, DateTime weeklyReportStartDateTime, DateTime weeklyReportEndDateTime
            );
        List<DealerAccountSummary> GetDealerAccountSummary(string idCompany, DateTime reportStartDateTime, DateTime reportEndDateTime);
        List<DealerAccountSummaryForDealer> GetDealerAccountSummaryForDealer(string idCompany, DateTime reportStartDateTime, DateTime reportEndDateTime);
        bool ConvertToInvoice(List<string> idList);

        AccountReportModel GetAccountReport(string idCompany, DateTime startDate, DateTime endDate , DateTime startDateTime , DateTime endDateTime);

        List<AccountReportCreditCardModel> GetAccountReportCreditCardDetail(string idCompany, DateTime startDate, DateTime endDate);
        List<AccountReportCreditCardModel> BankTransactionsDetailIncomeList(string idBank, DateTime startDate, DateTime endDate);
        List<AccountReportCreditCardModel> BankTransactionsDetailExpenseList(string idBank, DateTime startDate, DateTime endDate);
        List<AccountReportCreditCardModel> BankTransactionsDetailTransferList(string idBank, DateTime startDate, DateTime endDate);

        List<DealerTransactionQuery> GetRecordsByQueryParameter(string queryParameter);
        List<BankTransactionList> GetBankTransactionList(List<FieldParameter> parameters);
        List<CreditCardTransactionList> GetCreditCardTransactionList(List<FieldParameter> parameters);
        decimal GetDealerTransactionBalance(List<FieldParameter> parameters);

        GenericResponse SetStatusToFraud(SetStatusToFraudDto setStatusToFraudDto);
        List<DealerFraudPoolDto> GetDealerFraudPool(List<FieldParameter> parameters);
        List<CreditCardDetailedSearchDto> CreditCardDetailedSearch(List<FieldParameter> parameters);
        List<CustomerInfoDetailedSearchDto> CustomerInfoDetailedSearch(List<FieldParameter> parameters);
        List<TransferDetailedSearchDto> TransferDetailedSearch(List<FieldParameter> parameters);
    }
}
