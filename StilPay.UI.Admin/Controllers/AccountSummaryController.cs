using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StilPay.BLL.Abstract;
using StilPay.BLL;
using StilPay.Entities.Concrete;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using System.Collections.Generic;
using System;
using System.Linq;
using StilPay.DAL.Concrete;
using static StilPay.UI.Admin.Models.AccountingSummaryMonthlyEditViewModel;
using Microsoft.AspNetCore.WebUtilities;
using DocumentFormat.OpenXml.Spreadsheet;

namespace StilPay.UI.Admin.Controllers
{

    [Authorize(Roles = "Accounting")]
    public class AccountSummaryController : BaseController<AccountSummary>
    {
        private readonly IAccountSummaryManager _manager;
        private readonly IBankManager _bankManager;
        private readonly ICompanyBankAccountManager _companyBankAccountManager;
        private readonly ICompanyFinanceTransactionManager _companyFinanceTransferManager;
        private readonly IPaymentInstitutionManager _paymentInstitutionManager;
        private readonly ICompanyTransactionManager _companyTransactionManager;
        public readonly ICompanyManager _companyManager;
        public readonly ICreditCardAccountSummaryReportDetailManager _creditCardAccountSummaryReportDetailManager;
        public readonly IBankTransferAccountSummaryReportDetailManager _bankTransferAccountSummaryReportDetailManager;
        public readonly ICompanyRebateRequestManager _companyRebateRequestManager;
        public readonly ICompanyWithdrawalRequestManager _companyWithdrawalRequestManager;
        public readonly IPaymentTransferPoolManager _paymentTransferPoolManager;
        public readonly IPaymentNotificationManager _paymentNotificationManager;
        public readonly ICreditCardPaymentNotificationManager _creditCardPaymentNotificationManager;
        private readonly IForeignCreditCardPaymentNotificationManager _foreignCreditCardPaymentNotificationManager;
        private readonly ICompanyCurrencyManager _companyCurrencyManager;
        private readonly ICompanyInvoiceManager _companyInvoiceManager;

        private readonly SettingDAL _settingDAL = new SettingDAL();

        public AccountSummaryController(IAccountSummaryManager manager, IBankManager bankManager, ICompanyBankAccountManager companyBankAccountManager, ICompanyFinanceTransactionManager companyFinanceTransferManager, IHttpContextAccessor httpContext, IPaymentInstitutionManager paymentInstitutionManager, ICompanyTransactionManager companyTransactionManager, ICompanyManager companyManager, ICreditCardAccountSummaryReportDetailManager creditCardAccountSummaryReportDetailManager, IBankTransferAccountSummaryReportDetailManager bankTransferAccountSummaryReportDetailManager, ICompanyRebateRequestManager companyRebateRequestManager, ICompanyWithdrawalRequestManager companyWithdrawalRequestManager, IPaymentTransferPoolManager paymentTransferPoolManager, IPaymentNotificationManager paymentNotificationManager, ICreditCardPaymentNotificationManager creditCardPaymentNotificationManager, IForeignCreditCardPaymentNotificationManager foreignCreditCardPaymentNotificationManager, ICompanyCurrencyManager companyCurrencyManager, ICompanyInvoiceManager companyInvoiceManager) : base(httpContext)
        {
            _manager = manager;
            _bankManager = bankManager;
            _companyBankAccountManager = companyBankAccountManager;
            _companyFinanceTransferManager = companyFinanceTransferManager;
            _paymentInstitutionManager = paymentInstitutionManager;
            _companyTransactionManager = companyTransactionManager;
            _companyManager = companyManager;
            _creditCardAccountSummaryReportDetailManager = creditCardAccountSummaryReportDetailManager;
            _bankTransferAccountSummaryReportDetailManager = bankTransferAccountSummaryReportDetailManager;
            _companyRebateRequestManager = companyRebateRequestManager;
            _companyWithdrawalRequestManager = companyWithdrawalRequestManager;
            _paymentTransferPoolManager = paymentTransferPoolManager;
            _paymentNotificationManager = paymentNotificationManager;
            _creditCardPaymentNotificationManager = creditCardPaymentNotificationManager;
            _foreignCreditCardPaymentNotificationManager = foreignCreditCardPaymentNotificationManager;
            _companyCurrencyManager = companyCurrencyManager;
            _companyInvoiceManager = companyInvoiceManager;
        }

        public override IBaseBLL<AccountSummary> Manager()
        {
            return _manager;
        }

        [HttpPost]
        public IActionResult GetData()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = GetData(
                new FieldParameter("PageLenght", Enums.FieldType.Int, length),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue),
                new FieldParameter("IsMonthly", Enums.FieldType.Bit, null)
            );

            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;

            var result = new
            {
                recordsFiltered = recordsTotal,
                data = list
            };

