using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.BLL.Concrete;
using StilPay.Entities;
using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.UI.Admin.Infrastructures;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using static StilPay.UI.Admin.Models.CompanyEditViewModel;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Dealer")]
    public class DealerController : BaseController<Company>
    {
        private readonly ICompanyManager _manager;
        private readonly ICompanyCommissionManager _commissionManager;
        private readonly ICompanyIntegrationManager _integrationManager;
        private readonly ICompanyBankManager _companyBankManager;
        private readonly ICompanyUserManager _companyUserManager;
        private readonly ICompanyPaymentInstitutionManager _companyPaymentInstitutionManager;
        private readonly IPaymentInstitutionManager _paymentInstitutionManager;
        private readonly ICompanyBankAccountManager _companyBankAccountManager;
        private readonly ICompanyCurrencyManager _companyCurrencyManager;
        private readonly ICurrencyManager _currencyManager;
        private readonly ICompanyFraudControlManager _companyFraudControlManager;
        private readonly ICompanyWithdrawalRequestManager _companyWithdrawalRequestManager;
        private readonly ICompanyAutoNotificationSettingManager _companyAutoNotificationSettingManager;
        public DealerController(ICompanyManager manager, ICompanyCommissionManager commissionManager, ICompanyIntegrationManager integrationManager, ICompanyUserManager companyUserManager, ICompanyBankManager companyBankManager, IHttpContextAccessor httpContext, ICompanyPaymentInstitutionManager companyPaymentInstitutionManager, IPaymentInstitutionManager paymentInstitutionManager, ICompanyBankAccountManager companyBankAccountManager, ICompanyCurrencyManager companyCurrencyManager, ICurrencyManager currencyManager, ICompanyFraudControlManager companyFraudControlManager, ICompanyWithdrawalRequestManager companyWithdrawalRequestManager, ICompanyAutoNotificationSettingManager companyAutoNotificationSettingManager) : base(httpContext)
        {
            _manager = manager;
            _commissionManager = commissionManager;
            _integrationManager = integrationManager;
            _companyBankManager = companyBankManager;
            _companyUserManager = companyUserManager;
            _companyPaymentInstitutionManager = companyPaymentInstitutionManager;
            _paymentInstitutionManager = paymentInstitutionManager;
            _companyBankAccountManager = companyBankAccountManager;
            _companyCurrencyManager = companyCurrencyManager;
            _currencyManager = currencyManager;
            _companyFraudControlManager = companyFraudControlManager;
            _companyWithdrawalRequestManager = companyWithdrawalRequestManager;
            _companyAutoNotificationSettingManager = companyAutoNotificationSettingManager;
        }

        public override IBaseBLL<Company> Manager()
        {
            return _manager;
        }

        public override EditViewModel<Company> InitEditViewModel(string id)
        {
            var model = new CompanyEditViewModel();

            if (!string.IsNullOrEmpty(id))
            {
                model.entity = Manager().GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                model.Integration = _integrationManager.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                model.CompanyBanks = _companyBankManager.GetActiveList(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                model.CompanyBankAccounts = _companyBankAccountManager.GetList(new List<FieldParameter>()
                {
                    new FieldParameter("IDCompany", Enums.FieldType.NVarChar, "1312E00F-E83E-45B4-85C6-892396D12331")
                });

                ViewBag.CompanyBankAccounts = _companyBankAccountManager.GetList(new List<FieldParameter>()
                {
                    new FieldParameter("IDCompany", Enums.FieldType.NVarChar, id)
                });

                model.Commission = _commissionManager.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                model.PaymentInstitutions = _paymentInstitutionManager.GetList(null);

                model.CompanyUsers = _companyUserManager.GetAllCompanyUsers(id);

                model.PaymentsBanks = model.CompanyBanks;

                model.CompanyPaymentInstitutions = _companyPaymentInstitutionManager.GetList(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                model.CompanyAutoNotificationSettings = _companyAutoNotificationSettingManager.GetSingleByIDCompany(new List<FieldParameter>()
                {
                    new FieldParameter("IDCompany", Enums.FieldType.NVarChar, id)
                });

                foreach (var item in model.CompanyPaymentInstitutions)
                {
                    var temp = model.PaymentInstitutions.FirstOrDefault(pi => pi.ID == item.PaymentInstitutionID);

                    temp.IsVisibleCompanyPanel = true;
                }

                model.Currencies = _currencyManager.GetList(new List<FieldParameter> { new FieldParameter("IDCompany", Enums.FieldType.NVarChar, id) });

                model.CompanyCurrencies = _companyCurrencyManager.GetList(new List<FieldParameter> { new FieldParameter("IDCompany", Enums.FieldType.NVarChar, id)});

                model.IframeUseSetting.TransferBeUsed = model.Integration.TransferBeUsed;
                model.IframeUseSetting.CreditCardBeUsed = model.Integration.CreditCardBeUsed;
                model.IframeUseSetting.ForeignCreditCardBeUsed = model.Integration.ForeignCreditCardBeUsed;
                model.IframeUseSetting.WithdrawalApiBeUsed = model.Integration.WithdrawalApiBeUsed;

                model.CreditCardPaymentMethods.CreditCardPaymentWithParam = model.Integration.CreditCardPaymentWithParam;
                model.CreditCardPaymentMethods.CreditCardPaymentWithPayNKolay = model.Integration.CreditCardPaymentWithPayNKolay;
                model.CreditCardPaymentMethods.ForeignCreditCardPaymentWithPayNKolay = model.Integration.ForeignCreditCardPaymentWithPayNKolay;

                model.FraudControl = _companyFraudControlManager.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                var balances = _manager.GetBalance(id);
                if (balances != null)
                {
                    model.Balance.UsingBalance = balances.UsingBalance;
                    model.Balance.BlockedBalance = balances.BlockedBalance;
                    model.Balance.TotalBalance = balances.TotalBalance;
                    model.Balance.NegativeBalance = balances.NegativeBalanceLimit;
                }
            }

            _httpContext.HttpContext.Session.Write("CompanyEditViewModel", model);

            return model;
        }

        [HttpGet]
        public virtual IActionResult GetActives()
        {
            var list = _manager.GetActiveList(null);

            return Json(list);
        }

        [HttpPost]
        public IActionResult GetData()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = _manager.GetList(new List<FieldParameter>()
            {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Download(string formName, byte[] formFile)
        {
            return File(formFile, "application/octet-stream", formName);
        }

        [HttpGet]
        public IActionResult ShowFile(string id, string name)
        {
            var company = _manager.GetSingle(new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar,id)
            });

            if (name.Equals("IdentityFrontSide"))
                return new FileContentResult(company.IdentityFrontSide, "application/pdf");
            else if (name.Equals("IdentityBackSide"))
                return new FileContentResult(company.IdentityBackSide, "application/pdf");
            else if (name.Equals("TaxPlate"))
                return new FileContentResult(company.TaxPlate, "application/pdf");
            else if (name.Equals("SignatureCirculars"))
                return new FileContentResult(company.SignatureCirculars, "application/pdf");
            else if (name.Equals("TradeRegistryGazette"))
                return new FileContentResult(company.TradeRegistryGazette, "application/pdf");
            else if (name.Equals("Agreement"))
                return new FileContentResult(company.Agreement, "application/pdf");
            else
                return new FileContentResult(null, "application/pdf");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveIntegration(CompanyIntegration integration)
        {
            return Json(_integrationManager.Update(integration));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveCommission(CompanyCommission commission, int confirmCode)
        {
            var genericResponse = _httpContext.HttpContext.Session.ValidateConfirmCode("Commission_Rate_ConfirmCode", confirmCode);
            if (genericResponse.Status == "ERROR")
                return Json(genericResponse);

            return Json(_commissionManager.Update(commission));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetAutoWithdrawalLimit(CompanyEditViewModel companyEditViewModel, int autoWithdrawalLimitConfirmCode)
        {
            var genericResponse = _httpContext.HttpContext.Session.ValidateConfirmCode("AutoWithdrawalLimit_ConfirmCode", autoWithdrawalLimitConfirmCode);
            if (genericResponse.Status == "ERROR")
                return Json(genericResponse);

            var response = _manager.SetAutoWithdrawalLimit(companyEditViewModel.Commission.ID, companyEditViewModel.entity.AutoWithdrawalLimit);

            if (response.Status == "ERROR")
                return Json(new GenericResponse { Status = "ERROR", Message = response.Message });

            return Json(new GenericResponse { Status = "OK", Message = response.Message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetAutoTransferLimit(CompanyEditViewModel companyEditViewModel, int autoTransferLimitConfirmCode)
        {
            var genericResponse = _httpContext.HttpContext.Session.ValidateConfirmCode("AutoTransferLimit_ConfirmCode", autoTransferLimitConfirmCode);
            if (genericResponse.Status == "ERROR")
                return Json(genericResponse);

            var response = _manager.SetAutoTransferLimit(companyEditViewModel.Commission.ID, companyEditViewModel.entity.AutoTransferLimit);

            if (response.Status == "ERROR")
                return Json(new GenericResponse { Status = "ERROR", Message = response.Message });

            return Json(new GenericResponse { Status = "OK", Message = response.Message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetAutoCreditCardLimit(CompanyEditViewModel companyEditViewModel, int autoCreditCardLimitConfirmCode)
        {
            var genericResponse = _httpContext.HttpContext.Session.ValidateConfirmCode("AutoCreditCardLimit_ConfirmCode", autoCreditCardLimitConfirmCode);
            if (genericResponse.Status == "ERROR")
                return Json(genericResponse);

            var response = _manager.SetAutoCreditCardLimit(companyEditViewModel.Commission.ID, companyEditViewModel.entity.AutoCreditCardLimit);

            if (response.Status == "ERROR")
                return Json(new GenericResponse { Status = "ERROR", Message = response.Message });

            return Json(new GenericResponse { Status = "OK", Message = response.Message });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetAutoForeignCreditCardLimit(CompanyEditViewModel companyEditViewModel, int autoForeignCreditCardLimitConfirmCode)
        {
            var genericResponse = _httpContext.HttpContext.Session.ValidateConfirmCode("AutoForeignCreditCardLimit_ConfirmCode", autoForeignCreditCardLimitConfirmCode);
            if (genericResponse.Status == "ERROR")
                return Json(genericResponse);

            var response = _manager.SetAutoForeignCreditCardLimit(companyEditViewModel.Commission.ID, companyEditViewModel.entity.AutoForeignCreditCardLimit);

            if (response.Status == "ERROR")
                return Json(new GenericResponse { Status = "ERROR", Message = response.Message });

            return Json(new GenericResponse { Status = "OK", Message = response.Message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetUsingBalance(CompanyEditViewModel companyEditViewModel, int usingBalanceConfirmCode)
        {
            if (usingBalanceConfirmCode == 0)
                return Json(new GenericResponse { Status = "ERROR", Message = "Doğrulama kodu giriniz." });

            var genericResponse = _httpContext.HttpContext.Session.ValidateConfirmCode("UsingBalance_ConfirmCode", usingBalanceConfirmCode);
            if (genericResponse.Status == "ERROR")
                return Json(genericResponse);

            //if(companyEditViewModel.Balance.Amount > 100000.00M)
            //    return Json(new GenericResponse { Status = "ERROR", Message = "Maximum Tutarın Üstünde İşlem Yapmaya Çalıştınız" });

            var balances = _manager.GetBalance(companyEditViewModel.Commission.ID);
            if (balances != null)
            {
                //if (balances.UsingBalance <= 0)
                //    return Json(new GenericResponse { Status = "ERROR", Message = "Tuar 0'dan büyük olmalıdır" });

                if (companyEditViewModel.Balance.Type == "1")
                    balances.UsingBalance += companyEditViewModel.Balance.Amount;

                else if (companyEditViewModel.Balance.Type == "0")
                    balances.UsingBalance -= companyEditViewModel.Balance.Amount;

                else
                    return Json(new GenericResponse { Status = "ERROR", Message = "İşlem Tipi Hatası" });

                var response = _manager.SetBalance(companyEditViewModel.Commission.ID, balances.UsingBalance, null,null);

                if(response.Status == "ERROR")
                    return Json(new GenericResponse { Status = "ERROR", Message = response.Message });

                return Json(new GenericResponse { Status = "OK", Message = response.Message });
            }
            else
                return Json(new GenericResponse { Status = "ERROR", Message = "Bakiye Bilgisine Ulaşılamadı" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BalanceTransfer(CompanyEditViewModel companyEditViewModel)
        {
            if (string.IsNullOrEmpty(companyEditViewModel.BalanceTransfer.ConfirmCode))
                return Json(new GenericResponse { Status = "ERROR", Message = "Doğrulama kodu giriniz." });

            var genericResponse = _httpContext.HttpContext.Session.ValidateConfirmCode("BalanceTransfer_ConfirmCode",int.Parse(companyEditViewModel.BalanceTransfer.ConfirmCode));
            if (genericResponse.Status == "ERROR")
                return Json(genericResponse);

            if (companyEditViewModel.BalanceTransfer.ReceiverIDCompany == null && string.IsNullOrEmpty(companyEditViewModel.BalanceTransfer.ReceiverIDCompany))
                return Json(new GenericResponse { Status = "ERROR", Message = "Üye işyeri seçiniz" });

            if (companyEditViewModel.BalanceTransfer.ReceiverIDCompany == companyEditViewModel.BalanceTransfer.IDCompany)
                return Json(new GenericResponse { Status = "ERROR", Message = "Alıcı ve Gönderici aynı üye işyeri olamaz" });

            if (companyEditViewModel.BalanceTransfer.Amount <= 0)
                return Json(new GenericResponse { Status = "ERROR", Message = "Tuar 0'dan büyük olmalıdır" });

            var response = _manager.BalanceTransfer(companyEditViewModel.BalanceTransfer.IDCompany, companyEditViewModel.BalanceTransfer.ReceiverIDCompany, companyEditViewModel.BalanceTransfer.Amount);

            if (response.Status == "ERROR")
                return Json(new GenericResponse { Status = "ERROR", Message = response.Message });

            return Json(new GenericResponse { Status = "OK", Message = response.Message });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetCompanyStatus(CompanyEditViewModel companyEditViewModel, int companyStatusConfirmCode)
        {
            var genericResponse = _httpContext.HttpContext.Session.ValidateConfirmCode("UsingBalance_ConfirmCode", companyStatusConfirmCode);
            if (genericResponse.Status == "ERROR")
                return Json(genericResponse);

            var response = _manager.SetStatus(companyEditViewModel.Commission.ID, Convert.ToBoolean(companyEditViewModel.CompanyStatus));

            if (response.Status == "ERROR")
                return Json(new GenericResponse { Status = "ERROR", Message = response.Message });

            return Json(new GenericResponse { Status = "OK", Message = response.Message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetNegativeBalance(CompanyEditViewModel companyEditViewModel, int negativeBalanceConfirmCode)
        {
            var genericResponse = _httpContext.HttpContext.Session.ValidateConfirmCode("NegativeBalanceLimit_ConfirmCode", negativeBalanceConfirmCode);
            if (genericResponse.Status == "ERROR")
                return Json(genericResponse);

            var response = _manager.SetNegativeBalanceLimit(companyEditViewModel.Commission.ID, companyEditViewModel.NegativeBalanceLimit);

            if (response.Status == "ERROR")
                return Json(new GenericResponse { Status = "ERROR", Message = response.Message });

            return Json(new GenericResponse { Status = "OK", Message = response.Message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetAvailableBank(string id, string bankId, string companyBankAccountID, bool value)
        {
            try
            {
                if (value)
                    return Json(_companyBankManager.Insert(new CompanyBank()
                    {
                        ID = id,
                        IDBank = bankId,
                        CUser = IDUser,
                        CDate = DateTime.Now,
                        CompanyBankAccountID = companyBankAccountID
                    }));
                else
                    return Json(_companyBankManager.Delete(new CompanyBank()
                    {
                        ID = id,
                        IDBank = bankId,
                        CompanyBankAccountID = companyBankAccountID
                    }));

            }
            catch (System.Exception ex)
            {
                return Json(new GenericResponse { Status = "ERROR", Message = ex.Message });
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult SetPaymentsBank(string id, CompanyBank paymentsBank)
        //{
        //    var entity = _integrationManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, id) });

        //    if (paymentsBank.IsActiveForPaymentsBanks)
        //        entity.SIDBankForPayments = paymentsBank.IDBank;
        //    else
        //        entity.SIDBankForPayments = null;

        //    return Json(_integrationManager.Update(entity));

        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetIframeUseSettings(CompanyEditViewModel companyEditViewModel)
        {
            return Json(_integrationManager.SetIframeUseSettings(companyEditViewModel.entity.ID, companyEditViewModel.IframeUseSetting.TransferBeUsed, companyEditViewModel.IframeUseSetting.CreditCardBeUsed, companyEditViewModel.IframeUseSetting.ForeignCreditCardBeUsed, companyEditViewModel.IframeUseSetting.WithdrawalApiBeUsed));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetCreditCardPaymentMethod(CompanyEditViewModel companyEditViewModel)
        {
            return Json(_integrationManager.SetCreditCardPaymentMethod(companyEditViewModel.entity.ID,companyEditViewModel.CreditCardPaymentMethods.CreditCardPaymentWithParam, companyEditViewModel.CreditCardPaymentMethods.CreditCardPaymentWithPayNKolay, companyEditViewModel.CreditCardPaymentMethods.ForeignCreditCardPaymentWithPayNKolay));
        }

        public IActionResult RebateTransactions()
        {
            return View();
        }

        public IActionResult RebateTransactionDetail()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsertCompanyPaymentInstitution(string id , string paymentInstitutionId, bool value)
        {
            try
            {
                if(value)              
                    return Json(_companyPaymentInstitutionManager.Insert(new CompanyPaymentInstitution()
                    {
                        PaymentInstitutionID = paymentInstitutionId,
                        ID = id,
                        CUser = IDUser,
                        CDate = DateTime.Now,
                        IsActive = true
                    }));
                
                else
                    return Json(_companyPaymentInstitutionManager.Delete(new CompanyPaymentInstitution()
                    {
                        PaymentInstitutionID = paymentInstitutionId,
                        ID = id,
                    }));
            }
            catch (System.Exception ex)
            {
                return Json(new GenericResponse { Status = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCompanyPaymentInstitution(string id, string paymentInstitutionId, bool value)
        {
            try
            {
                return Json(_companyPaymentInstitutionManager.Update(new CompanyPaymentInstitution()
                {
                    PaymentInstitutionID = paymentInstitutionId,
                    ID = id,
                    MUser = IDUser,
                    MDate = DateTime.Now,
                    IsActive = value
                }));
            }
            catch (System.Exception ex)
            {
                return Json(new GenericResponse { Status = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsertCompanyCurrency(string id, string currencyCode, bool addCurrency)
        {
            try
            {
                if (addCurrency)
                    return Json(_companyCurrencyManager.Insert(new CompanyCurrency()
                    {
                        CurrencyCode = currencyCode,
                        IDCompany = id,
                        CUser = IDUser,
                        CDate = DateTime.Now
                    }));

                else
                    return Json(_companyCurrencyManager.Delete(new CompanyCurrency()
                    {
                        CurrencyCode = currencyCode,
                        IDCompany = id,
                    }));
            }
            catch (System.Exception ex)
            {
                return Json(new GenericResponse { Status = "ERROR", Message = ex.Message });
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult UpdateCompanyCurrency(string id, string currencyCode, bool canTakePaidFromCurrency)
        //{
        //    try
        //    {
        //        return Json(_companyCurrencyManager.Update(new CompanyCurrency()
        //        {
        //            CurrencyCode = currencyCode,
        //            IDCompany = id,
        //            IsActive = canTakePaidFromCurrency,
        //            MDate = DateTime.Now,
        //            MUser = IDUser,
        //        }));
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return Json(new GenericResponse { Status = "ERROR", Message = ex.Message });
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCompanyCurrency(CompanyCurrency companyCurrency)
        {
            try
            {
                return Json(_companyCurrencyManager.Update(companyCurrency));
            }
            catch (System.Exception ex)
            {
                return Json(new GenericResponse { Status = "ERROR", Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult NewDealer()
        {
            return View("NewDealer");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveNewDealer(NewDealerDto newDealerDto)
        {
            newDealerDto.IDUser = IDUser;
            return Json(_manager.NewDealerInsert(newDealerDto));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveFraudControlSetting(CompanyFraudControl fraudControl)
        {
            try
            {
                return Json(_companyFraudControlManager.Update(fraudControl));
            }
            catch (System.Exception ex)
            {
                return Json(new GenericResponse { Status = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateWithdrawalRequest(CompanyEditViewModel  companyEditViewModel)
        {
            try
            {
                var companyCreateWithdrawalRequest = companyEditViewModel.CompanyCreateWithdrawalRequest;

                var genericResponse = _httpContext.HttpContext.Session.ValidateConfirmCode("CompanyCreateWithdrawalRequestSendCode", int.Parse(companyCreateWithdrawalRequest.ConfirmCode));
                if (genericResponse.Status == "ERROR")
                    return Json(genericResponse);


                var amount = companyCreateWithdrawalRequest.Amount;
                var costTotal = companyCreateWithdrawalRequest.CurrencyCostTotal;
                var idCompany = companyCreateWithdrawalRequest.IDCompany;
                decimal usingBalance = 0.0m;

                if (companyCreateWithdrawalRequest.CurrencyCode == "TRY")
                {
                    var balances = _manager.GetBalance(idCompany);
                    if (balances != null)
                    {
                        usingBalance = balances.UsingBalance + balances.NegativeBalanceLimit;
                    }
                }
                else
                {
                    usingBalance = _companyCurrencyManager.GetList(new List<FieldParameter> { new FieldParameter("IDCompany", Enums.FieldType.NVarChar, idCompany) }).FirstOrDefault(f => f.CurrencyCode == companyCreateWithdrawalRequest.CurrencyCode).Balance;
                }

                if (usingBalance < amount + costTotal)
                    return Json(new GenericResponse { Status = "ERROR", Message = $"Üye işyerinin bakiyesi {usingBalance:n2} {companyCreateWithdrawalRequest.CurrencyCode}, işlem ücreti {costTotal:n2} {companyCreateWithdrawalRequest.CurrencyCode}. {usingBalance - costTotal:n2} {companyCreateWithdrawalRequest.CurrencyCode} bildirim yapabilirsiniz." });

                var companyWithdrawalRequest = new CompanyWithdrawalRequest();

                if (companyCreateWithdrawalRequest.BankAccountID != null)
                {
                    var companybankAccount = _companyBankAccountManager.GetSingle(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, companyCreateWithdrawalRequest.BankAccountID) });

                    if (companybankAccount != null)
                    {
                        companyWithdrawalRequest = new CompanyWithdrawalRequest()
                        {
                            CUser = "00000000-0000-0000-0000-000000000000",
                            CDate = DateTime.Now,
                            IDCompany = idCompany,
                            IDBank = companybankAccount.IDBank,
                            IBAN = companybankAccount.IBAN,
                            Title = companybankAccount.Title,
                            Amount = amount,
                            CostTotal = costTotal,
                            IsEFT = true,
                            Status = 8,
                            IsProcess = true,
                            CurrencyCode = companyCreateWithdrawalRequest.CurrencyCode
                        };
                    }
                    else
                        return Json(new GenericResponse { Status = "ERROR", Message = "Üye işyeri banka bilgilerinde bir hata mevcut." });
                }
                else
                {
                    companyWithdrawalRequest = new CompanyWithdrawalRequest()
                    {
                        CUser = "00000000-0000-0000-0000-000000000000",
                        CDate = DateTime.Now,
                        IDCompany = idCompany,
                        IDBank = "00",
                        IBAN = "-",
                        Title = "-",
                        Amount = amount,
                        CostTotal = costTotal,
                        IsEFT = true,
                        Status = 8,
                        IsProcess = true,
                        CurrencyCode = companyCreateWithdrawalRequest.CurrencyCode
                    };
                }

                var response = _companyWithdrawalRequestManager.Insert(companyWithdrawalRequest);

                if(response.Status == "OK")
                    return Json(new GenericResponse { Status = "OK", Message = "Çekim talebi başarıyla oluşturuldu"});
                else
                    return Json(new GenericResponse { Status = "ERROR", Message = "Çekim talebi başarıyla oluşturuldu" });

            }
            catch (System.Exception ex)
            {
                return Json(new GenericResponse { Status = "ERROR", Message = ex.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveCompanyAutoNotificationSettings(CompanyAutoNotificationSetting companyAutoNotificationSettings)
        {
            try
            {
                var entity = _companyAutoNotificationSettingManager.GetSingleByIDCompany(new List<FieldParameter>()
                {
                    new FieldParameter("IDCompany", Enums.FieldType.NVarChar, companyAutoNotificationSettings.IDCompany)
                });

                if (entity != null)
                    return Json(_companyAutoNotificationSettingManager.Update(companyAutoNotificationSettings));
                else
                    return Json(_companyAutoNotificationSettingManager.Insert(companyAutoNotificationSettings));


            }
            catch (System.Exception ex)
            {
                return Json(new GenericResponse { Status = "ERROR", Message = ex.Message });
            }
        }
    }
}
