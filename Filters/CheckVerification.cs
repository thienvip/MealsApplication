using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using src.Core.Domains;
using src.Core.Enums;
using src.Localization;
using src.Web.Common.Mvc.Alerts;

namespace MealApplication.Filters
{   
    public class CheckVerification : ActionFilterAttribute

    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as Controller;
            if (controller != null)
            {
                IMemoryCache _cache = context.HttpContext.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;
                IStringLocalizer<SharedResource> _sharedLocalizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
                if (_cache != null && _sharedLocalizer != null)
                {
                    var parent = _cache.Get("parent") as Parent;
                    if (parent != null && parent.IsVerified == false)
                    {
                        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Verification", action = "Check", id = (parent.LoginType == LoginTypeEnum.Email ? parent.Email : parent.PhoneNumber) })).WithError(_sharedLocalizer["message_entere_validation_code"]);
                    }
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
