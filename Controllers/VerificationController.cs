using Google.Protobuf;
using Hangfire;
using MealApplication.Extensions;
using MealApplication.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using src.Core;
using src.Core.Domains;
using src.Core.Enums;
using src.Emails;
using src.Localization.Resources;
using src.Repositories.EmailConfigs;
using src.Repositories.Parents;
using src.Repositories.Students;
using src.Web.Common.Mvc.Alerts;
using System.Runtime.InteropServices;

namespace MealApplication.Controllers
{
    public class VerificationController : Controller
    {
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly IParentRepository _parentRepository;
        private readonly IBackgroundJobClient _backgroungJobClient;
        private readonly IEmailConfigRepository _emailConfigRepository;
        private readonly INotificationExtension _notification;
        private readonly ILogger<VerificationController> _logger;
        private CCEmailSettings _appSettings;
        private readonly IStudentRepository _studentRepository;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _cache;
        public VerificationController(IStringLocalizer<SharedResource> sharedLocalizer, IParentRepository parentRepository, IBackgroundJobClient backgroungJobClient,
            IEmailConfigRepository emailConfigRepository, INotificationExtension notification, ILogger<VerificationController> logger, IOptions<CCEmailSettings> appSettings, IStudentRepository studentRepository, IEmailSender emailSender, IMemoryCache cache)
        {
            _sharedLocalizer = sharedLocalizer;
            _parentRepository = parentRepository;
            _backgroungJobClient = backgroungJobClient;
            _emailConfigRepository = emailConfigRepository;
            _notification = notification;
            _logger = logger;
            _appSettings = appSettings.Value;
            _studentRepository = studentRepository;
            _emailSender = emailSender;
            _cache = cache;

        }

        [CheckAccess]
        public async Task<IActionResult> Check(string id)
        {
            try {
                Random generator = new Random();
                String code = generator.Next(0, 1000000).ToString("D6");
                var parent = _cache.Get("parent") as Parent;

                if (string.IsNullOrEmpty(parent.VerifyCode))
                {
                    string host = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
                    if (CheckIsEmailOrPhoneNumberExt.IsValidEmail(id))
                    {
                        var isExistParent = _parentRepository.getParentByEmail(id);
                        if (isExistParent != null)
                        {
                            if (isExistParent.Status == (int)PhoneVerificationStatus.Pending)
                            {
                                if ((DateTime.Now.Date - isExistParent.UpdatedAt.Date).Days >= 0)
                                {
                                    parent.VerifyCode = code;
                                    parent.Status = (int)PhoneVerificationStatus.Pending;
                                    isExistParent.VerifyCode = code;
                                    isExistParent.UpdatedAt = DateTime.Now;
                                    isExistParent.Status = (int)PhoneVerificationStatus.Pending;
                                    _backgroungJobClient.Enqueue(() => SendEmailIncludeValidationCode(parent, host));
                                    await _parentRepository.updateParent(isExistParent);
                                }
                                parent.VerifyCode = isExistParent.VerifyCode;

                            }
                            if (isExistParent.Status == (int)PhoneVerificationStatus.Active)
                            {
                                parent.VerifyCode = code;
                                parent.Status = (int)PhoneVerificationStatus.Pending;
                                isExistParent.VerifyCode = code;
                                isExistParent.UpdatedAt = DateTime.Now;
                                isExistParent.Status = (int)PhoneVerificationStatus.Pending;
                                _backgroungJobClient.Enqueue(() => SendEmailIncludeValidationCode(parent, host));
                         
                                await _parentRepository.updateParent(isExistParent);
                            }
                        }
                        else
                        {
                            parent.VerifyCode = code;
                            parent.UpdatedAt = DateTime.Now;
                            _backgroungJobClient.Enqueue(() => SendEmailIncludeValidationCode(parent, host));
                            await _parentRepository.insertParent(parent);
                        }

                    }
                    if (CheckIsEmailOrPhoneNumberExt.IsPhoneNumber(id))
                    {
                        var isExistParent = await _parentRepository.getParentByPhone(id);
                        if (isExistParent != null)
                        {
                            if (isExistParent.Status == (int)PhoneVerificationStatus.Pending)
                            {
                                if ((DateTime.Now.Date - isExistParent.UpdatedAt.Date).Days > 0)
                                {
                                    parent.VerifyCode = code;
                                    parent.Status = (int)PhoneVerificationStatus.Pending;
                                    isExistParent.VerifyCode = code;
                                    isExistParent.UpdatedAt = DateTime.Now;
                                    isExistParent.Status = (int)PhoneVerificationStatus.Pending;
                                    // _backgroungJobClient.Enqueue(() => SendEmailIncludeValidationCode(parent, host));
                                    await SendEmailIncludeValidationCode(parent, host);

                                }
                                parent.VerifyCode = isExistParent.VerifyCode;

                            }
                            if (isExistParent.Status == (int)PhoneVerificationStatus.Active)
                            {
                                parent.VerifyCode = code;
                                parent.Status = (int)PhoneVerificationStatus.Pending;
                                isExistParent.VerifyCode = code;
                                isExistParent.UpdatedAt = DateTime.Now;
                                isExistParent.Status = (int)PhoneVerificationStatus.Pending;

                                _backgroungJobClient.Enqueue(() => SendSmsForAuthentication(parent));
                                if (!string.IsNullOrEmpty(parent.Email))
                                {
                                    await  SendEmailIncludeValidationCode(parent, host);
                                }

                                await _parentRepository.updateParent(isExistParent);
                            }
                            parent.Email = (parent.Email == null || string.IsNullOrEmpty(parent.Email)) ? isExistParent.Email : parent.Email;
                            parent.PhoneNumber = (parent.PhoneNumber == null || string.IsNullOrEmpty(parent.PhoneNumber)) ? isExistParent.Email : parent.PhoneNumber;
                        }
                        else
                        {
                            parent.VerifyCode = code;
                            parent.UpdatedAt = DateTime.Now;
                                _backgroungJobClient.Enqueue(() => SendSmsForAuthentication(parent));
                            if (!string.IsNullOrEmpty(parent.Email))
                            {
                                _backgroungJobClient.Enqueue(() => SendEmailIncludeValidationCode(parent, host));
                            }
                            await _parentRepository.insertParent(parent);
                        }
                    }
                    _cache.Set("parent", parent);
                }
               

                return View(parent);
            }
            catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
                return RedirectToAction("Index", "Home").WithError(_sharedLocalizer["message_something_went_wrong"]);
            }
        }

