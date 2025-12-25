using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.DAL.Concrete;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Dealer")]
    public class DealerWithdrawalTransactionController : BaseController<CompanyWithdrawalRequest>
    {
        private readonly ICompanyWithdrawalRequestManager _manager;
        private readonly ICompanyTransactionManager _companyTransactionManager;
        private readonly SettingDAL _settingDAL = new SettingDAL();

        public DealerWithdrawalTransactionController(ICompanyWithdrawalRequestManager manager, IHttpContextAccessor httpContext, ICompanyTransactionManager companyTransactionManager) : base(httpContext)
        {
            _manager = manager;
            _companyTransactionManager = companyTransactionManager;
        }

        public override IBaseBLL<CompanyWithdrawalRequest> Manager()
        {
            return _manager;
        }

        [HttpPost]
        public IActionResult GetData()
        {
            var length = int.Parse(HttpContext.Request.Form["length"]);
            var start = int.Parse(HttpContext.Request.Form["start"]);
            var searchValue = HttpContext.Request.Form["search[value]"];

            
            var list = GetData(
                new FieldParameter("Status", Enums.FieldType.Tinyint, string.IsNullOrEmpty(HttpContext.Request.Form["Status"].ToString()) ? (byte?)null : Convert.ToByte(HttpContext.Request.Form["Status"])),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, string.IsNullOrEmpty(HttpContext.Request.Form["IDCompany"].ToString()) ? "0" : HttpContext.Request.Form["IDCompany"].ToString() == "all" ? null : HttpContext.Request.Form["IDCompany"].ToString()),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["StartDate"].ToString())), 
                new FieldParameter("EndDate", Enums.FieldType.DateTime, Convert.ToDateTime(HttpContext.Request.Form["EndDate"].ToString())), 
                new FieldParameter("PageLenght", Enums.FieldType.Int, length), 
                new FieldParameter("OffsetValue", Enums.FieldType.Int, start),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, searchValue)
            );

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
        public IActionResult Cancel(string id, string description, string password)
        {
            var systemPassword = _settingDAL.GetList(null).FirstOrDefault(x => x.ParamType == "WithdrawalRequestCancelApprovedTransaction" && x.ParamDef == "password").ParamVal;

            if (password != systemPassword) return Json(new GenericResponse { Status = "ERROR", Message = "Parola hatalı." });

            var entity = _manager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, id) });

            if(entity == null) return Json(new GenericResponse { Status = "ERROR", Message = "Bir hata ile karşılaşıldı. Veri bulunamadı." });

            if(entity.Status != (int)Enums.StatusType.Confirmed) return Json(new GenericResponse { Status = "ERROR", Message = "İşlem onaylı durumda olmadığı için iptali sağlanamaz." });

            var companyTransactionEntity = _companyTransactionManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, id) });

            if (companyTransactionEntity == null || companyTransactionEntity.IDActionType != ((int)Enums.ActionType.DealerWithdrawal).ToString()) return Json(new GenericResponse { Status = "ERROR", Message = "Cari hareketlerde kayıt bulunamadı." });

            entity.Description = description;
            entity.MDate = DateTime.Now;
            entity.MUser = IDUser;
            entity.CompanyBankAccountID = null;
            entity.Status = (int)Enums.StatusType.Canceled;
            entity.SIDBank = null;
            return Json(_manager.SetStatus(entity));
            
        }
    }
}
