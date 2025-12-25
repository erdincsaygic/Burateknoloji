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
    [Authorize(Roles = "Blog")]
    public class BlogController : BaseController<Blog>
    {
        private readonly IBlogManager _manager;

        public BlogController(IBlogManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<Blog> Manager()
        {
            return _manager;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override IActionResult Save(Blog entity, IFormFile file)
        {
            if (file!=null)
            {
                byte[] imageData = null;

                using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)file.Length);
                }
                string base64String = Convert.ToBase64String(imageData);
                entity.Image = base64String;
            }
            if (!string.IsNullOrEmpty(entity.ID))

                return Json(Manager().Update(entity));
            else
                return Json(Manager().Insert(entity));
        }

        public override EditViewModel<Blog> InitEditViewModel(string id = null)
        {
            var model = new EditViewModel<Blog>();

            var entity = Manager().GetSingle(new List<FieldParameter>()
            {
                new FieldParameter("ID", Enums.FieldType.NVarChar, id)
            });
            model.entity = entity;

            if (string.IsNullOrEmpty(id))
            {
                model.entity.StatusFlag = true;
                model.entity.BlogDate = DateTime.Today;
            }
            return model;
        }
    }
}
