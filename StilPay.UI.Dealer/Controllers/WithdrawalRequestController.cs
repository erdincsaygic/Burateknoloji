using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.BLL.Concrete;
using StilPay.DAL.Concrete;
using StilPay.Entities.Concrete;
using StilPay.UI.Dealer.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.IsBankTransferService;
using StilPay.Utility.IsBankTransferService.IsBankTokenModel;
using StilPay.Utility.KuveytTurk;
using StilPay.Utility.KuveytTurk.KuveytTurkTransfer;
using StilPay.Utility.PayNKolay.Models;
using StilPay.Utility.PayNKolay.Models.ComplatePayment;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using ZiraatBankPaymentService;
using static StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPayment.IsBankPaymentRequestModel;
using static StilPay.Utility.IsBankTransferService.Models.IsBankPaymentService.IsBankPaymentValidation.IsBankPaymentValidationRequestModel;

namespace StilPay.UI.Dealer.Controllers
{
    [Authorize(Roles = "WithdrawalRequest")]
    public class WithdrawalRequestController : BaseController<CompanyWithdrawalRequest>
    {
        private readonly ICompanyWithdrawalRequestManager _manager;
        private readonly IBankManager _bankManager;
        private readonly ICompanyBankAccountManager _bankAccountManager;
        private readonly ICompanyManager _companyManager;
        private readonly ICompanyCommissionManager _companyCommissionManager;
        private readonly ICompanyIntegrationManager _companyIntegrationManager;
        private readonly IPublicHolidayManager _publicHolidayManager;
        private readonly ISystemSettingManager _systemSettingManager;
        private readonly ICallbackResponseLogManager _callbackResponseLogManager;
        private readonly ICompanyBankAccountManager _companyBankAccountManager;
        private readonly ICompanyCurrencyManager _companyCurrencyManager;
        private readonly SettingDAL _settingDAL = new SettingDAL();

        public WithdrawalRequestController(ICompanyWithdrawalRequestManager manager, IBankManager bankManager, IPublicHolidayManager publicHolidayManager, ICompanyCommissionManager companyCommissionManager, ICompanyManager companyManager, ICompanyBankAccountManager bankAccountManager, ICompanyIntegrationManager companyIntegrationManager, ISystemSettingManager systemSettingManager,ICallbackResponseLogManager callbackResponseLogManager ,IHttpContextAccessor httpContext, ICompanyBankAccountManager companyBankAccountManager,ICompanyCurrencyManager companyCurrencyManager) : base(httpContext)
        {
            _manager = manager;
            _bankManager = bankManager;
            _bankAccountManager = bankAccountManager;
            _companyManager = companyManager;
            _companyCommissionManager = companyCommissionManager;
            _companyIntegrationManager = companyIntegrationManager;
            _publicHolidayManager = publicHolidayManager;
            _systemSettingManager = systemSettingManager;
            _callbackResponseLogManager = callbackResponseLogManager;
            _companyBankAccountManager = companyBankAccountManager;
            _companyCurrencyManager = companyCurrencyManager;
        }

        public override IBaseBLL<CompanyWithdrawalRequest> Manager()
        {
            return _manager;
        }

