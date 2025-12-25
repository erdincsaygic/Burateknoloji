using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "CompanyFinanceTransaction")]
    public class CompanyFinanceTransactionController : BaseController<CompanyFinanceTransaction>
    {
        private readonly ICompanyFinanceTransactionManager _manager;

        public CompanyFinanceTransactionController(ICompanyFinanceTransactionManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<CompanyFinanceTransaction> Manager()
        {
            return _manager;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override IActionResult Save(CompanyFinanceTransaction entity, IFormFile file)
        {
            if (!string.IsNullOrEmpty(entity.ID))

                return Json(Manager().Update(entity));
            else
                return Json(Manager().Insert(entity));
        }

        public override EditViewModel<CompanyFinanceTransaction> InitEditViewModel(string id = null)
        {
            var model = new EditViewModel<CompanyFinanceTransaction>();

            var entity = Manager().GetSingle(new List<FieldParameter>()
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, id)
            });
            model.entity = entity;
            return model;
        }
    }
}
