using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using StilPay.BLL.Abstract;
using StilPay.Entities;
using System;
using System.Linq;
using System.Security.Claims;

namespace StilPay.UI.Admin.Infrastructures
{
    public class ActionFilter : IActionFilter
    {
        private readonly IAdministratorManager _manager;

        public ActionFilter(IAdministratorManager manager)
        {
            _manager = manager;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var methodType = context.HttpContext.Request.Method;

            if (methodType == "POST")
            {
                var param = context.ActionArguments.FirstOrDefault(p => p.Value is Entity);
                if (param.Value != null)
                {
                    ((Entity)param.Value).CDate = DateTime.Now;
                    ((Entity)param.Value).MDate = DateTime.Now;

                    var claim = context.HttpContext.User.FindFirst(f => f.Type == ClaimTypes.Sid);
                    if (claim != null)
                    {
                        string id = claim.Value;
                        ((Entity)param.Value).CUser = id;
                        ((Entity)param.Value).MUser = id;
                    }
                }

                param = context.ActionArguments.FirstOrDefault(p => p.Value is CompanyEntity);
                if (param.Value != null)
                {
                    var claim = context.HttpContext.User.FindFirst(f => f.Type == ClaimTypes.GroupSid);

                    if (claim != null)
                    {
                        string idCompany = claim.Value;
                        ((CompanyEntity)param.Value).IDCompany = idCompany;
                    }
                }
            }

            if (methodType == "GET" && _manager != null && context.HttpContext.Request.Headers["x-requested-with"] != "XMLHttpRequest")
            {
                var claim = context.HttpContext.User.FindFirst(f => f.Type == ClaimTypes.Sid);
                if (claim != null)
                {
                    var idAdministrator = claim.Value;
                    _manager.RefreshExitDate(idAdministrator);
                }
            }
        }
    }
}
