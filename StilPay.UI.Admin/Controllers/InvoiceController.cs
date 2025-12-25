using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Invoice")]
    public class InvoiceController : BaseController<CompanyInvoice>
    {
        private readonly ICompanyInvoiceManager _manager;

        public InvoiceController(ICompanyInvoiceManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<CompanyInvoice> Manager()
        {
            return _manager;
        }

        [HttpPost]
        public IActionResult GetData()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = _manager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["IDCompany"].ToString()) ? "0" : HttpContext.Request.Form["IDCompany"].ToString() == "all" ? null : HttpContext.Request.Form["IDCompany"].ToString()),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString())),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString())),
                new FieldParameter("PageLength", Enums.FieldType.Int, length),
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
        public IActionResult SendInvoiceToIntegrator(string idInvoice)
        {
            return Json(_manager.SendInvoiceToIntegrator(idInvoice));
        }

        [HttpPost]
        public IActionResult UpdateIncoiveStatus(string idInvoice, int status)
        {
            var entity = _manager.GetSingle(new List<FieldParameter>()
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, idInvoice),
            });

            entity.MUser = IDUser;
            entity.MDate = DateTime.Now;
            entity.Status = (byte)status;
            return Json(_manager.Update(entity));
        }

        [HttpPost]
        public IActionResult UpdateIncoive(string idInvoice, decimal netAmount, decimal taxAmount, decimal totalAmount, decimal exchangeRate)
        {
            var entity = _manager.GetSingle(new List<FieldParameter>()
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, idInvoice),
            });

            entity.NetAmount = netAmount == 0 ? entity.NetAmount : netAmount;
            entity.TaxAmount = taxAmount == 0 ? entity.TaxAmount : taxAmount;
            entity.TotalAmount = totalAmount == 1 ? entity.TotalAmount : totalAmount;
            entity.ExchangeRate = exchangeRate == 0 ? entity.ExchangeRate : exchangeRate;
            entity.MDate = DateTime.Now;
            entity.MUser = IDUser;

            var response = _manager.Update(entity);

            if(response.Status == "OK")
                return Json(new GenericResponse { Status = "OK", Message= "İşlem Başarılı", Data = entity });

            else
                return Json(new GenericResponse { Status = "ERROR", Message = response.Message });

        }


        [HttpGet]
        public string ExportExcel(string idInvoice)
        {
            return _manager.ExportExcel(idInvoice);
        }

        [HttpGet]
        public string ExportPDF(string idInvoice)
        {
            return _manager.ExportPDF(idInvoice);
        }
    }
}
