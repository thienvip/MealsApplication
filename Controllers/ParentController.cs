using MealApplication.Extensions;
using MealApplication.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using src.Core.Enums;
using src.Localization;
using src.Repositories.Parents;
using src.Web.Common.Mvc.Alerts;

namespace MealApplication.Controllers
{
    [AllowAnonymous]
    public class ParentController : Controller
    {
        private readonly IParentRepository _parentRepository;
        private readonly ILogger<ParentController> _logger;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly IMemoryCache _cache;
    

        public ParentController(IParentRepository parentRepository,
        ILogger<ParentController> logger, IStringLocalizer<SharedResource> sharedLocalizer, IMemoryCache cache)
        {
            _parentRepository = parentRepository;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
            _cache = cache;
        }
        [CheckVerification]
        public async Task<IActionResult> GetByEmailAddress(string email)
        {
            try
            {   
                if (CheckIsEmailOrPhoneNumberExt.IsValidEmail(email))
                {
                    var  parent =   _parentRepository.GetParentsByEmailAddressFromPhoebus(email);
                    if (parent != null)
                    {
                        parent.LoginType = LoginTypeEnum.Email;
                        _cache.Set("parent", parent);
                        HttpContext.Session.SetString("UserEmail", parent.Email);
                        return  RedirectToAction("Check", "Verification", new { id = email });
                    }
                } 
                //else if (CheckIsEmailOrPhoneNumberExt.IsPhoneNumber(email))
                //{
                //    if (email.Trim().Substring(0, 1) == "0")
                //    {
                //        email = "84" + email.Substring(1);
                //    }
                //    var parent =  _parentRepository.GetParentsByPhoneNumberFromPhoebus(email);
                //    if (parent != null)
                //    {
                //        parent.PhoneNumber = email;
                //        parent.LoginType = LoginTypeEnum.PhoneNumber;
                //        TempData.Set("parent", parent);

                //        return RedirectToAction("Check", "Verification", new { id = email });
                //    }
                //}
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
            return RedirectToAction("Index", "Home").WithError(_sharedLocalizer["message_email_not_exist"]);

        }
    }
}
