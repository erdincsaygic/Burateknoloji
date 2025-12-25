using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL.Abstract;
using StilPay.BLL.Concrete;
using StilPay.Entities.Concrete;
using StilPay.UI.WebSite.Areas.Panel.Models;
using StilPay.Utility.Helper;
using System.Collections.Generic;

namespace StilPay.UI.Controllers
{
    public class AnasayfaController : Controller
    {
        private readonly ISliderManager _sliderManager;
        protected readonly IHttpContextAccessor _httpContext;

        public AnasayfaController(ISliderManager sliderManager, IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
            _sliderManager = sliderManager;
        }

        public IActionResult Index()
        {
            var entity = _sliderManager.GetActiveList(null);

            ViewBag.Sliders = entity;

            return View();
        }

    }
}
