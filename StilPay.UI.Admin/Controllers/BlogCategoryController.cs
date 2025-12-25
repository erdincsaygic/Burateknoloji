using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using System.Collections.Generic;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Blog")]
    public class BlogCategoryController : BaseController<BlogCategory>
    {
        private readonly IBlogCategoryManager _manager;

        public BlogCategoryController(IBlogCategoryManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<BlogCategory> Manager()
        {
            return _manager;
        }

        public override EditViewModel<BlogCategory> InitEditViewModel(string id = null)
        {
            var model = new EditViewModel<BlogCategory>();
            model.entity.StatusFlag = true;

            if (!string.IsNullOrEmpty(id))
            {
                var entity = Manager().GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                model.entity = entity;
            }

            return model;
        }
    }
}
