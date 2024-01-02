using FluentValidation.Results;
using Hangfire;
using MealApplication.Extensions;
using MealApplication.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using src.Core;
using src.Core.Domains;
using src.Core.Enums;
using src.Emails;
using src.Localization;
using src.Repositories.EmailConfigs;
using src.Repositories.Meals;
using src.Repositories.Parents;
using src.Repositories.PowerOfAttorneys;
using src.Repositories.Students;
using src.Web.Common.Models.HandBookViewModels;
using src.Web.Common.Mvc.Alerts;
using src.Web.Common.Validation;
using System.Data;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MealApplication.Controllers
{
    [SessionTimeout]
    public class StudentsController : Controller
    {
        private readonly ILogger<StudentsController> _logger;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly IStudentRepository _studentRepository;
        private readonly IPowerOfAttorneyRepository _powerOfAttorneyRepository;
        private readonly IMealRepository _mealRepository;
        private readonly IParentRepository _parentRepository;
        private readonly IBackgroundJobClient _backgroungJobClient;
        private readonly IEmailConfigRepository _emailConfigRepository;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _cache;
        private CCEmailSettings _appSettings;
      

        public StudentsController(ILogger<StudentsController> logger, IStringLocalizer<SharedResource> sharedLocalizer, IStudentRepository studentRepository, IPowerOfAttorneyRepository powerOfAttorneyRepository,
            IParentRepository parentRepository, IBackgroundJobClient backgroungJobClient, IEmailConfigRepository emailConfigRepository, IOptions<CCEmailSettings> appSettings, IEmailSender emailSender, IMealRepository mealRepository, IMemoryCache cache)
        {
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
            _studentRepository = studentRepository;
            _powerOfAttorneyRepository = powerOfAttorneyRepository;
            _parentRepository = parentRepository;
            _backgroungJobClient = backgroungJobClient;
            _emailConfigRepository = emailConfigRepository;
            _appSettings = appSettings.Value;
            _emailSender = emailSender;
            _mealRepository = mealRepository;
            _cache = cache;
           
        }

        [CheckVerification]
        public async Task<IActionResult> Index()
        {
            try
            {               
                var parent = _cache.Get("parent") as Parent;
                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (parent == null )
                    return RedirectToAction("Index", "Home")
                        .WithError(_sharedLocalizer["message_something_went_wrong"]);
                if (string.IsNullOrEmpty(userEmail))
                {
                    return RedirectToAction("Index", "Home")
                        .WithError("Session is expired");
                }
                var students = await _studentRepository.GetStudentByParentEmail(parent.Email);
              
                var parentStudentViewModel = new ParentStudentViewModel()
                {                 
                    Parent = parent,
                    Students = students                    
                };

                TempData.Set("student", students);
               
                string host = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

                var isExistParent = _parentRepository.getParentByEmail(parent.Email);

                if (isExistParent != null)
                {
                    isExistParent.Status = (int)PhoneVerificationStatus.Active;
                    await _parentRepository.updateParent(isExistParent);
                   
                }
                
                return View(parentStudentViewModel);
            }
                catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return RedirectToAction("Index", "Home").WithError(_sharedLocalizer["message_something_went_wrong"]);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckAccess]
        public async Task<IActionResult> Submit(ParentStudentViewModel data, string id )
        {
            try {
                int rowIndex = -1;
                MealValidator validator = new MealValidator();
                string errorMessage = string.Empty;

                foreach (var student in data.Students )
                {
                    rowIndex++;
                    if (student.Meal != null)
                    {
                        if (data.Students.Count > 1 && data.Parent.ContactType == "div_delete_" + rowIndex)
                        {
                            continue;                       
                        } else
                        {
                            student.Meal.StudentCode = student.StudentCode;
                            student.Meal.StudentName = student.StudentName;
                            ValidationResult results = validator.Validate(student.Meal);
                            if (!results.IsValid)
                            {
                                foreach (var error in results.Errors)
                                {
                                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                                    errorMessage += error + "-";
                                }
                            
                                return RedirectToAction("Index", "Students").WithError(errorMessage); 
                            }
                        }
                    } 
                    
                }

                var parent = _cache.Get("parent") as Parent;

                parent.Email = parent.Email;
                parent.PhoneNumber = parent.PhoneNumber;

                _cache.Set("parent", parent);
 
                if (data.Students.Count < 0)
                {
                    return RedirectToAction("Index", "Home")
                        .WithError(_sharedLocalizer["message_something_went_wrong"]);
                }
                if (data.Students.Count() == 1)
                { 
                    //insert student
                    foreach (var student in data.Students)
                    {
                        student.Meal.ParentId = parent.Id;
                        student.Meal.CreatedAt = DateTime.Now;
                        student.Meal.Status = 0;
                        student.Meal.StudentCode = student.StudentCode;
                        student.Meal.StudentName = student.StudentName;
                        student.Meal.ClassName = student.ClassName;

                        try
                        {
                            await _studentRepository.InsertStudent(student);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, ex.Message);
                            return RedirectToAction("Index", "Students")
                                .WithError(_sharedLocalizer["NotFoundStudent"]);
                        }
                    }
                
                }else
                {
                    // Process students with more than 2 meals

                    var studentsWithMeals = data.Students.Where(student => student.Meal != null && student.Meal.TotalNumberofdays != null).ToList();
                    foreach (var student in studentsWithMeals)
                    {
                        student.Meal.StudentCode = student.StudentCode;
                        student.Meal.StudentName = student.StudentName;

                        ValidationResult results = validator.Validate(student.Meal);

                        if (!results.IsValid)
                        {
                            foreach (var error in results.Errors)
                            {
                                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                            }

                            return RedirectToAction("Index", "Students");
                        }

                        student.Meal.ParentId = parent.Id;
                        student.Meal.CreatedAt = DateTime.Now;
                        student.Meal.Status = 0;
                        student.Meal.StudentCode = student.StudentCode;
                        student.Meal.StudentName = student.StudentName;
                        student.Meal.ClassName = student.ClassName;

                        try
                        {
                            await _studentRepository.InsertStudent(student);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, ex.Message);
                            return RedirectToAction("Index", "Students")
                                .WithError(_sharedLocalizer["NotFoundStudent"]);
                        }
                    }
                }
                await _parentRepository.updateParent(parent);

                string host = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

                var isExistParent = _parentRepository.getParentByEmail(parent.Email);

                if (isExistParent != null)
                {
                    isExistParent.Status = (int)PhoneVerificationStatus.Active;
                    await _parentRepository.updateParent(isExistParent);
                    if (!string.IsNullOrEmpty(isExistParent.Email))
                    {
                        _backgroungJobClient.Enqueue(() => SendAnnounceEmailToParent(parent, host));
                    }
                }
                return RedirectToAction(nameof(Success));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return RedirectToAction(nameof(HomeController.Index), "Home")
                    .WithError(_sharedLocalizer["message_something_went_wrong"]);
            }

        }

        [CheckAccess]
        public async Task<IActionResult> Success()
        {
            try {
                var parent = _cache.Get("parent") as Parent;
                var students = new List<Student>();
                students = await _studentRepository.GetStudentByParentEmail(parent.Email);
                TempData.Set("parent", parent);
                return View(students);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return RedirectToAction("Index", "Home").WithError(_sharedLocalizer["message_something_went_wrong"]);
            }
        }

        public async Task SendAnnounceEmailToParent(Parent parent, string currentUrl)
        {
            if (!string.IsNullOrEmpty(parent.Email))
            {
                var emailModel = await _emailConfigRepository.getAllConfig();
                var students = await _studentRepository.GetStudentIncludeLatestPowerOfAttorneyByParentEmail(parent.Email);
                string template = string.Empty;
                foreach (var student in students)
                {
                    template = emailModel.confirmation_meal.Body;
                    template = MessageExtension.ReplaceForParentConfirmation(student, template, currentUrl);
                    List<MailboxAddress> ccEmail = new List<MailboxAddress>();
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
                    await _emailSender.SendEmail("qvthien6@gmail.com", emailModel.confirmation_meal.Subject, template, null, null);
                    
                    // await _emailSender.SendEmail(parent.Email, subject, template, null, ccEmail);
                }

            }
        }
        [SkipSessionTimeout]
        public async Task<IActionResult> MealDetails(string studenCode)
        {
            var entity = await _studentRepository.GetByStudentCodeIncludeMeal(studenCode);
            return View(entity);
        }

        public async Task<IActionResult> UpdateStatusOfMeal(int id, int status, bool applyForAll = false)
        {
            var parent = _cache.Get("parent") as Parent;
            if (parent == null)
                return Json(new { success = false, message = _sharedLocalizer["message_something_went_wrong"].Value.ToString() });
            TempData.Keep("parent");
            if (id == 0)
                return Json(new { success = false, message = _sharedLocalizer[""] });
            var poa = await _mealRepository.GetById(id);
            if (poa == null)
            {
                return Json(new { success = false, message = _sharedLocalizer["error_msg_can_not_find_meal_information"].Value.ToString() });
            }
            if (!applyForAll)
            {
                if (poa != null)
                {
                    if (poa.ParentId == parent.Id)
                    {
                        UpdateMeal(poa, status);
                        return Json(new { success = true, message = _sharedLocalizer["success_msg_update_success"].Value.ToString() });
                    }
                    else
                    {
                        return Json(new { success = false, message = _sharedLocalizer["error_msg_can_not_update_because_you_are_not_owner"].Value.ToString() });
                    }
                }
            }
     
            return Json(new { success = false, message = _sharedLocalizer["message_something_went_wrong"].Value.ToString() });
        }


        public async Task<IActionResult> DeleteMeal(int id)
        {
            var parent = _cache.Get("parent") as Parent;
            if (parent == null)
                return Json(new { success = false, message = _sharedLocalizer["message_something_went_wrong"].Value.ToString() });
            TempData.Keep("parent");
            if (id == 0)
                return Json(new { success = false, message = _sharedLocalizer[""] });
            var poa = await _mealRepository.GetById(id);
            if (poa == null)
            {
                return Json(new { success = false, message = _sharedLocalizer["error_msg_can_not_find_meal_information"].Value.ToString() });
            }
            if (poa != null)
            {
                if (poa.ParentId == parent.Id)
                {
                    DeleteMeal(poa);
                    return Json(new { success = true, message = _sharedLocalizer["success_msg_update_success"].Value.ToString() });
                }
                else
                {
                    return Json(new { success = false, message = _sharedLocalizer["error_msg_can_not_update_because_you_are_not_owner"].Value.ToString() });
                }
            }
            return Json(new { success = false, message = _sharedLocalizer["message_something_went_wrong"].Value.ToString() });
        }

        private void UpdateMeal(Meal obj, int status)
        {
            if (obj != null)
            {
                obj.Status = status;
                _mealRepository.Update(obj);
            }
        }

        private void DeleteMeal(Meal obj)
        {
            if (obj != null)
            {
                _mealRepository.Delete(obj);
            }
        }



    }
}
