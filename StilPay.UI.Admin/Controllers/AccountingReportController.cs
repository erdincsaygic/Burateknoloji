using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using HalkbankWSDL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.DAL.Concrete;
using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.UI.Admin.Infrastructures;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.IsBankTransferService;
using StilPay.Utility.IsBankTransferService.Models.IsBankAccounts;
using StilPay.Utility.IsBankTransferService.Models.IsBankToken;
using StilPay.Utility.KuveytTurk;
using StilPay.Utility.KuveytTurk.KuveytTurkAccountTransaction;
using StilPay.Utility.KuveytTurk.KuveytTurkToken;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static StilPay.UI.Admin.Models.GarantiAccountInfoModel;
using static StilPay.Utility.Helper.Enums;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Accounting")]
    public class AccountingReportController : BaseController<CompanyTransaction>
    {
        private readonly ICompanyTransactionManager _manager;
        private readonly IBankManager _bankManager;
        private readonly ICompanyBankAccountManager _companyBankAccountManager;
        private readonly ICompanyFinanceTransactionManager _companyFinanceTransferManager;
        private readonly IBankBalancesLogManager _bankBalancesLogManager;
        private readonly IAdministratorManager _administratorManager;
        private readonly IPaymentInstitutionManager _paymentInstitutionManager;
        private readonly IPaymentTransferPoolManager _paymentTransferPoolManager;
        private readonly ICompanyWithdrawalRequestManager _companyWithdrawalRequestManager;
        private readonly SettingDAL _settingDAL = new SettingDAL();

        public AccountingReportController(ICompanyTransactionManager manager, IBankManager bankManager, ICompanyBankAccountManager companyBankAccountManager, ICompanyFinanceTransactionManager companyFinanceTransferManager, IBankBalancesLogManager bankBalancesLogManager, IAdministratorManager administratorManager, IHttpContextAccessor httpContext, IPaymentInstitutionManager paymentInstitutionManager, IPaymentTransferPoolManager paymentTransferPoolManager, ICompanyWithdrawalRequestManager companyWithdrawalRequestManager) : base(httpContext)
        {
            _manager = manager;
            _bankManager = bankManager;
            _companyBankAccountManager = companyBankAccountManager;
            _companyFinanceTransferManager = companyFinanceTransferManager;
            _bankBalancesLogManager = bankBalancesLogManager;
            _administratorManager = administratorManager;
            _paymentInstitutionManager = paymentInstitutionManager;
            _paymentTransferPoolManager = paymentTransferPoolManager;
            _companyWithdrawalRequestManager = companyWithdrawalRequestManager;
        }

        public override IBaseBLL<CompanyTransaction> Manager()
        {
            return _manager;
        }

        [HttpPost]
        public override IActionResult Gets([FromBody] JObject jObj)
        {
            var list = _manager.GetAccountReport(null, Convert.ToDateTime(jObj["StartDate"].ToString()), Convert.ToDateTime(jObj["EndDate"].ToString()), Convert.ToDateTime(jObj["StartDateTime"].ToString()), Convert.ToDateTime(jObj["EndDateTime"].ToString()));
            return Json(list);
        }

        public IActionResult BankAccount()
        {
            var model = new BankAccountEditViewModel();

            var entity = _bankManager.GetActiveList(null);

            model.Banks = entity;

            return View(model);
        }

        public IActionResult EditBankAccount(string id = null)
        {
            var model = new CompanyBankAccount();

            if (!string.IsNullOrEmpty(id))
            {
                var entity = _companyBankAccountManager.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                model = entity;
            }

            return RedirectToAction("BankAccountDetail", model);
        }

        public IActionResult BankAccountDetail(CompanyBankAccount entity2 = null)
        {
            var model = new BankAccountEditViewModel();

            var entity = _bankManager.GetActiveList(null);

            model.Banks = entity;
            model.entity = entity2;
            return View(model);
        }

        public IActionResult GetBankAccount()
        {
            var entity = _companyBankAccountManager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, "1312E00F-E83E-45B4-85C6-892396D12331")
            });

            return Json(entity);
        }

        [HttpPost]
        public IActionResult DropBankAccount(CompanyBankAccount entity)
        {
            return Json(_companyBankAccountManager.Delete(entity));
        }

        [HttpPost]
        public IActionResult SaveMyBank(CompanyBankAccount entity)
        {
            entity.IDCompany = "1312E00F-E83E-45B4-85C6-892396D12331";

            if (!entity.ShowInSystemSettings)
            {
                entity.IsActiveForIFrame = false;
                entity.IsActiveByDefaultExpenseAccount = false;
            }

            if (!string.IsNullOrEmpty(entity.ID))

                return Json(_companyBankAccountManager.Update(entity));
            else
                return Json(_companyBankAccountManager.Insert(entity));
        }

        [HttpPost]
        public IActionResult CompanyFinanceTransferInsert(CompanyFinanceTransaction entity)
        {
            GenericResponse ErrorMesage = new GenericResponse();
            ErrorMesage.Message = "Lütfen Banka Seçiniz.";
            ErrorMesage.Status = "ERROR";
            if (entity.TransactionDetailType == ((int)FinanceExpenseType.KrediKartiCikis) && (entity.IDBankAccount2 == null) && (entity.PaymentMethod == ((int)PaymentMethod.CreditCard)))
            {
                return Json(ErrorMesage);
            }

            if (entity.TransactionDetailType == ((int)((int)FinanceIncomeType.BankaGiris)) && (entity.IDBankAccount2 == null) && (entity.PaymentMethod == ((int)PaymentMethod.TransferEftHavale)))
            {
                return Json(ErrorMesage);
            }

            if (entity.TransactionDetailType == ((int)((int)FinanceExpenseType.BankaCikis)) && (entity.IDBankAccount2 == entity.IDBankAccount) && (entity.PaymentMethod == ((int)PaymentMethod.TransferEftHavale)))
            {
                ErrorMesage.Message = "Transfer edilen banka seçilen banka ile aynı olamaz.";
                return Json(ErrorMesage);
            }

            entity.IDCompany = "1312E00F-E83E-45B4-85C6-892396D12331";
            entity.TransactionDate = DateTime.Now;
            entity.CUser = IDUser;

            if  (entity.PaymentMethod == ((int)PaymentMethod.CreditCard) && (entity.TransactionDetailType == (int)FinanceExpenseType.KrediKartiCikis))
            {
                entity.Piece = entity.Piece;
            }

            var response = _companyFinanceTransferManager.Insert(entity);

            entity.Piece = 0;

            if (entity.PaymentMethod == ((int)PaymentMethod.CreditCard) && entity.TransactionDetailType == ((int)FinanceExpenseType.KrediKartiCikis)) 
            {
                entity.PaymentMethod = 1;
                entity.IDBankAccount = entity.IDBankAccount2;
                entity.TransactionType = 1;
                entity.TransactionDetailType = ((int)FinanceIncomeType.BankaGiris);

                response = _companyFinanceTransferManager.Insert(entity);

            } 
            else if (entity.PaymentMethod == ((int)PaymentMethod.TransferEftHavale) && (entity.TransactionDetailType == (int)FinanceExpenseType.BankaCikis))
            {
                String tempBank = entity.IDBankAccount;

                entity.PaymentMethod = 1;
                entity.IDBankAccount = entity.IDBankAccount2;
                entity.IDBankAccount2 = tempBank;
                entity.TransactionType = 1;
                entity.TransactionDetailType = ((int)FinanceIncomeType.BankaGiris);

                response = _companyFinanceTransferManager.Insert(entity);
            }

            if (response.Status == "ERROR")
            {
                return Json(response);
            }
            else {
                return Json(response);

            }
        }

        [HttpPost]
        public IActionResult PaymentInstitutionTransferInsert(CompanyFinanceTransaction entity)
        {
            if(entity.CompanyIntegrationType == 0)
                return Json(new GenericResponse { Status= "ERROR", Message= "Ödeme kuruluşu seçin"});

            if (entity.Amount == 0)
                return Json(new GenericResponse { Status = "ERROR", Message = "Tutar girin" });

            entity.IDCompany = "1312E00F-E83E-45B4-85C6-892396D12331";
            entity.TransactionDate = DateTime.Now;
            entity.CUser = IDUser;
            entity.PaymentMethod = 1;
            entity.TransactionType = 2;
            entity.TransactionDetailType = 2;
            entity.CompanyIntegrationType = entity.CompanyIntegrationType;

            var response = _companyFinanceTransferManager.Insert(entity);

   
            entity.PaymentMethod = 2;
            entity.TransactionType = 1;
            entity.TransactionDetailType = 2;

            response = _companyFinanceTransferManager.Insert(entity);
            
             return Json(response);          
        }

        public IActionResult BankTransactions()
        {
            var model = new AccountingReportEditViewModel();

            //var entity2 = _companyBankAccountManager.GetActiveList(new List<FieldParameter>()
            //{
            //     new FieldParameter("IDCompany", Enums.FieldType.NVarChar, "1312E00F-E83E-45B4-85C6-892396D12331"),
            //});

            var entity = _companyBankAccountManager.CompanyBankAccountSum("1312E00F-E83E-45B4-85C6-892396D12331", null, null, null);

            var paymentInstitutions = 


            ViewBag.CompanyBankAccount = entity;
            ViewBag.PaymentInstitution = _paymentInstitutionManager.GetList(null);

            return View(model);
        }

        public IActionResult BankTransactionDetail()
        {
            return View();
        }

        [HttpGet]
        public string GetBankName(string id)
        {
            var bank = _companyBankAccountManager.GetSingle(new List<FieldParameter>()
            {
                 new FieldParameter("ID", Enums.FieldType.NVarChar, id),
            });

            return $"Banka: {bank.Name} / IBAN: {bank.IBAN}";
        }

        public IActionResult CreditCardTransactions()
        {

            var model = new AccountingReportEditViewModel();            

            List<BankAccountSumModel> entity = _companyBankAccountManager.CreditCardAccountSum("1312E00F-E83E-45B4-85C6-892396D12331", null, null, null);
            ViewBag.CompanyCreditCardAccount = entity;

            var entity2 = _companyBankAccountManager.GetActiveList(new List<FieldParameter>()
            {
                 new FieldParameter("IDCompany", Enums.FieldType.NVarChar, "1312E00F-E83E-45B4-85C6-892396D12331"),
            });
            ViewBag.CompanyBankAccount = entity2;

            return View(model);
        }

        [HttpPost]
        public IActionResult CreditCardTransactionsDetailList([FromBody] JObject jObj , int id = 0)
        {

            var model = _httpContext.HttpContext.Session.Read<AccountReportCreditCardModel>("AccountReportCreditCardModel");
            var list = _manager.GetAccountReportCreditCardDetail(model.PaymentMethod.ToString(), Convert.ToDateTime(jObj["StartDate"].ToString()), Convert.ToDateTime(jObj["EndDate"].ToString()));
            return Json(list);
        }

        [HttpPost]
        public IActionResult BankTransactionsDetailIncomeList([FromBody] JObject jObj, int id = 0)
        {

            var model = _httpContext.HttpContext.Session.Read<AccountReportCreditCardModel>("AccountReportCreditCardModel");
            var list = _manager.BankTransactionsDetailIncomeList(model.ID, Convert.ToDateTime(jObj["StartDate"].ToString()), Convert.ToDateTime(jObj["EndDate"].ToString()));
            return Json(list);
        }

        [HttpPost]
        public IActionResult BankTransactionsDetailExpenseList([FromBody] JObject jObj, int id = 0)
        {

            var model = _httpContext.HttpContext.Session.Read<AccountReportCreditCardModel>("AccountReportCreditCardModel");
            var list = _manager.BankTransactionsDetailExpenseList(model.ID, Convert.ToDateTime(jObj["StartDate"].ToString()), Convert.ToDateTime(jObj["EndDate"].ToString()));
            return Json(list);
        }

        [HttpPost]
        public IActionResult BankTransactionsDetailPaymentInstitutionsList([FromBody] JObject jObj, int id = 0)
        {

            var model = _httpContext.HttpContext.Session.Read<AccountReportCreditCardModel>("AccountReportCreditCardModel");
            var list = _manager.BankTransactionsDetailTransferList(model.ID, Convert.ToDateTime(jObj["StartDate"].ToString()), Convert.ToDateTime(jObj["EndDate"].ToString())).Where(x => x.PaymentMethodName != "");
            return Json(list);
        }

        [HttpPost]
        public IActionResult BankTransactionsDetailIncomeBankList([FromBody] JObject jObj, int id = 0)
        {

            var model = _httpContext.HttpContext.Session.Read<AccountReportCreditCardModel>("AccountReportCreditCardModel");
            var list = _manager.BankTransactionsDetailTransferList(model.ID, Convert.ToDateTime(jObj["StartDate"].ToString()), Convert.ToDateTime(jObj["EndDate"].ToString())).Where(x => x.PaymentMethodName == "" && x.TransactionType == 1);
            return Json(list);
        }

        [HttpPost]
        public IActionResult BankTransactionsDetailExpenseBankList([FromBody] JObject jObj, int id = 0)
        {

            var model = _httpContext.HttpContext.Session.Read<AccountReportCreditCardModel>("AccountReportCreditCardModel");
            var list = _manager.BankTransactionsDetailTransferList(model.ID, Convert.ToDateTime(jObj["StartDate"].ToString()), Convert.ToDateTime(jObj["EndDate"].ToString())).Where(x => x.PaymentMethodName == "" && x.TransactionType == 2);
            return Json(list);
        }

        public IActionResult CreditCardTransactionsDetail(int id = 0)
        {
            if (id != 0)
            {
                _httpContext.HttpContext.Session.Remove("AccountReportCreditCardModel");

                AccountReportCreditCardModel accountReportCreditCardModel = new AccountReportCreditCardModel();
                accountReportCreditCardModel.PaymentMethod = id;
                _httpContext.HttpContext.Session.Write("AccountReportCreditCardModel", accountReportCreditCardModel);
            }

            var model = new AccountingReportEditViewModel();

            return View(model);
        }

        public IActionResult BankTransactionsDetail(string id = "0")
        {
            if (id != "0")
            {
                _httpContext.HttpContext.Session.Remove("AccountReportCreditCardModel");

                AccountReportCreditCardModel accountReportCreditCardModel = new AccountReportCreditCardModel();
                accountReportCreditCardModel.ID = id;
                _httpContext.HttpContext.Session.Write("AccountReportCreditCardModel", accountReportCreditCardModel);
            }

            var model = new AccountingReportEditViewModel();

            return View(model);
        }

        #region Bank Balances
        public IActionResult GetBankBalances()
        {
            var user = _administratorManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, IDUser) });

            var systemSetting = _settingDAL.GetList(null).Where(x => x.ParamType == "AuthBankBalances").Select(f => f.ParamVal).ToList();

            string[] splitPhone = systemSetting[0].Split(',');

            if (!splitPhone.Contains(user.Phone))
                return Json(new GenericResponse { Status = "ERROR", Message = "Bakiye Sorgulama ve Görüntüleme Yetkiniz Bulunmmaktadır." });

            var companyBankAccounts = _companyBankAccountManager.GetActiveList(new List<FieldParameter>()
            {
                 new FieldParameter("IDCompany", Enums.FieldType.NVarChar, "1312E00F-E83E-45B4-85C6-892396D12331"),
            });


            #region Kuveyt 

            var kuveyIncomeBankStartDate2 = _paymentTransferPoolManager.GetBankLastActivity("07");
            var kuveyExpenseBankStartDate2 = _companyWithdrawalRequestManager.GetBankLastActivity("07");


            var kuveyIncomeBankStartDate = _paymentTransferPoolManager.GetBankLastActivity("36");
            var kuveyExpenseBankStartDate = _companyWithdrawalRequestManager.GetBankLastActivity("36");

            var kuveytIncomeAccountDate = kuveyIncomeBankStartDate.TransactionDate?.ToString("yyyy-MM-dd");
            var kuveytExpenseAccountDate = kuveyExpenseBankStartDate.TransactionDate?.ToString("yyyy-MM-dd");

            var kuveytIncomeAccountDate2 = kuveyIncomeBankStartDate2.TransactionDate?.ToString("yyyy-MM-dd");
            var kuveytExpenseAccountDate2 = kuveyExpenseBankStartDate2.TransactionDate?.ToString("yyyy-MM-dd");

            var kuveyTurkIntegrationValues = tSQLBankManager.GetSystemSettingValues("KuveytTurkClient");

            var kuveytTurkTokenModel = new KuveytTurkTokenRequestModel()
            {
                client_id = kuveyTurkIntegrationValues.FirstOrDefault(f => f.ParamDef == "kuveytturk_client_id").ParamVal,
                client_secret = kuveyTurkIntegrationValues.FirstOrDefault(f => f.ParamDef == "kuveytturk_client_secret").ParamVal
            };

            var kuveytTurkToken = (kuveyIncomeBankStartDate != null && kuveytExpenseAccountDate != null) ? KuveytTurkGetToken.GetAccessToken(kuveytTurkTokenModel) : null;

            var kuveytTurkTokenModel2 = new KuveytTurkTokenRequestModel()
            {
                client_id = "c0b9e377-2d6b-47ed-ab92-05a0fabf6f6f",
                client_secret = "9e72452f-44b9-4026-aa04-fe02da8d4889-1be13037-3120-4248-9171-b7176d563f1b"
            };

            var kuveytTurkToken2 = (kuveytIncomeAccountDate2 != null && kuveytExpenseAccountDate2 != null) ? KuveytTurkGetToken.GetAccessToken(kuveytTurkTokenModel2) : null;

            #endregion

            #region IsBank

            var accounts = new GenericResponseDataModel<IsBankAccountsResponseModel.Root>();
            var isBankTransferIntegrationValues = tSQLBankManager.GetSystemSettingValues("IsBankTransfer");
            var exitAccount_id = isBankTransferIntegrationValues.FirstOrDefault(f => f.ParamDef == "debtor_account_id").ParamVal;
            var isBankTokenModel = new IsBankTokenRequestModel
            {
                Authorization = isBankTransferIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_auth").ParamVal,
                client_id = isBankTransferIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_id").ParamVal,
                client_secret = isBankTransferIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_secret").ParamVal,
                username = isBankTransferIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_username").ParamVal,
                password = isBankTransferIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_password").ParamVal
            };

            var isBankToken = IsBankGetAccessToken.GetAccessToken(isBankTokenModel);

            if (isBankToken != null && isBankToken.Status == "OK" && isBankToken.Data != null)
            {
                var isBankAccountRequestModel = new IsBankAccountsRequestModel()
                {
                    authorization = isBankToken.Data.access_token,
                    certificate = isBankTransferIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_certificate").ParamVal,
                    client_id = isBankTokenModel.client_id,
                    client_secret = isBankTokenModel.client_secret
                };

                accounts = IsBankGetAccounts.GetAccounts(isBankAccountRequestModel);
            }

            #endregion

            #region OVG IsBank

            var ovgIsBankStartDate = _paymentTransferPoolManager.GetBankLastActivity("33");

            Dictionary<string, string> header = new Dictionary<string, string>();
            Dictionary<string, object> body = new Dictionary<string, object>();

            if (ovgIsBankStartDate != null)
            {
                var ovgIsBankSettings = _settingDAL.GetList(null).Where(x => x.ParamType == "OVGIsBankWsdl").ToList();
                header.Clear(); body.Clear();
                body.Add("uid", ovgIsBankSettings.FirstOrDefault(f => f.ParamDef == "uid").ParamVal);
                body.Add("pwd", ovgIsBankSettings.FirstOrDefault(f => f.ParamDef == "pwd").ParamVal);
            }

            #endregion

            #region Halkbank
            var halkbankWsdl = _settingDAL.GetList(null).Where(x => x.ParamType == "HalkbankWsdl").ToList();
            var halkbankService = new HesapEkstreOrtakClient(HesapEkstreOrtakClient.EndpointConfiguration.basicHttpEndpoint);
            halkbankService.ClientCredentials.UserName.UserName = halkbankWsdl.FirstOrDefault(f => f.ParamDef == "username").ParamVal;
            halkbankService.ClientCredentials.UserName.Password = halkbankWsdl.FirstOrDefault(f => f.ParamDef == "password").ParamVal;
            #endregion

            #region Garanti Bankası

            var garantiBankSettings = _settingDAL.GetList(null).Where(x => x.ParamType == "GarantiBank").ToList();

            Dictionary<string, string> headerGaranti = new Dictionary<string, string>();
            Dictionary<string, object> bodyGaranti = new Dictionary<string, object>();

            headerGaranti.Clear(); bodyGaranti.Clear();

            bodyGaranti.Add("grant_type", "client_credentials");
            bodyGaranti.Add("scope", "oob");
            bodyGaranti.Add("client_id", garantiBankSettings.FirstOrDefault(f => f.ParamDef == "client_id").ParamVal);
            bodyGaranti.Add("client_secret", garantiBankSettings.FirstOrDefault(f => f.ParamDef == "client_secret").ParamVal);
            bodyGaranti.Add("redirect_uri", garantiBankSettings.FirstOrDefault(f => f.ParamDef == "redirect_uri").ParamVal);

            var garantiToken = tHttpClientManager<GarantiTokenModel>.PostFormDataGetJson("https://apis.garantibbva.com.tr/auth/oauth/v2/token", headerGaranti, bodyGaranti);
            var garantiAccountInfo = new GarantiAccountInfo();

            if (garantiToken != null && !string.IsNullOrEmpty(garantiToken.access_token))
            {
                headerGaranti.Clear(); bodyGaranti.Clear();

                headerGaranti.Add("Authorization", string.Concat(garantiToken.token_type, " ", garantiToken.access_token));
                bodyGaranti.Add("consentId", garantiBankSettings.FirstOrDefault(f => f.ParamDef == "consent_id").ParamVal);

                garantiAccountInfo = tHttpClientManager<GarantiAccountInfo>.PostJsonDataGetJson("https://apis.garantibbva.com.tr/balancesandmovements/accountinformation/account/v1/getaccountinformation", headerGaranti, bodyGaranti);

            }

            #endregion

            #region Stilpay Garanti Bankası

            var spGarantiBankSettings = _settingDAL.GetList(null).Where(x => x.ParamType == "SPGarantiBank").ToList();

            Dictionary<string, string> spHeaderGaranti = new Dictionary<string, string>();
            Dictionary<string, object> spBodyGaranti = new Dictionary<string, object>();

            spHeaderGaranti.Clear(); spBodyGaranti.Clear();

            spBodyGaranti.Add("grant_type", "client_credentials");
            spBodyGaranti.Add("scope", "oob");
            spBodyGaranti.Add("client_id", spGarantiBankSettings.FirstOrDefault(f => f.ParamDef == "client_id").ParamVal);
            spBodyGaranti.Add("client_secret", spGarantiBankSettings.FirstOrDefault(f => f.ParamDef == "client_secret").ParamVal);
            spBodyGaranti.Add("redirect_uri", spGarantiBankSettings.FirstOrDefault(f => f.ParamDef == "redirect_uri").ParamVal);

            var spGarantiToken = tHttpClientManager<GarantiTokenModel>.PostFormDataGetJson("https://apis.garantibbva.com.tr/auth/oauth/v2/token", spHeaderGaranti, spBodyGaranti);
            var spGarantiAccountInfo = new GarantiAccountInfo();

            if (spGarantiToken != null && !string.IsNullOrEmpty(spGarantiToken.access_token))
            {
                spHeaderGaranti.Clear(); spBodyGaranti.Clear();

                spHeaderGaranti.Add("Authorization", string.Concat(spGarantiToken.token_type, " ", spGarantiToken.access_token));
                spBodyGaranti.Add("consentId", spGarantiBankSettings.FirstOrDefault(f => f.ParamDef == "consent_id").ParamVal);

                spGarantiAccountInfo = tHttpClientManager<GarantiAccountInfo>.PostJsonDataGetJson("https://apis.garantibbva.com.tr/balancesandmovements/accountinformation/account/v1/getaccountinformation", spHeaderGaranti, spBodyGaranti);
            }

            #endregion

            foreach (var item in companyBankAccounts.OrderBy(x => x.IDBank))
            {
                // İş bankası
                if (item.IDBank == "03")
                {
                    if (item.IsExitAccount)
                    {
                        if (accounts != null && accounts.Status == "OK" && accounts.Data != null)
                        {
                            var bankBalancesLog = new BankBalancesLog()
                            {
                                Balance = Convert.ToDecimal(accounts.Data.data.Where(x => x.account_id == exitAccount_id).FirstOrDefault().account_balance, CultureInfo.InvariantCulture),
                                Iban = item.IBAN,
                                IsExitAccount = true,
                                IDBank = item.IDBank,
                                CUser = IDUser,
                                CDate = DateTime.Now,
                                BankTitle = item.Title,
                                BankName = item.Name
                            };

                            var resp = _bankBalancesLogManager.Insert(bankBalancesLog);
                        }
                    }

                    else
                    {
                        if (accounts != null && accounts.Status == "OK" && accounts.Data != null)
                        {
                            var bankBalancesLog = new BankBalancesLog()
                            {
                                Balance = Convert.ToDecimal(accounts.Data.data.Where(x => x.account_id != exitAccount_id).FirstOrDefault().account_balance, CultureInfo.InvariantCulture),
                                Iban = item.IBAN,
                                IsExitAccount = false,
                                IDBank = item.IDBank,
                                CUser = IDUser,
                                CDate = DateTime.Now,
                                BankTitle = item.Title,
                                BankName = item.Name
                            };

                            var resp = _bankBalancesLogManager.Insert(bankBalancesLog);
                        }
                    }
                }

                // OVG İş bankası
                if (item.IDBank == "33" && ovgIsBankStartDate != null)
                {
                    if (!item.IsExitAccount)
                    {
                        body.Add("BeginDate ", ovgIsBankStartDate.TransactionDate);
                        body.Add("EndDate ", ovgIsBankStartDate.TransactionDate);

                        var transactionModelIsBank = tHttpClientManager<IsTransactionModel>.PostFormDataGetXML(string.Concat("https://posmatik2.isbank.com.tr", "/authenticate.aspx"), header, body);

                        if (transactionModelIsBank != null && transactionModelIsBank.Hesaplar != null && transactionModelIsBank.Hesaplar.Hesap != null)
                        {
                            var hesap = transactionModelIsBank.Hesaplar.Hesap.FirstOrDefault(f => f.Tanimlamalar != null);

                            if (hesap != null)
                            {
                                var bankBalancesLog = new BankBalancesLog()
                                {
                                    Balance = hesap.Tanimlamalar.Bakiye,
                                    Iban = item.IBAN,
                                    IsExitAccount = false,
                                    IDBank = item.IDBank,
                                    CUser = IDUser,
                                    CDate = DateTime.Now,
                                    BankTitle = item.Title,
                                    BankName = item.Name
                                };

                                var resp = _bankBalancesLogManager.Insert(bankBalancesLog);
                            }
                        }
                    }
                }

                // Halkbankası
                if (item.IDBank == "04")
                {
                    if (!item.IsExitAccount)
                    {
                        var halkbankStartDate = _paymentTransferPoolManager.GetBankLastActivity("04");

                        if (halkbankStartDate != null)
                        {
                            var request = new HesapEkstreRequest()
                            {
                                BaslangicTarihi = (DateTime)halkbankStartDate.TransactionDate,
                                BitisTarihi = (DateTime)halkbankStartDate.TransactionDate,
                            };

                            var transactionModel = halkbankService.EkstreSorgulamaAsync(request).Result;

                            if (transactionModel != null && transactionModel.HataKodu == "0" && transactionModel.Hesaplar != null && !item.IsExitAccount)
                            {
                                var bankBalancesLog = new BankBalancesLog()
                                {
                                    Balance = Convert.ToDecimal(transactionModel.Hesaplar.FirstOrDefault().KullanilabilirBakiye),
                                    Iban = item.IBAN,
                                    IsExitAccount = false,
                                    IDBank = item.IDBank,
                                    CUser = IDUser,
                                    CDate = DateTime.Now,
                                    BankTitle = item.Title,
                                    BankName = item.Name
                                };

                                var resp = _bankBalancesLogManager.Insert(bankBalancesLog);
                            }
                        }
                    }

                }

                // Kuveytturk
                if (item.IDBank == "07")
                {
                    if (kuveytTurkToken2 != null && kuveytTurkToken2.Status == "OK" && kuveytTurkToken2.Data != null)
                    {
                        if (item.IsExitAccount)
                        {
                            var url = String.Concat("https://gateway.kuveytturk.com.tr/v3/accounts/2/transactions", $"?beginDate={kuveytExpenseAccountDate2}&endDate={DateTime.Now:yyyy-MM-dd}");

                            var rsa = KuveytTurkRSAKeyGenerator.RSAKeyGenerator(@"-----BEGIN RSA PRIVATE KEY-----
        MIIEogIBAAKCAQBqmwZrSKTUTooNq+VHWDT9V+p+qxCuh+YWUb+8qX8Jeq6d0+TW
        zx6moOhJyc4lbIO8gilWle1fN3embSNAT8bMoRi6Xr2ZEWAZPF04VI8/WpyZNQwh
        WPwZHLFfd5jAucprbelTevE5X8WMLiL3bveMvgjcvpTsexLA5lmgdou4plQUVqJc
        nHJJJO0kaR4wPbKbEeTUZ511Uaw9OZ4rXq5DOHy1CqVhsCiKfwXMy6t8x8wh2ijj
        +11Zvh3wH+4pDDiR5CKdMXvglnXfwxObmbKTbBi/cfziFa8zSm7gaTP2gCQ7a2zs
        TtgQl0DNQNKdBHYCf6LOj106cepRp01dnHsVAgMBAAECggEAJb/lowHjVEbHfhXb
        p8rlYLzMDbS3wIXhBRBHrB/9Gzc1NDA/fY10Vh7ugoqSlA/8CjmxN7b5ilkS5n0J
        GZHmXLnDDuPTkatkcys7+2F+JDoK7/mn5PsksiPF739jOQPRWP9fuy7y0pVGV+BS
        g3no8Q6uBrT5+U+PkX1ASaEQ0v6jD3+wkUXrplEoymWtz0SK1U5vUzYNxB+9S1lE
        8bLRPuPPy5NXv/hux5FBsWDm9UPVvAcJ7W3qdpGAdkywluN12iCrROebOfv99rF1
        DTjN9j8awLcIYxvyXFaLH0RbAYQTNtvclT3xkU9H2mmOhX9MH+6G+Jo8w/if/CV0
        WW4YAQKBgQDPKBNcBsOTkODVzkYgS4mGECIhnfuHVL8r1RhietOmYoBaJtatKU+x
        JQGTWOv2ipHcr8BvrMmh5ryJ+LFuDo3T3RsE9zUBcFGeOIsNEEGNJ/u1K46x4Pit
        67ckJ5yqNcsiwqcldxEATwc2x2MYCJF/ZtJvgiGuKV5Ot+DZyZIRFQKBgQCDvbbN
        pNX7DmNoTIuhiogBD0CXhaeiAudnXXTba8ywVnnpZhea3C/8uuG9TZQB4CTZG/BK
        faFCJTQtdMm6sdGMkPkFDbl+43ymStE30CAKKUBZKpCUjChGDkIvjeHsq6ihu6t5
        6dAhCIRHNIBW/XIS7v+rnSnFoldwWZz4B9dCAQKBgDhct/+222l/5pxldhD9XFp8
        czzgRfpJJYZggTTyJDnF3RQqMwiED+mrnuUfMXwvsYXwz5PS2D1TkQKdBnFiRlZZ
        dyt/sw1EKQC6c6LHRH6KXWKqijV9d0uisX6FxItO/YjkmyOHZLnHxrexwhVc53FZ
        YXHzXwSKvtz+DJBU1ogNAoGAQUfR/McQjY5MrhM4Ib0+tZ+0NyEwtvRPbIX/8PbT
        ABJp6MEBM2imksqcL6zwiZljSP4yLQdh0CAVYez8RXn1x3zTGLD7WSgqzVBHqiuE
        pORaEZUo/aMSFdzc6SmaaSeKsVIIn6m/y46n1Yzrh6+hRkaOBKElYNyYDYEqajGg
        dgECgYEAo5clCbOsF9skOVC6e61k3aUz0d7eGuJuwPO1Ceo1tZ9CsLpVMXBtOqqP
        kjdw2yIQfSRAD237XKAgoREJR4S4eUofp0KLGDl8neEsOoqRx8iQKgJp/7kgC/Y8
        1oSsNa4CI5OkEkd4m6HqNktWw7kEvCFb/vxUU4fisDREEqCxJPQ=
        -----END RSA PRIVATE KEY-----", kuveytTurkToken2.Data.access_token, "GET", null, url);

                            var kuveytTurkAccountTransactionRequestModel = new KuveytTurkAccountTransactionRequestModel()
                            {
                                Authorization = kuveytTurkToken2.Data.access_token,
                                Signature = rsa,
                                url = url
                            };

                            var transactionModel = KuveytTurkGetAccountTransaction.GetAccountTransaction(kuveytTurkAccountTransactionRequestModel);

                            if (transactionModel != null && transactionModel.Data != null && transactionModel.Status == "OK")
                            {
                                var bankBalancesLog = new BankBalancesLog()
                                {
                                    Balance = Convert.ToDecimal(transactionModel.Data.value.accountActivities.OrderByDescending(o => Convert.ToDateTime(o.date)).FirstOrDefault().balance, CultureInfo.InvariantCulture),
                                    Iban = item.IBAN,
                                    IsExitAccount = true,
                                    IDBank = item.IDBank,
                                    CUser = IDUser,
                                    CDate = DateTime.Now,
                                    BankTitle = item.Title,
                                    BankName = item.Name
                                };

                                var resp = _bankBalancesLogManager.Insert(bankBalancesLog);
                            }
                        }
                        else
                        {
                            var url = String.Concat("https://gateway.kuveytturk.com.tr/v3/accounts/1/transactions", $"?beginDate={kuveytIncomeAccountDate2}&endDate={kuveytIncomeAccountDate2}");

                            var rsa = KuveytTurkRSAKeyGenerator.RSAKeyGenerator(@"-----BEGIN RSA PRIVATE KEY-----
        MIIEogIBAAKCAQBqmwZrSKTUTooNq+VHWDT9V+p+qxCuh+YWUb+8qX8Jeq6d0+TW
        zx6moOhJyc4lbIO8gilWle1fN3embSNAT8bMoRi6Xr2ZEWAZPF04VI8/WpyZNQwh
        WPwZHLFfd5jAucprbelTevE5X8WMLiL3bveMvgjcvpTsexLA5lmgdou4plQUVqJc
        nHJJJO0kaR4wPbKbEeTUZ511Uaw9OZ4rXq5DOHy1CqVhsCiKfwXMy6t8x8wh2ijj
        +11Zvh3wH+4pDDiR5CKdMXvglnXfwxObmbKTbBi/cfziFa8zSm7gaTP2gCQ7a2zs
        TtgQl0DNQNKdBHYCf6LOj106cepRp01dnHsVAgMBAAECggEAJb/lowHjVEbHfhXb
        p8rlYLzMDbS3wIXhBRBHrB/9Gzc1NDA/fY10Vh7ugoqSlA/8CjmxN7b5ilkS5n0J
        GZHmXLnDDuPTkatkcys7+2F+JDoK7/mn5PsksiPF739jOQPRWP9fuy7y0pVGV+BS
        g3no8Q6uBrT5+U+PkX1ASaEQ0v6jD3+wkUXrplEoymWtz0SK1U5vUzYNxB+9S1lE
        8bLRPuPPy5NXv/hux5FBsWDm9UPVvAcJ7W3qdpGAdkywluN12iCrROebOfv99rF1
        DTjN9j8awLcIYxvyXFaLH0RbAYQTNtvclT3xkU9H2mmOhX9MH+6G+Jo8w/if/CV0
        WW4YAQKBgQDPKBNcBsOTkODVzkYgS4mGECIhnfuHVL8r1RhietOmYoBaJtatKU+x
        JQGTWOv2ipHcr8BvrMmh5ryJ+LFuDo3T3RsE9zUBcFGeOIsNEEGNJ/u1K46x4Pit
        67ckJ5yqNcsiwqcldxEATwc2x2MYCJF/ZtJvgiGuKV5Ot+DZyZIRFQKBgQCDvbbN
        pNX7DmNoTIuhiogBD0CXhaeiAudnXXTba8ywVnnpZhea3C/8uuG9TZQB4CTZG/BK
        faFCJTQtdMm6sdGMkPkFDbl+43ymStE30CAKKUBZKpCUjChGDkIvjeHsq6ihu6t5
        6dAhCIRHNIBW/XIS7v+rnSnFoldwWZz4B9dCAQKBgDhct/+222l/5pxldhD9XFp8
        czzgRfpJJYZggTTyJDnF3RQqMwiED+mrnuUfMXwvsYXwz5PS2D1TkQKdBnFiRlZZ
        dyt/sw1EKQC6c6LHRH6KXWKqijV9d0uisX6FxItO/YjkmyOHZLnHxrexwhVc53FZ
        YXHzXwSKvtz+DJBU1ogNAoGAQUfR/McQjY5MrhM4Ib0+tZ+0NyEwtvRPbIX/8PbT
        ABJp6MEBM2imksqcL6zwiZljSP4yLQdh0CAVYez8RXn1x3zTGLD7WSgqzVBHqiuE
        pORaEZUo/aMSFdzc6SmaaSeKsVIIn6m/y46n1Yzrh6+hRkaOBKElYNyYDYEqajGg
        dgECgYEAo5clCbOsF9skOVC6e61k3aUz0d7eGuJuwPO1Ceo1tZ9CsLpVMXBtOqqP
        kjdw2yIQfSRAD237XKAgoREJR4S4eUofp0KLGDl8neEsOoqRx8iQKgJp/7kgC/Y8
        1oSsNa4CI5OkEkd4m6HqNktWw7kEvCFb/vxUU4fisDREEqCxJPQ=
        -----END RSA PRIVATE KEY-----", kuveytTurkToken2.Data.access_token, "GET", null, url);

                            var kuveytTurkAccountTransactionRequestModel = new KuveytTurkAccountTransactionRequestModel()
                            {
                                Authorization = kuveytTurkToken2.Data.access_token,
                                Signature = rsa,
                                url = url
                            };

                            var transactionModel = KuveytTurkGetAccountTransaction.GetAccountTransaction(kuveytTurkAccountTransactionRequestModel);

                            if (transactionModel != null && transactionModel.Data != null && transactionModel.Status == "OK")
                            {
                                var bankBalancesLog = new BankBalancesLog()
                                {
                                    Balance = Convert.ToDecimal(transactionModel.Data.value.accountActivities.OrderByDescending(o => Convert.ToDateTime(o.date)).FirstOrDefault().balance, CultureInfo.InvariantCulture),
                                    Iban = item.IBAN,
                                    IsExitAccount = false,
                                    IDBank = item.IDBank,
                                    CUser = IDUser,
                                    CDate = DateTime.Now,
                                    BankTitle = item.Title,
                                    BankName = item.Name
                                };

                                var resp = _bankBalancesLogManager.Insert(bankBalancesLog);
                            }
                        }
                    }
                }

                if (item.IDBank == "36")
                {
                    if (kuveytTurkToken != null && kuveytTurkToken.Status == "OK" && kuveytTurkToken.Data != null)
                    {
                        if (item.IsExitAccount)
                        {
                            var url = String.Concat("https://gateway.kuveytturk.com.tr/v3/accounts/2/transactions", $"?beginDate={kuveytExpenseAccountDate}&endDate={kuveytExpenseAccountDate}");

                            var rsa = KuveytTurkRSAKeyGenerator.RSAKeyGenerator(kuveyTurkIntegrationValues.FirstOrDefault(f => f.ParamDef == "kuveytturk_rsa_private_key").ParamVal, kuveytTurkToken.Data.access_token, "GET", null, url);

                            var kuveytTurkAccountTransactionRequestModel = new KuveytTurkAccountTransactionRequestModel()
                            {
                                Authorization = kuveytTurkToken.Data.access_token,
                                Signature = rsa,
                                url = url
                            };

                            var transactionModel = KuveytTurkGetAccountTransaction.GetAccountTransaction(kuveytTurkAccountTransactionRequestModel);

                            if (transactionModel != null && transactionModel.Data != null && transactionModel.Status == "OK")
                            {
                                var bankBalancesLog = new BankBalancesLog()
                                {
                                    Balance = Convert.ToDecimal(transactionModel.Data.value.accountActivities.OrderByDescending(o => Convert.ToDateTime(o.date)).FirstOrDefault().balance, CultureInfo.InvariantCulture),
                                    Iban = item.IBAN,
                                    IsExitAccount = true,
                                    IDBank = item.IDBank,
                                    CUser = IDUser,
                                    CDate = DateTime.Now,
                                    BankTitle = item.Title,
                                    BankName = item.Name
                                };

                                var resp = _bankBalancesLogManager.Insert(bankBalancesLog);
                            }
                        }
                        else
                        {
                            var url = String.Concat("https://gateway.kuveytturk.com.tr/v3/accounts/1/transactions", $"?beginDate={kuveytIncomeAccountDate}&endDate={kuveytIncomeAccountDate}");

                            var rsa = KuveytTurkRSAKeyGenerator.RSAKeyGenerator(kuveyTurkIntegrationValues.FirstOrDefault(f => f.ParamDef == "kuveytturk_rsa_private_key").ParamVal, kuveytTurkToken.Data.access_token, "GET", null, url);

                            var kuveytTurkAccountTransactionRequestModel = new KuveytTurkAccountTransactionRequestModel()
                            {
                                Authorization = kuveytTurkToken.Data.access_token,
                                Signature = rsa,
                                url = url
                            };

                            var transactionModel = KuveytTurkGetAccountTransaction.GetAccountTransaction(kuveytTurkAccountTransactionRequestModel);

                            if (transactionModel != null && transactionModel.Data != null && transactionModel.Status == "OK")
                            {
                                var bankBalancesLog = new BankBalancesLog()
                                {
                                    Balance = Convert.ToDecimal(transactionModel.Data.value.accountActivities.OrderByDescending(o => Convert.ToDateTime(o.date)).FirstOrDefault().balance, CultureInfo.InvariantCulture),
                                    Iban = item.IBAN,
                                    IsExitAccount = false,
                                    IDBank = item.IDBank,
                                    CUser = IDUser,
                                    CDate = DateTime.Now,
                                    BankTitle = item.Title,
                                    BankName = item.Name
                                };

                                var resp = _bankBalancesLogManager.Insert(bankBalancesLog);
                            }
                        }
                    }
                }

                //Garanti
                if (item.IDBank == "05")
                {
                    if (garantiAccountInfo != null && garantiAccountInfo.Accounts != null)
                    {
                        var balances = garantiAccountInfo.Accounts.FirstOrDefault(x => x.IBAN == item.IBAN.Replace(" ", ""));

                        if (balances != null)
                        {
                            var amount = balances.Balances.FirstOrDefault(f => f.Type == "AvailableBalance").Amount;

                            var bankBalancesLog = new BankBalancesLog()
                            {
                                Balance = Convert.ToDecimal(amount, CultureInfo.InvariantCulture),
                                Iban = item.IBAN,
                                IsExitAccount = false,
                                IDBank = item.IDBank,
                                CUser = IDUser,
                                CDate = DateTime.Now,
                                BankTitle = item.Title,
                                BankName = item.Name
                            };

                            var resp = _bankBalancesLogManager.Insert(bankBalancesLog);

                        }
                    }
                }

                //Stilpay Garanti
                if (item.IDBank == "34")
                {
                    if (spGarantiAccountInfo != null && spGarantiAccountInfo.Accounts != null)
                    {
                        var balances = spGarantiAccountInfo.Accounts.FirstOrDefault(x => x.IBAN == item.IBAN.Replace(" ", ""));

                        if (balances != null)
                        {
                            var amount = balances.Balances.FirstOrDefault(f => f.Type == "AvailableBalance").Amount;

                            var bankBalancesLog = new BankBalancesLog()
                            {
                                Balance = Convert.ToDecimal(amount, CultureInfo.InvariantCulture),
                                Iban = item.IBAN,
                                IsExitAccount = false,
                                IDBank = item.IDBank,
                                CUser = IDUser,
                                CDate = DateTime.Now,
                                BankTitle = item.Title,
                                BankName = item.Name
                            };

                            var resp = _bankBalancesLogManager.Insert(bankBalancesLog);

                        }
                    }
                }

                //Papara
                if(item.IDBank == "38")
                {
                    Dictionary<string, string> paparaHeader = new Dictionary<string, string>
                    {
                        { "ApiKey", tSQLBankManager.GetSystemSettingValues("Papara").FirstOrDefault(x => x.ParamDef == "api_key").ParamVal }
                    };

                    var jsonResponse = tHttpClientManager<JObject>.GetRequestGetJObject("https://merchant-api.papara.com/account", paparaHeader);
                    JObject data = JObject.Parse(jsonResponse.ToString());
                    bool succeeded = (bool)data["succeeded"];

                    if (succeeded)
                    {
                        decimal amount = (decimal)data["data"]["balances"][0]["totalBalance"];

                        var bankBalancesLog = new BankBalancesLog()
                        {
                            Balance = amount,
                            Iban = item.IBAN,
                            IsExitAccount = false,
                            IDBank = item.IDBank,
                            CUser = IDUser,
                            CDate = DateTime.Now,
                            BankTitle = item.Title,
                            BankName = item.Name
                        };

                        var resp = _bankBalancesLogManager.Insert(bankBalancesLog);

                    }
                    
                }
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult GetBankBalancesLogData()
        {
            var user = _administratorManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, IDUser) });

            var systemSetting = _settingDAL.GetList(null).Where(x => x.ParamType == "AuthBankBalances").Select(f => f.ParamVal).ToList();

            string[] splitPhone = systemSetting[0].Split(',');

            if (!splitPhone.Contains(user.Phone))
                return Json(new GenericResponse { Status = "ERROR", Message = "Bakiye Sorgulama ve Görüntüleme Yetkiniz Bulunmamaktadır." });

            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];



            var list = _bankBalancesLogManager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("StartDate", Enums.FieldType.DateTime, string.IsNullOrEmpty(HttpContext.Request.Form["StartDate"].ToString()) ? (DateTime?)null : Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString())),
               new FieldParameter("EndDate", Enums.FieldType.DateTime, string.IsNullOrEmpty(HttpContext.Request.Form["EndDate"].ToString()) ? (DateTime?)null : Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString())),
                new FieldParameter("PageLenght", Enums.FieldType.Int, length),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue)

            });

            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;

            var result = new
            {
                recordsFiltered = recordsTotal,
                data = list
            };

            return Json(result);
        }
        #endregion

        #region BankTransactionList

        [HttpGet]
        public IActionResult BankTransactionList(string id)
        {
            var model = _companyBankAccountManager.GetSingle(new List<FieldParameter>()
            {
                 new FieldParameter("ID", Enums.FieldType.NVarChar, id),
            });

            return View("BankTransactionList", model);
        }

        [HttpPost]
        public IActionResult GetIncomeBankTransactionList(string id)
        {
            var bankAccount = _companyBankAccountManager.GetSingle(new List<FieldParameter>()
            {
                 new FieldParameter("ID", Enums.FieldType.NVarChar, id),
            });

            //var bankTransactionList = new List<BankTransactionList>();

            //var bankTransactionExpenseList = _manager.BankTransactionsDetailExpenseList(id, Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString()), Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString()));

            //var bankTransactionIncomeList = _manager.BankTransactionsDetailIncomeList(id, Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString()), Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString()));

            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            if (bankAccount.IsExitAccount)
            {
                var list = _manager.GetBankTransactionList(new List<FieldParameter>()
                {
                    new FieldParameter("IDBank", Enums.FieldType.NVarChar, bankAccount.IDBank),
                    new FieldParameter("IDBankAccount", Enums.FieldType.NVarChar, bankAccount.ID),
                    new FieldParameter("StartDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString())),
                    new FieldParameter("EndDate", Enums.FieldType.DateTime,  Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString())),
                    new FieldParameter("IsExitAccount", Enums.FieldType.Bit, bankAccount.IsExitAccount),
                    new FieldParameter("PageLength", Enums.FieldType.Int, length),
                    new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                    new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue),
                });

                var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;

                var result = new
                {
                    recordsFiltered = recordsTotal,
                    data = list
                };

                return Json(result);
            }
            else
            {
                //var list = _paymentTransferPoolManager.GetList(new List<FieldParameter>() 
                //{
                //    new FieldParameter("Status", Enums.FieldType.Tinyint, null),
                //    new FieldParameter("StartDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString())),
                //    new FieldParameter("EndDate", Enums.FieldType.DateTime,  Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString())),
                //    new FieldParameter("PageLenght", Enums.FieldType.Int, length),
                //    new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                //    new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue),
                //    new FieldParameter("IDBank", Enums.FieldType.NVarChar, bankAccount.IDBank)
                //});

                var list = _manager.GetBankTransactionList(new List<FieldParameter>()
                {
                    new FieldParameter("IDBank", Enums.FieldType.NVarChar, bankAccount.IDBank),
                    new FieldParameter("IDBankAccount", Enums.FieldType.NVarChar, bankAccount.ID),
                    new FieldParameter("StartDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString())),
                    new FieldParameter("EndDate", Enums.FieldType.DateTime,  Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString())),
                    new FieldParameter("IsExitAccount", Enums.FieldType.Bit, bankAccount.IsExitAccount),
                    new FieldParameter("PageLength", Enums.FieldType.Int, length),
                    new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                    new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue),
                });

                var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;

                var result = new
                {
                    recordsFiltered = recordsTotal,
                    data = list
                };

                return Json(result);
            }
            
        }
        #endregion

        #region CreditCardTransactionList
        [HttpGet]
        public IActionResult CreditCardTransactionList(string id)
        {
            var model = _paymentInstitutionManager.GetList(null).FirstOrDefault(f => f.ID == id);
            return View("CreditCardTransactionList",model);
        }

        [HttpPost]
        public IActionResult GetCreditCardTransactionList(string id)
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = _manager.GetCreditCardTransactionList(new List<FieldParameter>()
            {
                new FieldParameter("PaymentInstitutionID", Enums.FieldType.NVarChar, id),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString())),
                new FieldParameter("EndDate", Enums.FieldType.DateTime,  Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString())),
                new FieldParameter("PageLength", Enums.FieldType.Int, length),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue),
            });

            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;

            var result = new
            {
                recordsFiltered = recordsTotal,
                data = list
            };

            return Json(result);
        }


        [HttpPost]
        public IActionResult PaymentInstitutionSetEndOfDayTime(string paymentInstitutionId, string endofdaytime)
        {

            var entity = _paymentInstitutionManager.GetSingle(new List<FieldParameter>()
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, paymentInstitutionId)
            });

            entity.EndOfDayTime = endofdaytime;

            return Json(_paymentInstitutionManager.Update(entity));

        }


        
        #endregion
    }
}
