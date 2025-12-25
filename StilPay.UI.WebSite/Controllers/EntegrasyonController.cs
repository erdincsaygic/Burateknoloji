using Microsoft.AspNetCore.Mvc;
using System.Net;
using System;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace StilPay.UI.WebSite.Controllers
{
    public class EntegrasyonController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult IFrame()
        {
            return View("IFrame");
        }
        public IActionResult IFrameTransfer()
        {
            return View("IFrameTransfer");
        }
        public IActionResult IFrameCreditCard()
        {
            return View("IFrameCreditCard");
        }
        public IActionResult IFrameForeignCreditCard()
        {
            return View("IFrameForeignCreditCard");
        }

        public IActionResult CallbackResponseInfo()
        {
            return View("CallbackResponseInfo");
        }
        public IActionResult AutoCallback()
        {
            return View("AutoCallback");
        }

        public IActionResult IFrameGetToken()
        {
            return View("IFrameGetToken");
        }

        public IActionResult Withdrawal()
        {
            return View("Withdrawal");
        }

        public IActionResult ExampleCode()
        {
            return View("ExampleCode");
        }

        public IActionResult DownloadRarFile()
        {
            string rarFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "integration", "postman-collection", "STILPAY.postman_collection.rar");

            if (System.IO.File.Exists(rarFilePath))
            {
                var fileStream = System.IO.File.OpenRead(rarFilePath);
                var contentType = "application/x-rar-compressed";
                var fileDownloadName = "STILPAY.postman_collection.rar";

                return File(fileStream, contentType, fileDownloadName);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
