using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using MySqlX.XDevAPI.Common;
using src.Localization;
using src.Web.Common.Mvc.Alerts;

namespace MealApplication.Filters
{
    public class SessionTimeout: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            //Check if the skipSessionTimeout atrribute is applied to the action 
            var skipAtribute = context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(SkipSessionTimeout));
            if (!skipAtribute)
            {
                var userEmail = context.HttpContext.Session.GetString("UserEmail");
                IStringLocalizer<SharedResource> _sharedLocalizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
                if (string.IsNullOrEmpty(userEmail))
                {
                    context.Result = new RedirectToActionResult("Index", "Home", null)
                                        .WithError(_sharedLocalizer["message_user_expired"]);
                }
            }
            base.OnActionExecuting(context);
        }
    }
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class SkipSessionTimeout : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }
    }
}
