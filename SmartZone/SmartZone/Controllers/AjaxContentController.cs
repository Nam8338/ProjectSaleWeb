using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartZone.Controllers
{
    public class AjaxContentController : Controller
    {
        public INotyfService _notyfService { get; } //Import services

        public AjaxContentController(INotyfService notyfService)
        {
            _notyfService = notyfService;
        }

        public IActionResult HeaderCart()
        {
            return ViewComponent("HeaderCart");
        }
        public IActionResult NumberCart()
        {
            return ViewComponent("NumberCart");
        }
    }
}