        [HttpPost]
        public override IActionResult Gets([FromBody] JObject jObj)
        {
            var list = GetData(
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.All),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, IDCompany),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, Convert.ToDateTime(jObj["StartDate"].ToString())),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, Convert.ToDateTime(jObj["EndDate"].ToString()))
            );

            return Json(list);
        }

        [HttpPost]
        public IActionResult GetData()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = GetData(
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.All),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, IDCompany),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString())),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString())),
                new FieldParameter("PageLenght", Enums.FieldType.Int, length),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue)

            );

            var recordsTotal = list.Count != 0 ? list.FirstOrDefault().TotalRecords : 0;

            var result = new
            {
                recordsFiltered = recordsTotal,
                data = list
            };

            return Json(result);
        }

        public override EditViewModel<CompanyWithdrawalRequest> InitEditViewModel(string id = null)
        {
            var model = new WithdrawalRequestEditViewModel();

            if (!string.IsNullOrEmpty(id))
            {
                var entity = Manager().GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                model.entity = entity;
            }

            model.Banks = _bankManager.GetActiveList(new List<FieldParameter>()
            {
                new FieldParameter("IsAdminPanelRequest", Enums.FieldType.Bit, false)
            });

            model.BankAccounts = _bankAccountManager.GetActiveList(new List<FieldParameter>
            {
                new FieldParameter("IDCompany",Enums.FieldType.NVarChar, IDCompany)
            });

            var companyCommission = _companyCommissionManager.GetSingle(new List<FieldParameter>
            {
                new FieldParameter("ID",Enums.FieldType.NVarChar, IDCompany)
            });

            if (companyCommission != null)
            {
                model.WithdrawalTransferAmount = companyCommission.WithdrawalTransferAmount;
                model.WithdrawalEftAmount = companyCommission.WithdrawalEftAmount;
                model.WithdrawalForeignCurrencySwiftAmount = companyCommission.WithdrawalForeignCurrencySwiftAmount;
            }

            model.CompanyCurrencies = _companyCurrencyManager.GetList(new List<FieldParameter>
            {
                new FieldParameter("IDCompany",Enums.FieldType.NVarChar, IDCompany)
            });

            var balances = _companyManager.GetBalance(IDCompany);
            if (balances != null)
            {
                ViewBag.UsingBalance = balances.UsingBalance;
                ViewBag.BlockedBalance = balances.BlockedBalance;
                ViewBag.TotalBalance = balances.TotalBalance;
                ViewBag.NegativeBalanceLimit = balances.NegativeBalanceLimit;
            }

            return model;
        }

        [HttpPost]
        public JsonResult CheckIbanAndTitle(CompanyWithdrawalRequest companyWithdrawalRequest, string iban, string title)
        {
            var company = _companyManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, companyWithdrawalRequest.IDCompany) });

            if (company != null)
            {
                var check = "TR" + companyWithdrawalRequest.IBAN == company.ProgressPaymentIban && companyWithdrawalRequest.Title == company.ProgressPaymentAccountHolder;

                if (check)
                {
                    return Json(new { match = true });
                }
                return Json(new { match = false });

            }

            return Json(new { match = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override IActionResult Save(CompanyWithdrawalRequest entity)
        {
            var systemSetting = _settingDAL.GetList(null).Where(x => x.ParamType == "EftHours" || x.ParamType == "DealerMaxWithdrawalLimit" || x.ParamType == "MaxFastAmountLimit" || x.ParamType == "DealerMaxSwiftAmountLimit").ToList();

            var maxFastAmountLimit = decimal.Parse(systemSetting.FirstOrDefault(x => x.ParamType == "MaxFastAmountLimit" && x.ParamDef == "max_fast_amount_limit").ParamVal);

            var dealerMaxWithdrawalLimit = decimal.Parse(systemSetting.FirstOrDefault(x => x.ParamType == "DealerMaxWithdrawalLimit" && x.ParamDef == "dealer_max_withdrawal_limit").ParamVal);

            if (entity.Amount <= 0)
                return Json(new GenericResponse { Status = "ERROR", Message = "Lütfen Tutar Giriniz." });

            if (entity.Amount> maxFastAmountLimit && entity.IsEFT == false)
                return Json(new GenericResponse { Status = "ERROR", Message = $"Maximum {maxFastAmountLimit:n2} TL Fast İşlemi Yapabilirsiniz." });

            if (entity.Amount > dealerMaxWithdrawalLimit)
                return Json(new GenericResponse { Status = "ERROR", Message = $"Maximum {dealerMaxWithdrawalLimit:n2} TL İşlem Yapabilirsiniz." });

            var activeBank = _companyBankAccountManager.GetActiveList(new List<FieldParameter>() { new FieldParameter("IDCompany", Enums.FieldType.NVarChar, "1312E00F-E83E-45B4-85C6-892396D12331") }).Where(x => x.IsActiveByDefaultExpenseAccount).FirstOrDefault();

            var useZiraatBank = false;
            var useIsBank = false;
            var useKuveytTurk = false;
            var eftStartTime = "";
            var eftEndTime = "";
            var checkForDefaultIDBank =  "";
            var companyBankAccountID = "";
            var fastControlBank = "";
            var day = DateTime.Now.DayOfWeek;
            var publicHolidayCheck = _publicHolidayManager.GetSingle(new List<FieldParameter>() { new FieldParameter("HolidayDate", Enums.FieldType.DateTime, DateTime.Now.Date) });
            var datetime = DateTime.Now;
            DateTime? eftStartDateTime = null;
            DateTime? eftEndDateTime = null;

            if (systemSetting != null && systemSetting.Count > 0)
            {
                foreach (var item in systemSetting)
                {
                    switch (item.ParamDef)
                    {
                        case "EftStartTime":
                            eftStartTime = item.ParamVal;
                            break;

                        case "EftEndTime":
                            eftEndTime = item.ParamVal;
                            break;
                    }
                }
            }

            if (entity.CurrencyCode != "TRY")
            {
                var getCompanyCurrencies = _companyCurrencyManager.GetList(new List<FieldParameter> { new FieldParameter("IDCompany", Enums.FieldType.NVarChar, IDCompany) });

                var hasAuthority = getCompanyCurrencies.FirstOrDefault(x => x.CurrencyCode == entity.CurrencyCode);

                if (!hasAuthority.CanCreateWithdrawalRequest)
                    return Json(new GenericResponse { Status = "ERROR", Message = $"{entity.CurrencyCode} Para Biriminde Çekim Talebi Oluşturma Yetkiniz Bulunmamaktadır." });


                var dealerMaxSwiftAmountLimit = decimal.Parse(systemSetting.FirstOrDefault(x => x.ParamType == "DealerMaxSwiftAmountLimit" && x.ParamDef == "dealer_max_swift_amount_limit").ParamVal);

                if (entity.Amount > dealerMaxSwiftAmountLimit)
                    return Json(new GenericResponse { Status = "ERROR", Message = $"Maximum {dealerMaxSwiftAmountLimit:n2} Döviz Birimi İşlemi Yapabilirsiniz." });

                entity.IBAN = "TR" + entity.IBAN;
                entity.Bank = _bankManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, entity.IDBank) }).Name;
                entity.Title = string.Join(" ", entity.Title.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)).Trim();

                if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
                    return Json(new GenericResponse { Status = "ERROR", Message = "Haftasonu Sadece Swift İşlemi Yapılamamaktadır." });

                if (publicHolidayCheck != null)
                    return Json(new GenericResponse { Status = "ERROR", Message = "Resmi Tatil Günlerinde Swift İşlemi Yapılamamaktadır." });

                eftStartDateTime = new DateTime(datetime.Year, datetime.Month, datetime.Day, int.Parse(eftStartTime[..2]), int.Parse(eftStartTime[3..]), 0);
                eftEndDateTime = new DateTime(datetime.Year, datetime.Month, datetime.Day, int.Parse(eftEndTime[..2]), int.Parse(eftEndTime[3..]), 0);

                if (datetime < eftStartDateTime || datetime > eftEndDateTime)
                    return Json(new GenericResponse { Status = "ERROR", Message = "Mesai Saatleri Dışında Swift İşlemi Yapılamamaktadır." });

                entity.Status = (byte)Enums.StatusType.Process;
                entity.IsProcess = true;

                return Json(Manager().Insert(entity));
            }

            if (activeBank != null)
            {
                switch (activeBank.IDBank)
                {
                    case "08":
                        useZiraatBank = true;
                        companyBankAccountID = activeBank.ID;
                        checkForDefaultIDBank = "08";
                        break;

                    case "03":
                        useIsBank = true;
                        companyBankAccountID = activeBank.ID;
                        checkForDefaultIDBank = "03";
                        break;

                    case "07":
                        useKuveytTurk = true;
                        companyBankAccountID = activeBank.ID;
                        checkForDefaultIDBank = "07";
                        break;

                    case "36":
                        useKuveytTurk = true;
                        companyBankAccountID = activeBank.ID;
                        checkForDefaultIDBank = "36";
                        break;

                }
            }

            if (entity.IDBank == "07" && checkForDefaultIDBank == "36")
                fastControlBank = "36";
            else
                fastControlBank = entity.IDBank;

            if ((day == DayOfWeek.Saturday || day == DayOfWeek.Sunday) && entity.IsEFT == true && fastControlBank != checkForDefaultIDBank)
                return Json(new GenericResponse { Status = "ERROR", Message = "Haftasonu Sadece FAST İşlemi Yapılmaktadır." });

            if (publicHolidayCheck != null && entity.IsEFT == true && fastControlBank != checkForDefaultIDBank)
                return Json(new GenericResponse { Status = "ERROR", Message = "Resmi Tatil Günlerinde Sadece FAST İşlemi Yapılmaktadır." });

             eftStartDateTime = new DateTime(datetime.Year, datetime.Month, datetime.Day, int.Parse(eftStartTime[..2]), int.Parse(eftStartTime[3..]), 0);
             eftEndDateTime = new DateTime(datetime.Year, datetime.Month, datetime.Day, int.Parse(eftEndTime[..2]), int.Parse(eftEndTime[3..]), 0);

            if ((datetime < eftStartDateTime || datetime > eftEndDateTime) && entity.IsEFT == true && fastControlBank != checkForDefaultIDBank)
                return Json(new GenericResponse { Status = "ERROR", Message = "Mesai Saatleri Dışında Sadece FAST İşlemi Yapılmaktadır. Lütfen Fast İşlemini Seçiniz." });

            entity.IBAN = "TR" + entity.IBAN;
            entity.Bank = _bankManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, entity.IDBank) }).Name;

            var company = _companyManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, entity.IDCompany) });

            var balances = _companyManager.GetBalance(entity.IDCompany);

            if (balances == null || balances.UsingBalance + balances.NegativeBalanceLimit < entity.Amount)
                return Json(new GenericResponse { Status = "ERROR", Message = "Mevcut bakiyeniz talep tutarı için yetersiz." });

            entity.DealerDescription = string.IsNullOrWhiteSpace(entity.DealerDescription) ? null : entity.DealerDescription.Trim();
            entity.Title = string.Join(" ", entity.Title.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)).Trim(); 
            entity.Status = (byte)Enums.StatusType.Pending;
            var saveEntity = Manager().Insert(entity);  

            if(saveEntity.Status == "OK")
            {
                var savedEntity = Manager().GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, saveEntity.Data) });
                var callbackEntity = new CallbackResponseLog();
                var opt = new JsonSerializerOptions() { WriteIndented = true };

                if (company.AutoWithdrawalLimit >= savedEntity.Amount)
                {
                    if (useZiraatBank)
                    {
                        var ziraatService = new NkyParaTransferiWSSoapClient(NkyParaTransferiWSSoapClient.EndpointConfiguration.NkyParaTransferiWSSoap);
                        var securedWebServiceHeader = new SecuredWebServiceHeader();

                        if (savedEntity.IDBank == "08")
                        {
                            var havaleResponse = ziraatService.HavaleYapAsync(securedWebServiceHeader, "97736040", "5002", "", "", savedEntity.IBAN.Replace(" ", ""), "TRY", savedEntity.Amount.ToString(), string.IsNullOrWhiteSpace(savedEntity.DealerDescription) ? $"{company.Title} Çekim Talebi" : savedEntity.DealerDescription, savedEntity.TransactionNr, "", "", "", "", "", "", "", "", "", "", "").Result;

                            callbackEntity.TransactionID = savedEntity.TransactionNr;
                            callbackEntity.ServiceType = "Ziraat Bankası Havale";
                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(havaleResponse.HavaleYapResult, opt);
                            callbackEntity.IDCompany = company.ID;
                            callbackEntity.TransactionType = "Havale";
                            _callbackResponseLogManager.Insert(callbackEntity);

                            if (havaleResponse.HavaleYapResult.CevapKodu != "0")
                                return Json(new GenericResponse { Status = "ERROR", Message = havaleResponse.HavaleYapResult.CevapMesaji });

                            else
                            {
                                savedEntity.Status = (byte)Enums.StatusType.Confirmed;
                                savedEntity.Description = "Çekim Limiti Aşılmadığı İçin Otomatik Onaylandı";
                                savedEntity.MDate = DateTime.Now;
                                savedEntity.MUser = "00000000-0000-0000-0000-000000000000";
                                savedEntity.SIDBank = "08";
                                savedEntity.CompanyBankAccountID = companyBankAccountID;
                                return Json(_manager.SetStatus(savedEntity));
                            }
                        }

                        else
                        {   
                            var eftResponse = ziraatService.EftYapAsync(securedWebServiceHeader, "97736040", "5002", savedEntity.Bank, "", "", savedEntity.IBAN.Replace(" ", ""), savedEntity.Title, DateTime.Now.ToString("yyyyMMdd"), savedEntity.Amount.ToString(), savedEntity.TransactionNr, string.IsNullOrWhiteSpace(savedEntity.DealerDescription) ? $"{company.Title} Çekim Talebi" : savedEntity.DealerDescription, "", "", "", "", "", "", "", "", "", "").Result;

                            callbackEntity.TransactionID = savedEntity.TransactionNr;
                            callbackEntity.ServiceType = "Ziraat Bankası EFT";
                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(eftResponse.EftYapResult, opt);
                            callbackEntity.IDCompany = company.ID;
                            callbackEntity.TransactionType = "EFT";
                            _callbackResponseLogManager.Insert(callbackEntity);

                            if (eftResponse.EftYapResult.CevapKodu != "0")
                            {
                                return Json(new GenericResponse { Status = "ERROR", Message = eftResponse.EftYapResult.CevapMesaji });
                            }

                            else
                            {
                                savedEntity.Status = (byte)Enums.StatusType.Confirmed;
                                savedEntity.Description = "Çekim Limiti Aşılmadığı İçin Otomatik Onaylandı";
                                savedEntity.MDate = DateTime.Now;
                                savedEntity.MUser = "00000000-0000-0000-0000-000000000000";
                                savedEntity.SIDBank = "08";
                                savedEntity.CompanyBankAccountID = companyBankAccountID;
                                return Json(_manager.SetStatus(savedEntity));
                            }
                        }
                    }

                    if(useIsBank)
                    {
                        var isBankIntegrationValues = _settingDAL.GetList(null).Where(x => x.ParamType == "IsBankTransfer").ToList();

                        if (savedEntity.IDBank == "03")
                        {
                            var isBankRemittancePaymentValidationRequest = new IsBankPaymentValidationRequest
                            {
                                apiUrl = "/api/isbank/v1/remittance-validations",
                                debtor_account_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "debtor_account_id").ParamVal,
                                isbank_client_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_id").ParamVal,
                                isbank_client_secret = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_secret").ParamVal,
                                isbank_client_certificate = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_certificate").ParamVal,
                                authorization = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_auth").ParamVal,
                                username = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_username").ParamVal,
                                password = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_password").ParamVal,
                                amount = savedEntity.Amount,
                                creditor_iban = savedEntity.IBAN.Replace(" ", ""),
                                description = string.IsNullOrWhiteSpace(savedEntity.DealerDescription) ? $"{company.Title} Çekim Talebi" : savedEntity.DealerDescription,
                                payment_reference_id = savedEntity.RequestNr,
                                customer_ip = "89.252.138.236",
                            };

                            var isBankRemittancePaymentValidationResponse = IsBankPaymentValidation.IsBankPaymentValidationRequest(isBankRemittancePaymentValidationRequest);

                            callbackEntity.TransactionID = savedEntity.TransactionNr;
                            callbackEntity.ServiceType = "İş Bankası Payment Validation";
                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankRemittancePaymentValidationResponse, opt);
                            callbackEntity.IDCompany = company.ID;
                            callbackEntity.TransactionType = "HAVALE PARA GÖNDERİMİ ÖN ONAY SONUC";
                            _callbackResponseLogManager.Insert(callbackEntity);

                            if (isBankRemittancePaymentValidationResponse.Status == "OK")
                            {
                                var isBankRemittancePaymentRequest = new IsBankPaymentRequest
                                {
                                    apiUrl = "/api/isbank/v1/remittance-payments",
                                    debtor_account_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "debtor_account_id").ParamVal,
                                    isbank_client_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_id").ParamVal,
                                    isbank_client_secret = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_secret").ParamVal,
                                    isbank_client_certificate = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_certificate").ParamVal,
                                    authorization = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_auth").ParamVal,
                                    username = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_username").ParamVal,
                                    password = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_password").ParamVal,
                                    amount = savedEntity.Amount,
                                    creditor_iban = savedEntity.IBAN.Replace(" ", ""),
                                    description = string.IsNullOrWhiteSpace(savedEntity.DealerDescription) ? $"{company.Title} Çekim Talebi" : savedEntity.DealerDescription,
                                    payment_reference_id = savedEntity.RequestNr,
                                    query_number = isBankRemittancePaymentValidationResponse.Data.data.query_number,
                                    customer_ip = "89.252.138.236",
                                    idempotency_key = savedEntity.ID,
                                };

                                var isBankRemittancePaymentRequestResponse = IsBankPayment.IsBankPaymentRequest(isBankRemittancePaymentRequest);

                                callbackEntity.TransactionID = savedEntity.TransactionNr;
                                callbackEntity.ServiceType = "İş Bankası Payment Response";
                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankRemittancePaymentRequestResponse, opt);
                                callbackEntity.IDCompany = company.ID;
                                callbackEntity.TransactionType = "HAVALE PARA GÖNDERİMİ SONUC";
                                _callbackResponseLogManager.Insert(callbackEntity);

                                if (isBankRemittancePaymentRequestResponse.Status == "OK")
                                {
                                    savedEntity.Status = (byte)Enums.StatusType.Confirmed;
                                    savedEntity.Description = "Çekim Limiti Aşılmadığı İçin Otomatik Onaylandı";
                                    savedEntity.MDate = DateTime.Now;
                                    savedEntity.MUser = "00000000-0000-0000-0000-000000000000";
                                    savedEntity.IsBankQueryNr = isBankRemittancePaymentRequestResponse.Data.data.query_number;
                                    savedEntity.SIDBank = "03";
                                    savedEntity.CompanyBankAccountID = companyBankAccountID;
                                    return Json(_manager.SetStatus(savedEntity));
                                }
                                else
                                    return Json(isBankRemittancePaymentRequestResponse);
                            }
                            else
                                return Json(isBankRemittancePaymentValidationResponse);
                        }
                        else
                        {

                            var isBankEftPaymentValidationRequest = new IsBankPaymentValidationRequest
                            {
                                apiUrl = "/api/isbank/v1/eft-validations",
                                debtor_account_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "debtor_account_id").ParamVal,
                                isbank_client_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_id").ParamVal,
                                isbank_client_secret = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_secret").ParamVal,
                                isbank_client_certificate = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_certificate").ParamVal,
                                authorization = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_auth").ParamVal,
                                username = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_username").ParamVal,
                                password = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_password").ParamVal,
                                amount = savedEntity.Amount,
                                creditor_name = savedEntity.Title,
                                creditor_iban = savedEntity.IBAN.Replace(" ", ""),
                                description = string.IsNullOrWhiteSpace(savedEntity.DealerDescription) ? $"{company.Title} Çekim Talebi" : savedEntity.DealerDescription,
                                payment_reference_id = savedEntity.RequestNr,
                                customer_ip = "89.252.138.236",
                            };

                            var isBankEftPaymentValidationRequestResponse = IsBankPaymentValidation.IsBankPaymentValidationRequest(isBankEftPaymentValidationRequest);

                            callbackEntity.TransactionID = savedEntity.TransactionNr;
                            callbackEntity.ServiceType = "İş Bankası Payment Validation";
                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankEftPaymentValidationRequestResponse, opt);
                            callbackEntity.IDCompany = company.ID;
                            callbackEntity.TransactionType = "EFT PARA GÖNDERİMİ ÖN ONAY SONUC";
                            _callbackResponseLogManager.Insert(callbackEntity);

                            if (isBankEftPaymentValidationRequestResponse.Status == "OK")
                            {
                                var isBankEftPaymentRequest = new IsBankPaymentRequest
                                {
                                    apiUrl = "/api/isbank/v1/eft-payments",
                                    debtor_account_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "debtor_account_id").ParamVal,
                                    isbank_client_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_id").ParamVal,
                                    isbank_client_secret = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_secret").ParamVal,
                                    isbank_client_certificate = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_certificate").ParamVal,
                                    authorization = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_auth").ParamVal,
                                    username = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_username").ParamVal,
                                    password = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_password").ParamVal,
                                    amount = savedEntity.Amount,
                                    creditor_name = savedEntity.Title,
                                    creditor_iban = savedEntity.IBAN.Replace(" ", ""),
                                    description = string.IsNullOrWhiteSpace(savedEntity.DealerDescription) ? $"{company.Title} Çekim Talebi" : savedEntity.DealerDescription,
                                    payment_reference_id = savedEntity.RequestNr,
                                    idempotency_key = savedEntity.ID,
                                    query_number = isBankEftPaymentValidationRequestResponse.Data.data.query_number,
                                    customer_ip = "89.252.138.236",
                                };

                                var isBankEftPaymentRequestResponse = IsBankPayment.IsBankPaymentRequest(isBankEftPaymentRequest);

                                callbackEntity.TransactionID = savedEntity.TransactionNr;
                                callbackEntity.ServiceType = "İş Bankası Payment Response";
                                callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankEftPaymentRequestResponse, opt);
                                callbackEntity.IDCompany = company.ID;
                                callbackEntity.TransactionType = "EFT PARA GÖNDERİMİ SONUC";
                                _callbackResponseLogManager.Insert(callbackEntity);

                                if (isBankEftPaymentRequestResponse.Status == "OK")
                                {
                                    savedEntity.Status = (byte)Enums.StatusType.Confirmed;
                                    savedEntity.Description = "Çekim Limiti Aşılmadığı İçin Otomatik Onaylandı";
                                    savedEntity.MDate = DateTime.Now;
                                    savedEntity.MUser = "00000000-0000-0000-0000-000000000000";
                                    savedEntity.IsBankQueryNr = isBankEftPaymentRequestResponse.Data.data.query_number;
                                    savedEntity.SIDBank = "03";
                                    savedEntity.CompanyBankAccountID = companyBankAccountID;
                                    return Json(_manager.SetStatus(savedEntity));
                                }
                                else
                                    return Json(isBankEftPaymentRequestResponse);
                            }

                            else
                            {
                                if (isBankEftPaymentValidationRequestResponse.Data != null && isBankEftPaymentValidationRequestResponse.Data.infos != null &&
                                   (isBankEftPaymentValidationRequestResponse.Data.infos.FirstOrDefault().code == "MRD110" || isBankEftPaymentValidationRequestResponse.Data.infos.FirstOrDefault().code == "MRD132"))
                                {
                                    var isBankFastPaymentValidationRequest = new IsBankPaymentValidationRequest
                                    {
                                        apiUrl = "/api/isbank/v1/fast-validations",
                                        debtor_account_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "debtor_account_id").ParamVal,
                                        isbank_client_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_id").ParamVal,
                                        isbank_client_secret = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_secret").ParamVal,
                                        isbank_client_certificate = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_certificate").ParamVal,
                                        authorization = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_auth").ParamVal,
                                        username = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_username").ParamVal,
                                        password = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_password").ParamVal,
                                        amount = savedEntity.Amount,
                                        creditor_name = savedEntity.Title,
                                        creditor_iban = savedEntity.IBAN.Replace(" ", ""),
                                        description = string.IsNullOrWhiteSpace(savedEntity.DealerDescription) ? $"{company.Title} Çekim Talebi" : savedEntity.DealerDescription,
                                        payment_reference_id = savedEntity.RequestNr,
                                        customer_ip = "89.252.138.236",
                                    };

                                    var isBankFastPaymentValidationRequestResponse = IsBankPaymentValidation.IsBankPaymentValidationRequest(isBankFastPaymentValidationRequest);

                                    callbackEntity.TransactionID = savedEntity.TransactionNr;
                                    callbackEntity.ServiceType = "İş Bankası Payment Validation";
                                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankFastPaymentValidationRequestResponse, opt);
                                    callbackEntity.IDCompany = company.ID;
                                    callbackEntity.TransactionType = "FAST PARA GÖNDERİMİ ÖN ONAY SONUC";
                                    _callbackResponseLogManager.Insert(callbackEntity);

                                    if (isBankFastPaymentValidationRequestResponse.Status == "OK")
                                    {
                                        var isBankFastPaymentRequest = new IsBankPaymentRequest
                                        {
                                            apiUrl = "/api/isbank/v1/fast-payments",
                                            debtor_account_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "debtor_account_id").ParamVal,
                                            isbank_client_id = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_id").ParamVal,
                                            isbank_client_secret = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_secret").ParamVal,
                                            isbank_client_certificate = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_client_certificate").ParamVal,
                                            authorization = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_auth").ParamVal,
                                            username = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_username").ParamVal,
                                            password = isBankIntegrationValues.FirstOrDefault(f => f.ParamDef == "isbank_token_password").ParamVal,
                                            amount = savedEntity.Amount,
                                            creditor_name = savedEntity.Title,
                                            creditor_iban = savedEntity.IBAN.Replace(" ", ""),
                                            description = string.IsNullOrWhiteSpace(savedEntity.DealerDescription) ? $"{company.Title} Çekim Talebi" : savedEntity.DealerDescription,
                                            payment_reference_id = savedEntity.RequestNr,
                                            idempotency_key = savedEntity.ID,
                                            query_number = isBankFastPaymentValidationRequestResponse.Data.data.query_number,
                                            customer_ip = "89.252.138.236",
                                        };

                                        var isBankFastPaymentRequestResponse = IsBankPayment.IsBankPaymentRequest(isBankFastPaymentRequest);

                                        callbackEntity.TransactionID = savedEntity.TransactionNr;
                                        callbackEntity.ServiceType = "İş Bankası Payment Response";
                                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(isBankFastPaymentRequestResponse, opt);
                                        callbackEntity.IDCompany = company.ID;
                                        callbackEntity.TransactionType = "FAST PARA GÖNDERİMİ SONUC";
                                        _callbackResponseLogManager.Insert(callbackEntity);

                                        if (isBankFastPaymentRequestResponse.Status == "OK")
                                        {
                                            savedEntity.Status = (byte)Enums.StatusType.Confirmed;
                                            savedEntity.Description = "Çekim Limiti Aşılmadığı İçin Otomatik Onaylandı";
                                            savedEntity.MDate = DateTime.Now;
                                            savedEntity.MUser = "00000000-0000-0000-0000-000000000000";
                                            savedEntity.IsBankQueryNr = isBankFastPaymentRequestResponse.Data.data.query_number;
                                            savedEntity.SIDBank = "03";
                                            savedEntity.CompanyBankAccountID = companyBankAccountID;
                                            return Json(_manager.SetStatus(savedEntity));
                                        }
                                        else
                                            return Json(isBankFastPaymentRequestResponse);
                                    }
                                    else
                                        return Json(isBankFastPaymentValidationRequestResponse);
                                }
                                else
                                    return Json(isBankEftPaymentValidationRequestResponse);
                            }
                        }
                    }

                    if (useKuveytTurk)
                    {
                        var kuveyTurkIntegrationValues = _settingDAL.GetList(null).Where(x => x.ParamType == "KuveytTurkTransfer").ToList();

                        var kuveytTurkTransferRequestModel = new KuveytTurkTransferRequestModel()
                        {
                            CorporateWebUserName = kuveyTurkIntegrationValues.FirstOrDefault(f => f.ParamDef == "web_username").ParamVal,
                            SenderAccountSuffix = int.Parse(kuveyTurkIntegrationValues.FirstOrDefault(f => f.ParamDef == "account_suffix").ParamVal),
                            TransferType = int.Parse(kuveyTurkIntegrationValues.FirstOrDefault(f => f.ParamDef == "transfer_type").ParamVal),
                            MoneyTransferAmount = savedEntity.Amount,
                            ReceiverIBAN = savedEntity.IBAN.Replace(" ", ""),
                            ReceiverName = savedEntity.Title,
                            MoneyTransferDescription = string.IsNullOrWhiteSpace(savedEntity.DealerDescription) ? $"{savedEntity.RequestNr} - {company.Title} Çekim Talebi" : savedEntity.RequestNr + " / " + savedEntity.DealerDescription,
                            TransactionGuid = savedEntity.RequestNr
                        };

                        var kuveytTurkTransferResponse = KuveytTurkMoneyTransfer.MoneyTranfer(kuveytTurkTransferRequestModel);

                        callbackEntity.TransactionID = savedEntity.RequestNr;
                        callbackEntity.ServiceType = "Kuveyt Türk Bankası Payment Response";
                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(kuveytTurkTransferResponse, opt);
                        callbackEntity.IDCompany = company.ID;
                        callbackEntity.TransactionType = "PARA GÖNDERİMİ SONUC";
                        _callbackResponseLogManager.Insert(callbackEntity);

                        savedEntity.Status = (byte)Enums.StatusType.Process;
                        savedEntity.Description = "İşleme Alındı";
                        savedEntity.MDate = DateTime.Now;
                        savedEntity.MUser = "00000000-0000-0000-0000-000000000000";
                        savedEntity.IsBankQueryNr = "-";
                        savedEntity.SIDBank = checkForDefaultIDBank;
                        savedEntity.CompanyBankAccountID = companyBankAccountID;
                        savedEntity.IsProcess = true;
                        var response = _manager.SetStatus(savedEntity);

                        return Json(new GenericResponse { Status = "OK" });

                        //if (kuveytTurkTransferResponse.Status == "OK")
                        //{
                        //    savedEntity.Status = (byte)Enums.StatusType.Confirmed;
                        //    savedEntity.Description = "Çekim Limiti Aşılmadığı İçin Otomatik Onaylandı";
                        //    savedEntity.MDate = DateTime.Now;
                        //    savedEntity.MUser = "00000000-0000-0000-0000-000000000000";
                        //    savedEntity.IsBankQueryNr = kuveytTurkTransferResponse.Data.value.moneyTransferTransactionId.ToString();
                        //    savedEntity.SIDBank = "07";
                        //    savedEntity.CompanyBankAccountID = companyBankAccountID;
                        //    return Json(_manager.SetStatus(savedEntity));
                        //}
                        //else
                        //    return Json(kuveytTurkTransferResponse);
                    }
                    else
                        return Json(new GenericResponse { Status = "ERROR", Message = "Şu anda İşlem Gerçekleştirilemiyor. Lütfen Bizimle İletişime Geçin." });
                }
                else
                    return Json(saveEntity);
            }
            else
                return Json(new GenericResponse { Status = "ERROR", Message = saveEntity.Message });

        }
    }
}
