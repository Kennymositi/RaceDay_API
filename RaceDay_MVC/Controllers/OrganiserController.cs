using Microsoft.AspNetCore.Mvc;
using RaceDay_MVC.Models;
using RaceDay_MVC.Services;

namespace RaceDay_MVC.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly ApiService _api;

        public OrganiserController(ApiService api)
        {
            _api = api;
        }

        // Check that the logged-in user is an organiser
        private bool IsOrganiser()
        {
            return HttpContext.Session.GetString("Role") == "ORGANISER";
        }

        // Organiser dashboard
        public IActionResult Dashboard()
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            return View();
        }

        // View events belonging to the logged-in organiser
        public async Task<IActionResult> Events()
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            string token = HttpContext.Session.GetString("Token") ?? "";
            string userId = HttpContext.Session.GetString("UserId") ?? "";

            try
            {
                var allEvents = await _api.GetAsync<List<EventViewModel>>(
                    "api/events", token) ?? new List<EventViewModel>();

                var myEvents = allEvents
                    .Where(e => e.OrganiserId.ToString() == userId)
                    .ToList();

                return View(myEvents);
            }
            catch
            {
                TempData["Error"] = "Unable to load events.";
                return View(new List<EventViewModel>());
            }
        }

        // Display create event form
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            await LoadEventTypes();

            return View();
        }

        // Save new event
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventViewModel model)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                await LoadEventTypes();
                return View(model);
            }

            string token = HttpContext.Session.GetString("Token") ?? "";

            var response = await _api.PostAsync(
                "api/events",
                new
                {
                    name = model.Name,
                    description = model.Description,
                    event_date = model.EventDate,
                    location = model.Location,
                    distance = model.Distance,
                    event_type_id = model.EventTypeId
                },
                token);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Event created successfully!";
                return RedirectToAction("Events");
            }

            var error = await response.Content.ReadAsStringAsync();

            ModelState.AddModelError("", "Unable to create event: " + error);

            await LoadEventTypes();
            return View(model);
        }

        // Display edit event form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            string token = HttpContext.Session.GetString("Token") ?? "";
            string userId = HttpContext.Session.GetString("UserId") ?? "";

            var eventItem = await _api.GetAsync<EventViewModel>(
                $"api/events/{id}");

            if (eventItem == null ||
                eventItem.OrganiserId.ToString() != userId)
            {
                return NotFound();
            }

            await LoadEventTypes();

            return View(eventItem);
        }

        // Update event
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventViewModel model)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                await LoadEventTypes();
                return View(model);
            }

            string token = HttpContext.Session.GetString("Token") ?? "";

            var response = await _api.PutAsync(
                $"api/events/{id}",
                new
                {
                    name = model.Name,
                    description = model.Description,
                    event_date = model.EventDate,
                    location = model.Location,
                    distance = model.Distance,
                    event_type_id = model.EventTypeId
                },
                token);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Event updated successfully!";
                return RedirectToAction("Events");
            }

            ModelState.AddModelError("", "Unable to update event.");

            await LoadEventTypes();
            return View(model);
        }

        // Delete event
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            string token = HttpContext.Session.GetString("Token") ?? "";

            var response = await _api.DeleteAsync(
                $"api/events/{id}", token);

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Event deleted successfully!";
            else
                TempData["Error"] =
                    "Unable to delete event. It may have related enrolments or categories.";

            return RedirectToAction("Events");
        }

        // Load event types for the dropdown
        private async Task LoadEventTypes()
        {
            var types = await _api.GetAsync<List<EventTypeViewModel>>(
                "api/event-types");

            ViewBag.EventTypes = types ?? new List<EventTypeViewModel>();
        }
    }
}