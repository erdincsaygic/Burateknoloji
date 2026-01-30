using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.BLL.Concrete;
using StilPay.DAL.Concrete;
using StilPay.Entities;
using StilPay.Entities.Concrete;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.Models;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static StilPay.UI.Admin.Models.GarantiAccountInfoModel;
using static StilPay.Utility.Helper.Enums;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Dealer")]
    public class DealerTransferPayPoolController : BaseController<PaymentTransferPool>
    {
        private readonly IPaymentTransferPoolManager _manager;
        private readonly IPaymentNotificationManager _paymentNotificationManager;
        private readonly ICompanyIntegrationManager _companyIntegrationManager;
        private readonly ICallbackResponseLogManager _callbackResponseLogManager;
        private readonly ICompanyBankAccountManager _companyBankAccountManager;
        private readonly ICompanyManager _companyManager;
        private readonly IAdministratorManager _administratorManager;
        private readonly ICompanyTransactionManager _companyTransactionManager;
        private readonly IPaymentTransferPoolDescriptionControlManager _paymentTransferPoolDescriptionControlManager;
        private readonly SettingDAL _settingDAL = new SettingDAL();
        public DealerTransferPayPoolController(IPaymentTransferPoolManager manager, IPaymentNotificationManager paymentNotificationManager,
            ICompanyIntegrationManager companyIntegrationManager, ICallbackResponseLogManager callbackResponseLogManager, IHttpContextAccessor httpContext, ICompanyBankAccountManager companyBankAccountManager, ICompanyManager companyManager, IAdministratorManager administratorManager, IPaymentTransferPoolDescriptionControlManager paymentTransferPoolDescriptionControlManager, ICompanyTransactionManager companyTransactionManager) : base(httpContext)
        {
            _manager = manager;
            _paymentNotificationManager = paymentNotificationManager;
            _companyIntegrationManager = companyIntegrationManager;
            _callbackResponseLogManager = callbackResponseLogManager;
            _companyBankAccountManager = companyBankAccountManager;
            _companyManager = companyManager;
            _administratorManager = administratorManager;
            _companyTransactionManager = companyTransactionManager;
            _paymentTransferPoolDescriptionControlManager = paymentTransferPoolDescriptionControlManager;
        }

        public override IBaseBLL<PaymentTransferPool> Manager()
        {
            return _manager;
        }

        public override IActionResult Index()
        {
            ViewBag.Companies = _companyManager.GetActiveList(null);
            ViewBag.CompanyBankAccounts = _companyBankAccountManager.GetActiveList(new List<FieldParameter>() { new FieldParameter("IDCompany", Enums.FieldType.NVarChar, "1312E00F-E83E-45B4-85C6-892396D12331") }).Where(x => !x.IsExitAccount).ToList();

            return View();
        }


        [HttpPost]
        public IActionResult GetData()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var startDate = Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString());
            var endDate = Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString());

            var startDateTime = Convert.ToDateTime(HttpContext.Request.Form["StartDateTime"].ToString());
            var endDateTime = Convert.ToDateTime(HttpContext.Request.Form["EndDateTime"].ToString());

            if (startDateTime.Hour == 0 || startDateTime.Minute == 0 || startDateTime.Second == 0)
            {
                startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDateTime.Hour, startDateTime.Minute, startDateTime.Second);
            }
            if (endDateTime.Hour == 0 && endDateTime.Minute == 0 && endDate.Second == 0)
            {
                endDate = endDate.AddDays(1);
            }
            else
            {
                endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, endDateTime.Hour, endDateTime.Minute, endDateTime.Second);
            }

            var list = GetData(
                new FieldParameter("Status", Enums.FieldType.Tinyint, string.IsNullOrEmpty(HttpContext.Request.Form["Status"].ToString()) ? (byte?)null : Convert.ToByte(HttpContext.Request.Form["Status"])),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, startDate),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, endDate),
                new FieldParameter("PageLength", Enums.FieldType.Int, length),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue),
                new FieldParameter("IDBank", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["IDBank"].ToString()) ? null : HttpContext.Request.Form["IDBank"].ToString()),
                new FieldParameter("IsHaveReferenceNr", Enums.FieldType.Bit, string.IsNullOrEmpty(HttpContext.Request.Form["IsHaveReferenceNr"].ToString()) ? (byte?)null : Convert.ToByte(HttpContext.Request.Form["IsHaveReferenceNr"].ToString())),
                new FieldParameter("ResponseStatus", Enums.FieldType.Int, string.IsNullOrEmpty(HttpContext.Request.Form["ResponseStatus"].ToString()) ? (int?)null : Convert.ToInt32(HttpContext.Request.Form["ResponseStatus"].ToString())),
               new FieldParameter("IsCaughtInFraudControl", Enums.FieldType.Bit, string.IsNullOrEmpty(HttpContext.Request.Form["IsCaughtInFraudControl"].ToString()) ? (byte?)null : Convert.ToByte(HttpContext.Request.Form["IsCaughtInFraudControl"].ToString()))
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
        public IActionResult PairingProcess(string pyID)
        {   
            ViewBag.PyID = pyID;
            return View("PairingProcess");
        }

        [HttpPost]
        public IActionResult PairingProcessData()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = GetData(
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.Pending),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("PageLength", Enums.FieldType.Int, length),
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Pairing(string pyID, string tpID)
        {
            var responseStatus = 0;
            var py = _paymentNotificationManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, pyID) });
            var tp = _manager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, tpID) });

            if((!py.IsCaughtInFraudControl && !string.IsNullOrEmpty(py.TransactionKey)) || py.Status != (byte)Enums.StatusType.Pending)
                return Json(new GenericResponse { Status = "ERROR", Message = "Eşleştirmek İstediğiniz Bildirim İçin Daha Önce İşlem Yapılmış." });

            if (!string.IsNullOrEmpty(tp.ResponseTransactionNr) || tp.Status != (byte)Enums.StatusType.Pending)
                return Json(new GenericResponse { Status = "ERROR", Message = "Eşleştirmek İstediğiniz Havuzdaki İşlem İçin Daha Önce İşlem Yapılmış." });

            if (py.Amount != tp.Amount)
                return Json(new GenericResponse { Status = "ERROR", Message = "Eşleştirmek İstediğiniz Bildirim ile Seçilen Havuzdaki İşlem Tutarı Uymamaktadır." });

            if(string.IsNullOrEmpty(tp.SenderName) || string.IsNullOrWhiteSpace(tp.SenderName))
            {
                var culture = new System.Globalization.CultureInfo("tr-TR");

                string descriptionUpper = tp.Description.ToUpper(culture);
                string searchKeyUpper = py.SenderName.Trim().ToUpper(culture);

                if (!descriptionUpper.Contains(searchKeyUpper))
                {
                    return Json(new GenericResponse { Status = "ERROR", Message = "Eşleştirmek İstediğiniz Bildirimdeki Gönderici Adı Havuzdaki İşlemin Açıklama Kısmında Bulunamadı." });
                }
            }
            else
            {
                if (py.SenderName[..1].ToLower() != tp.SenderName[..1].ToLower())
                    return Json(new GenericResponse { Status = "ERROR", Message = "Eşleştirmek İstediğiniz Bildirim ile Seçilen Havuzdaki İşlemin Gönderici Adının İlk Harfi Uyuşmamaktadır." });
            }

            var companyIntegration = _companyIntegrationManager.GetByServiceId(py.ServiceID);
            
            if (!tSQLBankManager.HasNotificationTransaction(tp.TransactionKey) && (py.IsCaughtInFraudControl || !tSQLBankManager.HasPaymentTransaction(py.TransactionKey)))
            {
                var transactionNr = tSQLBankManager.AddNotificationTransaction(Convert.ToDateTime(string.Concat(Convert.ToDateTime(py.ActionDate).ToString("dd.MM.yyyy"), " ", py.ActionTime.ToString())), DateTime.Now, Convert.ToDateTime(tp.TransactionDate), tp.IDBank, py.ServiceID, py.TransactionID, tp.TransactionKey, Convert.ToDecimal(tp.Amount, CultureInfo.InvariantCulture), tp.Description, py.IDMember, py.SenderName, py.SenderIdentityNr, false, true);


                if (!string.IsNullOrEmpty(transactionNr))
                {
                    py.Status = (byte)Enums.StatusType.Confirmed;
                    py.MDate = DateTime.Now;
                    py.MUser = IDUser;
                    py.Description = "İşlem Başarılı";
                    var responsePy = _paymentNotificationManager.SetStatus(py);

                    if (responsePy.Status == "OK")
                    {
                        var dataCallback = new
                        {
                            status_code = "OK",
                            service_id = py.ServiceID,
                            status_type = 0,
                            ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                            data = new { transaction_id = py.TransactionID, sp_transactionNr = py.TransactionNr, amount = py.Amount, sp_id = py.ID },
                            user_entered_data = new { member = py.Member, sender_name = py.SenderName, action_date = py.ActionDate, action_time = py.ActionTime, amount = py.Amount, user_ip = py.MemberIPAddress, user_port = py.MemberPort,
                                bank_description = tp.Description
                            }
                        };
                      
                        var response = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(companyIntegration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                        if (response != null && response.Result != null && response.Result.Status == "OK")
                        {
                            tSQLBankManager.AcceptNotificationTransaction(transactionNr);
                            responseStatus = 1;
                        }

                        tSQLBankManager.SetPaymentNotificationIbanAndBank(py.ID, tp.SenderIban, tp.IDBank, tp.TransactionKey);
                        tSQLBankManager.SetPaymentTransferPool(tp.ID, (int)StatusType.Confirmed, py.TransactionNr, py.TransactionID, "İşlem Başarılı");

                        var opt = new JsonSerializerOptions() { WriteIndented = true };
                        tSQLBankManager.AddCallbackResponseLog(py.TransactionID, "STILPAY", System.Text.Json.JsonSerializer.Serialize(dataCallback, opt), companyIntegration.ID, "Ödeme Bildirimi", responseStatus);

                        return Json(new GenericResponse { Status = "OK", Message = "Eşleştirme İşlemi Başarılı" });
                    }

                    return Json(new GenericResponse { Status = "ERROR", Message = "Update Hatası(Payment Notifications)! Lütfen Yetkilinize Başvurun." });
                }

                return Json(new GenericResponse { Status = "ERROR", Message = "Insert Hatası(Notification Transactions)! Lütfen Yetkilinize Başvurun." });
            }

            return Json(new GenericResponse { Status = "ERROR", Message = "Hata! Daha Önce Onaylanmış Bildirim" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RebateUnmatched(string bankId, string tpID)
        {
            return Json(_manager.SetStatus(tpID, (byte)Enums.StatusType.Canceled));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetStatusNotPairing(string tpID)
        {
            return Json(_manager.SetStatus(tpID, (byte)Enums.StatusType.Pending));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PairingWithDealer(string tpID, string dealerId)
        {
            var entity = _manager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", FieldType.NVarChar, tpID) });
            var py = _paymentNotificationManager.GetSingleByTransactionKey(entity.TransactionKey);
            var companyIntegration = _companyIntegrationManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", FieldType.NVarChar, dealerId) });
            var admin = _administratorManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", FieldType.NVarChar, IDUser) });

            var companyTransaction = _companyTransactionManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", FieldType.NVarChar, entity.ID) });

            if(companyTransaction != null && companyTransaction.IDActionType == ((int)ActionType.TransferPoolManualMatching).ToString())
                return Json(new GenericResponse() { Status = "ERROR", Message = "İşlem Üye İşyerinde Hareketlerinde Kayıtlı" });

            if (py != null)
            {
                tSQLBankManager.SetPaymentTransferPool(entity.ID, (byte)StatusType.Confirmed, py.TransactionNr, py.TransactionID, $"{admin.Name} Tarafından Manuel Eşleştirildi.");

                return Json(new GenericResponse() { Status = "OK", Message = "İşlem Başarılı" });
            }
            else
            {
                var balances = _companyManager.GetBalance(dealerId);
                if (balances != null)
                {
                    var res = _companyManager.SetBalance(dealerId, balances.UsingBalance + entity.Amount, ((int)ActionType.TransferPoolManualMatching).ToString(), entity.ID);

                    if (res.Status == "OK")
                    {
                        tSQLBankManager.SetPaymentTransferPool(tpID, (byte)StatusType.Confirmed, companyIntegration.ServiceID,"", $"{admin.Name} Tarafından Manuel Eşleştirildi.");

                        return Json(new GenericResponse() { Status = "OK", Message = "İşlem Başarılı" });
                    }
                    else
                        return Json(new GenericResponse() { Status = "ERROR", Message = "Üye İşyeri Bakiyesi Gümcellenemedi" });
                }
                else
                    return Json(new GenericResponse() { Status = "ERROR", Message = "Üye İşyeri Bakiyesi Çekilemedi" });
            }
        }


        [HttpPost]
        public IActionResult SearchInBank([FromBody] JObject jObj)
        {
            var garantiBankSettings = _settingDAL.GetList(null).Where(x => x.ParamType == "GarantiBank").ToList();
            var garantiBankSettingsSP = _settingDAL.GetList(null).Where(x => x.ParamType == "SPGarantiBank").ToList();
            
            var startDateTime = Convert.ToDateTime(jObj["StartDateTime"].ToString());
            var endDateTime = Convert.ToDateTime(jObj["EndDateTime"].ToString());

            var startDate = Convert.ToDateTime(jObj["StartDate"].ToString());
            var endDate = Convert.ToDateTime(jObj["EndDate"].ToString());

            var reqStartDateTime = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDateTime.Hour, startDateTime.Minute, startDateTime.Second);
            var reqEndDateTime = new DateTime(endDate.Year, endDate.Month, endDate.Day, endDateTime.Hour, endDateTime.Minute, endDateTime.Second);

            Dictionary<string, string> headerGaranti = new Dictionary<string, string>();
            Dictionary<string, object> bodyGaranti = new Dictionary<string, object>();

            var ibanList = new[]
            {
                "TR040020500009814244300001",
                "TR740020500009814244300002",
                "TR570006200006000006289748",
                "TR510020500009868130200002",
                "TR780020500009868130200001",
                "TR260006200111800006295880"
            };

            headerGaranti.Clear(); bodyGaranti.Clear();

            bodyGaranti.Add("grant_type", "client_credentials");
            bodyGaranti.Add("scope", "oob");
            bodyGaranti.Add("client_id", garantiBankSettings.FirstOrDefault(f => f.ParamDef == "client_id").ParamVal);
            bodyGaranti.Add("client_secret", garantiBankSettings.FirstOrDefault(f => f.ParamDef == "client_secret").ParamVal);
            bodyGaranti.Add("redirect_uri", garantiBankSettings.FirstOrDefault(f => f.ParamDef == "redirect_uri").ParamVal);

            var garantiToken = tHttpClientManager<GarantiTokenModel>.PostFormDataGetJson("https://apis.garantibbva.com.tr/auth/oauth/v2/token", headerGaranti, bodyGaranti);

            var garantiTransaction = new GarantiTransactionModel();
            var searchInBankModelList = new List<SearchInBankModel>();

            if (garantiToken != null && !string.IsNullOrEmpty(garantiToken.access_token))
            {
                headerGaranti.Clear(); bodyGaranti.Clear();

                headerGaranti.Add("Authorization", string.Concat(garantiToken.token_type, " ", garantiToken.access_token));
                bodyGaranti.Add("consentId", garantiBankSettings.FirstOrDefault(f => f.ParamDef == "consent_id").ParamVal);
                bodyGaranti.Add("unitNum", "1118");
                bodyGaranti.Add("accountNum", "6295880");
                bodyGaranti.Add("IBAN", "TR260006200111800006295880");
                bodyGaranti.Add("transactionId", "");
                bodyGaranti.Add("pageIndex", "1");
                bodyGaranti.Add("pageSize", "1500");
                bodyGaranti.Add("startDate", reqStartDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                bodyGaranti.Add("endDate", reqEndDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff"));

                garantiTransaction = tHttpClientManager<GarantiTransactionModel>.PostJsonDataGetJson("https://apis.garantibbva.com.tr/balancesandmovements/accountinformation/transaction/v1/gettransactions", headerGaranti, bodyGaranti);
            }

            headerGaranti.Clear(); bodyGaranti.Clear();

            bodyGaranti.Add("grant_type", "client_credentials");
            bodyGaranti.Add("scope", "oob");
            bodyGaranti.Add("client_id", garantiBankSettingsSP.FirstOrDefault(f => f.ParamDef == "client_id").ParamVal);
            bodyGaranti.Add("client_secret", garantiBankSettingsSP.FirstOrDefault(f => f.ParamDef == "client_secret").ParamVal);
            bodyGaranti.Add("redirect_uri", garantiBankSettingsSP.FirstOrDefault(f => f.ParamDef == "redirect_uri").ParamVal);

            var garantiTokenSP = tHttpClientManager<GarantiTokenModel>.PostFormDataGetJson("https://apis.garantibbva.com.tr/auth/oauth/v2/token", headerGaranti, bodyGaranti);

            var garantiTransactionSP = new GarantiTransactionModel();
            var searchInBankModelListSP = new List<SearchInBankModel>();

            if (garantiTokenSP != null && !string.IsNullOrEmpty(garantiTokenSP.access_token))
            {
                headerGaranti.Clear(); bodyGaranti.Clear();

                headerGaranti.Add("Authorization", string.Concat(garantiTokenSP.token_type, " ", garantiTokenSP.access_token));
                bodyGaranti.Add("consentId", garantiBankSettingsSP.FirstOrDefault(f => f.ParamDef == "consent_id").ParamVal);
                bodyGaranti.Add("unitNum", "060");
                bodyGaranti.Add("accountNum", "6289748");
                bodyGaranti.Add("IBAN", "TR570006200006000006289748");
                bodyGaranti.Add("transactionId", "");
                bodyGaranti.Add("pageIndex", "1");
                bodyGaranti.Add("pageSize", "1500");
                bodyGaranti.Add("startDate", reqStartDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                bodyGaranti.Add("endDate", reqEndDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff"));

                garantiTransactionSP = tHttpClientManager<GarantiTransactionModel>.PostJsonDataGetJson("https://apis.garantibbva.com.tr/balancesandmovements/accountinformation/transaction/v1/gettransactions", headerGaranti, bodyGaranti);
            }

            if (garantiTransaction != null && garantiTransaction.Transactions != null)
            {
                foreach (var item in garantiTransaction.Transactions.OrderByDescending(o => Convert.ToDateTime(o.TransactionInstanceId)))
                {
                    if (!tSQLBankManager.HasPaymentTransferPool(item.TransactionInstanceId) && item.Amount < 150000 && !item.Explanation.Contains("Sanal POS Hakedi") && !item.Explanation.Contains("SANAL POS HAKEDİ") && (item.ClasificationCode.Equals("NTRF") || item.ClasificationCode.Equals("NEFT") || item.ClasificationCode.Equals("NMSC")) && !ibanList.Any(iban => item.EnrichmentInformation
                        .FirstOrDefault(f => !string.IsNullOrEmpty(f.EnrichmentCode) && f.EnrichmentCode == "CORRACNT")
                        ?.EnrichmentValue.SenderIban?.Contains(iban) == true))
                    {
                        var senderName = item.EnrichmentInformation.Any(f => !string.IsNullOrEmpty(f.EnrichmentCode) && f.EnrichmentCode == "MUS") ? item.EnrichmentInformation.FirstOrDefault(f => !string.IsNullOrEmpty(f.EnrichmentCode) && f.EnrichmentCode == "MUS").EnrichmentValue.SenderName : "";

                        var searchInBankModel = new SearchInBankModel()
                        {
                            TransactionDate = Convert.ToDateTime(item.TransactionInstanceId),
                            Amount = item.Amount,
                            Description = item.Explanation,
                            SenderName = string.IsNullOrEmpty(senderName) || string.IsNullOrWhiteSpace(senderName) ? item.Explanation : senderName,
                            Bank = "Garanti",
                            TransactionKey = item.TransactionInstanceId,
                        };

                        searchInBankModelList.Add(searchInBankModel);
                    }
                }
            }

            if (garantiTransactionSP != null && garantiTransactionSP.Transactions != null)
            {
                foreach (var item in garantiTransactionSP.Transactions.OrderByDescending(o => Convert.ToDateTime(o.TransactionInstanceId)))
                {
                    if (!tSQLBankManager.HasPaymentTransferPool(item.TransactionInstanceId) && item.Amount < 150000 && !item.Explanation.Contains("Sanal POS Hakedi") && !item.Explanation.Contains("SANAL POS HAKEDİ") && (item.ClasificationCode.Equals("NTRF") || item.ClasificationCode.Equals("NEFT") || item.ClasificationCode.Equals("NMSC")) && !ibanList.Any(iban => item.EnrichmentInformation
                        .FirstOrDefault(f => !string.IsNullOrEmpty(f.EnrichmentCode) && f.EnrichmentCode == "CORRACNT")
                        ?.EnrichmentValue.SenderIban?.Contains(iban) == true))
                    {
                        var senderName = item.EnrichmentInformation.Any(f => !string.IsNullOrEmpty(f.EnrichmentCode) && f.EnrichmentCode == "MUS") ? item.EnrichmentInformation.FirstOrDefault(f => !string.IsNullOrEmpty(f.EnrichmentCode) && f.EnrichmentCode == "MUS").EnrichmentValue.SenderName : "";

                        var searchInBankModel = new SearchInBankModel()
                        {
                            TransactionDate = Convert.ToDateTime(item.TransactionInstanceId),
                            Amount = item.Amount,
                            Description = item.Explanation,
                            SenderName = string.IsNullOrEmpty(senderName) || string.IsNullOrWhiteSpace(senderName) ? item.Explanation : senderName,
                            Bank = "Garanti SP",
                            TransactionKey = item.TransactionInstanceId,
                        };

                        searchInBankModelList.Add(searchInBankModel);
                    }
                }
            }

            return Json(searchInBankModelList);
        }

        [HttpPost]
        public IActionResult SaveSelectedDataToTransferPool([FromBody] JObject jObj)
        {
            var settings = new JsonSerializerSettings
            {
                DateParseHandling = DateParseHandling.None // Tarihleri string olarak koru
            };

            var transactionModel = JsonConvert.DeserializeObject<SearchInBankModel>(jObj.ToString(), settings);

            if (transactionModel != null) 
            {
                if (tSQLBankManager.HasPaymentTransferPool(transactionModel.TransactionKey))
                    return Json(new GenericResponse { Status = "ERROR", Message = "Aktarılmak istenilen işlem havuzda mevcut." });
                else
                {
                    var idBank = transactionModel.Bank == "Garanti" ? "05" : "34";
                    var companyBankAccountId = transactionModel.Bank == "Garanti" ? "CB301CEE-B18E-4CEF-9332-90E245A3A62B" : "2322C27B-45D6-456C-85BC-B66A779E0A4D";
                    var IDOut = tSQLBankManager.AddPaymentTransferPool(transactionModel.TransactionDate, idBank, transactionModel.SenderName ?? transactionModel.Description, "", transactionModel.Amount, transactionModel.TransactionKey, transactionModel.Description, companyBankAccountId);

                    if(IDOut != null)
                        return Json(new GenericResponse { Status = "OK", Message = "İşlem başarıyla havuza aktarıldı." });
                    else
                        return Json(new GenericResponse { Status = "ERROR", Message = "Aktarım sırasında bir hata ile karşılaşıldı." });
                }
            }
            return Json(new GenericResponse { Status = "ERROR", Message = "Bilinmeyen işlem hatası." });
        }

        [HttpPost]
        public async Task<IActionResult> SetSenderNameByPin([FromBody] SetSenderNameByPinRequest req)
        {
            if (string.IsNullOrWhiteSpace(req?.TpId) ||
                string.IsNullOrWhiteSpace(req.Pin) ||
                string.IsNullOrWhiteSpace(req.SenderName))
            {
                return Json(new { status = "ERROR", message = "tpId, pin ve gönderici adı zorunlu" });
            }
            if (req.Pin != "112233")
                return Json(new { status = "ERROR", message = "PIN hatalı" });
            var updatedName = tSQLBankManager.SetPaymentTransferPoolSenderNameById(req.TpId, req.SenderName);
            if (string.IsNullOrWhiteSpace(updatedName))
                return Json(new { status = "ERROR", message = "Kayıt bulunamadı / güncellenmedi" });
            return Json(new
            {
                status = "OK",
                message = "Gönderici adı güncellendi",
                senderName = req.SenderName,
            });
        }
        public class SetSenderNameByPinRequest
        {
            public string TpId { get; set; }
            public string Pin { get; set; }
            public string SenderName { get; set; }
        }

    }
}