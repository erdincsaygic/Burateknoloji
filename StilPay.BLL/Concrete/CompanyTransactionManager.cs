using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.Parasut.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StilPay.BLL.Concrete
{
    public class CompanyTransactionManager : BaseBLL<CompanyTransaction>, ICompanyTransactionManager
    {
        public CompanyTransactionManager(ICompanyTransactionDAL dal) : base(dal)
        {
        }

        public List<CompanyTransaction> GetProcess(string idCompany, DateTime startDate, DateTime endDate, int pageLenght, int offsetValue, string searchValue, bool isMonthlyAccountSummary)
        {
            return ((ICompanyTransactionDAL)_dal).GetProcess(idCompany, startDate, endDate, pageLenght, offsetValue, searchValue, isMonthlyAccountSummary);
        }

        public List<CompanyTransaction> GetProcess(string idCompany, DateTime startDate, DateTime endDate, string iDActionType, string iDCompanyInvoice, bool invoiceStatus)
        {
            return ((ICompanyTransactionDAL)_dal).GetProcess(idCompany, startDate, endDate, iDActionType, iDCompanyInvoice, invoiceStatus);
        }

        public List<CompanyTransaction> GetListOld(string idCompany, DateTime? startDate, DateTime? endDate)
        {
            return ((ICompanyTransactionDAL)_dal).GetListOld(idCompany, startDate, endDate);
        }

        public List<MonthlyAccountSummary> GetMonthlyAccountSummary(DateTime dailyReportStartDateTime, DateTime dailyReportEndDateTime, DateTime reportStartDateTime, DateTime reportEndDateTime, DateTime weeklyReportStartDateTime, DateTime weeklyReportEndDateTime)
        {
            return ((ICompanyTransactionDAL)_dal).GetMonthlyAccountSummary(dailyReportStartDateTime, dailyReportEndDateTime, reportStartDateTime, reportEndDateTime, weeklyReportStartDateTime, weeklyReportEndDateTime);
        }

        public List<DealerAccountSummary> GetDealerAccountSummary(string idCompany, DateTime reportStartDateTime, DateTime reportEndDateTime)
        {
            return ((ICompanyTransactionDAL)_dal).GetDealerAccountSummary(idCompany, reportStartDateTime, reportEndDateTime);
        }

        public List<DealerAccountSummaryForDealer> GetDealerAccountSummaryForDealer(string idCompany, DateTime reportStartDateTime, DateTime reportEndDateTime)
        {
            return ((ICompanyTransactionDAL)_dal).GetDealerAccountSummaryForDealer(idCompany, reportStartDateTime, reportEndDateTime);
        }

        public bool ConvertToInvoice(List<string> idList)
        {
            var _companyInvoiceDAL = new DAL.Concrete.CompanyInvoiceDAL();
            var _companyDAL = new DAL.Concrete.CompanyDAL();
            var _actionTypeDAL = new DAL.Concrete.ActionTypeDAL();
            var _settingDAL = new DAL.Concrete.SettingDAL();

            try
            {
                if (idList == null || idList.Count == 0)
                {
                    return false;
                }

                List<CompanyTransaction> companyTransactions = new List<CompanyTransaction>();

                foreach (var item in idList)
                {
                    var companyTransaction = ((ICompanyTransactionDAL)_dal).GetSingle(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, item) });
                    if (companyTransaction != null)
                    {
                        companyTransactions.Add(companyTransaction);
                    }
                }
                
                if (companyTransactions.Count == 0)
                {
                    return false;
                }

                var company = _companyDAL.GetSingle(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, companyTransactions.FirstOrDefault().IDCompany) });

                var actionType = _actionTypeDAL.GetSingle(new List<FieldParameter> { new FieldParameter("IDActionType", Enums.FieldType.NVarChar, companyTransactions.FirstOrDefault().IDActionType) });

                var setting = _settingDAL.GetList(new List<FieldParameter> { new FieldParameter("ParamType", Enums.FieldType.NVarChar, "PARASUT") });

                var invoice = new CompanyInvoice()
                {
                    IDCompany = company.ID,
                    Company = company.Title,
                    InvoiceNumber = 0,
                    TaxAmount = Math.Round(companyTransactions.Sum(x => x.CommissionTaxAmount),2),
                    NetAmount = Math.Round(companyTransactions.Sum(x => x.CommissionNetAmount),2),
                    TotalAmount = Math.Round(companyTransactions.Sum(x => x.Commission),2),
                    SendStatus = false,
                    CDate = DateTime.Now,
                    CUser = "A1042EA4-069B-4EF5-8B86-D02B5E57EA02",
                    ParasutPrintUrl = ""
                };

                var model = new InvoiceModel
                {
                    Amount = decimal.ToDouble(invoice.NetAmount),
                    ContactID = company.IDParasut,
                    Tax = 18,
                    InvoiceDate = DateTime.Now,
                    Description = "",
                    ProductID = actionType.IDParasut,
                };

                ResponseModel<InvoiceResponseModel> response = ParasutAktar(model, company, setting);

                if (!response.Status)
                {
                    return false;
                }



                invoice.InvoiceNumber = int.Parse(response.Data.data.id);
                invoice.ParasutPrintUrl = response.Data.data.attributes.print_url;

                var invoiceID = _companyInvoiceDAL.Insert(invoice);

                foreach (var item in companyTransactions)
                {
                    item.IDCompanyInvoice = invoiceID.ToString();
                    ((ICompanyTransactionDAL)_dal).Update(item);
                }

                company.IDParasut = response.Data.data.contact_id;
                _companyDAL.Update(company);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private ResponseModel<InvoiceResponseModel> ParasutAktar(InvoiceModel invoice, Company company, List<Setting> settings)
        {
            var result = new ResponseModel<InvoiceResponseModel>();

            AuthModel auth = new AuthModel();

            auth.grant_type = settings.FirstOrDefault(x => x.ParamType == "PARASUT" && x.ParamDef == "grant_type").ParamVal;
            auth.client_id = settings.FirstOrDefault(x => x.ParamType == "PARASUT" && x.ParamDef == "client_id").ParamVal;
            auth.client_secret = settings.FirstOrDefault(x => x.ParamType == "PARASUT" && x.ParamDef == "client_secret").ParamVal;
            auth.redirect_uri = settings.FirstOrDefault(x => x.ParamType == "PARASUT" && x.ParamDef == "redirect_uri").ParamVal;
            auth.username = settings.FirstOrDefault(x => x.ParamType == "PARASUT" && x.ParamDef == "username").ParamVal;
            auth.password = settings.FirstOrDefault(x => x.ParamType == "PARASUT" && x.ParamDef == "password").ParamVal;

            string baseUrl = settings.FirstOrDefault(x => x.ParamType == "PARASUT" && x.ParamDef == "base_url").ParamVal;
            string companyID = settings.FirstOrDefault(x => x.ParamType == "PARASUT" && x.ParamDef == "company_id").ParamVal;

            var authentication = Utility.Parasut.Authentication.GetAccessToken(auth, baseUrl);
            if (!authentication.Status)
            {
                result.Status = false;
                result.Message = authentication.Message;
                return result;
            }

            if (string.IsNullOrEmpty(invoice.ContactID))
            {
                CustomerModel customer = new CustomerModel
                {
                    Name = company.Name,
                    Email = company.Email,
                    Address = company.Address,
                    Iban = "",
                    Phone = company.Phone,
                    TaxNumber = company.TaxNr,
                    TaxOffice = company.TaxOffice
                };
                var createCustomer = Utility.Parasut.Customer.Create(customer, companyID, baseUrl, authentication.Data.access_token);
                if (!createCustomer.Status)
                {
                    result.Status = false;
                    result.Message = createCustomer.Message;
                    return result;
                }
                else
                {
                    invoice.ContactID = createCustomer.Data.data.id;
                }
            }

            var createInvoice = Utility.Parasut.Invoice.Create(invoice, companyID, baseUrl, authentication.Data.access_token);
            createInvoice.Data.data.contact_id = invoice.ContactID;
            return createInvoice;
        }

        public AccountReportModel GetAccountReport(string idCompany, DateTime startDate, DateTime endDate, DateTime startDateTime, DateTime endDateTime)
        {
            return ((ICompanyTransactionDAL)_dal).GetAccountReport(idCompany, startDate, endDate, startDateTime, endDateTime);
        }

        public List<AccountReportCreditCardModel> GetAccountReportCreditCardDetail(string idCompany, DateTime startDate, DateTime endDate)
        {
            return ((ICompanyTransactionDAL)_dal).GetAccountReportCreditCardDetail(idCompany, startDate, endDate);
        }

        public List<AccountReportCreditCardModel> BankTransactionsDetailIncomeList(string idBank, DateTime startDate, DateTime endDate)
        {
            return ((ICompanyTransactionDAL)_dal).BankTransactionsDetailIncomeList(idBank, startDate, endDate);
        }

        public List<AccountReportCreditCardModel> BankTransactionsDetailExpenseList(string idBank, DateTime startDate, DateTime endDate)
        {
            return ((ICompanyTransactionDAL)_dal).BankTransactionsDetailExpenseList(idBank, startDate, endDate);
        }

        public List<AccountReportCreditCardModel> BankTransactionsDetailTransferList(string idBank, DateTime startDate, DateTime endDate)
        {
            return ((ICompanyTransactionDAL)_dal).BankTransactionsDetailTransferList(idBank, startDate, endDate);
        }

        public List<DealerTransactionQuery> GetRecordsByQueryParameter(string queryParameter)
        {
            return ((ICompanyTransactionDAL)_dal).GetRecordsByQueryParameter(queryParameter);
        }

        public List<BankTransactionList> GetBankTransactionList(List<FieldParameter> parameters)
        {
            return ((ICompanyTransactionDAL)_dal).GetBankTransactionList(parameters);
        }
        public List<CreditCardTransactionList> GetCreditCardTransactionList(List<FieldParameter> parameters)
        {
            return ((ICompanyTransactionDAL)_dal).GetCreditCardTransactionList(parameters);
        }

        public decimal GetDealerTransactionBalance(List<FieldParameter> parameters)
        {
            return ((ICompanyTransactionDAL)_dal).GetDealerTransactionBalance(parameters);
        }

        public GenericResponse SetStatusToFraud(SetStatusToFraudDto setStatusToFraudDto)
        {
            try
            {
                var id = ((ICompanyTransactionDAL)_dal).SetStatusToFraud(setStatusToFraudDto);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = id
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


        public List<DealerFraudPoolDto> GetDealerFraudPool(List<FieldParameter> parameters)
        {
            return ((ICompanyTransactionDAL)_dal).GetDealerFraudPool(parameters);
        }

        public List<CreditCardDetailedSearchDto> CreditCardDetailedSearch(List<FieldParameter> parameters)
        {
            return ((ICompanyTransactionDAL)_dal).CreditCardDetailedSearch(parameters);
        }

        public List<CustomerInfoDetailedSearchDto> CustomerInfoDetailedSearch(List<FieldParameter> parameters)
        {
            return ((ICompanyTransactionDAL)_dal).CustomerInfoDetailedSearch(parameters);
        }

        public List<TransferDetailedSearchDto> TransferDetailedSearch(List<FieldParameter> parameters)
        {
            return ((ICompanyTransactionDAL)_dal).TransferDetailedSearch(parameters);
        }
    }
}
