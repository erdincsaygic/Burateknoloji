using StilPay.Entities.Concrete;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.BLL.Abstract
{
    public interface ICompanyBankAccountManager : IBaseBLL<CompanyBankAccount>
    {
        List<BankAccountSumModel> CompanyBankAccountSum(string idCompany, string IDBank, DateTime? startDate, DateTime? endDate);

        List<BankAccountSumModel> CreditCardAccountSum(string idCompany, string IDBank, DateTime? startDate, DateTime? endDate);

        GenericResponse SetIsActiveByDefault(string id, bool IsActiveByDefaultExpenseAccount);
    }
}
