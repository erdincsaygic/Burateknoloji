using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL.Abstract;
using StilPay.BLL;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System.Collections.Generic;
using System.Linq;
using StilPay.UI.WebSite.Models;

namespace StilPay.UI.WebSite.Controllers
{
    public class BlogController : Controller
    {
        protected readonly IHttpContextAccessor _httpContext;
        private readonly IBlogCategoryManager _categorymanager;
        private readonly IBlogManager _blogmanager;
        public BlogController(IBlogCategoryManager categorymanager, IBlogManager blogmanager,IHttpContextAccessor httpContext)
        {
            _categorymanager = categorymanager;
            _blogmanager = blogmanager;
            _httpContext = httpContext;
        }


        [HttpGet]
        public virtual IActionResult Index()
        {
            List<Blog> blog = GetDataBlog();
            ViewBag.Category = GetDataCategory();
            return View(blog);
        }    
        [HttpGet]
        public virtual IActionResult Detay(string id)
        {
            EditViewModel<Blog> model = InitEditViewModel(id);
            Blog detail = model.entity;
            return View(detail);
        }
        [NonAction]
        public virtual EditViewModel<Blog> InitEditViewModel(string id = null)
        {
            var model = new EditViewModel<Blog>();

            if (!string.IsNullOrEmpty(id))
            {
                var entity = _blogmanager.GetSingle(new List<FieldParameter>()
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id)
                });

                model.entity = entity;
            }

            return model;
        }
        [NonAction]
        protected List<BlogCategory> GetDataCategory(params FieldParameter[] prmtrs)
        {
            var list = _categorymanager.GetList(prmtrs.ToList());

            return list;
        }
        [NonAction]
        protected List<Blog> GetDataBlog(params FieldParameter[] prmtrs)
        {
            var list = _blogmanager.GetList(prmtrs.ToList());

            return list;
        }
    }
}
