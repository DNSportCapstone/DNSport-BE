﻿using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class ServiceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
