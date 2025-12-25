using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;

namespace StilPay.UI.Dealer.Controllers
{
    [Authorize(Roles = "Visitor")]
    public class ApplicationController : BaseController<CompanyApplication>
    {
        ICompanyApplicationManager _manager;

        public ApplicationController(ICompanyApplicationManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<CompanyApplication> Manager()
        {
            return _manager;
        }

        public override IActionResult Index()
        {
            var entity = _manager.GetSingle(new List<FieldParameter>{
                new FieldParameter("ID", Enums.FieldType.NVarChar, IDCompany)
            });

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CompanyApplication entity, IFormFile FileIdentityFrontSide, IFormFile FileIdentityBackSide, IFormFile FileTaxPlate, IFormFile FileSignatureCirculars, IFormFile FileTradeRegistryGazette, IFormFile FileAgreement)
        {
            if (FileIdentityFrontSide != null && FileIdentityFrontSide.Length > 0 && FileIdentityFrontSide.ContentType == "application/pdf")
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    FileIdentityFrontSide.CopyTo(ms);
                    entity.IdentityFrontSide = ms.ToArray();
                }
            }

            if (FileIdentityBackSide != null && FileIdentityBackSide.Length > 0 && FileIdentityBackSide.ContentType == "application/pdf")
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    FileIdentityBackSide.CopyTo(ms);
                    entity.IdentityBackSide = ms.ToArray();
                }
            }

            if (FileTaxPlate != null && FileTaxPlate.Length > 0 && FileTaxPlate.ContentType == "application/pdf")
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    FileTaxPlate.CopyTo(ms);
                    entity.TaxPlate = ms.ToArray();
                }
            }

            if (FileSignatureCirculars != null && FileSignatureCirculars.Length > 0 && FileSignatureCirculars.ContentType == "application/pdf")
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    FileSignatureCirculars.CopyTo(ms);
                    entity.SignatureCirculars = ms.ToArray();
                }
            }

            if (FileTradeRegistryGazette != null && FileTradeRegistryGazette.Length > 0 && FileTradeRegistryGazette.ContentType == "application/pdf")
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    FileTradeRegistryGazette.CopyTo(ms);
                    entity.TradeRegistryGazette = ms.ToArray();
                }
            }

            if (FileAgreement != null && FileAgreement.Length > 0 && FileAgreement.ContentType == "application/pdf")
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    FileAgreement.CopyTo(ms);
                    entity.Agreement = ms.ToArray();
                }
            }

            var response = _manager.Update(entity);

            return Json(response);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Frame()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public override IActionResult Save(CompanyApplication entity)
        {
            return base.Save(entity);
        }

    }
}
