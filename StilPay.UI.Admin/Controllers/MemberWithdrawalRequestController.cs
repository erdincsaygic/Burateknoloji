using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using ZiraatBankPaymentService;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "PendingProcess")]
    public class MemberWithdrawalRequestController : BaseController<MemberWithdrawalRequest>
    {
        private readonly IMemberWithdrawalRequestManager _manager;

        public MemberWithdrawalRequestController(IMemberWithdrawalRequestManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<MemberWithdrawalRequest> Manager()
        {
            return _manager;
        }

        public IActionResult GetData()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            var list = Manager().GetList(new List<FieldParameter>()
            {
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.Pending),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetStatus(MemberWithdrawalRequest entity)
        {
            if (entity.IDBank == "08")
            {
                var ziraatService = new NkyParaTransferiWSSoapClient(NkyParaTransferiWSSoapClient.EndpointConfiguration.NkyParaTransferiWSSoap);
                var securedWebServiceHeader = new SecuredWebServiceHeader();

                var response = ziraatService.HavaleYapAsync(securedWebServiceHeader, "97736040", "5002", "", "", entity.IBAN.Replace(" ", ""), "TRY", entity.Amount.ToString(), $"{DateTime.Now.ToString()} Tarihinde {entity.Member} Kullanıcısı Çekim Talebi", $"{DateTime.Now.ToString()} Çekim", "", "", "", "", "", "", "", "", "", "", "").Result;

                if (response.HavaleYapResult.CevapKodu != "0")
                {
                    return Json(new GenericResponse { Status = "ERROR", Message = response.HavaleYapResult.CevapMesaji });
                }

                return Json(_manager.SetStatus(entity));
            }

            return Json(_manager.SetStatus(entity));
        }
    }
}
