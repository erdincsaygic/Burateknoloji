using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;using Microsoft.VisualBasic;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.BLL.Concrete;
using StilPay.DAL.Concrete;
using StilPay.Entities.Concrete;
using StilPay.UI.Dealer.Models;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StilPay.UI.Dealer.Controllers
{
    [Authorize(Roles = "Account")]
    public class DealerController : BaseController<Company>
    {
        private readonly ICompanyManager _manager;
        private readonly ICompanyIntegrationManager _integrationManager;
        private readonly ICompanyBankManager _companyBankManager;
        private readonly IBankManager _bankManager;
        private readonly ICompanyPaymentInstitutionManager _companyPaymentInstitutionManager;


        public DealerController(ICompanyManager manager, ICompanyIntegrationManager integrationManager, ICompanyBankManager companyBankManager, IHttpContextAccessor httpContext, IBankManager bankManager, ICompanyPaymentInstitutionManager companyPaymentInstitutionManager) : base(httpContext)
        {
            _manager = manager;
            _integrationManager = integrationManager;
            _companyBankManager = companyBankManager;
            _bankManager = bankManager;
            _companyPaymentInstitutionManager = companyPaymentInstitutionManager;
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
                model.entity = _manager.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                model.Integration = _integrationManager.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                model.CompanyPaymentInstitutions = _companyPaymentInstitutionManager.GetList(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                model.CompanyBanks = _companyBankManager.GetActiveList(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, id) }).ToList();

            }

            return model;
        }

        public IActionResult Settings()
        {
            var model = InitEditViewModel(IDCompany);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveIntegration(CompanyIntegration integration)
        {
            return Json(_integrationManager.Update(integration));
        }

        public IActionResult Information()
        {
            var model = _manager.GetSingle(new List<FieldParameter>()
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, IDCompany)
            });

            return View(model);
        }

        public IActionResult Integration()
        {
            var model = _integrationManager.GetSingle(new List<FieldParameter>()
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, IDCompany)
            });

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCompanyPaymentInstitution(string paymentInstitutionId, bool value)
        {
            try
            {
                return Json(_companyPaymentInstitutionManager.Update(new CompanyPaymentInstitution()
                {
                    PaymentInstitutionID = paymentInstitutionId,
                    ID = IDCompany,
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
    }
}