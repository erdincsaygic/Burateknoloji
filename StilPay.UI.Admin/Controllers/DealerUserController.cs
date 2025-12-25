using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL.Abstract;
using StilPay.BLL;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System.Collections.Generic;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Dealer")]
    public class DealerUserController : BaseController<CompanyUser>
    {
        private readonly ICompanyUserManager _manager;

        public DealerUserController(ICompanyUserManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<CompanyUser> Manager()
        {
            return _manager;
        }

        [HttpGet]
        public IActionResult DealerUserDetail(string id) 
        {
            var entity = Manager().GetSingle(new List<FieldParameter>()
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, id)
            });

            return View(entity);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(CompanyUser companyUser)
        {
            var entity = Manager().GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, companyUser.ID) });
            
            entity.Name = companyUser.Name;
            entity.IPAddress = companyUser.IPAddress;
            entity.Password = companyUser.Password;
            entity.Email = companyUser.Email;
            entity.Phone = companyUser.Phone;
            entity.StatusFlag = companyUser.StatusFlag;
            

            return Json(Manager().Update(entity));
        }
    }
}
