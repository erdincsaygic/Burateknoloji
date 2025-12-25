using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL.Abstract;
using StilPay.BLL;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.UI.Admin.Models;
using System.Collections.Generic;
using StilPay.BLL.Concrete;
using StilPay.DAL.Concrete;
using System.Linq;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "SystemSetting")]
    public class SystemSettingController : BaseController<Setting>
    {
        private readonly IBankManager _bankManager;
        private readonly IPaymentInstitutionManager _paymentInstitutionManager;
        private readonly ICompanyBankAccountManager _companyBankAccountManager;
        private readonly SettingDAL _settingDAL = new SettingDAL();

        public SystemSettingController(IBankManager bankManager, IHttpContextAccessor httpContext, IPaymentInstitutionManager paymentInstitutionManager, ICompanyBankAccountManager companyBankAccountManager) : base(httpContext)
        {
            _bankManager = bankManager;
            _paymentInstitutionManager = paymentInstitutionManager;
            _companyBankAccountManager = companyBankAccountManager;
        }

        public override IBaseBLL<Setting> Manager()
        {
            throw new System.NotImplementedException();
        }

        public override IActionResult Index()
        {
            var model = new SystemSettingEditViewModel
            {
                Settings = _settingDAL.GetList(null).ToList(),
                PaymentInstitutions = _paymentInstitutionManager.GetList(null).Where(s => s.Show).ToList(),
                //Banks = _bankManager.GetBanksForIframeSetting(),
                CompanyBankAccounts = _companyBankAccountManager.GetActiveList(new List<FieldParameter>()
                {
                    new FieldParameter("IDCompany", Enums.FieldType.NVarChar, "1312E00F-E83E-45B4-85C6-892396D12331"),
                })
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetIFrameBank(CompanyBankAccount companyIncomeBank)
        {
            var entity = _companyBankAccountManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, companyIncomeBank.ID) });

            entity.IsActiveForIFrame = companyIncomeBank.IsActiveForIFrame;

            return Json(_companyBankAccountManager.Update(entity));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Setting setting)
        {
            try
            {
                if(setting.ParamDef == "EftEndTime" || setting.ParamDef == "EftStartTime")
                    setting.ActivatedForGeneralUse = true;

                var settings = _settingDAL.GetList(null);
                string res = _settingDAL.Update(setting);
            }
            catch (System.Exception ex)
            {
                return Json(new GenericResponse { Status = "ERROR", Message = ex.Message });
            }

            return Json("OK");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePaymentInstitution(string id, bool isActive, int consecutiveTransactionLimit)
        {
            try
            {
                return Json(_paymentInstitutionManager.Update(new PaymentInstitution() { ID = id, IsActive = isActive, ConsecutiveTransactionLimit = consecutiveTransactionLimit }));
            }
            catch (System.Exception ex)
            {
                return Json(new GenericResponse { Status = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateWithdrawalBank(string id, bool IsActiveByDefaultExpenseAccount)
        {
            try
            {
                return Json(_companyBankAccountManager.SetIsActiveByDefault(id, IsActiveByDefaultExpenseAccount));
            }
            catch (System.Exception ex)
            {
                return Json(new GenericResponse { Status = "ERROR", Message = ex.Message });
            }
        }
    }
}
