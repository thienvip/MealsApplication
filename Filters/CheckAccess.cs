using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using src.Core.Domains;
using src.Core.Enums;
using src.Localization;
using src.Web.Common.Mvc.Alerts;
using System.Drawing.Text;

namespace MealApplication.Filters
{
    public class CheckAccess : ActionFilterAttribute
    {
        public override void OnActionExecuting (ActionExecutingContext context) {
                var controller = context.Controller as Controller;
                    IMemoryCache _cache = context.HttpContext.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;
                    IStringLocalizer<SharedResource> _sharedLocalizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
                    if (controller != null)
                    {
                        if (_cache != null && _sharedLocalizer != null)
                        {
                                var parent = _cache.Get("parent") as Parent;
                                if (parent == null)
                                {
                                    context.Result = new RedirectToActionResult("Index", "Home", null)
                                    .WithError(_sharedLocalizer["message_something_went_wrong"]);
                                }
                        }
                    
                }

                base.OnActionExecuting(context);

        }

    }
}
