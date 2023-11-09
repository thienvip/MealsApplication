using MealApplication.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using src.Core;
using src.Core.Domains;
using src.Localization.Resources;
using src.Repositories.Meals;

namespace MealApplication.Areas.Administration.Controllers
{
    [Area(Constants.Areas.Administration)]
    [Authorize(Policy = Constants.RoleNames.Administrator)]
    public class DashboardController : Controller
    {
        private readonly IMealRepository _mealRepository;
        public DashboardController(IMealRepository mealRepository)
        {
            _mealRepository = mealRepository;
        }
        public async Task<IActionResult> Index()
        {
            var meals = await _mealRepository.GetAll() ;
            return View(meals);
        }
    }
}
