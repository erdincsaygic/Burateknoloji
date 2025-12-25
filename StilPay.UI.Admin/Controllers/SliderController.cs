using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL.Abstract;
using StilPay.BLL;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Blog")]
    public class SliderController : BaseController<Slider>
    {
        private readonly ISliderManager _manager;

        public SliderController(ISliderManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<Slider> Manager()
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
        [ValidateAntiForgeryToken]
        public override IActionResult Save(Slider entity, IFormFile file)
        {
            if (file != null)
            {
                byte[] imageData = null;

                using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)file.Length);
                }
                string base64String = Convert.ToBase64String(imageData);
                entity.ImageUrl = "data:" + file.ContentType + ";base64," + base64String;
            }
            if (!string.IsNullOrEmpty(entity.ID))

                return Json(Manager().Update(entity));
            else
                return Json(Manager().Insert(entity));
        }
    }
}
