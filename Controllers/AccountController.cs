using MealApplication.Areas.Administration.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using src.Core;
using src.Localization;
using src.Repositories.Authentication;
using src.Repositories.Roles;
using src.Repositories.Users;
using src.Web.Common.Models.AccountViewModels;
using src.Web.Common.Security;
using System;

namespace MealApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ISignInManager _signInManager;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IDateTime _dateTime;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ISignInManager signInManager, IStringLocalizer<SharedResource> sharedLocalizer, IAuthenticationService authenticationService, IOptions<AppSettings> appSettings, IUserRepository userRepository, IRoleRepository roleRepository, IDateTime dateTime, ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _sharedLocalizer = sharedLocalizer;
            _authenticationService = authenticationService;
            _appSettings = appSettings;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _dateTime = dateTime;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string? returnUrl )
        {
            await _signInManager.SignOutAsync();
            ViewData["ReturnUrl"] = returnUrl == null ? "/" : returnUrl ;
            var model = new LoginViewModel();
            return View("Login", model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //bool result = _authenticationService.ValidateUser(_appSettings.Value.Application.Domain, model.UserName, model.Password);
                    bool result = true;
                    if (result)
                    {
                        var user = await _userRepository.GetUserByUserNameAsync(model.UserName);
                        if (user != null && user.IsActive)
                        {
                            var roleNames = (await _roleRepository.GetRolesForUserAsync(user.Id)).Select(r => r.Name)
                                .ToList();
                            await _signInManager.SignInAsync(user, roleNames);
                            user.LastLoginDate = _dateTime.Now;
                            await _userRepository.UpdateUserAsync(user);
                            string t = _sharedLocalizer["IncorrectUsernameOrPassword"];
                            _logger.LogInformation("Login Successful: {UserUserName}", user.UserName);
                            if (!string.IsNullOrEmpty(returnUrl) && !string.Equals(returnUrl, "/") &&
                                Url.IsLocalUrl(returnUrl))
                                return RedirectToLocal(returnUrl);
                            return RedirectToAction(nameof(DashboardController.Index), "Dashboard",
                                 new { area = Constants.Areas.Administration });
                            

                        }
                        _logger.LogError("Authorization Fail: {ModelUserName}", model.UserName);
                        ModelState.AddModelError("", _sharedLocalizer["AccessDenied"]);
                    }
                    else
                    {
                        _logger.LogError("Login Fail: {ModelUserName} - Incorrect username or password. ",
                            model.UserName);
                        ModelState.AddModelError("", _sharedLocalizer["IncorrectUsernameOrPassword"]);
                    }

                }

            }
            catch 
            {

                ModelState.AddModelError(string.Empty, _sharedLocalizer["InvalidLogin"]);
                return View(model);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Logout Successful: {IdentityName}", User.Identity.Name);
            return RedirectToAction(nameof(AccountController.Login));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return LocalRedirect(returnUrl);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }


    }
}
