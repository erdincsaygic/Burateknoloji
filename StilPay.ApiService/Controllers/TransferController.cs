using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StilPay.ApiService.Models;
using StilPay.BLL.Abstract;
using StilPay.DAL.Concrete;
using StilPay.Entities;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.KuveytTurk;
using StilPay.Utility.KuveytTurk.KuveytTurkTransfer;
using StilPay.Utility.ToslaSanalPos;
using StilPay.Utility.ToslaSanalPos.Models.ToslaGetRefCode;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using ZiraatBankPaymentService;
using static StilPay.ApiService.Models.ExternalCallback;
using static StilPay.Utility.Helper.Enums;

namespace StilPay.ApiService.Controllers
{
    [ApiController]
    [Route("api/transfer")]
    public class TransferController : ControllerBase
    {
        private readonly ICompanyIntegrationManager _companyIntegrationManager;
        private readonly ICompanyBankManager _companyBankManager;
        private readonly ICompanyWithdrawalRequestManager _companyWithdrawalRequestManager;
        private readonly ICompanyManager _companyManager;
        private readonly ICompanyCommissionManager _companyCommissionManager;
        private readonly IPublicHolidayManager _publicHolidayManager;
        private readonly IBankManager _bankManager;
        private readonly ISystemSettingManager _systemSettingManager;
        private readonly ICompanyRebateRequestManager _companyRebateRequestManager;
        private readonly IPaymentNotificationManager _paymentNotificationManager;
        private readonly ICreditCardPaymentNotificationManager _creditCardPaymentNotificationManager;
        private readonly ICallbackResponseLogManager _callbackResponseLogManager;
        private readonly ICompanyPaymentInstitutionManager _companyPaymentInstitutionManager;
        private readonly IPaymentInstitutionManager _paymentInstitutionManager;
        private readonly IMemberManager _memberManager;
        private readonly ICompanyBankAccountManager _companyBankAccountManager;
        private readonly ICurrencyManager _currencyManager;
        private readonly ICompanyCurrencyManager _companyCurrencyManager;
        private readonly ICustomerInfoManager _customerInfoManager;
        private readonly ICompanyTransactionManager _companyTransactionManager;

        private readonly SettingDAL _settingDAL = new SettingDAL();


        NkyParaTransferiWSSoapClient ziraatService = new NkyParaTransferiWSSoapClient(NkyParaTransferiWSSoapClient.EndpointConfiguration.NkyParaTransferiWSSoap);
        SecuredWebServiceHeader securedWebServiceHeader = new SecuredWebServiceHeader();

        public TransferController(ICompanyIntegrationManager companyIntegrationManager, IBankManager bankManager, ICompanyBankManager companyBankManager, IPublicHolidayManager publicHolidayManager, ICompanyCommissionManager companyCommissionManager, ICompanyManager companyManager, ICompanyWithdrawalRequestManager companyWithdrawalRequestManager,ICompanyRebateRequestManager companyRebateRequestManager, ISystemSettingManager      systemSettingManager, IPaymentNotificationManager paymentNotificationManager, ICreditCardPaymentNotificationManager creditCardPaymentNotificationManager, ICallbackResponseLogManager callbackResponseLogManager, ICompanyPaymentInstitutionManager companyPaymentInstitutionManager, IMemberManager memberManager, IPaymentInstitutionManager paymentInstitutionManager, ICompanyBankAccountManager companyBankAccountManager, ICompanyCurrencyManager companyCurrencyManager, ICurrencyManager currencyManager, ICustomerInfoManager customerInfoManager, ICompanyTransactionManager companyTransactionManager)
        {
            _companyIntegrationManager = companyIntegrationManager;
            _companyBankManager = companyBankManager;
            _companyWithdrawalRequestManager = companyWithdrawalRequestManager;
            _companyManager = companyManager;
            _companyCommissionManager = companyCommissionManager;
            _publicHolidayManager = publicHolidayManager;
            _bankManager = bankManager;
            _systemSettingManager = systemSettingManager;
            _companyRebateRequestManager = companyRebateRequestManager;
            _paymentNotificationManager = paymentNotificationManager;
            _creditCardPaymentNotificationManager = creditCardPaymentNotificationManager;
            _callbackResponseLogManager = callbackResponseLogManager;
            _companyPaymentInstitutionManager = companyPaymentInstitutionManager;
            _memberManager = memberManager;
            _paymentInstitutionManager = paymentInstitutionManager;
            _companyBankAccountManager = companyBankAccountManager;
            _currencyManager = currencyManager;
            _companyCurrencyManager = companyCurrencyManager;
            _customerInfoManager = customerInfoManager;
            _companyTransactionManager = companyTransactionManager;
        }

        /// <summary>
        /// Kullanılabilir Bankaları Sorgulama
        /// </summary>
        /// <param name="service_id"></param>
        /// <param name="ciphered"></param>
        /// <remarks>     
        /// SAMPLE REQUEST:
        /// 
        /// {
        /// 
        ///     "service_id": "0000",
        ///     "ciphered": "aaabbbd8f80196fbe36a2bffca8ef4bae576",
        ///     
        /// }
        /// 
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Kullanılabilir Bankalar Listelenir</response>
        /// <response code="400">Request veya Response Hatası</response>
        /// <response code="401">Servis Key-Gizli Anahtar Hatası</response>
        [HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status401Unauthorized)]
        [Consumes(System.Net.Mime.MediaTypeNames.Application.Json)]
        [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
        [HttpPost("banks")]
        public IActionResult Banks([FromBody] JObject jobj)
        {
            try
            {
                var companyIntegration = _companyIntegrationManager.GetByServiceId(jobj.GetValue("service_id").ToString());
                var encrypted = tMD5Manager.EncryptBasic(companyIntegration.SecretKey);
                if (!encrypted.Equals(jobj.GetValue("ciphered").ToString()))
                {
                    return Unauthorized();
                }
                else
                {
                    var list = _companyBankManager.GetActiveList(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, companyIntegration.ID) });

                    var banks = list.Select(f => new { bank_id = f.IDBank, bank = string.Concat(f.Bank), title = f.Title, branch = f.Branch, account_nr = f.AccountNr, iban = f.IBAN, img = string.Concat("https://burateknoloji.com/areas/panel/img/banks/", f.Img) }).ToList();

                    return Ok(banks);
                }
            }
            catch { }

