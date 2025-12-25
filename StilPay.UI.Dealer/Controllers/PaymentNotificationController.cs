using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.BLL.Concrete;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace StilPay.UI.Dealer.Controllers
{
    [Authorize(Roles = "PaymentNotification")]
    public class PaymentNotificationController : BaseController<PaymentNotification>
    {
        private readonly IPaymentNotificationManager _manager;
        private readonly ICompanyBankManager _companyBankManager;
        private readonly ICompanyIntegrationManager _companyIntegrationManager;
        private readonly IMemberManager _memberManager;

        public PaymentNotificationController(IPaymentNotificationManager manager, IHttpContextAccessor httpContext, ICompanyBankManager companyBankManager, ICompanyIntegrationManager companyIntegrationManager, IMemberManager memberManager) : base(httpContext)
        {
            _manager = manager;
            _companyBankManager = companyBankManager;
            _companyIntegrationManager = companyIntegrationManager;
            _memberManager = memberManager;
        }

        public override IBaseBLL<PaymentNotification> Manager()
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
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.All),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, IDCompany),
                new FieldParameter("IsAutoNotification", Enums.FieldType.Tinyint, null),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["IDMember"].ToString()) ? null : HttpContext.Request.Form["IDMember"].ToString()),
                new FieldParameter("StartDate",  Enums.FieldType.DateTime, string.IsNullOrEmpty(HttpContext.Request.Form["StartDate"].ToString()) ? (DateTime?)null : Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString())),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, string.IsNullOrEmpty(HttpContext.Request.Form["StartDate"].ToString()) ? (DateTime?)null : Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString())),
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

        [HttpGet]
        public IActionResult Add()
        {
            var model = new PaymentNotification();

            var companyBanks = _companyBankManager.GetActiveList(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, IDCompany) }).ToList();

            if (companyBanks == null || companyBanks.Count() == 0)
            {
                companyBanks = _companyBankManager.GetList(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, IDCompany) }).ToList();
            }

            ViewBag.CompanyBanks = companyBanks;

            return View();
        }

        public override IActionResult Save(PaymentNotification entity)
        {
            if(entity == null) return Json(new GenericResponse { Status = "ERROR", Message = "Model eksik veya hatalı" });

            if (entity.Amount <= 0)
                return Json(new GenericResponse { Status = "ERROR", Message = "Tutar 0'dan büyük olmalı." });
               
            entity.Status = (byte)Enums.StatusType.Pending;
            entity.IsAutoNotification = true;

            string transactionId = DateTime.Now.Ticks.ToString("D16");
            var integ = _companyIntegrationManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar,IDCompany) });

            entity.TransactionID = transactionId;
            entity.ServiceID = integ.ServiceID;
            entity.Phone = Phone;

            var member = _memberManager.GetMember(Phone);
            if (member == null)
            {
                var newMember = new Entities.Concrete.Member();

                newMember.CUser = newMember.MUser = "00000000-0000-0000-0000-000000000000";
                newMember.Phone = Phone;
                newMember.StatusFlag = true;
                newMember.IDMemberType = "00000000-0000-0000-0000-000000000000";
                newMember.IdentityNr = "11111111111";
                newMember.ServiceID = integ.ServiceID;
                newMember.Name = entity.SenderName;
                newMember.Email = "otomatik@otomatik.com";
                newMember.BirthYear = "2023";
                newMember.CDate = DateTime.Now;
                var response2 = _memberManager.Insert(newMember);

                if (response2.Status == "OK")
                {
                    entity.IDMember = response2.Data.ToString();
                    entity.SenderIdentityNr = "11111111111";
                }
            }
            else
            {
                entity.IDMember = member.ID;
                entity.SenderIdentityNr = "11111111111";
            }

            GenericResponse response = _manager.Insert(entity);
            if (response.Status.Equals("OK"))
            {
                var connection = _httpContext.HttpContext.Connection;
                _manager.SetMemberIPAdress(response.Data.ToString(), connection.RemoteIpAddress.ToString(), connection.RemotePort.ToString());

            }

            return Json(response);
        }
    }
}