            return Json(result);
        }

        [HttpGet]
        public IActionResult Detail(string id)
        {
            var entity = _manager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, id) });

            var model = new AccountingSummaryEditViewModel
            {
                PaymentInstitutions = _paymentInstitutionManager.GetList(null),
                CompanyBankAccounts = null,
                CreditCardAccountSummaryReportDetails = _creditCardAccountSummaryReportDetailManager.GetList(new List<FieldParameter>() { new FieldParameter("ReportNo", Enums.FieldType.Int, entity.ReportNo )}),
                BankTransferAccountSummaryReportDetails = _bankTransferAccountSummaryReportDetailManager.GetList(new List<FieldParameter>() { new FieldParameter("ReportNo", Enums.FieldType.Int, entity.ReportNo) }),
                AccountSummary = entity
            };

            return View("AccountSummaryDetail", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateNewReport()
        {
            var reportStartDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);

            var companiesTotalBalance = 0.0M;
            var companies = _companyManager.GetActiveList(null);

            var totalBankExpenseAmount = 0.0M;
            var totalBankIncomeAmount = 0.0M;
            //var rebateNetTotal = 0.0M;

            var previosReportEntity = _manager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("PageLenght", Enums.FieldType.Int, 1),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null),
                new FieldParameter("IsMonthly", Enums.FieldType.Bit, false)
            }).FirstOrDefault();

            if (previosReportEntity == null)
            {
                var data2 = new AccountSummary()
                {
                    CDate = reportStartDateTime,
                    CUser = IDUser,
                    WithdrawalRequestProfit = 0,
                    PaymentNotificationProfit = 0,
                    UnmatchedPaymentNotificationBalance = 0,
                    CreditCardPaymentNotificationProfit = 0,
                    ExpenseAmount = 0,
                    IncomeAmount = 0,
                    DealerTotalBalance = 0,
                    Profit = 0,
                    NetAmount = 0,
                    Difference = 0,
                    IsMonthly = false,
                    RebateExpenseProfitAmount = 0
                };

                return Json(_manager.Insert(data2));
            }

            foreach (var company in companies)
            {
                var companyBalance = _companyManager.GetBalance(company.ID);

                var companyWithdrawal = _companyWithdrawalRequestManager.GetList(new List<FieldParameter>
                    {
                        new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.Pending),
                        new FieldParameter("IDCompany", Enums.FieldType.NVarChar, company.ID),
                        new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                        new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                        new FieldParameter("PageLenght", Enums.FieldType.Int, 9999),
                        new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                        new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null),
                        new FieldParameter("WithProcess", Enums.FieldType.Bit, true)
                    });

                var companyRebate = _companyRebateRequestManager.GetList(new List<FieldParameter>
                    {
                        new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.Pending),
                        new FieldParameter("IDCompany", Enums.FieldType.NVarChar, company.ID),
                        new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                        new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                        new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                        new FieldParameter("PageLenght", Enums.FieldType.Int, 9999),
                        new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                        new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null),
                        new FieldParameter("WithProcess", Enums.FieldType.Bit, true)
                    });

                //if (companyRebate.Count > 0)
                //{
                //    foreach (var item in companyRebate)
                //    {
                //        var companyTransaction = _companyTransactionManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, item.PaymentRecordID) });
                //        rebateNetTotal += item.IsPartialRebate ? item.Amount - ((item.Amount * companyTransaction.CommissionRate) / 100 ) : companyTransaction.NetTotal;
                //    }
                //}

                var getCompaniesCurrencyBalance = _companyCurrencyManager.GetList(new List<FieldParameter> { new FieldParameter("IDCompany", Enums.FieldType.NVarChar, company.ID) });

                companiesTotalBalance += Math.Round(companyBalance.TotalBalance, 2) + companyWithdrawal.Sum(s => s.Amount + s.CostTotal) + companyRebate.Sum(s => s.Amount);

                companiesTotalBalance += getCompaniesCurrencyBalance.Sum(s => s.Balance + s.BlockedBalance);
            }

            var model = new AccountingSummaryEditViewModel
            {
                PaymentInstitutions = _paymentInstitutionManager.GetList(null),
                CompanyBankAccounts = _companyBankAccountManager.CompanyBankAccountSum("1312E00F-E83E-45B4-85C6-892396D12331", null, null, null),          
                CreditCardAccountSummaryReportDetails = new List<CreditCardAccountSummaryReportDetail>(),
                BankTransferAccountSummaryReportDetails = new List<BankTransferAccountSummaryReportDetail>()
            };

            foreach (var item in model.CompanyBankAccounts)
            {
                //var companyBankAccount = _companyBankAccountManager.CompanyBankAccountSum("1312E00F-E83E-45B4-85C6-892396D12331", item.ID,null,null);

                totalBankExpenseAmount += _companyTransactionManager.BankTransactionsDetailExpenseList(item.ID, 
                   new DateTime(previosReportEntity.CDate.Year, previosReportEntity.CDate.Month, previosReportEntity.CDate.Day, previosReportEntity.CDate.Hour, previosReportEntity.CDate.Minute, 0),
                   new DateTime(reportStartDateTime.Year, reportStartDateTime.Month, reportStartDateTime.Day, reportStartDateTime.Hour, reportStartDateTime.Minute, 0)).Sum(s => s.Amount);

                totalBankIncomeAmount += _companyTransactionManager.BankTransactionsDetailIncomeList(item.ID,
                    new DateTime(previosReportEntity.CDate.Year, previosReportEntity.CDate.Month, previosReportEntity.CDate.Day, previosReportEntity.CDate.Hour, previosReportEntity.CDate.Minute, 0),
                    new DateTime(reportStartDateTime.Year, reportStartDateTime.Month, reportStartDateTime.Day, reportStartDateTime.Hour, reportStartDateTime.Minute, 0)).Sum(s => s.Amount);

                model.BankTransferAccountSummaryReportDetails.Add(new BankTransferAccountSummaryReportDetail()
                {
                    Balance = item.Amount,
                    BankName = item.Name,
                    IDBank = item.IDBank,
                    IsExitAccount = item.IsExitAccount
                });
            }

            List<BankAccountSumModel> ccSumModel = _companyBankAccountManager.CreditCardAccountSum("1312E00F-E83E-45B4-85C6-892396D12331", null, null, null);

            foreach (var item in model.PaymentInstitutions)
            {
                model.CreditCardAccountSummaryReportDetails.Add(new CreditCardAccountSummaryReportDetail()
                {
                    Balance = ccSumModel.FirstOrDefault(f => f.ID == item.ID).Amount,
                    PaymentInstitutionID = item.ID,
                    PaymentInstitutionName = item.Name
                });
            }

            var ccPool = _creditCardPaymentNotificationManager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.PayPool),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null),
                new FieldParameter("IsAutoNotification", Enums.FieldType.Tinyint, null),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("PageLenght", Enums.FieldType.Int, 9999),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar,null),
            });

            var creditCardPoolAmount = (decimal)(ccPool.Count > 0 ? ccPool.Sum(x => x.PaymentInstitutionNetAmount) : 0);

            var foreignCCPool = _foreignCreditCardPaymentNotificationManager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.PayPool),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null),
                new FieldParameter("IsAutoNotification", Enums.FieldType.Tinyint, null),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("PageLenght", Enums.FieldType.Int, 9999),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar,null),
            });

            var foreignCreditCardPoolAmount = (decimal)(foreignCCPool.Count > 0 ? foreignCCPool.Sum(x => x.PaymentInstitutionNetAmount) : 0);

            var dealerFraudPool = _companyTransactionManager.GetDealerFraudPool(new List<FieldParameter>()
            {
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime,null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("PageLength", Enums.FieldType.Int, 9999),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null)
            });

            creditCardPoolAmount += foreignCreditCardPoolAmount;

            var dealerFraudPoolAmount = dealerFraudPool.Count > 0 ? dealerFraudPool.Sum(x => x.PaymentInstitutionNetAmount) : 0;

            //var dealerFraudExpenseProfitAmount = dealerFraudPool.Where(x => (x.PaymentMethodType == (int)PaymentMethod.CreditCard || x.PaymentMethodType == (int)PaymentMethod.ForeignCreditCard) && x.MDate >= previosReportEntity.CDate).ToList().Count > 0 ? dealerFraudPool.Where(x => (x.PaymentMethodType == (int)PaymentMethod.CreditCard || x.PaymentMethodType == (int)PaymentMethod.ForeignCreditCard) && x.MDate >= previosReportEntity.CDate).Sum(x => x.PaymentInstitutionNetAmount - x.DealerCommission) : 0;

            var accountReport = _companyTransactionManager.GetAccountReport(null, previosReportEntity.CDate, reportStartDateTime.Date, previosReportEntity.CDate, reportStartDateTime);

            var positiveAmount = model.CreditCardAccountSummaryReportDetails.Sum(s => s.Balance) + model.BankTransferAccountSummaryReportDetails.Sum(s => s.Balance);

            var negativeAmount = companiesTotalBalance + accountReport.TransferNotMatchedAmount + creditCardPoolAmount + dealerFraudPoolAmount;

            var profit = accountReport.CreditCardCommissionProfitAmount + accountReport.ForeignCreditCardCommissionProfitAmount + accountReport.TransferCommissionProfitAmount + accountReport.WithdrawalTotalTransactionFeeAmount + totalBankIncomeAmount;

            var netAmount = positiveAmount - negativeAmount;

            var data = new AccountSummary()
            {
                CDate = reportStartDateTime,
                CUser = IDUser,
                WithdrawalRequestProfit =  accountReport.WithdrawalTotalTransactionFeeAmount,
                PaymentNotificationProfit = accountReport.TransferCommissionProfitAmount,
                UnmatchedPaymentNotificationBalance = accountReport.TransferNotMatchedAmount,
                CreditCardPaymentNotificationProfit = accountReport.CreditCardCommissionProfitAmount,
                ExpenseAmount = totalBankExpenseAmount,
                IncomeAmount = totalBankIncomeAmount,
                DealerTotalBalance = companiesTotalBalance,
                Profit = profit,
                NetAmount = netAmount,
                Difference = (netAmount + totalBankExpenseAmount + accountReport.RebateExpenseProfitAmount + accountReport.FraudExpenseProfitAmount) - ( previosReportEntity.NetAmount + profit ),
                IsMonthly = false,
                RebateExpenseProfitAmount = accountReport.RebateExpenseProfitAmount,
                CreditCardPoolBalance = creditCardPoolAmount,
                FraudPoolBalance = dealerFraudPoolAmount,
                FraudExpenseProfitAmount = accountReport.FraudExpenseProfitAmount,
                ForeignCreditCardPaymentNotificationProfit = accountReport.ForeignCreditCardCommissionProfitAmount,
            };

            var response = _manager.Insert(data);
            if(response != null && response.Status == "OK")
            {
                var insertedEntity = _manager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, response.Data) });

                foreach (var item in model.CreditCardAccountSummaryReportDetails)
                {
                    item.AccountSummaryReportNo = insertedEntity.ReportNo;

                    _creditCardAccountSummaryReportDetailManager.Insert(item);
                }

                foreach (var item in model.BankTransferAccountSummaryReportDetails)
                {
                    item.AccountSummaryReportNo = insertedEntity.ReportNo;

                    _bankTransferAccountSummaryReportDetailManager.Insert(item);
                }

                model.AccountSummary = insertedEntity;

                return View("AccountSummaryDetail", model);
            }


            return Json(response);

        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var entity = _manager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, id) });
            
            return Json(_manager.Delete(entity));
        }

        [HttpGet]
        public IActionResult MonthlyAccountSummaryIndex()
        {
            //var model = new AccountingSummaryEditViewModel();
            return View("MonthlyAccountSummary");
        }

        [HttpGet]
        public IActionResult GetDataMonthlyAccountSummary([FromQuery] int year, [FromQuery] int month)
        {
            if(year == 0 || month == 0)
            {
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
            }

            var previosMonthlyReportMonth = month - 1;
            var previosMonthlyReportYear = year;
            var previosMonthlyReportRequest = false;

            if (previosMonthlyReportMonth == 0)
            {
                previosMonthlyReportMonth = 12;  // Aralık ayını temsil etmesi için
                previosMonthlyReportYear = year - 1;
            }

            if (month != DateTime.Now.Month || year != DateTime.Now.Year)
                previosMonthlyReportRequest = true;

            var dailyReportStartDateTime = DateTime.Now.Date;
            var dailyReportEndDateTime = dailyReportStartDateTime.Date.AddDays(1).AddTicks(-1);
            var reportStartDateTime = new DateTime(year, month, 1, 0, 0, 0);
            var reportEndDateTime = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);

            var weeklyReportStartDateTime = dailyReportStartDateTime.Date.AddDays(-(int)dailyReportStartDateTime.DayOfWeek + (int)DayOfWeek.Monday);
            var weeklyReportEndDateTime = weeklyReportStartDateTime.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);

            var companiesTotalBalance = 0.0M;
            var companies = _companyManager.GetActiveList(null);

            var totalBankExpenseAmount = 0.0M;
            var totalBankIncomeAmount = 0.0M;
            //var rebateNetTotal = 0.0M;

            //var previosMonthlyReportList = _manager.GetList(new List<FieldParameter>()
            //{
            //    new FieldParameter("PageLenght", Enums.FieldType.Int, int.MaxValue),
            //    new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
            //    new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null),
            //    new FieldParameter("IsMonthly", Enums.FieldType.Bit, true)
            //}).ToList();

            //var previosMonthlyReportEntity = previosMonthlyReportList.Any(x => x.CDate.Date == reportStartDateTime.Date) ? previosMonthlyReportList.FirstOrDefault(x => x.CDate.Month == month) : null;

            var previosMonthlyReportEntity = _manager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("PageLenght", Enums.FieldType.Int, 10),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null),
                new FieldParameter("IsMonthly", Enums.FieldType.Bit, true),
                new FieldParameter("Month", Enums.FieldType.Int, previosMonthlyReportMonth),
                new FieldParameter("Year", Enums.FieldType.Int, previosMonthlyReportYear),

            }).OrderBy(x => x.ReportNo).FirstOrDefault();

            var model = new AccountingSummaryMonthlyEditViewModel
            {
                PaymentInstitutions = _paymentInstitutionManager.GetList(null),
                CompanyBankAccounts = _companyBankAccountManager.CompanyBankAccountSum("1312E00F-E83E-45B4-85C6-892396D12331", null, null, null),
                MonthlyCreditCardAccountSummaryReportDetails = new List<MonthlyCreditCardAccountSummaryReportDetail>(),
                MonthlyBankTransferAccountSummaryReportDetails = new List<MonthlyBankTransferAccountSummaryReportDetail>(),
                MonthlyForeignCreditCardAccountSummaryReportDetails = new List<MonthlyForeignCreditCardAccountSummaryReportDetail>(),
                DealerTransactionDetails = new List<DealerTransactionDetail>(),
                SelectedMonth = month,
                SelectedYear = year,
            };

            //var monthlyCompanyTransactionsAll = _companyTransactionManager.GetProcess(null, reportStartDateTime, reportEndDateTime, int.MaxValue, 0, null, true);

            var monthlyAccountSummary = _companyTransactionManager.GetMonthlyAccountSummary(dailyReportStartDateTime, dailyReportEndDateTime, reportStartDateTime, reportEndDateTime, weeklyReportStartDateTime, weeklyReportEndDateTime);

            var monthlyCompanyTransactionsWithdrawal = monthlyAccountSummary.Where(x => x.WithdrawalRequestSIDBank != null).ToList();
            var monthlyCompanyTransactionsTransfer = monthlyAccountSummary.Where(x => x.PaymentNotificationSIDBank != null).ToList();
            var monthlyCompanyTransactionsCreditCard = monthlyAccountSummary.Where(x => x.CreditCardPaymentMethodID != null).ToList();
            var monthlyCompanyTransactionsForeignCreditCard = monthlyAccountSummary.Where(x => x.ForeignCreditCardPaymentMethodID != null).ToList();
            var monthlyCompanyTransactionsDealer = monthlyAccountSummary.Where(x => x.IDCompany != null).ToList();

            var monthlyCompanyTransactionsPool = monthlyAccountSummary.Where(x => x.PaymentTranferPoolBankID != null).ToList();

            //var paymentTransferPool = _paymentTransferPoolManager.GetList(new List<FieldParameter>() {
            //    new FieldParameter("Status", Enums.FieldType.Tinyint, Enums.StatusType.Confirmed),
            //    new FieldParameter("StartDate", Enums.FieldType.DateTime, reportStartDateTime),
            //    new FieldParameter("EndDate", Enums.FieldType.DateTime, reportEndDateTime),
            //    new FieldParameter("PageLength", Enums.FieldType.Int, int.MaxValue),
            //    new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
            //    new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null)
            //});

            foreach (var company in companies)
            {
                var companyBalance = _companyManager.GetBalance(company.ID);

                var companyWithdrawal = _companyWithdrawalRequestManager.GetList(new List<FieldParameter>
                {
                    new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.Pending),
                    new FieldParameter("IDCompany", Enums.FieldType.NVarChar, company.ID),
                    new FieldParameter("StartDate", Enums.FieldType.DateTime, reportStartDateTime),
                    new FieldParameter("EndDate", Enums.FieldType.DateTime, reportEndDateTime),
                    new FieldParameter("PageLenght", Enums.FieldType.Int, 9999),
                    new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                    new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null),
                    new FieldParameter("WithProcess", Enums.FieldType.Bit, true)
                });

                var companyRebate = _companyRebateRequestManager.GetList(new List<FieldParameter>
                {
                    new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.Pending),
                    new FieldParameter("IDCompany", Enums.FieldType.NVarChar, company.ID),
                    new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                    new FieldParameter("StartDate", Enums.FieldType.DateTime, reportStartDateTime),
                    new FieldParameter("EndDate", Enums.FieldType.DateTime, reportEndDateTime),
                    new FieldParameter("PageLenght", Enums.FieldType.Int, 9999),
                    new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                    new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null),
                    new FieldParameter("WithProcess", Enums.FieldType.Bit, true)
                });

                model.DealerTransactionDetails.Add(new DealerTransactionDetail()
                {
                    IDCompany = company.ID,
                    CompanyName = company.Title,
                    TotalTransactionAmount = monthlyCompanyTransactionsDealer.Any(w => w.IDCompany == company.ID) ? monthlyCompanyTransactionsDealer.FirstOrDefault(w => w.IDCompany == company.ID).CompanyTransactionTotalAmount : 0m,
                    TransactionProfitAmount = monthlyCompanyTransactionsDealer.Any(w => w.IDCompany == company.ID) ? monthlyCompanyTransactionsDealer.FirstOrDefault(w => w.IDCompany == company.ID).CompanyTransactionProfit : 0m,
                    TotalWithdrawalTransactionAmount = monthlyCompanyTransactionsDealer.Any(w => w.IDCompany == company.ID) ? monthlyCompanyTransactionsDealer.FirstOrDefault(w => w.IDCompany == company.ID).CompanyWithdrawalTransactionTotalAmount : 0m,
                    WithdrawalTransactionCount = monthlyCompanyTransactionsDealer.Any(w => w.IDCompany == company.ID) ? monthlyCompanyTransactionsDealer.FirstOrDefault(w => w.IDCompany == company.ID).CompanyWithdrawalTransactionCount : 0m,

                });

                //if (companyRebate.Count > 0)
                //{
                //    foreach (var item in companyRebate)
                //    {
                //        var companyTransaction = _companyTransactionManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, item.PaymentRecordID) });
                //        rebateNetTotal += item.IsPartialRebate ? item.Amount - ((item.Amount * companyTransaction.CommissionRate) / 100) : companyTransaction.NetTotal;
                //    }
                //}
   

                var getCompaniesCurrencyBalance = _companyCurrencyManager.GetList(new List<FieldParameter> { new FieldParameter("IDCompany", Enums.FieldType.NVarChar, company.ID) });

                companiesTotalBalance += Math.Round(companyBalance.TotalBalance, 2) + companyWithdrawal.Sum(s => s.Amount + s.CostTotal) + companyRebate.Sum(s => s.Amount);

                companiesTotalBalance += getCompaniesCurrencyBalance.Sum(s => s.Balance + s.BlockedBalance);
            }

            foreach (var item in model.CompanyBankAccounts)
            {             
                totalBankExpenseAmount += _companyTransactionManager.BankTransactionsDetailExpenseList(item.ID, reportStartDateTime, reportEndDateTime).Sum(s => s.Amount);
                totalBankIncomeAmount += _companyTransactionManager.BankTransactionsDetailIncomeList(item.ID, reportStartDateTime, reportEndDateTime).Sum(s => s.Amount);

                model.MonthlyBankTransferAccountSummaryReportDetails.Add(new MonthlyBankTransferAccountSummaryReportDetail()
                {
                    Balance = monthlyCompanyTransactionsPool.Any(x => x.PaymentTranferPoolBankID == item.IDBank) ? monthlyCompanyTransactionsPool.FirstOrDefault(x => x.PaymentTranferPoolBankID == item.IDBank).PaymentTranferPoolSumTotal : 0m,
                    BalanceForNetCalculate = item.Amount,
                    BankName = item.Name,
                    IDBank = item.IDBank,
                    IsExitAccount = item.IsExitAccount,
                    WithdrawalBankBalance = monthlyCompanyTransactionsWithdrawal.Any(w => w.WithdrawalRequestSIDBank == item.IDBank) ? monthlyAccountSummary.FirstOrDefault(w => w.WithdrawalRequestSIDBank == item.IDBank).WithdrawalRequestTotalAmount : 0m,
                    WithdrawalTotalProfit = monthlyCompanyTransactionsWithdrawal.Any(w => w.WithdrawalRequestSIDBank == item.IDBank)  ? monthlyAccountSummary.FirstOrDefault(w => w.WithdrawalRequestSIDBank == item.IDBank).WithdrawalRequestProfit : 0m,
                    TotalProfit = monthlyAccountSummary.Any(f => f.PaymentNotificationSIDBank == item.IDBank) ? monthlyAccountSummary.FirstOrDefault(f => f.PaymentNotificationSIDBank == item.IDBank).PaymentNotificationTotalAmount : 0m
                });
            }
           
            List<BankAccountSumModel> ccSumModel = _companyBankAccountManager.CreditCardAccountSum("1312E00F-E83E-45B4-85C6-892396D12331", null, null, null);

            foreach (var item in model.PaymentInstitutions)
            {
                if (item.UseForForeignCard)
                {
                    model.MonthlyForeignCreditCardAccountSummaryReportDetails.Add(new MonthlyForeignCreditCardAccountSummaryReportDetail()
                    {
                        Balance = monthlyCompanyTransactionsForeignCreditCard.Any(f => f.ForeignCreditCardPaymentMethodID == item.ID) ? monthlyCompanyTransactionsForeignCreditCard.FirstOrDefault(f => f.ForeignCreditCardPaymentMethodID == item.ID).ForeignCreditCardSumTotal : 0m,
                        PaymentInstitutionID = item.ID,
                        PaymentInstitutionName = item.Name,
                        TotalProfit = monthlyCompanyTransactionsForeignCreditCard.Any(w => w.ForeignCreditCardPaymentMethodID == item.ID) ? monthlyCompanyTransactionsForeignCreditCard.FirstOrDefault(w => w.ForeignCreditCardPaymentMethodID == item.ID).ForeignCreditCardProfit : 0m,
                        BalanceForNetCalculate = ccSumModel.FirstOrDefault(f => f.ID == item.ID).Amount
                        //PaymentInstitutionProfit = monthlyCompanyTransactionsAll.Where(w => w.CreditCardPaymentMethodID.ToString() == item.ID).Select(s => s.Total).FirstOrDefault()
                    });
                }
                else
                {
                    model.MonthlyCreditCardAccountSummaryReportDetails.Add(new MonthlyCreditCardAccountSummaryReportDetail()
                    {
                        Balance = monthlyCompanyTransactionsCreditCard.Any(f => f.CreditCardPaymentMethodID == item.ID) ? monthlyCompanyTransactionsCreditCard.FirstOrDefault(f => f.CreditCardPaymentMethodID == item.ID).CreditCardSumTotal : 0m,
                        PaymentInstitutionID = item.ID,
                        PaymentInstitutionName = item.Name,
                        TotalProfit = monthlyCompanyTransactionsCreditCard.Any(w => w.CreditCardPaymentMethodID == item.ID) ? monthlyCompanyTransactionsCreditCard.FirstOrDefault(w => w.CreditCardPaymentMethodID == item.ID).CreditCardProfit : 0m,
                        BalanceForNetCalculate = ccSumModel.FirstOrDefault(f => f.ID == item.ID).Amount
                        //PaymentInstitutionProfit = monthlyCompanyTransactionsAll.Where(w => w.CreditCardPaymentMethodID.ToString() == item.ID).Select(s => s.Total).FirstOrDefault()
                    });
                }
            }

            var ccPool = _creditCardPaymentNotificationManager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.PayPool),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null),
                new FieldParameter("IsAutoNotification", Enums.FieldType.Tinyint, null),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("PageLenght", Enums.FieldType.Int, 9999),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar,null)
            });

            var creditCardPoolAmount = (decimal)(ccPool.Count > 0 ? ccPool.Sum(x => x.PaymentInstitutionNetAmount) : 0);

            var foreignCCPool = _foreignCreditCardPaymentNotificationManager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.PayPool),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null),
                new FieldParameter("IsAutoNotification", Enums.FieldType.Tinyint, null),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("PageLenght", Enums.FieldType.Int, 9999),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar,null),
            });

            var foreignCreditCardPoolAmount = (decimal)(foreignCCPool.Count > 0 ? foreignCCPool.Sum(x => x.PaymentInstitutionNetAmount) : 0);

            creditCardPoolAmount += foreignCreditCardPoolAmount;

            var dealerFraudPool = _companyTransactionManager.GetDealerFraudPool(new List<FieldParameter>()
            {
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime,null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("PageLength", Enums.FieldType.Int, 9999),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null)
            });

            var dealerFraudPoolAmount = dealerFraudPool.Count > 0 ? dealerFraudPool.Sum(x => x.PaymentInstitutionNetAmount) : 0;

            //var dealerFraudExpenseProfitAmount = dealerFraudPool.Where(x => (x.PaymentMethodType == (int)PaymentMethod.CreditCard || x.PaymentMethodType == (int)PaymentMethod.ForeignCreditCard)).ToList().Count > 0 ? dealerFraudPool.Where(x => (x.PaymentMethodType == (int)PaymentMethod.CreditCard || x.PaymentMethodType == (int)PaymentMethod.ForeignCreditCard)).Sum(x => x.PaymentInstitutionNetAmount - x.DealerCommission) : 0;

            var accountReport = _companyTransactionManager.GetAccountReport(null, reportStartDateTime.Date, reportEndDateTime.Date, reportStartDateTime, reportEndDateTime);

            var positiveAmount = model.MonthlyCreditCardAccountSummaryReportDetails.Sum(s => s.BalanceForNetCalculate) + model.MonthlyBankTransferAccountSummaryReportDetails.Sum(s => s.BalanceForNetCalculate) + model.MonthlyForeignCreditCardAccountSummaryReportDetails.Sum(s => s.BalanceForNetCalculate);

            var negativeAmount = companiesTotalBalance + accountReport.TransferNotMatchedAmount + creditCardPoolAmount + dealerFraudPoolAmount;

            var profit = accountReport.ForeignCreditCardCommissionProfitAmount + accountReport.CreditCardCommissionProfitAmount + accountReport.TransferCommissionProfitAmount + accountReport.WithdrawalTotalTransactionFeeAmount + totalBankIncomeAmount;

            var netAmount = positiveAmount - negativeAmount;
            var rebateNetAmount = monthlyAccountSummary.Any(x => x.RebateNetAmount != 0) ? monthlyAccountSummary.FirstOrDefault(x => x.RebateNetAmount != 0).RebateNetAmount : 0m;

            var data = new AccountingSummaryMonthlyEditViewModel.MonthlyAccountSummary()
            {
                WithdrawalRequestProfit = accountReport.WithdrawalTotalTransactionFeeAmount,
                PaymentNotificationProfit = accountReport.TransferCommissionProfitAmount,
                CreditCardPaymentNotificationProfit = accountReport.CreditCardCommissionProfitAmount,
                ExpenseAmount = totalBankExpenseAmount,
                IncomeAmount = totalBankIncomeAmount,
                Profit = profit,
                NetAmount = netAmount,
                Difference = (netAmount + totalBankExpenseAmount + rebateNetAmount + accountReport.FraudExpenseProfitAmount) - (previosMonthlyReportEntity.NetAmount + profit),
                IsMonthly = true,
                PreviousReportNetAmount = previosMonthlyReportEntity.NetAmount,
                DailyTotalPaymentAmount = !previosMonthlyReportRequest && monthlyAccountSummary.Any(x => x.DailyTotalPaymentAmount != 0) ? monthlyAccountSummary.FirstOrDefault(x => x.DailyTotalPaymentAmount != 0).DailyTotalPaymentAmount : 0m,
                WeeklyTotalPaymentAmount = !previosMonthlyReportRequest && monthlyAccountSummary.Any(x => x.WeeklyTotalPaymentAmount != 0) ? monthlyAccountSummary.FirstOrDefault(x => x.WeeklyTotalPaymentAmount != 0).WeeklyTotalPaymentAmount : 0m,
                MonthlyTotalPaymentAmount = monthlyAccountSummary.Any(x => x.MonthlyTotalPaymentAmount != 0) ? monthlyAccountSummary.FirstOrDefault(x => x.MonthlyTotalPaymentAmount != 0).MonthlyTotalPaymentAmount : 0m,
                RebateNetAmount = rebateNetAmount,
                CreditCardPoolBalance = creditCardPoolAmount,
                FraudPoolBalance = dealerFraudPoolAmount,
                FraudExpenseProfitAmount = accountReport.FraudExpenseProfitAmount,
                ForeignCreditCardPaymentNotificationProfit = accountReport.ForeignCreditCardCommissionProfitAmount,
            };

            model.MonthlyAccountSummaries = data;
            return View("MonthlyAccountSummary", model);
        }

        [HttpGet]
        public IActionResult DealerAccountSummary()
        {
            ViewBag.Companies = _companyManager.GetActiveList(null);
            return View("DealerAccountSummary");
        }

        [HttpPost]
        public IActionResult DealerAccountSummaryPartialView([FromBody] JObject jObj)
        {
            var model = new DealerAccountSummaryEditViewModel();

            var startDate = jObj["StartDate"].ToString();
            var endDate = jObj["EndDate"].ToString();
            var startDateTime = jObj["StartDateTime"].ToString();
            var endDateTime = jObj["EndDateTime"].ToString();

            //if (startDate == endDate || endDateTime == "00:00:00")
            //{
            //    endDateTime = "23:59:59";
            //}

            var startDateTimeParsed = DateTime.Parse($"{startDate} {startDateTime}");
            var endDateTimeParsed = DateTime.Parse($"{endDate} {endDateTime}");

            var dealerAccountSummary = _companyTransactionManager.GetDealerAccountSummary(jObj["IDCompany"].ToString(), startDateTimeParsed, endDateTimeParsed);

            //var dealerFraudPool = _companyTransactionManager.GetDealerFraudPool(new List<FieldParameter>()
            //{
            //    new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null),
            //    new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
            //    new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
            //    new FieldParameter("PageLength", Enums.FieldType.Int, 9999),
            //    new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
            //    new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null)
            //});

            //var dealerFraudExpenseProfitAmount = dealerFraudPool.Where(x => x.PaymentMethodType == (int)PaymentMethod.CreditCard).ToList().Count > 0 ? dealerFraudPool.Where(x => x.PaymentMethodType == (int)PaymentMethod.CreditCard).Sum(x => x.PaymentInstitutionNetAmount - x.DealerCommission) : 0;

            if(dealerAccountSummary.Count > 0)
            {
                var data = new DealerAccountSummaryEditViewModel.DealerAccountSummary()
                {
                    WithdrawalRequestProfit = dealerAccountSummary.FirstOrDefault().WithdrawalRequestProfit,
                    WithdrawalRequestTotalAmount = dealerAccountSummary.FirstOrDefault().WithdrawalRequestTotalAmount,
                    PaymentNotificationProfit = dealerAccountSummary.FirstOrDefault().PaymentNotificationTotalProfit,
                    PaymentNotificationTotalAmount = dealerAccountSummary.FirstOrDefault().PaymentNotificationTotalAmount,
                    CreditCardPaymentNotificationProfit = dealerAccountSummary.FirstOrDefault().CreditCardProfit,
                    CreditCardPaymentNotificationTotalAmount = dealerAccountSummary.FirstOrDefault().CreditCardSumTotal,
                    CreditCardCount = dealerAccountSummary.FirstOrDefault().CreditCardCount,
                    PaymentNotificationCount = dealerAccountSummary.FirstOrDefault().PaymentNotificationCount,
                    WithdrawalRequestCount = dealerAccountSummary.FirstOrDefault().WithdrawalRequestCount,
                    RebateNetAmount = dealerAccountSummary.FirstOrDefault().RebateNetAmount,
                    FraudExpenseProfitAmount = dealerAccountSummary.FirstOrDefault().CreditCardFraudExpenseAmount,
                    BankCardTypeCount = dealerAccountSummary.FirstOrDefault().BankCardTypeCount,
                    BankCardTypePaymentNotificationProfit = dealerAccountSummary.FirstOrDefault().BankCardTypePaymentNotificationProfit,
                    BankCardTypePaymentNotificationTotalAmount = dealerAccountSummary.FirstOrDefault().BankCardTypePaymentNotificationTotalAmount,
                    AverageCommissionRate = dealerAccountSummary.FirstOrDefault().AverageCommissionRate,
                    ForeignCreditCardSummaries = dealerAccountSummary.FirstOrDefault().ForeignCreditCardSummaries
                };

                model.DealerAccountSummaries = data;
                model.DealerAccountSummaries.IDCompany = jObj["IDCompany"].ToString();
                model.DealerAccountSummaries.StartDate = startDateTimeParsed;
                model.DealerAccountSummaries.EndDate = endDateTimeParsed;
            }
            else
            {
                var data = new DealerAccountSummaryEditViewModel.DealerAccountSummary();
                model.DealerAccountSummaries = data;
            }

            return PartialView("DealerAccountSummaryPartialView", model);
        }

        [HttpPost]
        public IActionResult CreateCompanyInvoice([FromBody] DealerAccountSummaryEditViewModel model)
        {
            if(model == null)
                return Json(new GenericResponse { Status = "ERROR", Message = "Listeleme işlemi yapılmadan faturalandırma yapılamaz" });

    
            var checkInvoiceDateOverlap = _companyInvoiceManager.CheckInvoiceDateOverlap(model.DealerAccountSummaries.IDCompany, model.DealerAccountSummaries.StartDate, model.DealerAccountSummaries.EndDate);

            if (checkInvoiceDateOverlap)
                return Json(new GenericResponse { Status = "ERROR", Message = "Seçilen tarih aralığında fatura mevut." });

            var taxRate = int.Parse(_settingDAL.GetList(null).FirstOrDefault(x => x.ParamType == "CompanyInvoiceTaxRate" && x.ParamDef == "tax_rate").ParamVal);

            var foreignCreditCardTRYProfit = 0.0m;

            if (model.DealerAccountSummaries.ForeignCreditCardSummaries != null)
            {
                foreach (var item in model.DealerAccountSummaries.ForeignCreditCardSummaries.Where(x => x.Currency == "TRY"))
                {
                    foreignCreditCardTRYProfit = item.Profit;
                }
            }
            
            var totalAmount = model.DealerAccountSummaries.PaymentNotificationProfit + model.DealerAccountSummaries.CreditCardPaymentNotificationProfit + model.DealerAccountSummaries.BankCardTypePaymentNotificationProfit + model.DealerAccountSummaries.WithdrawalRequestProfit + foreignCreditCardTRYProfit;

            if(totalAmount > 0)
            {
                var netAmount = totalAmount / (100 + taxRate) * 100m;

                var taxAmount = totalAmount - netAmount;

                var companyInvoice = new CompanyInvoice
                {
                    CDate = DateTime.Now,
                    CUser = IDUser,
                    IDCompany = model.DealerAccountSummaries.IDCompany,
                    InvoiceStartDateTime = model.DealerAccountSummaries.StartDate,
                    InvoiceEndDateTime = model.DealerAccountSummaries.EndDate,
                    NetAmount = netAmount,
                    TaxAmount = taxAmount,
                    TotalAmount = totalAmount,
                    CurrencyCode = "TRY",
                    TaxRate = taxRate
                };

                var resp = _companyInvoiceManager.Insert(companyInvoice);


                if (resp.Status == "ERROR")
                    return Json(new GenericResponse { Status = "ERROR", Message = $"TRY para biriminde fatura oluşturulamadı" });
            }

            if (model.DealerAccountSummaries.ForeignCreditCardSummaries != null)
            {
                foreach (var item in model.DealerAccountSummaries.ForeignCreditCardSummaries.Where(x => x.Currency != "TRY"))
                {
                    var foreignCurrencyTotalAmount = item.Profit + item.WithdrawalRequestSummary.WithdrawalRequestProfit;

                    var foreignCurrencyNetAmount = foreignCurrencyTotalAmount / (100 + taxRate) * 100m;

                    var foreignCurrencyTaxAmount = foreignCurrencyTotalAmount - foreignCurrencyNetAmount;

                    var foreignCurrencyCompanyInvoice = new CompanyInvoice
                    {
                        CDate = DateTime.Now,
                        CUser = IDUser,
                        IDCompany = model.DealerAccountSummaries.IDCompany,
                        InvoiceStartDateTime = model.DealerAccountSummaries.StartDate,
                        InvoiceEndDateTime = model.DealerAccountSummaries.EndDate,
                        NetAmount = foreignCurrencyNetAmount,
                        TaxAmount = foreignCurrencyTaxAmount,
                        TotalAmount = foreignCurrencyTotalAmount,
                        CurrencyCode = item.Currency,
                        TaxRate = taxRate
                    };

                    var response = _companyInvoiceManager.Insert(foreignCurrencyCompanyInvoice);

                    if (response.Status == "ERROR")
                        return Json(new GenericResponse { Status = "ERROR", Message = $"{item.Currency} para biriminde fatura oluşturulamadı" });
                }
            }

            return Ok();
        }
        
    }
}
