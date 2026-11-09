using Microsoft.AspNetCore.Mvc;
using RaceDay_MVC.Models;
using RaceDay_MVC.Services;

namespace RaceDay_MVC.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly ApiService _api;

        public ParticipantController(ApiService api)
        {
            _api = api;
        }

        // Participant Dashboard
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Role") != "PARTICIPANT")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // Browse available events
        public async Task<IActionResult> Events()
        {
            if (HttpContext.Session.GetString("Role") != "PARTICIPANT")
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var events = await _api.GetAsync<List<EventViewModel>>(
                    "api/events"
                );

                return View(events ?? new List<EventViewModel>());
            }
            catch
            {
                ViewBag.Error = "Unable to load events. Please try again.";
                return View(new List<EventViewModel>());
            }
        }
    

    // View event details and available categories
public async Task<IActionResult> EventDetails(int id)
        {
            if (HttpContext.Session.GetString("Role") != "PARTICIPANT")
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var eventItem = await _api.GetAsync<EventViewModel>(
                    $"api/events/{id}");

                if (eventItem == null)
                {
                    return NotFound();
                }

                var categories = await _api.GetAsync<List<CategoryViewModel>>(
                    $"api/categories/event/{id}");

                ViewBag.Categories = categories ?? new List<CategoryViewModel>();

                return View(eventItem);
            }
            catch
            {
                TempData["Error"] = "Unable to load event details.";
                return RedirectToAction("Events");
            }
        }

        // Enrol in an event category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enrol(int eventId, int categoryId)
        {
            if (HttpContext.Session.GetString("Role") != "PARTICIPANT")
            {
                return RedirectToAction("Login", "Account");
            }

            string? token = HttpContext.Session.GetString("Token");

            var enrollmentData = new
            {
                event_id = eventId,
                category_id = categoryId
            };

            try
            {
                var response = await _api.PostAsync(
                    "api/enrollments",
                    enrollmentData,
                    token
                );

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "You have successfully enrolled in this event!";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();

                    TempData["Error"] =
                        $"API Error ({(int)response.StatusCode}): {error}";
                }
            }
            catch
            {
                TempData["Error"] = "Unable to connect to the enrolment service.";
            }

            return RedirectToAction("EventDetails", new { id = eventId });
        }

        public async Task<IActionResult> MyEnrollments()
        {
            if (HttpContext.Session.GetString("Role") != "PARTICIPANT")
            {
                return RedirectToAction("Login", "Account");
            }
            string? token = HttpContext.Session.GetString("Token");
            try
            {
                var enrollments =
                    await _api.GetAsync<List<EnrollmentViewModel>>(
                        "api/enrollments/my", token);
                return View(enrollments ?? new List<EnrollmentViewModel>());
            }
            catch
            {
                ViewBag.Error = "Unable to load your enrolments.";
                return View(new List<EnrollmentViewModel>());
            }
        }

        public async Task<IActionResult> MyResults()
        {
            if (HttpContext.Session.GetString("Role") != "PARTICIPANT")
            {
                return RedirectToAction("Login", "Account");
            }
            string? token = HttpContext.Session.GetString("Token");
            try
            {
                var results =
                    await _api.GetAsync<List<ResultViewModel>>(
                        "api/results/my", token);
                return View(results ?? new List<ResultViewModel>());
            }
            catch
            {
                ViewBag.Error = "Unable to load your results.";
                return View(new List<ResultViewModel>());
            }
        }

        // View participant profile
        public async Task<IActionResult> Profile()
        {
            if (HttpContext.Session.GetString("Role") != "PARTICIPANT")
            {
                return RedirectToAction("Login", "Account");
            }
            string? token = HttpContext.Session.GetString("Token");
            try
            {
                var profile = await _api.GetAsync<ProfileViewModel>(
                    "api/users/me", token);
                if (profile == null)
                    return NotFound();
                return View(profile);
            }
            catch
            {
                TempData["Error"] = "Unable to load your profile.";
                return RedirectToAction("Dashboard");
            }
        }
        // Update participant profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "PARTICIPANT")
            {
                return RedirectToAction("Login", "Account");
            }
            if (!ModelState.IsValid)
                return View(model);
            string? token = HttpContext.Session.GetString("Token");
            try
            {
                var response = await _api.PutAsync(
                    "api/users/me", model, token);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Profile updated successfully.";
                    return RedirectToAction("Profile");
                }
                ModelState.AddModelError("", "Unable to update profile.");
            }
            catch
            {
                ModelState.AddModelError("", "Could not connect to the API.");
            }
            return View(model);
        }

    }
}