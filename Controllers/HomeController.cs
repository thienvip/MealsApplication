
using Hangfire;
using Hangfire.Common;
using Hangfire.Storage;
using MealApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using src.Core.Domains;
using src.Localization;
using src.Repositories.Parents;
using src.Web.Common.Mvc.Alerts;
using System.Diagnostics;

namespace MealApplication.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IMemoryCache _cache;

        public HomeController(IMemoryCache cache) { 
            _cache = cache;
        }
        
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            _cache.Remove("parent");
            RecurringJob.AddOrUpdate("daily-email-job", () => Console.WriteLine("11") , Cron.Daily(1, 0));
           
            //using (var connection = JobStorage.Current.GetConnection())
            //{
            //    var recurringJob = connection.GetRecurringJobs().Find( rj => rj.Id == "daily-email-job");
                
            //    if (recurringJob != null)
            //    {
            //        var nextExecution = recurringJob.NextExecution;
            //        Console.WriteLine("Next execution date and time: " + nextExecution);
            //    }
            //    else
            //    {
            //        Console.WriteLine("Recurring job not found.");
            //    }
            //}

            return View();
        }
    }
}