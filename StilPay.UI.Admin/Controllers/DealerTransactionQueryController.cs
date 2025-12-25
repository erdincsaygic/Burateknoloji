using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StilPay.BLL.Abstract;
using StilPay.BLL;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System.Collections.Generic;
using System;
using StilPay.UI.Admin.Models;
using StilPay.Entities.Dto;
using static StilPay.Utility.Helper.Enums;
using System.Linq;
using StilPay.BLL.Concrete;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Dealer")]
    public class DealerTransactionQueryController : BaseController<CompanyTransaction>
    {
        private readonly ICompanyTransactionManager _manager;
        private readonly IPaymentNotificationManager _paymentNotificationManager;

        public DealerTransactionQueryController(ICompanyTransactionManager manager, IPaymentNotificationManager paymentNotificationManager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
            _paymentNotificationManager = paymentNotificationManager;
        }

        public override IBaseBLL<CompanyTransaction> Manager()
        {
            return _manager;
        }

        [HttpPost]
        public IActionResult Search(string queryParameter)
        {
            var model = new DealerTransactionQueryEditViewModel()
            {
                DealerTransactionQuery = _manager.GetRecordsByQueryParameter(queryParameter)
            };

            if(model.DealerTransactionQuery.Count == 0 || (model.DealerTransactionQuery.Count == 1 && model.DealerTransactionQuery.Any(x => x.TableWithTheTransaction == (byte)Enums.TableWithTheTransaction.CompanyRebateRequests)))        
                return Json(new GenericResponse { Status = "ERROR", Message = "Kayıt Bulunamadı" , Data = model });
            
            var hasRebateEntity = model.DealerTransactionQuery.FirstOrDefault(x => x.TableWithTheTransaction == (byte)Enums.TableWithTheTransaction.CompanyRebateRequests);

            foreach (var item in model.DealerTransactionQuery.Where(x => x.TableWithTheTransaction != (byte)Enums.TableWithTheTransaction.CompanyRebateRequests))
            {
                switch (item.TableWithTheTransaction)
                {
                    case (byte)Enums.TableWithTheTransaction.PaymentNotification:
                        if (item.Status == (byte)Enums.StatusType.Pending)
                            model.Url = $"/PaymentNotification/Edit/{item.ID}";
                        else
                        {
                            if (hasRebateEntity != null)
                            {
                                if (hasRebateEntity.Status == (byte)Enums.StatusType.Pending)
                                    model.SecondUrl = $"/DealerRebateRequest/Edit/{hasRebateEntity.ID}";                           
                                else
                                    model.SecondUrl = $"/DealerRebateTransaction/Edit/{hasRebateEntity.ID}";
                            }

                            model.Url = $"/DealerPaymentTransaction/Edit/{item.ID}";
                        }
                        break;

                    case (byte)Enums.TableWithTheTransaction.CreditCardPaymentNotification:
                        if (item.Status == (byte)Enums.StatusType.Pending)
                            model.Url = $"/CreditCardPaymentNotification/Edit/{item.ID}";
                        else
                        {
                            if (hasRebateEntity != null)
                            {
                                if (hasRebateEntity.Status == (byte)Enums.StatusType.Pending)
                                    model.SecondUrl = $"/DealerRebateRequest/Edit/{hasRebateEntity.ID}";
                                else
                                    model.SecondUrl = $"/DealerRebateTransaction/Edit/{hasRebateEntity.ID}";
                            }

                            model.Url = $"/DealerCreditCardTransaction/Edit/{item.ID}";
                        }
                        break;

                    case (byte)Enums.TableWithTheTransaction.ForeignCreditCardPaymentNotification:
                        if (item.Status == (byte)Enums.StatusType.Pending)
                            model.Url = $"/ForeignCreditCardPaymentNotification/Edit/{item.ID}";
                        else
                            model.Url = $"/DealerForeignCreditCardTransaction/Edit/{item.ID}";
                        break;
                            
                    case (byte)Enums.TableWithTheTransaction.CompanyWithdrawalRequests:
                        if (item.Status == (byte)Enums.StatusType.Pending)
                            model.Url = $"/DealerWithdrawalRequest/Edit/{item.ID}";
                        else
                            model.Url = $"/DealerWithdrawalTransaction/Edit/{item.ID}";
                        break;

                    //case (byte)Enums.TableWithTheTransaction.CompanyRebateRequests:
                    //    if (item.Status == (byte)Enums.StatusType.Pending)
                    //        model.Url = $"/DealerRebateRequest/Edit/{item.ID}";
                    //    else
                    //        model.Url = $"/DealerRebateTransaction/Edit/{item.ID}";
                    //    break;
                }
            }

            return Json( new GenericResponse { Data = model });
        }


        [HttpGet]
        public IActionResult CreditCardDetailedSearch()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreditCardDetailedSearchList()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var cardNumber = HttpContext.Request.Form["CardNumber"].ToString();
 
            if (!string.IsNullOrEmpty(cardNumber) && cardNumber.Contains("-"))
                cardNumber = $"{cardNumber[..4]}********{cardNumber[5..]}"; 
       

            var list = _manager.CreditCardDetailedSearch(new List<FieldParameter>()
            {
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["IDCompany"].ToString()) ? "0" : HttpContext.Request.Form["IDCompany"].ToString() == "all" ? null : HttpContext.Request.Form["IDCompany"].ToString()),
                new FieldParameter("Status", Enums.FieldType.Tinyint, string.IsNullOrEmpty(HttpContext.Request.Form["Status"].ToString()) ? (byte?)null : Convert.ToByte(HttpContext.Request.Form["Status"])),
                new FieldParameter("SenderName", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["SenderName"].ToString()) ? null : Convert.ToString(HttpContext.Request.Form["SenderName"])),
                new FieldParameter("SenderPhone", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["SenderPhone"].ToString()) ? null : Convert.ToString(HttpContext.Request.Form["SenderPhone"])),
                new FieldParameter("Amount", Enums.FieldType.Decimal, string.IsNullOrEmpty(HttpContext.Request.Form["Amount"].ToString()) ? 0 : Convert.ToDecimal(HttpContext.Request.Form["Amount"])),
                new FieldParameter("CardNumber", Enums.FieldType.NVarChar, cardNumber),
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
        public IActionResult PaymentNotificationSearchList()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = _paymentNotificationManager.GetList(new List<FieldParameter> {
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.Confirmed),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null),
                new FieldParameter("IsAutoNotification", Enums.FieldType.Tinyint, null),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("SenderName", Enums.FieldType.NVarChar, HttpContext.Request.Form["SenderName"].ToString()),
                new FieldParameter("PageLenght", Enums.FieldType.Int, length),
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

        [HttpGet]
        public IActionResult CustomerInfoDetailedSearch()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CustomerInfoDetailedSearchList()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];


            var list = _manager.CustomerInfoDetailedSearch(new List<FieldParameter>()
            {
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["IDCompany"].ToString()) ? "0" : HttpContext.Request.Form["IDCompany"].ToString() == "all" ? null : HttpContext.Request.Form["IDCompany"].ToString()),
                new FieldParameter("Status", Enums.FieldType.Tinyint, string.IsNullOrEmpty(HttpContext.Request.Form["Status"].ToString()) ? (byte?)null : Convert.ToByte(HttpContext.Request.Form["Status"])),
                new FieldParameter("CustomerName", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["CustomerName"].ToString()) ? null : Convert.ToString(HttpContext.Request.Form["CustomerName"])),
                new FieldParameter("CustomerPhone", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["CustomerPhone"].ToString()) ? null : Convert.ToString(HttpContext.Request.Form["CustomerPhone"])),
                new FieldParameter("CustomerEmail", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["CustomerEmail"].ToString()) ? null : Convert.ToString(HttpContext.Request.Form["CustomerEmail"])),
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

        [HttpGet]
        public IActionResult TransferDetailedSearch()
        {
            return View();
        }

        [HttpPost]
        public IActionResult TransferDetailedSearchList()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];


            var list = _manager.TransferDetailedSearch(new List<FieldParameter>()
            {
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["IDCompany"].ToString()) ? "0" : HttpContext.Request.Form["IDCompany"].ToString() == "all" ? null : HttpContext.Request.Form["IDCompany"].ToString()),
                new FieldParameter("Status", Enums.FieldType.Tinyint, string.IsNullOrEmpty(HttpContext.Request.Form["Status"].ToString()) ? (byte?)null : Convert.ToByte(HttpContext.Request.Form["Status"])),
                new FieldParameter("SenderName", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["SenderName"].ToString()) ? null : Convert.ToString(HttpContext.Request.Form["SenderName"])),
                new FieldParameter("SenderPhone", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["SenderPhone"].ToString()) ? null : Convert.ToString(HttpContext.Request.Form["SenderPhone"])),
                new FieldParameter("SenderReferenceNr", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["SenderReferenceNr"].ToString()) ? null : Convert.ToString(HttpContext.Request.Form["SenderReferenceNr"])),
               new FieldParameter("Amount", Enums.FieldType.Decimal, string.IsNullOrEmpty(HttpContext.Request.Form["Amount"].ToString()) ? 0 : Convert.ToDecimal(HttpContext.Request.Form["Amount"])),
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
    }
}
