﻿using Microsoft.AspNetCore.Mvc;

namespace Atron.WebViews.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}