            return BadRequest();
        }

        /// <summary>
        /// Bakiye Sorgulama
        /// </summary>
        /// <param name="service_id"></param>
        /// <param name="ciphered"></param>
        /// <remarks>     
        /// SAMPLE REQUEST:
        /// 
        /// {
        /// 
        ///     "service_id": "0000",
        ///     "ciphered": "aaabbbd8f80196fbe36a2bffca8ef4bae576",
        ///     
        /// }
        /// 
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Kullanılabilir Bakiyeler Listelenir</response>
        /// <response code="400">Request veya Response Hatası</response>
        /// <response code="401">Servis Key-Gizli Anahtar Hatası</response>
        [HttpPost("accountbalance")]
        public IActionResult AccountBalance([FromBody] JObject jobj)
        {
            try
            {
                JToken tokenServiceId = jobj["service_id"];
                if (tokenServiceId == null || string.IsNullOrEmpty(tokenServiceId.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Service ID Gönderilmedir" });
                }
                JToken tokenCiphered = jobj["ciphered"];
                if (tokenCiphered == null || string.IsNullOrEmpty(tokenCiphered.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Ciphered Gönderilmedir" });
                }

                var companyIntegration = _companyIntegrationManager.GetByServiceId(jobj.GetValue("service_id").ToString());

                if (companyIntegration == null)
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Service ID Hatalı. Lütfen Bizimle İletişime Geçiniz" });

                var encrypted = tMD5Manager.EncryptBasic(companyIntegration.SecretKey);
                if (!encrypted.Equals(jobj.GetValue("ciphered").ToString()))
                {
                    return Unauthorized();
                }

                else
                {
                    var company = _companyManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, companyIntegration.ID) });

                    var balances = _companyManager.GetBalance(company.ID);

                    
                    if (balances != null)
                    {
                        return Ok(new GenericResponseApi
                        {
                            ResponseStatus = 1,
                            Status = "OK",
                            Data = new { using_balance = balances.UsingBalance, blocked_balance = balances.BlockedBalance, totalBalance = balances.TotalBalance },
                        });
                    }
                }
            }
            catch { }

            return BadRequest();
        }

        /// <summary>
        /// Çekim Talebi
        /// </summary>
        /// <param name="service_id"></param>
        /// <param name="ciphered"></param>
        /// <param name="data"></param>
        /// <remarks>     
        /// SAMPLE REQUEST:
        /// 
        /// {
        /// 
        ///     "service_id": "0000",
        ///     "ciphered": "aaabbbd8f80196fbe36a2bffca8ef4bae576",
        ///     "data" :{
        ///        "title"          : "Stilpay", // Alıcı Ad Soyad
        ///        "iban"           : "TR11 1111 1111 1111 1111 1111 11", // Alıcı Iban Numarası 
        ///        "amount"         : 10, // Tutar 
        ///        "is_eft"         : true, // İşlem EFT ise 'true', FAST ise 'false'
        ///        "request_nr"     :"Test1234" // Unique ID
        ///     }
        ///     
        /// }
        /// 
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Çekim Talebi Başarıyla Oluşturuldu</response>
        /// <response code="400">Request veya Response Hatası</response>
        /// <response code="401">Servis Key-Gizli Anahtar Hatası</response>

        [HttpPost("withdrawalrequest")]
        public IActionResult WithdrawalRequest([FromBody] JObject jobj)
        {
            try
            {
                JToken tokenServiceId = jobj["service_id"];
                if (tokenServiceId == null || string.IsNullOrEmpty(tokenServiceId.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Service ID Gönderilmedir" });
                }
                JToken tokenCiphered = jobj["ciphered"];
                if (tokenCiphered == null || string.IsNullOrEmpty(tokenCiphered.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Ciphered Gönderilmedir" });
                }
                JToken tokenData = jobj["data"];
                if (tokenData == null || string.IsNullOrEmpty(tokenData.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Data Gönderilmedir" });
                }

                var values = jobj["data"].ToObject<Dictionary<string, object>>();

                string[] stringArray = { "request_nr", "iban", "amount", "is_eft", "title" };

                foreach (var item in stringArray)
                {
                    if (!values.Keys.Any(x => x.ToLower() == item))
                        return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{item} Alanı Gönderilmelidir" });

                    else
                    {
                        values.TryGetValue(item, out var value);

                        if (value is null || (value is string && String.Empty.Equals(value)))
                            return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{item} Alanı Boş Gönderilemez" });

                        if (item == "amount")
                        {
                            if (!decimal.TryParse(value.ToString(), out decimal decimalValue) || decimalValue <= 0)
                            {
                                return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{item} Alanını Decimal Formatında ve 0'dan Büyük Olarak Gönderiniz" });
                            }
                        }
                    }
                }

                var companyIntegration = _companyIntegrationManager.GetByServiceId(jobj.GetValue("service_id").ToString());

                if (companyIntegration == null)
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Service ID Hatalı. Lütfen Bizimle İletişime Geçiniz" });

                if (!companyIntegration.WithdrawalApiBeUsed)
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "API Erişim Yetkiniz Bulunmamaktadır. Lütfen Bizimle İletişime Geçiniz" });

                var encrypted = tMD5Manager.EncryptBasic(companyIntegration.SecretKey);
                if (!encrypted.Equals(jobj.GetValue("ciphered").ToString()))
                {
                    return Unauthorized();
                }

                else
                {
                    var company = _companyManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, companyIntegration.ID) });

                    var callbackEntity = new CallbackResponseLog();
                    var opt = new JsonSerializerOptions() { WriteIndented = true };

                    callbackEntity.TransactionID = jobj["data"]["request_nr"].ToString();
                    callbackEntity.ServiceType = company.Title;
                    callbackEntity.IDCompany = companyIntegration.ID;
                    callbackEntity.Callback = jobj.ToString();
                    callbackEntity.TransactionType = "Withdrawal API Request";
                    _callbackResponseLogManager.Insert(callbackEntity);

                    var companyCommisson = _companyCommissionManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, companyIntegration.ID) });
                    var banks = _bankManager.GetList(null);

                    var data = JsonConvert.DeserializeAnonymousType(jobj.GetValue("data").ToString(), new WithdrawalRequestModel { request_nr = "", iban = "", title = "", amount = 0.0m, is_eft = false, description = "" });

                    Regex regex = new Regex(@"^TR\d{7}[0-9]{17}$");

                    if (!regex.IsMatch(data.iban.Replace(" ", "")))
                        return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Lütfen Iban Bilgisini Doğru Formatta Giriniz" });


                    var bankCode = data.iban.Replace(" ", "")[5..9];
                    var IDBank = banks.FirstOrDefault(x => x.Branch.Equals(bankCode))?.ID;

                    if (IDBank == null)
                        return BadRequest(new GenericResponse { Status = "ERROR", Message = "Iban Bilgisi İle Eşleşen Banka Bulunamadı.Lütfen Bizimle İletişime Geçiniz" });

                    var activeBank = _companyBankAccountManager.GetActiveList(new List<FieldParameter>() { new FieldParameter("IDCompany", Enums.FieldType.NVarChar, "1312E00F-E83E-45B4-85C6-892396D12331") }).Where(x => x.IsActiveByDefaultExpenseAccount).FirstOrDefault();

                    var systemSetting = _settingDAL.GetList(null).Where(x => (x.ParamType == "Payments" || x.ParamType == "EftHours") && x.ActivatedForGeneralUse).ToList();

                    var eftStartTime = "";
                    var eftEndTime = "";

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

                    var fastControlIDBank = "";

                    if (IDBank == "07" && activeBank.IDBank == "36")
                        fastControlIDBank = "36";
                    else
                        fastControlIDBank = IDBank;

                    var day = DateTime.Now.DayOfWeek;
                    if ((day == DayOfWeek.Saturday || day == DayOfWeek.Sunday) && data.is_eft == true && fastControlIDBank != activeBank.IDBank)
                        return BadRequest(new GenericResponse { Status = "ERROR", Message = "Haftasonu Sadece FAST İşlemi Yapılmaktadır" });

                    var publicHolidayCheck = _publicHolidayManager.GetSingle(new List<FieldParameter>() { new FieldParameter("HolidayDate", Enums.FieldType.DateTime, DateTime.Now.Date) });

                    if (publicHolidayCheck != null && data.is_eft == true && fastControlIDBank != activeBank.IDBank)
                        return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Resmi Tatil Günlerinde Sadece FAST İşlemi Yapılmaktadır" });

                    var datetime = DateTime.Now;
                    var eftStartHour = int.Parse(eftStartTime[..2]);
                    var eftStarMinute = int.Parse(eftStartTime[3..]);

                    var eftEndHour = int.Parse(eftEndTime[..2]);
                    var eftEndMinute = int.Parse(eftEndTime[3..]);

                    var eftStartDateTime =  new DateTime(datetime.Year, datetime.Month, datetime.Day, eftStartHour, eftStarMinute, 0);
                    var eftEndDateTime = new DateTime(datetime.Year, datetime.Month, datetime.Day, eftEndHour, eftEndMinute, 0);

                    if ((datetime < eftStartDateTime || datetime > eftEndDateTime) && data.is_eft == true && fastControlIDBank != activeBank.IDBank)
                        return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Mesai Saatleri Dışında Sadece FAST İşlemi Yapılmaktadır. Lütfen is_eft Alanını 'false' Gönderiniz" });

                    var costTotal = data.is_eft == true ? companyCommisson.WithdrawalTransferAmount : companyCommisson.WithdrawalEftAmount;

                    if (data.amount > 100000 && data.is_eft == false)
                        return BadRequest(new GenericResponseApi { Status = "ERROR", Message = "Maximum 100.000 TL Fast İşlemi Yapabilirsiniz" });

                    var response = new GenericResponse();

                    var balances = _companyManager.GetBalance(company.ID);

                    if (balances == null || ( balances.UsingBalance + balances.NegativeBalanceLimit < data.amount + costTotal ))
                        return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Mevcut Bakiyeniz Çekim Talebi Tutarı İçin Yetersiz" });

                    var checkIfExist = _companyWithdrawalRequestManager.GetSingleByRequestNr(data.request_nr);

                    if (checkIfExist != null && checkIfExist.Status == (byte)Enums.StatusType.Pending)
                        return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{checkIfExist.RequestNr} Numaralı İşleminiz İşlem Aşamasındadır. İşlem Sonuçlanana Kadar Bekleyiniz" });

                    if (checkIfExist != null && checkIfExist.Status == (byte)Enums.StatusType.Confirmed)
                        return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{checkIfExist.RequestNr} Numaralı İşleminiz Onaylanmıştır." });

                    if (checkIfExist != null && checkIfExist.Status == (byte)Enums.StatusType.Canceled)
                        return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{checkIfExist.RequestNr} Numaralı İşleminiz {checkIfExist.Description} Nedeni İle İptal Edilmiştir." });

                    data.title = string.Join(" ", data.title.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)).Trim();

                    var model = new CompanyWithdrawalRequest
                    {
                        CUser = "00000000-0000-0000-0000-000000000000",
                        CDate = DateTime.Now,
                        IDCompany = companyIntegration.ID,
                        RequestNr = data.request_nr,
                        IDBank = IDBank,
                        IBAN = data.iban,
                        Title = data.title,
                        Amount = data.amount,
                        CostTotal = costTotal,
                        IsEFT = data.is_eft,
                        Status = 1,
                        DealerDescription = string.IsNullOrWhiteSpace(data.description) ? null : data.description.Trim(),
                        IsProcess = false,
                    };

                    response = _companyWithdrawalRequestManager.Insert(model);

                    if(response.Status == "OK")
                    {
                        return Ok(new GenericResponseApi
                        {
                            Status = "OK",
                            ResponseStatus = 1,
                            Data = new { request_nr = model.RequestNr },
                        });                       
                    }
                    else
                    {
                        return BadRequest(new GenericResponseApi
                        {
                            Status = "ERROR",
                            ResponseStatus = 0,
                            Data = new { request_nr = model.RequestNr },
                            Message = response.Message
                        });
                    }
                }
            }
            catch { }

            return BadRequest();
        }

        /// <summary>
        /// Çekim Talebi Durum Sorgulama
        /// </summary>
        /// <param name="service_id"></param>
        /// <param name="ciphered"></param>
        /// <param name="data"></param>
        /// <remarks>     
        /// SAMPLE REQUEST:
        /// 
        /// {
        /// 
        ///     "service_id": "0000",
        ///     "ciphered": "aaabbbd8f80196fbe36a2bffca8ef4bae576",
        ///     "data" :{
        ///        "request_nr"     :"Test1234" // Unique ID
        ///     }
        ///     
        /// }
        /// 
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Çekim Talebi Başarıyla Oluşturuldu</response>
        /// <response code="400">Request veya Response Hatası</response>
        /// <response code="401">Servis Key-Gizli Anahtar Hatası</response>
        [HttpPost("withdrawalresult")]
        public IActionResult WithdrawalResult([FromBody] JObject jobj)
        {
            try
            {
                JToken tokenServiceId = jobj["service_id"];
                if (tokenServiceId == null || string.IsNullOrEmpty(tokenServiceId.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Service ID Gönderilmedir" });
                }
                JToken tokenCiphered = jobj["ciphered"];
                if (tokenCiphered == null || string.IsNullOrEmpty(tokenCiphered.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Ciphered Gönderilmedir" });
                }
                JToken tokenData = jobj["data"];
                if (tokenData == null || string.IsNullOrEmpty(tokenData.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Data Gönderilmedir" });
                }

                var values = jobj["data"].ToObject<Dictionary<string, object>>();

                if (!values.Keys.Any(x => x.ToLower() == "request_nr"))
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "request_nr Alanı Gönderilmelidir" });

                else
                {
                    values.TryGetValue("request_nr", out var value);

                    if (value is null || (value is string && String.Empty.Equals(value)))
                        return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "request_nr Alanı Boş Gönderilemez" });
                }

                var companyIntegration = _companyIntegrationManager.GetByServiceId(jobj.GetValue("service_id").ToString());

                if (companyIntegration == null)
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Service ID Hatalı. Lütfen Bizimle İletişime Geçiniz" });

                var encrypted = tMD5Manager.EncryptBasic(companyIntegration.SecretKey);

                if (!encrypted.Equals(jobj.GetValue("ciphered").ToString()))
                {
                    return Unauthorized();
                }
                else
                {
                    var data = JsonConvert.DeserializeAnonymousType(jobj.GetValue("data").ToString(), new { request_nr = "" });

                    var model = _companyWithdrawalRequestManager.GetSingleByRequestNr(data.request_nr);

                    if(model != null)
                    {
                        return Ok(new GenericResponseApi
                        {
                            ResponseStatus = 1,
                            Status = (model.Status == (byte)Enums.StatusType.Pending ? "BEKLİYOR" : (model.Status == (byte)Enums.StatusType.Confirmed ? "ONAYLANDI" : "İPTAL EDİLDİ")),
                            Data = new { request_nr = model.RequestNr },
                            Message = model.Description
                        });
                    }
                    else
                    {
                        return BadRequest(new GenericResponseApi
                        {
                            ResponseStatus = 0,
                            Status = "ERROR",
                            Data = new { request_nr = data.request_nr },
                            Message = "Veri Bulunamadı"
                        });
                    }
                }

            }
            catch { }

            return BadRequest();
        }

        /// <summary>
        /// Ödeme Bildirimleri Listesi
        /// </summary>
        /// <param name="service_id"></param>
        /// <param name="ciphered"></param>
        /// <param name="data"></param>
        /// <remarks>     
        /// SAMPLE REQUEST:
        /// 
        /// {
        /// 
        ///     "service_id": "0000",
        ///     "ciphered": "aaabbbd8f80196fbe36a2bffca8ef4bae576",
        ///     "data" :{
        ///        "startDate"     :"yyyy-MM-ddTHH:mm:ss" 
        ///        "endDate"     :"yyyy-MM-ddTHH:mm:ss" 
        ///     }
        ///     
        /// }
        /// 
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Çekim Talebi Başarıyla Oluşturuldu</response>
        /// <response code="400">Request veya Response Hatası</response>
        /// <response code="401">Servis Key-Gizli Anahtar Hatası</response>
        [HttpPost("getpaymentnotifications")]
        public IActionResult GetPaymentNotifications([FromBody] JObject jobj)
        {
            try
            {
                JToken tokenServiceId = jobj["service_id"];
                if (tokenServiceId == null || string.IsNullOrEmpty(tokenServiceId.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Service ID Gönderilmedir" });
                }
                JToken tokenCiphered = jobj["ciphered"];
                if (tokenCiphered == null || string.IsNullOrEmpty(tokenCiphered.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Ciphered Gönderilmedir" });
                }

                var startDate = new DateTime();
                var endDate = new DateTime();

                JToken tokenData = jobj["data"];
                if (tokenData != null && !string.IsNullOrEmpty(tokenData.ToString()))
                {
                    var data = JsonConvert.DeserializeAnonymousType(jobj.GetValue("data").ToString(), new { startDate = "", endDate = "" });

                    if (!string.IsNullOrEmpty(data.startDate) && !string.IsNullOrWhiteSpace(data.startDate))
                    {
                        if (DateTime.TryParseExact(data.startDate, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime parsedStartDate))
                        {
                            startDate = parsedStartDate;
                        }
                        else
                        {
                            return BadRequest(new GenericResponseApi
                            {
                                ResponseStatus = 0,
                                Status = "ERROR",
                                Message = "Başlangıç tarihi 'yyyy-MM-ddTHH:mm:ss' formatında olmalıdır."
                            });
                        }
                    }
                    else
                    {
                        return BadRequest(new GenericResponseApi
                        {
                            ResponseStatus = 0,
                            Status = "ERROR",
                            Message = "Başlangıç tarihi 'startDate' parametresi ve 'yyyy-MM-ddTHH:mm:ss' formatı ile zorunlu alan."
                        });
                    }

                    if (!string.IsNullOrEmpty(data.endDate))
                    {
                        if (DateTime.TryParseExact(data.endDate, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime parsedEndDate))
                        {
                            endDate = parsedEndDate;
                        }
                        else
                        {
                            return BadRequest(new GenericResponseApi
                            {
                                ResponseStatus = 0,
                                Status = "ERROR",                          
                                Message = "Bitiş tarihi 'yyyy-MM-ddTHH:mm:ss' formatında olmalıdır."
                            });
                        }
                    }
                    else
                    {
                        return BadRequest(new GenericResponseApi
                        {
                            ResponseStatus = 0,
                            Status = "ERROR",
                            Message = "Bitiş tarihi 'endDate' parametresi ve 'yyyy-MM-ddTHH:mm:ss' formatı ile zorunlu alan."
                        });
                    }
                }
                else
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "data Alanı Gönderilmedir" });

                var companyIntegration = _companyIntegrationManager.GetByServiceId(jobj.GetValue("service_id").ToString());

                if (companyIntegration == null)
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Service ID Hatalı. Lütfen Bizimle İletişime Geçiniz" });

                var encrypted = tMD5Manager.EncryptBasic(companyIntegration.SecretKey);

                if (!encrypted.Equals(jobj.GetValue("ciphered").ToString()))
                {
                    return Unauthorized();
                }
                else
                {
                    var paymentNotifications = _paymentNotificationManager.GetPaymentNotificationsAPI(new List<FieldParameter>()
                    {
                        new FieldParameter("ServiceID", Enums.FieldType.NVarChar, jobj.GetValue("service_id").ToString()),
                        new FieldParameter("StartDate", Enums.FieldType.DateTime, startDate),
                        new FieldParameter("EndDate", Enums.FieldType.DateTime, endDate)
                    });

                    if (paymentNotifications != null)
                    {
                        return Ok(new GenericResponseApi
                        {
                            ResponseStatus = 1,
                            Status = "OK",
                            Message = "İşlem Başarılı",
                            Data = new { count = paymentNotifications.Count, notifications = paymentNotifications }
                        });
                    }
                    else
                    {
                        return BadRequest(new GenericResponseApi
                        {
                            ResponseStatus = 0,
                            Status = "ERROR",
                            Message = "Bir hata ile karşılaşıldı."
                        });
                    }
                }

            }
            catch { }

            return BadRequest();
        }

        /// <summary>
        /// Kredi Kartı Ödemeleri Listesi
        /// </summary>
        /// <param name="service_id"></param>
        /// <param name="ciphered"></param>
        /// <param name="data"></param>
        /// <remarks>     
        /// SAMPLE REQUEST:
        /// 
        /// {
        /// 
        ///     "service_id": "0000",
        ///     "ciphered": "aaabbbd8f80196fbe36a2bffca8ef4bae576",
        ///     "data" :{
        ///        "startDate"     :"yyyy-MM-ddTHH:mm:ss" 
        ///        "endDate"     :"yyyy-MM-ddTHH:mm:ss" 
        ///     }
        ///     
        /// }
        /// 
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Çekim Talebi Başarıyla Oluşturuldu</response>
        /// <response code="400">Request veya Response Hatası</response>
        /// <response code="401">Servis Key-Gizli Anahtar Hatası</response>
        [HttpPost("getcreditcardtransactions")]
        public IActionResult GetCreditCardTransactions([FromBody] JObject jobj)
        {
            try
            {
                JToken tokenServiceId = jobj["service_id"];
                if (tokenServiceId == null || string.IsNullOrEmpty(tokenServiceId.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Service ID Gönderilmedir" });
                }
                JToken tokenCiphered = jobj["ciphered"];
                if (tokenCiphered == null || string.IsNullOrEmpty(tokenCiphered.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Ciphered Gönderilmedir" });
                }

                var startDate = new DateTime();
                var endDate = new DateTime();

                JToken tokenData = jobj["data"];
                if (tokenData != null && !string.IsNullOrEmpty(tokenData.ToString()))
                {
                    var data = JsonConvert.DeserializeAnonymousType(jobj.GetValue("data").ToString(), new { startDate = "", endDate = "" });

                    if (!string.IsNullOrEmpty(data.startDate) && !string.IsNullOrWhiteSpace(data.startDate))
                    {
                        if (DateTime.TryParseExact(data.startDate, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime parsedStartDate))
                        {
                            startDate = parsedStartDate;
                        }
                        else
                        {
                            return BadRequest(new GenericResponseApi
                            {
                                ResponseStatus = 0,
                                Status = "ERROR",
                                Message = "Başlangıç tarihi 'yyyy-MM-ddTHH:mm:ss' formatında olmalıdır."
                            });
                        }
                    }
                    else
                    {
                        return BadRequest(new GenericResponseApi
                        {
                            ResponseStatus = 0,
                            Status = "ERROR",
                            Message = "Başlangıç tarihi 'startDate' parametresi ve 'yyyy-MM-ddTHH:mm:ss' formatı ile zorunlu alan."
                        });
                    }

                    if (!string.IsNullOrEmpty(data.endDate))
                    {
                        if (DateTime.TryParseExact(data.endDate, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime parsedEndDate))
                        {
                            endDate = parsedEndDate;
                        }
                        else
                        {
                            return BadRequest(new GenericResponseApi
                            {
                                ResponseStatus = 0,
                                Status = "ERROR",
                                Message = "Bitiş tarihi 'yyyy-MM-ddTHH:mm:ss' formatında olmalıdır."
                            });
                        }
                    }
                    else
                    {
                        return BadRequest(new GenericResponseApi
                        {
                            ResponseStatus = 0,
                            Status = "ERROR",
                            Message = "Bitiş tarihi 'endDate' parametresi ve 'yyyy-MM-ddTHH:mm:ss' formatı ile zorunlu alan."
                        });
                    }
                }
                else
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "data Alanı Gönderilmedir" });

                var companyIntegration = _companyIntegrationManager.GetByServiceId(jobj.GetValue("service_id").ToString());

                if (companyIntegration == null)
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Service ID Hatalı. Lütfen Bizimle İletişime Geçiniz" });

                var encrypted = tMD5Manager.EncryptBasic(companyIntegration.SecretKey);

                if (!encrypted.Equals(jobj.GetValue("ciphered").ToString()))
                {
                    return Unauthorized();
                }
                else
                {
                    var paymentNotifications = _creditCardPaymentNotificationManager.GetCreditCardTransactionsAPI(new List<FieldParameter>()
                    {
                        new FieldParameter("ServiceID", Enums.FieldType.NVarChar, jobj.GetValue("service_id").ToString()),
                        new FieldParameter("StartDate", Enums.FieldType.DateTime, startDate),
                        new FieldParameter("EndDate", Enums.FieldType.DateTime, endDate)
                    });

                    if (paymentNotifications != null)
                    {
                        return Ok(new GenericResponseApi
                        {
                            ResponseStatus = 1,
                            Status = "OK",
                            Message = "İşlem Başarılı",
                            Data = new { count = paymentNotifications.Count, notifications = paymentNotifications }
                        });
                    }
                    else
                    {
                        return BadRequest(new GenericResponseApi
                        {
                            ResponseStatus = 0,
                            Status = "ERROR",
                            Message = "Bir hata ile karşılaşıldı."
                        });
                    }
                }

            }
            catch { }

            return BadRequest();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("getmanuelnotifylist")]
        public IActionResult GetManuelNotifyList()
        {
            try
            {
                // Veriyi al
                var paymentMethodCounts = tSQLBankManager.GetManuelNotifyList();

                // Eğer veri yoksa, "no_data" olarak döndür
                if (paymentMethodCounts.Count == 0)
                {
                    return Ok(new
                    {
                        status = "no_data",
                        message = "No data available"
                    });
                }

                // Veriyi JSON olarak döndür
                return Ok(new
                {
                    status = "success",
                    data = paymentMethodCounts
                });
            }
            catch (Exception ex)
            {
                // Hata durumunda uygun yanıtı döndür
                return StatusCode(500, new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }


        //[HttpPost("rebaterequest")]
        //public IActionResult RebateRequest([FromBody] JObject jobj)
        //{
        //    try
        //    {
        //        JToken tokenServiceId = jobj["service_id"];
        //        if (tokenServiceId == null || string.IsNullOrEmpty(tokenServiceId.ToString()))
        //        {
        //            return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Service ID Gönderilmedir" });
        //        }
        //        JToken tokenCiphered = jobj["ciphered"];
        //        if (tokenCiphered == null || string.IsNullOrEmpty(tokenCiphered.ToString()))
        //        {
        //            return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Ciphered Gönderilmedir" });
        //        }
        //        JToken tokenData = jobj["data"];
        //        if (tokenData == null || string.IsNullOrEmpty(tokenData.ToString()))
        //        {
        //            return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Data Gönderilmedir" });
        //        }

        //        var values = jobj["data"].ToObject<Dictionary<string, object>>();

        //        string[] stringArray = { "request_nr", "amount" };

        //        foreach (var item in stringArray)
        //        {
        //            if (!values.Keys.Any(x => x.ToLower() == item))
        //                return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{item} Alanı Gönderilmelidir" });

        //            else
        //            {
        //                values.TryGetValue(item, out var value);

        //                if (value is null || (value is string && String.Empty.Equals(value)))
        //                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{item} Alanı Boş Gönderilemez" });

        //                if (item == "amount")
        //                {
        //                    values.TryGetValue(item, out var val);
        //                    if (value is string || String.Empty.Equals(value))
        //                        return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{item} Alanını Decimal Formatında '0.0M' Olarak Gönderiniz" });
        //                }
        //            }
        //        }

        //        var companyIntegration = _companyIntegrationManager.GetByServiceId(jobj.GetValue("service_id").ToString());

        //        if (companyIntegration == null)
        //            return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Service ID Hatalı. Lütfen Bizimle İletişime Geçiniz" });

        //        var encrypted = tMD5Manager.EncryptBasic(companyIntegration.SecretKey);
        //        if (!encrypted.Equals(jobj.GetValue("ciphered").ToString()))
        //        {
        //            return Unauthorized();
        //        }

        //        else
        //        {
        //            var data = JsonConvert.DeserializeAnonymousType(jobj.GetValue("data").ToString(), new { request_nr = "", amount = 0.0m });

        //            var response = new GenericResponse();

        //            var balances = _companyManager.GetBalance(companyIntegration.ID);

        //            if (balances == null || balances.UsingBalance + balances.NegativeBalanceLimit < data.amount )
        //                return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Mevcut Bakiyeniz İade Talebi Tutarı İçin Yetersiz" });


        //            var checkIfExist = _companyRebateRequestManager.GetSingle(new List<FieldParameter>() { new FieldParameter("TransactionID", Enums.FieldType.NVarChar, data.request_nr) });

        //            if (checkIfExist != null && checkIfExist.Status == (byte)Enums.StatusType.Pending)
        //                return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{checkIfExist.TransactionID} Numaralı İşleminiz İşlem Aşamasındadır. İşlem Sonuçlanana Kadar Bekleyiniz" });

        //            if (checkIfExist != null && checkIfExist.Status == (byte)Enums.StatusType.Confirmed)
        //                return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{checkIfExist.TransactionID} Numaralı İşleminiz Onaylanmıştır." });

        //            if (checkIfExist != null && checkIfExist.Status == (byte)Enums.StatusType.Canceled)
        //                return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{checkIfExist.TransactionID} Numaralı İşleminiz {checkIfExist.Description} Nedeni İle İptal Edilmiştir." });

        //            var pyEntity = _paymentNotificationManager.GetSingleByTransactionID(data.request_nr);
        //            var cdEntity = _creditCardPaymentNotificationManager.GetSingleByTransactionID(data.request_nr);

        //            if (pyEntity == null && cdEntity == null)
        //                return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{data.request_nr} Numaralı İşlem Bulunamadı." });

        //            if(pyEntity != null && pyEntity.Status == (byte)Enums.StatusType.Pending)
        //                return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{data.request_nr} Numaralı İşlem Tamamlanmadığı İçin İade Talebi Oluşturulamaz." });

        //            if (pyEntity != null && pyEntity.Status == (byte)Enums.StatusType.Canceled)
        //                return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{data.request_nr} Numaralı İşlem İptal Edildiği İçin İade Talebi Oluşturulamaz." });

        //            if (cdEntity != null && cdEntity.Status == (byte)Enums.StatusType.Pending)
        //                return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{data.request_nr} Numaralı İşlem Tamamlanmadığı İçin İade Talebi Oluşturulamaz." });

        //            if (cdEntity != null && cdEntity.Status == (byte)Enums.StatusType.Canceled)
        //                return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{data.request_nr} Numaralı İşlem İptal Edildiği İçin İade Talebi Oluşturulamaz." });


        //            if (pyEntity != null  && pyEntity.Status == (byte)Enums.StatusType.Confirmed && pyEntity.Amount != data.amount)
        //                return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{data.request_nr} Numaralı İşlem Gönderilen Tutar Değeri İle Uyuşmamaktadır." });

        //            if (cdEntity != null && cdEntity.Status == (byte)Enums.StatusType.Confirmed && cdEntity.Amount != data.amount)
        //                return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{data.request_nr} Numaralı İşlem Gönderilen Tutar Değeri İle Uyuşmamaktadır." });

        //            var model = new CompanyRebateRequest
        //            {
        //                CUser = "00000000-0000-0000-0000-000000000000",
        //                CDate = DateTime.Now,
        //                IDCompany = companyIntegration.ID,
        //                TransactionID = data.request_nr,
        //                IDBank = pyEntity != null ? pyEntity.IDBank : "00",
        //                ActionDate = pyEntity != null ? pyEntity.ActionDate : cdEntity.ActionDate,
        //                ActionTime = pyEntity != null ? pyEntity.ActionTime : cdEntity.ActionTime,
        //                Amount = data.amount,
        //                IDMember = pyEntity != null ? pyEntity.IDMember : cdEntity.IDMember,
        //                SenderName = pyEntity != null ? pyEntity.SenderName : cdEntity.SenderName,
        //                SenderIdentityNr = pyEntity != null ? pyEntity.SenderIdentityNr : cdEntity.SenderIdentityNr,
        //                Status = 1
        //            };

        //            response = _companyRebateRequestManager.Insert(model);

        //            if (response.Status == "OK")
        //            {
        //                var dataCallback = new
        //                {
        //                    status_code = "OK",
        //                    service_id = companyIntegration.ServiceID,
        //                    status_type = 0,
        //                    ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
        //                    data = new { transaction_id = data.request_nr, amount = data.amount, message = "İade Talebi İşleme Alındı. Lütfen İşlem Sonuçlanana Kadar Bekleyiniz." }
        //                };

        //                tHttpClientManager<JObject>.PostJsonDataGetJson(companyIntegration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "withdrawal", dataCallback } });

        //                return Ok(new GenericResponseApi
        //                {
        //                    Status = "OK",
        //                    ResponseStatus = 1,
        //                    Data = new { request_nr = data.request_nr },
        //                });
        //            }
        //            else
        //            {
        //                return BadRequest(new GenericResponseApi
        //                {
        //                    Status = "ERROR",
        //                    ResponseStatus = 0,
        //                    Data = new { request_nr = data.request_nr },
        //                    Message = "Şu Anda İsteğiniz Devreye Alınamıyor. Lütfen Bizimle İletişime Geçiniz."
        //                });
        //            }
        //        }
        //    }
        //    catch { }

        //    return BadRequest();
        //}

        //[HttpPost("rebateresult")]
        //public IActionResult RebateResult([FromBody] JObject jobj)
        //{
        //    try
        //    {
        //        JToken tokenServiceId = jobj["service_id"];
        //        if (tokenServiceId == null || string.IsNullOrEmpty(tokenServiceId.ToString()))
        //        {
        //            return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Service ID Gönderilmedir" });
        //        }
        //        JToken tokenCiphered = jobj["ciphered"];
        //        if (tokenCiphered == null || string.IsNullOrEmpty(tokenCiphered.ToString()))
        //        {
        //            return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Ciphered Gönderilmedir" });
        //        }
        //        JToken tokenData = jobj["data"];
        //        if (tokenData == null || string.IsNullOrEmpty(tokenData.ToString()))
        //        {
        //            return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Data Gönderilmedir" });
        //        }

        //        var values = jobj["data"].ToObject<Dictionary<string, object>>();

        //        if (!values.Keys.Any(x => x.ToLower() == "request_nr"))
        //            return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "request_nr Alanı Gönderilmelidir" });

        //        else
        //        {
        //            values.TryGetValue("request_nr", out var value);

        //            if (value is null || (value is string && String.Empty.Equals(value)))
        //                return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "request_nr Alanı Boş Gönderilemez" });
        //        }

        //        var companyIntegration = _companyIntegrationManager.GetByServiceId(jobj.GetValue("service_id").ToString());

        //        if (companyIntegration == null)
        //            return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Service ID Hatalı. Lütfen Bizimle İletişime Geçiniz" });

        //        var encrypted = tMD5Manager.EncryptBasic(companyIntegration.SecretKey);

        //        if (!encrypted.Equals(jobj.GetValue("ciphered").ToString()))
        //        {
        //            return Unauthorized();
        //        }

        //        else
        //        {
        //            var data = JsonConvert.DeserializeAnonymousType(jobj.GetValue("data").ToString(), new { request_nr = "" });

        //            var model = _companyRebateRequestManager.GetSingleByTransactionID(data.request_nr);

        //            if (model != null)
        //            {
        //                return Ok(new GenericResponseApi
        //                {
        //                    ResponseStatus = 1,
        //                    Status = (model.Status == (byte)Enums.StatusType.Pending ? "BEKLİYOR" : (model.Status == (byte)Enums.StatusType.Confirmed ? "ONAYLANDI" : "İPTAL EDİLDİ")),
        //                    Data = new { request_nr = model.TransactionID },
        //                    Message = model.Description
        //                });
        //            }
        //            else
        //            {
        //                return BadRequest(new GenericResponseApi
        //                {
        //                    ResponseStatus = 0,
        //                    Status = "ERROR",
        //                    Data = new { request_nr = data.request_nr },
        //                    Message = "Veri Bulunamadı"
        //                });
        //            }
        //        }

        //    }
        //    catch { }

        //    return BadRequest();
        //}

        [HttpPost("frame")]
        public IActionResult Post([FromBody] JObject jobj)
        {
            try
            {
                JToken tokenServiceId = jobj["service_id"];
                if (tokenServiceId == null || string.IsNullOrEmpty(tokenServiceId.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "service_id Alanı Gönderilmedir" });
                }
                JToken tokenCiphered = jobj["ciphered"];
                if (tokenCiphered == null || string.IsNullOrEmpty(tokenCiphered.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "ciphered Alanı Gönderilmedir" });
                }
                JToken tokenData = jobj["data"];
                if (tokenData == null || string.IsNullOrEmpty(tokenData.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "data Alanı Gönderilmedir" });
                }


                var companyIntegration = _companyIntegrationManager.GetByServiceId(jobj.GetValue("service_id").ToString());
                var encrypted = companyIntegration == null ? "" : tMD5Manager.EncryptBasic(companyIntegration.SecretKey);
                if (string.IsNullOrEmpty(encrypted) && !encrypted.Equals(jobj.GetValue("ciphered").ToString()))
                {
                    return Unauthorized();
                }

                var values = jobj["data"].ToObject<Dictionary<string, object>>();

                string[] stringArray = { "transaction_id", "amount" };

                foreach (var item in stringArray)
                {
                    if (!values.Keys.Any(x => x.ToLower() == item))
                        return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{item} Alanı Gönderilmelidir" });

                    values.TryGetValue(item, out var value);

                    if (value is null || (value is string && String.Empty.Equals(value)))
                        return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{item} Alanı Boş Gönderilemez" });

                    if (item == "amount")
                    {
                        if (!decimal.TryParse(value?.ToString(), out var amount) || amount <= 0)
                        {
                            return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Amount Alanı 0'dan Büyük Olmalı ve '0.0M' Formatında Olmalıdır" });
                        }
                    }
                }

                var checkIfExist = _companyTransactionManager.GetRecordsByQueryParameter(jobj["data"]["transaction_id"].ToString());

                if (checkIfExist != null && checkIfExist.Any(x => x.TableWithTheTransaction == 1 || x.TableWithTheTransaction == 2 || x.TableWithTheTransaction == 3))
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{jobj["data"]["transaction_id"]} Numaralı İşlem Sistemde Mevcut" });

                if (jobj["data"]["payment_method_type_id"] != null)
                {
                    var paymentMethodTypeId = jobj["data"]["payment_method_type_id"].ToString();

                    var creditCardPaymentMethodType = (Enums.CreditCardPaymentMethodType)int.Parse(paymentMethodTypeId);

                    if (!Enum.IsDefined(typeof(CreditCardPaymentMethodType), creditCardPaymentMethodType))
                        return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "payment_method_type_id Değeri Bulunamadı" });

                    //var companyPaymentInstitution = _companyPaymentInstitutionManager.GetList(new List<FieldParameter>() { new FieldParameter("ID", FieldType.NVarChar, companyIntegration.ID) }).FirstOrDefault(x => x.PaymentInstitutionID == paymentMethodTypeId && (x.IsActive || x.IsActiveForAPI));

                    //if (companyPaymentInstitution == null)
                    //{
                    //    string enumName = Enum.GetName(typeof(CreditCardPaymentMethodType), int.Parse(paymentMethodTypeId));

                    //    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{enumName} Ödeme Platformu Firmanız İçin Aktif Değil" });
                    //}
                }

                if (jobj["data"]["currencyCode"] != null)
                {
                    var currencyCode = jobj["data"]["currencyCode"].ToString();

                    if(currencyCode != "TRY") 
                    {
                        var checkCurrencyCode = _currencyManager.GetList(new List<FieldParameter> { new FieldParameter("CurrencyCode", Enums.FieldType.NVarChar, currencyCode) }).FirstOrDefault();

                        if (checkCurrencyCode == null)
                            return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Geçersiz 'currencyCode' (Para Birimi)" });

                        var companyCurrencyCode = _companyCurrencyManager.GetList(new List<FieldParameter>() { new FieldParameter("IDCompany", FieldType.NVarChar, companyIntegration.ID) }).FirstOrDefault(x => x.CurrencyCode == checkCurrencyCode.CurrencyCode && x.IsActive);

                        if (companyCurrencyCode == null)
                        {
                            return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{checkCurrencyCode.CurrencyCode} Para Birimi Firmanız İçin Aktif Değil" });
                        }
                    }
                }

                jobj["data"]["creation_time"] = DateTime.Now;

                var transaction = HttpUtility.UrlEncode(tMD5Manager.Encrypt(companyIntegration.SecretKey, jobj.GetValue("data").ToString()));

                var callbackEntity = new CallbackResponseLog();
                var opt = new JsonSerializerOptions() { WriteIndented = true };

                callbackEntity.TransactionID = jobj["data"]["transaction_id"].ToString();
                callbackEntity.ServiceType = _companyManager.GetSingle(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, companyIntegration.ID)}).Title;
                callbackEntity.IDCompany = companyIntegration.ID;
                callbackEntity.Callback = jobj.ToString();
                callbackEntity.TransactionType = "Get IFrame Request";
                _callbackResponseLogManager.Insert(callbackEntity);

                if(jobj["data"]["customerInfo"] != null)
                {
                    var customerInfoFromJson = JsonConvert.DeserializeObject<CustomerInfo>(jobj["data"]["customerInfo"].ToString());

                    if(customerInfoFromJson != null)
                    {
                        var customerInfo = new CustomerInfo
                        {
                            CDate = DateTime.Now,
                            CUser = "00000000-0000-0000-0000-000000000000",
                            ServiceID = jobj["service_id"].ToString(),
                            CustomerName = customerInfoFromJson.CustomerName,
                            CustomerEmail = customerInfoFromJson.CustomerEmail,
                            CustomerPhone = customerInfoFromJson.CustomerPhone,
                            TransactionID = jobj["data"]["transaction_id"].ToString()
                        };

                        _customerInfoManager.Insert(customerInfo);
                    }

                }

                return Ok(transaction);

                
            }
            catch { }

            return BadRequest();
        }

        [HttpPost("gettoslapayment")]
        public IActionResult GetToslaPayment([FromBody] JObject jobj)
        {
            try
            {
                JToken tokenServiceId = jobj["service_id"];
                if (tokenServiceId == null || string.IsNullOrEmpty(tokenServiceId.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "service_id Alanı Gönderilmedir" });
                }
                JToken tokenCiphered = jobj["ciphered"];
                if (tokenCiphered == null || string.IsNullOrEmpty(tokenCiphered.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "ciphered Alanı Gönderilmedir" });
                }
                JToken tokenData = jobj["data"];
                if (tokenData == null || string.IsNullOrEmpty(tokenData.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "data Alanı Gönderilmedir" });
                }

                var companyIntegration = _companyIntegrationManager.GetByServiceId(jobj.GetValue("service_id").ToString());

                var companyPaymentInstitution = _companyPaymentInstitutionManager.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", FieldType.NVarChar, companyIntegration.ID),
                    new FieldParameter("PaymentInstitutionID", FieldType.NVarChar, ((int)Enums.CreditCardPaymentMethodType.Tosla).ToString())
                });
            
                if(companyPaymentInstitution == null || !companyPaymentInstitution.IsActive)
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Tosla Kullanımınız Devre Dışı" });

                var encrypted = companyIntegration == null ? "" : tMD5Manager.EncryptBasic(companyIntegration.SecretKey);
                if (string.IsNullOrEmpty(encrypted) && !encrypted.Equals(jobj.GetValue("ciphered").ToString()))
                {
                    return Unauthorized();
                }

                var values = jobj["data"].ToObject<Dictionary<string, object>>();

                string[] stringArray = { "processId", "phoneNumber", "amount" };

                foreach (var item in stringArray)
                {
                    if (!values.Keys.Any(x => x == item))
                        return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{item} Alanı Gönderilmelidir" });

                    else
                    {
                        values.TryGetValue(item, out var value);

                        if (value is null || (value is string && String.Empty.Equals(value)))
                            return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = $"{item} Alanı Boş Gönderilemez" });
                    }
                }

                var toslaSettings = _settingDAL.GetList(new List<FieldParameter>()
                {
                    new FieldParameter("ParamType", FieldType.NVarChar, "Tosla")
                });

                var data = JsonConvert.DeserializeAnonymousType(jobj.GetValue("data").ToString(), new ToslaGetRefCodeRequestModel { processId = "", phoneNumber = "", amount = 0.0m });

                //var checkPendingTransaction = _creditCardPaymentNotificationManager.GetSingle(new List<FieldParameter>()
                //{
                //    new FieldParameter("Phone", FieldType.NVarChar, data.phoneNumber)
                //});

                //if(checkPendingTransaction != null && checkPendingTransaction.Status == (byte)Enums.StatusType.Pending && checkPendingTransaction.CreditCardPaymentMethodID == (byte)Enums.CreditCardPaymentMethodType.Tosla )
                //{
                //    return BadRequest(new GenericResponseApi
                //    {
                //        Status = "ERROR",
                //        ResponseStatus = 0,
                //        Message = $"{data.phoneNumber} numaralı kullanıcının bekleyen ödeme talebi mevcut"
                //    });
                //}

                var callbackEntity = new CallbackResponseLog();
                var opt = new JsonSerializerOptions() { WriteIndented = true };

                data.companyId = int.Parse(toslaSettings.FirstOrDefault(x => x.ParamDef == "companyId").ParamVal);

                var member = _memberManager.GetMember(data.phoneNumber);

                if (member == null)
                {
                    var newMember = new Entities.Concrete.Member();

                    newMember.CUser = newMember.MUser = "00000000-0000-0000-0000-000000000000";
                    newMember.Phone = data.phoneNumber;
                    newMember.IDMemberType = "00000000-0000-0000-0000-000000000000";
                    newMember.StatusFlag = true;
                    newMember.IdentityNr = "11111111111";
                    newMember.ServiceID = jobj["service_id"].ToString();
                    newMember.Name = "Tosla Üyesi";
                    newMember.Email = "otomatik@otomatik.com";
                    newMember.BirthYear = "2024";
                    newMember.CDate = DateTime.Now;
                    var response = _memberManager.Insert(newMember);

                    if (response.Status == "OK")
                    {
                        var creditCardPaymentNotification = new CreditCardPaymentNotification
                        {
                            TransactionID = data.processId,
                            ActionDate = DateTime.Now.Date,
                            ActionTime = DateTime.Now.ToString("HH:MM"),
                            Amount = data.amount,
                            Description = "Toslaya İletildi",
                            IDMember = response.Data.ToString(),
                            Phone = data.phoneNumber,
                            Status = (byte)Enums.StatusType.Pending,
                            SenderName = " ",
                            SenderIdentityNr = "11111111111",
                            ServiceID = jobj["service_id"].ToString(),
                            CardNumber = " ",
                            ParamCommission =  0,
                            PayNKolayCommission =  0,
                            MUser = "00000000-0000-0000-0000-000000000000",
                            CreditCardPaymentMethodID = (byte)Enums.CreditCardPaymentMethodType.Tosla,
                            CardTypeId = (int)Enums.CreditCardType.Tosla
                        };

                        var responseInsert = _creditCardPaymentNotificationManager.Insert(creditCardPaymentNotification);

                        if (responseInsert.Status == "OK")
                        {
                            data.phoneNumber = "9" + data.phoneNumber;

                            var toslaPaymentRequest = ToslaGetRefCodeRequest.GetRefCode(data);

                            callbackEntity.TransactionID = creditCardPaymentNotification.TransactionID;
                            callbackEntity.ServiceType = "TOSLA";
                            callbackEntity.IDCompany = companyIntegration.ID;
                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(toslaPaymentRequest, opt);
                            callbackEntity.TransactionType = "TOSLA GET REF CODE REQUEST RESPONSE";
                            _callbackResponseLogManager.Insert(callbackEntity);

                            if (toslaPaymentRequest != null && toslaPaymentRequest.Status == "OK" && toslaPaymentRequest.Data != null && toslaPaymentRequest.Data.result && toslaPaymentRequest.Data.refCode != null)
                            {
                                creditCardPaymentNotification.TransactionReferenceCode = toslaPaymentRequest.Data.refCode;
                                creditCardPaymentNotification.ID = responseInsert.Data.ToString();
                                creditCardPaymentNotification.MUser = "00000000-0000-0000-0000-000000000000";
                                creditCardPaymentNotification.MDate = DateTime.Now;
                                _creditCardPaymentNotificationManager.SetStatus(creditCardPaymentNotification);

                                return Ok(new GenericResponseApi
                                {
                                    Status = "OK",
                                    ResponseStatus = 1,
                                    Message = "İstek Toslaya İletildi"
                                });
                            }
                            else
                            {
                                if (toslaPaymentRequest != null && toslaPaymentRequest.Data != null && toslaPaymentRequest.Data.errorCode == 3)
                                {
                                    creditCardPaymentNotification.Description = $"{data.phoneNumber[1..]} numaralı kullanıcının bekleyen ödeme talebi mevcut";
                                    creditCardPaymentNotification.Status = (byte)Enums.StatusType.Canceled;
                                    creditCardPaymentNotification.MUser = "00000000-0000-0000-0000-000000000000";
                                    creditCardPaymentNotification.MDate = DateTime.Now;
                                    creditCardPaymentNotification.ID = responseInsert.Data.ToString();
                                    _creditCardPaymentNotificationManager.SetStatus(creditCardPaymentNotification);

                                    return BadRequest(new GenericResponseApi
                                    {
                                        Status = "ERROR",
                                        ResponseStatus = 0,
                                        Message = $"{data.phoneNumber[1..]} numaralı kullanıcının bekleyen ödeme talebi mevcut"
                                    });
                                }
                                else
                                {
                                    creditCardPaymentNotification.Description = toslaPaymentRequest.Data.errorMessage == "Customer with given id not found" ? $"{data.phoneNumber[1..]} numaralı telefon tosla sisteminde bulunamadı" : toslaPaymentRequest.Data.errorMessage ?? "Bir Hata İle Karşılaşıldı";
                                    creditCardPaymentNotification.Status = (byte)Enums.StatusType.Canceled;
                                    creditCardPaymentNotification.MUser = "00000000-0000-0000-0000-000000000000";
                                    creditCardPaymentNotification.MDate = DateTime.Now;
                                    creditCardPaymentNotification.ID = responseInsert.Data.ToString();
                                    _creditCardPaymentNotificationManager.SetStatus(creditCardPaymentNotification);

                                    return BadRequest(new GenericResponseApi
                                    {
                                        Status = "ERROR",
                                        ResponseStatus = 0,
                                        Message = toslaPaymentRequest.Data.errorMessage == "Customer with given id not found" ? $"{data.phoneNumber[1..]} numaralı telefon tosla sisteminde bulunamadı" : toslaPaymentRequest.Data.errorMessage ?? "Bir Hata İle Karşılaşıldı"
                                    });
                                }
                            }
                        }
                        else
                        {
                            return BadRequest(new GenericResponseApi
                            {
                                Status = "ERROR",
                                ResponseStatus = 0,
                                Message = "Şu Anda İsteğiniz Devreye Alınamıyor. Lütfen Bizimle İletişime Geçiniz."
                            });
                        }
                    }
                }
                else
                {
                    var creditCardPaymentNotification = new CreditCardPaymentNotification
                    {
                        TransactionID = data.processId,
                        ActionDate = DateTime.Now.Date,
                        ActionTime = DateTime.Now.ToString("HH:MM"),
                        Amount = data.amount,
                        Description = "Toslaya İletildi",
                        IDMember = member.ID,
                        Phone = data.phoneNumber,
                        Status = (byte)Enums.StatusType.Pending,
                        SenderName = member.Name,
                        SenderIdentityNr = "11111111111",
                        ServiceID = jobj["service_id"].ToString(),
                        CardNumber = " ",
                        ParamCommission = 0,
                        PayNKolayCommission = 0,
                        MUser = "00000000-0000-0000-0000-000000000000",
                        CreditCardPaymentMethodID = (byte)Enums.CreditCardPaymentMethodType.Tosla,
                        CardTypeId = (int)Enums.CreditCardType.Tosla
                    };

                    var responseInsert = _creditCardPaymentNotificationManager.Insert(creditCardPaymentNotification);

                    if (responseInsert.Status == "OK")
                    {
                        data.phoneNumber = "9" + data.phoneNumber;
                        var toslaPaymentRequest = ToslaGetRefCodeRequest.GetRefCode(data);

                        callbackEntity.TransactionID = creditCardPaymentNotification.TransactionID;
                        callbackEntity.ServiceType = "TOSLA";
                        callbackEntity.IDCompany = companyIntegration.ID;
                        callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(toslaPaymentRequest, opt);
                        callbackEntity.TransactionType = "TOSLA GET REF CODE REQUEST RESPONSE";
                        _callbackResponseLogManager.Insert(callbackEntity);

                        if (toslaPaymentRequest != null && toslaPaymentRequest.Status == "OK" && toslaPaymentRequest.Data != null && toslaPaymentRequest.Data.result && toslaPaymentRequest.Data.refCode != null) 
                        {
                            creditCardPaymentNotification.TransactionReferenceCode = toslaPaymentRequest.Data.refCode;
                            creditCardPaymentNotification.ID = responseInsert.Data.ToString();
                            _creditCardPaymentNotificationManager.SetStatus(creditCardPaymentNotification);

                            return Ok(new GenericResponseApi
                            {
                                Status = "OK",
                                ResponseStatus = 1,
                                Message = "İstek Toslaya İletildi"
                            });
                        }
                        else
                        {
                            if(toslaPaymentRequest != null && toslaPaymentRequest.Data != null && toslaPaymentRequest.Data.errorCode == 3)
                            {
                                creditCardPaymentNotification.Description = $"{data.phoneNumber[1..]} numaralı kullanıcının bekleyen ödeme talebi mevcut";
                                creditCardPaymentNotification.Status = (byte)Enums.StatusType.Canceled;
                                creditCardPaymentNotification.MUser = "00000000-0000-0000-0000-000000000000";
                                creditCardPaymentNotification.MDate = DateTime.Now;
                                creditCardPaymentNotification.ID = responseInsert.Data.ToString();
                                _creditCardPaymentNotificationManager.SetStatus(creditCardPaymentNotification);

                                return BadRequest(new GenericResponseApi
                                {
                                    Status = "ERROR",
                                    ResponseStatus = 0,
                                    Message = $"{data.phoneNumber[1..]} numaralı kullanıcının bekleyen ödeme talebi mevcut"
                                });
                            }
                            else
                            {
                                creditCardPaymentNotification.Description = toslaPaymentRequest.Data.errorMessage == "Customer with given id not found" ? $"{data.phoneNumber[1..]} numaralı telefon tosla sisteminde bulunamadı" : toslaPaymentRequest.Data.errorMessage ?? "Bir Hata İle Karşılaşıldı";
                                creditCardPaymentNotification.Status = (byte)Enums.StatusType.Canceled;
                                creditCardPaymentNotification.MUser = "00000000-0000-0000-0000-000000000000";
                                creditCardPaymentNotification.MDate = DateTime.Now;
                                creditCardPaymentNotification.ID = responseInsert.Data.ToString();
                                _creditCardPaymentNotificationManager.SetStatus(creditCardPaymentNotification);

                                return BadRequest(new GenericResponseApi
                                {
                                    Status = "ERROR",
                                    ResponseStatus = 0,
                                    Message = toslaPaymentRequest.Data.errorMessage == "Customer with given id not found" ? $"{data.phoneNumber[1..]} numaralı telefon tosla sisteminde bulunamadı" : toslaPaymentRequest.Data.errorMessage ?? "Bir Hata İle Karşılaşıldı"
                                });
                            }
                        }
                    }
                    else
                    {
                        return BadRequest(new GenericResponseApi
                        {
                            Status = "ERROR",
                            ResponseStatus = 0,
                            Message = "Şu Anda İsteğiniz Devreye Alınamıyor. Lütfen Bizimle İletişime Geçiniz."
                        });
                    }
                }
            }
            catch { }

            return BadRequest();
        }


        [HttpPost("Test")]
        public IActionResult Test()
        {
            var kuveyTurkIntegrationValues = _settingDAL.GetList(null).Where(x => x.ParamType == "KuveytTurkTransfer").ToList();

            var kuveytTurkTransferRequestModel = new KuveytTurkTransferRequestModel()
            {
                CorporateWebUserName = kuveyTurkIntegrationValues.FirstOrDefault(f => f.ParamDef == "web_username").ParamVal,
                SenderAccountSuffix = 1,
                TransferType = int.Parse(kuveyTurkIntegrationValues.FirstOrDefault(f => f.ParamDef == "transfer_type").ParamVal),
                MoneyTransferAmount = 110000.00m,
                ReceiverIBAN = "TR740020500009814244300002",
                ReceiverName = "STILPAY ELEKTRONİK TİCARET ANONİM ŞİRKETİ",
                MoneyTransferDescription = "",
                TransactionGuid = "d78e8bf2-6155-91a-856b-ab0201a82"
            };

            var kuveytTurkTransferResponse = KuveytTurkMoneyTransfer.MoneyTranfer(kuveytTurkTransferRequestModel);


            return Ok(kuveytTurkTransferResponse);
        }


        /// <summary>
        /// Transaction Sonucunu Döner
        /// </summary>
        /// <param name="service_id"></param>
        /// <param name="ciphered"></param>
        /// <param name="data"></param>
        /// <remarks>     
        /// SAMPLE REQUEST:
        /// {
        /// 
        ///     "service_id": "0000",
        ///     "ciphered": "aaabbbd8f80196fbe36a2bffca8ef4bae576",
        ///     "data" :{
        ///        "transactionID"     :"xxxxx" 
        ///     }
        ///     
        /// }
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpPost("gettransaction")]
        public IActionResult GetTransation([FromBody] JObject jobj)
        {
            try
            {
                JToken tokenServiceId = jobj["service_id"];
                if (tokenServiceId == null || string.IsNullOrEmpty(tokenServiceId.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Service ID Gönderilmedir" });
                }
                JToken tokenCiphered = jobj["ciphered"];
                if (tokenCiphered == null || string.IsNullOrEmpty(tokenCiphered.ToString()))
                {
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Ciphered Gönderilmedir" });
                }

                string transactionID = "";
                JToken tokenData = jobj["data"];
                if (tokenData != null && !string.IsNullOrEmpty(tokenData.ToString()))
                {
                    var data = JsonConvert.DeserializeAnonymousType(jobj.GetValue("data").ToString(), new { transactionID = "" });

                    if (string.IsNullOrEmpty(data.transactionID))
                    {
                        return BadRequest(new GenericResponseApi
                        {
                            ResponseStatus = 0,
                            Status = "ERROR",
                            Message = "transactionID zorunlu alan."
                        });
                    }

                    transactionID = data.transactionID;
                }
                else
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "data Alanı Gönderilmedir" });

                var companyIntegration = _companyIntegrationManager.GetByServiceId(jobj.GetValue("service_id").ToString());

                if (companyIntegration == null)
                    return BadRequest(new GenericResponseApi { ResponseStatus = 0, Status = "ERROR", Message = "Service ID Hatalı. Lütfen Bizimle İletişime Geçiniz" });

                var encrypted = tMD5Manager.EncryptBasic(companyIntegration.SecretKey);

                if (!encrypted.Equals(jobj.GetValue("ciphered").ToString()))
                {
                    return Unauthorized();
                }
                else
                {
                    var paymentNotifications = _paymentNotificationManager.GetSingleByTransactionID(transactionID);

                    if (paymentNotifications != null)
                    {
                        var dataCallback = new
                        {
                            status_code = paymentNotifications.Status == (byte)Enums.StatusType.Pending ? "Pending" : paymentNotifications.Status == (byte)Enums.StatusType.Confirmed ? "Confirmed" : "Canceled",
                            service_id = paymentNotifications.ServiceID,
                            status_type = 0,
                            ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                            data = new { transaction_id = paymentNotifications.TransactionID, sp_transactionNr = paymentNotifications.TransactionNr, amount = paymentNotifications.Amount, sp_id = paymentNotifications.ID, message = paymentNotifications.Description },
                            user_entered_data = new { member = paymentNotifications.Member, sender_name = paymentNotifications.SenderName, action_date = paymentNotifications.ActionDate, action_time = paymentNotifications.ActionTime, amount = paymentNotifications.Amount, user_ip = paymentNotifications.MemberIPAddress, user_port = paymentNotifications.MemberPort }
                        };

                        return Ok(new GenericResponseApi
                        {
                            ResponseStatus = 1,
                            Status = "OK",
                            Message = "İşlem Başarılı",
                            Data = dataCallback
                        });
                    }
                    else
                    {
                        var creditCardPaymentNotifications = _creditCardPaymentNotificationManager.GetSingleByTransactionID(transactionID);

                        if (creditCardPaymentNotifications != null)
                        {
                            var dataCallback = new
                            {
                                status_code = creditCardPaymentNotifications.Status == (byte)Enums.StatusType.Pending ? "Pending" : creditCardPaymentNotifications.Status == (byte)Enums.StatusType.Confirmed ? "Confirmed" : "Canceled",
                                status_type = 1,
                                service_id = creditCardPaymentNotifications.ServiceID,
                                ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                data = new { transaction_id = creditCardPaymentNotifications.TransactionID, sp_transactionNr = creditCardPaymentNotifications.TransactionNr, amount = creditCardPaymentNotifications.Amount, sp_id = creditCardPaymentNotifications.ID, message = creditCardPaymentNotifications.Description },
                                user_entered_data = new { member = creditCardPaymentNotifications.Member, sender_name = creditCardPaymentNotifications.SenderName, action_date = creditCardPaymentNotifications.ActionDate, action_time = creditCardPaymentNotifications.ActionTime, creditCard = creditCardPaymentNotifications.CardNumber, amount = creditCardPaymentNotifications.Amount, user_ip = creditCardPaymentNotifications.MemberIPAddress, user_port = creditCardPaymentNotifications.MemberPort }
                            };

                            return Ok(new GenericResponseApi
                            {
                                ResponseStatus = 1,
                                Status = "OK",
                                Message = "İşlem Başarılı",
                                Data = dataCallback
                            });
                        }
                        else
                            return BadRequest(new GenericResponseApi
                            {
                                ResponseStatus = 0,
                                Status = "ERROR",
                                Message = "İşlem bulunamadı."
                            });
                    }
                }

            }
            catch { }

            return BadRequest();
        }
    }
}