        [CheckAccess]
        public IActionResult CheckVerification(string validateCode)
        {
            try
            {
                var parent = _cache.Get("parent") as Parent;
       
                //if (!parent.VerifyCode.Contains(validateCode))
                //{
                //    TempData.Keep("parent");
                //    return RedirectToAction("Check", "Verification", new { id = (parent.LoginType == LoginTypeEnum.Email ? parent.Email : parent.PhoneNumber) }).WithError(_sharedLocalizer["message_invalid_code"]);
                //}
               
                parent.IsVerified = true;
                parent.ConfirmProcess = ConfirmProcessEnum.Idle;
                _cache.Set("parent", parent);
                return RedirectToAction("Index", "Students");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return RedirectToAction("Index", "Home").WithError(_sharedLocalizer["message_something_went_wrong"]);
            }
        }

        public async Task SendSmsForAuthentication(Parent parent)
        {
            var emailModel = await _emailConfigRepository.getAllConfig();
            var body = MessageExtension.ReplaceForSmsValidation(parent, emailModel.sms_validate.Body);
            // var result = _notification.sendSMS("84935920610", body);
            var result = _notification.sendSMS(parent.PhoneNumber, body);
            if (result == true)
                _logger.LogInformation("Sent SMS for authentication to {ParentPhoneNumber} at {Now}", parent.PhoneNumber, DateTime.Now);
        }

        public async Task SendEmailIncludeValidationCode(Parent parentsInformation, string currentUrl)
        {
            if (!string.IsNullOrEmpty(parentsInformation.Email))
            {
                var emailModel =await _emailConfigRepository.getAllConfig();
                string template = emailModel.sms_validate.Body;
                template = MessageExtension.ReplaceForValidation(parentsInformation, template);
                string subject = emailModel.validate.Subject;

                List<MailboxAddress> ccEmail = new List<MailboxAddress>();
                var students = await _studentRepository.GetStudentByParentEmail(parentsInformation.Email);
                foreach (var student in students)
                {
                    switch (student.CampusCode)
                    {
                        case "161":
                            ccEmail.Add(MailboxAddress.Parse(_appSettings.BTH));
                            break;
                        case "162":
                            ccEmail.Add(MailboxAddress.Parse(_appSettings.BTH));
                            break;
                        case "07":
                            ccEmail.Add(MailboxAddress.Parse(_appSettings.PXL));
                            break;
                        case "18":
                            ccEmail.Add(MailboxAddress.Parse(_appSettings.HVT));
                            break;
                        case "13":
                            ccEmail.Add(MailboxAddress.Parse(_appSettings.SR));
                            break;
                        case "24":
                            ccEmail.Add(MailboxAddress.Parse(_appSettings.SL));
                            break;
                        case "22":
                            ccEmail.Add(MailboxAddress.Parse(_appSettings.GH));
                            break;
                        case "25":
                            ccEmail.Add(MailboxAddress.Parse(_appSettings.RS));
                            break;
                        default:
                            break;
                    }
                }
                // await _emailSender.SendEmail(parentsInformation.Email, emailModel.validate.Subject, template, null, ccEmail);
                await _emailSender.SendEmail("qvthien6@gmail.com", emailModel.validate.Subject, template, null, null);
            }
        }
        [CheckAccess]
        public async Task<IActionResult> ResentSms()
        {
            Random generator = new Random();
            String code = generator.Next(0, 10000000).ToString("D6");
            var parent = _cache.Get("parent") as Parent;
            var isExistParent = new Parent();
             string host = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            isExistParent =  _parentRepository.getParentByEmail(parent.Email);
            if (isExistParent != null)
            {
                if (isExistParent.ResentSms > 5 || isExistParent.Status == (int)PhoneVerificationStatus.Locked)
                {
                    parent.Status = (int)PhoneVerificationStatus.Locked;
                    await _parentRepository.updateParent(isExistParent);
                    return Json(new { success = false, message = _sharedLocalizer["message_input_wrong_code_many_time"] });
                }
                parent.VerifyCode = code;
                parent.ResentSms += 1;
                isExistParent.VerifyCode = code;
                isExistParent.UpdatedAt = DateTime.Now;
                isExistParent.Status = (int)PhoneVerificationStatus.Pending;
                _backgroungJobClient.Enqueue(() => SendEmailIncludeValidationCode(parent, host));
                await _parentRepository.updateParent(isExistParent);
                TempData.Set("parent", parent);
            }

            else
            {
                return Json(new { success = false, message = _sharedLocalizer["message_something_went_wrong"] });
            }
            return Json(new { success = true, message = _sharedLocalizer["message_sent_new_validation_code"] });
        }
    }
}
