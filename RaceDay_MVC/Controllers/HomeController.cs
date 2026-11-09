using Microsoft.AspNetCore.Mvc;
using RaceDay_MVC.Models;
using System.Diagnostics;

namespace RaceDay_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") == "ORGANISER")
                return RedirectToAction("Dashboard", "Organiser");

            if (HttpContext.Session.GetString("Role") == "PARTICIPANT")
                return RedirectToAction("Dashboard", "Participant");

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
