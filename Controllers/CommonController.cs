﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MealApplication.Controllers
{
    public class CommonController : Controller
    {
        //
        // GET: /Common/Error
        public IActionResult Error(string id = "")
        {
            switch (id)
            {
                case "403":
                    return View("AccessDenied");
                case "404":
                    return View("PageNotFound");
                default:
                    return View();
            }
        }

        //
        // GET: /Common/PageNotFound
        [AllowAnonymous]
        public IActionResult PageNotFound()
        {
            Response.StatusCode = 404;

            return View();
        }

        //
        // GET: /Common/AntiForgery
        [AllowAnonymous]
        public IActionResult AntiForgery()
        {
            return View();
        }

        //
        // GET: /Common/AccessDenied
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
