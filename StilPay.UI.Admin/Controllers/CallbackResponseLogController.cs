using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL.Abstract;
using StilPay.BLL;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System.Collections.Generic;
using StilPay.UI.Admin.Models;
using Newtonsoft.Json.Linq;
using DocumentFormat.OpenXml.ExtendedProperties;
using System;
using System.Text;
using System.Linq;
using StilPay.Utility.Worker;
using System.Security.Cryptography.Xml;
using RestSharp;
using StilPay.Utility.Models;
using System.Text.Json;
using StilPay.Entities;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using StilPay.BLL.Concrete;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize]
    public class CallbackResponseLogController : BaseController<CallbackResponseLog>
    {
        private readonly ICallbackResponseLogManager _callbackResponseLogManager;
        private readonly ICompanyIntegrationManager _companyIntegrationManager;

        public CallbackResponseLogController(ICallbackResponseLogManager manager, ICallbackResponseLogManager callbackResponseLogManager, ICompanyIntegrationManager companyIntegrationManager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _callbackResponseLogManager = callbackResponseLogManager;
            _companyIntegrationManager = companyIntegrationManager;
        }

        public override IBaseBLL<CallbackResponseLog> Manager()
        {
            return _callbackResponseLogManager;
        }

        [HttpPost]
        public IActionResult GetData()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = _callbackResponseLogManager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("ServiceType", Enums.FieldType.NVarChar, null),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["IDCompany"].ToString()) ? "0" : HttpContext.Request.Form["IDCompany"].ToString() == "all" ? null : HttpContext.Request.Form["IDCompany"].ToString()),
                 new FieldParameter("StartDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString())),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString())),
                new FieldParameter("TransactionID", Enums.FieldType.DateTime, null),
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
        public IActionResult GetCallbacks([FromBody] JObject jObj)
        {
            var list = _callbackResponseLogManager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("ServiceType", Enums.FieldType.NVarChar, null),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar,  null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("TransactionID", Enums.FieldType.NVarChar, jObj["TransactionID"].ToString() )
            });
            foreach (var item in list)
            {
                var jsonConverter = Newtonsoft.Json.JsonConvert.DeserializeObject(item.Callback);
                item.Callback = jsonConverter.ToString();
            }
            return Json(list);
        }

        [HttpPost]
        public IActionResult SendCallbackAgain(string transactionId, string serviceId, string idCompany)
        {
            int responseStatus = 0;
            string callbackUrl = "";
            string callbackTag = "";

            var companyIntegration = serviceId == null ? _companyIntegrationManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, idCompany) }) : _companyIntegrationManager.GetByServiceId(serviceId);

            var callback = _callbackResponseLogManager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("ServiceType", Enums.FieldType.NVarChar, "STILPAY"),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar,  companyIntegration.ID),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("TransactionID", Enums.FieldType.NVarChar, transactionId )
            }).Last();

            if(callback != null)
            {
                var deserializeSettings = new JsonSerializerSettings
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeHtml // Unicode karakterlerini kullan
                };

                var deserialize = JsonConvert.DeserializeObject(callback.Callback, deserializeSettings);

                JObject dataObject = (JObject)deserialize;

                if(dataObject["status_type"].ToString() == "2")
                {
                    callbackUrl = companyIntegration.WithdrawalRequestCallback;
                    callbackTag = "withdrawal";
                }
                else
                {
                    callbackTag = "transaction";

                    // reference_nr alanını kontrol et
                    if (dataObject["data"]["reference_nr"] != null)
                    {
                        callbackUrl = tSQLBankManager.GetCompanyAutoNotificationSettingByIDCompany(companyIntegration.ID).CallbackUrl;
                    }
                    else
                    {
                        callbackUrl = companyIntegration.CallbackUrl;
                    }
                }

                var response = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(callbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { callbackTag, deserialize } });

                if (response != null && response.Result != null && !string.IsNullOrEmpty(response.Result.Status))
                {
                    responseStatus = response.Result.Status switch
                    {
                        "OK" => 1,
                        "RED" => 2,
                        "ERROR" => 3,
                        _ => 0,
                    };
                }

                var IDout = tSQLBankManager.AddCallbackResponseLog(transactionId, "STILPAY", deserialize.ToString(), companyIntegration.ID, callback.TransactionType, responseStatus);

                if(IDout != null)
                {
                    var insertedEntity = _callbackResponseLogManager.GetList(new List<FieldParameter>()
                    {
                        new FieldParameter("ServiceType", Enums.FieldType.NVarChar, "STILPAY"),
                        new FieldParameter("IDCompany", Enums.FieldType.NVarChar,  companyIntegration.ID),
                        new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                        new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                        new FieldParameter("TransactionID", Enums.FieldType.NVarChar, transactionId )
                    }).Last();

                    return Json(new GenericResponse { Status = "OK", Data = insertedEntity, Message = "İşlem başarılı." });
                }
                else
                    return Json(new GenericResponse { Status = "ERROR", Message = "Callback kayıt edilemedi." });
            }
            else
                return Json(new GenericResponse { Status = "ERROR", Message = "Daha önce atılan callback bulunamadı." });
        }
    }
}
