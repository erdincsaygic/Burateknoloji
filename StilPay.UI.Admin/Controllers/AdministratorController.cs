using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using System.Collections.Generic;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : BaseController<Administrator>
    {
        private readonly IAdministratorManager _manager;

        public AdministratorController(IAdministratorManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<Administrator> Manager()
        {
            return _manager;
        }

        public override EditViewModel<Administrator> InitEditViewModel(string id = null)
        {
            var isSuperAdmin = Manager().GetSingle(new List<FieldParameter>()
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, IDUser)
            });

            var model = new EditViewModel<Administrator>();

            var entity = Manager().GetSingle(new List<FieldParameter>()
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, id)
            });

            model.entity = entity;

            if (isSuperAdmin.Phone == "05319679872")
                model.entity.ShowRoles = true;
            else
                model.entity.ShowRoles = false;

            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override IActionResult Save(Administrator entity, IFormFile file)
        {
            if (!entity.ShowRoles)
            {

                var oldEntity = Manager().GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, entity.ID)
                });

                entity.AdministratorRoles = oldEntity.AdministratorRoles;
            }

            if (!string.IsNullOrEmpty(entity.ID))
                return Json(Manager().Update(entity));
            else
                return Json(Manager().Insert(entity));
        }

        public IActionResult InOut()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetInOuts()
        {

            var list = _manager.GetInOuts();

            return Json(list);
        }

        public IActionResult Log()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetLogs([FromBody] JObject jObj)
        {
            var list = _manager.GetLogs(jObj["IDAdministrator"].ToString());

            return Json(list);
        }

    }
}
