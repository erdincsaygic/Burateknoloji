using Microsoft.AspNetCore.Mvc.Filters;
using StilPay.Entities;
using System;
using System.Linq;
using System.Security.Claims;

namespace StilPay.UI.WebSite.Areas.Panel.Infrastructures
{
    public class ActionFilter : IActionFilter
    {
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

                param = context.ActionArguments.FirstOrDefault(p => p.Value is MemberEntity);
                if (param.Value != null)
                {
                    var claim = context.HttpContext.User.FindFirst(f => f.Type == ClaimTypes.Sid);

                    if (claim != null)
                    {
                        string idMember = claim.Value;
                        ((MemberEntity)param.Value).IDMember = idMember;
                    }
                }
            }
        }
    }
}
