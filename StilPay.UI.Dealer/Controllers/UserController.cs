using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.UI.Dealer.Infrastructures;
using StilPay.UI.Dealer.Models;
using StilPay.Utility.Helper;
using System.Collections.Generic;

namespace StilPay.UI.Dealer.Controllers
{
    [Authorize(Roles = "Account")]
    public class UserController : BaseController<CompanyUser>
    {
        private readonly ICompanyUserManager _manager;

        public UserController(ICompanyUserManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<CompanyUser> Manager()
        {
            return _manager;
        }

        public override EditViewModel<CompanyUser> InitEditViewModel(string id = null)
        {
            var model = new EditViewModel<CompanyUser>();

            var entity = Manager().GetSingle(new List<FieldParameter>()
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, id)
            });

            model.entity = entity;

            return model;
        }

        public IActionResult MyAccount()
        {
            var model = InitEditViewModel(IDUser);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveMyAccount(CompanyUser entity)
        {
            entity.ID = IDUser;

            return Json(_manager.SaveMyAccount(entity));
        }

        public IActionResult Security()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(CompanyUser entity, int confirmCode)
        {
            var genericResponse = _httpContext.HttpContext.Session.ValidateConfirmCode("User_Security_ConfirmCode", confirmCode);
            if (genericResponse.Status == "ERROR")
                return Json(genericResponse);

            entity.ID = IDUser;

            return Json(_manager.ResetPassword(entity));
        }
    }
}
