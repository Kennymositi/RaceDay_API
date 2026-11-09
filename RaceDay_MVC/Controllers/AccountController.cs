using Microsoft.AspNetCore.Mvc;
using RaceDay_MVC.Models;
using RaceDay_MVC.Services;
using System.Net.Http.Json;

namespace RaceDay_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _api;

        public AccountController(ApiService api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _api.PostAsync(
                "api/auth/login",
                new
                {
                    email = model.Email,
                    password = model.Password
                });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("",
                    "Invalid email or password.");
                return View(model);
            }

            var result = await response.Content
                .ReadFromJsonAsync<ApiAuthResponse>();

            if (result?.User == null ||
                string.IsNullOrEmpty(result.Token))
            {
                ModelState.AddModelError("",
                    "Unable to process login.");
                return View(model);
            }

            HttpContext.Session.SetString("Token", result.Token);
            HttpContext.Session.SetString(
                "UserId", result.User.UserId.ToString());
            HttpContext.Session.SetString(
                "FullName", result.User.FirstName + " " +
                            result.User.LastName);
            HttpContext.Session.SetString(
                "Email", result.User.Email);
            HttpContext.Session.SetString(
                "Role", result.User.Role);

            if (result.User.Role == "ORGANISER")
                return RedirectToAction("Dashboard", "Organiser");

            return RedirectToAction("Dashboard", "Participant");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (model.Role != "ORGANISER" &&
                model.Role != "PARTICIPANT")
            {
                ModelState.AddModelError(
                    "Role", "Please select a valid role.");
            }

            if (!ModelState.IsValid)
                return View(model);

            var response = await _api.PostAsync(
                "api/auth/register",
                new
                {
                    first_name = model.FirstName,
                    last_name = model.LastName,
                    email = model.Email,
                    phone = model.Phone,
                    password = model.Password,
                    role = model.Role
                });

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError("",
                    "Registration failed. " + error);

                return View(model);
            }

            TempData["Success"] =
                "Registration successful. Please log in.";

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}
