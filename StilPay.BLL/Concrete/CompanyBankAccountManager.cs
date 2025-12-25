using DocumentFormat.OpenXml.Drawing.Diagrams;
using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.BLL.Concrete
{
    public class CompanyBankAccountManager : BaseBLL<CompanyBankAccount>, ICompanyBankAccountManager
    {
        public CompanyBankAccountManager(ICompanyBankAccountDAL dal) : base(dal)
        {

        }

        public List<BankAccountSumModel> CompanyBankAccountSum(string idCompany, string IDBank, DateTime? startDate, DateTime? endDate)
        {
            return ((ICompanyBankAccountDAL)_dal).CompanyBankAccountSum(idCompany, IDBank, startDate, endDate);
        }

        public List<BankAccountSumModel> CreditCardAccountSum(string idCompany, string IDBank, DateTime? startDate, DateTime? endDate)
        {
            return ((ICompanyBankAccountDAL)_dal).CreditCardAccountSum(idCompany, IDBank, startDate, endDate);
        }

        public GenericResponse SetIsActiveByDefault(string id, bool IsActiveByDefaultExpenseAccount)
        {
            try
            {
                var response = ((ICompanyBankAccountDAL)_dal).SetIsActiveByDefault(id, IsActiveByDefaultExpenseAccount);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse
                {
                    Status = "ERROR",
                    Message = ex.Message
                };
            }
        }

    }
}
