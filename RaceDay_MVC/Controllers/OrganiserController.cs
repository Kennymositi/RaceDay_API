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

        // View categories belonging to one of the organiser's events
        public async Task<IActionResult> Categories(int eventId)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            string token = HttpContext.Session.GetString("Token") ?? "";
            string userId = HttpContext.Session.GetString("UserId") ?? "";

            var events = await _api.GetAsync<List<EventViewModel>>(
                "api/events", token) ?? new List<EventViewModel>();

            var myEvent = events.FirstOrDefault(e =>
                e.EventId == eventId &&
                e.OrganiserId.ToString() == userId);

            if (myEvent == null)
                return NotFound();

            var categories = await _api.GetAsync<List<CategoryViewModel>>(
                $"api/categories/event/{eventId}");

            ViewBag.EventId = eventId;
            ViewBag.EventName = myEvent.Name;

            return View(categories ?? new List<CategoryViewModel>());
        }

        // Display the add-category form
        [HttpGet]
        public async Task<IActionResult> AddCategory(int eventId)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            string token = HttpContext.Session.GetString("Token") ?? "";
            string userId = HttpContext.Session.GetString("UserId") ?? "";

            var events = await _api.GetAsync<List<EventViewModel>>(
                "api/events", token) ?? new List<EventViewModel>();

            var myEvent = events.FirstOrDefault(e =>
                e.EventId == eventId &&
                e.OrganiserId.ToString() == userId);

            if (myEvent == null)
                return NotFound();

            return View(new CategoryViewModel { EventId = eventId });
        }

        // Save a new category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(CategoryViewModel model)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            string token = HttpContext.Session.GetString("Token") ?? "";

            var response = await _api.PostAsync(
                "api/categories",
                new
                {
                    event_id = model.EventId,
                    category_name = model.CategoryName,
                    category_type = model.CategoryType,
                    category_value = model.CategoryValue,
                    description = model.Description
                },
                token);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Category added successfully!";
                return RedirectToAction("Categories",
                    new { eventId = model.EventId });
            }

            string error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "Unable to add category: " + error);

            return View(model);
        }

        // Display the edit-category form
        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            string token = HttpContext.Session.GetString("Token") ?? "";
            string userId = HttpContext.Session.GetString("UserId") ?? "";

            var allCategories = await _api.GetAsync<List<CategoryViewModel>>(
                "api/categories");

            var category = allCategories?.FirstOrDefault(c => c.CategoryId == id);

            if (category == null)
                return NotFound();

            var events = await _api.GetAsync<List<EventViewModel>>(
                "api/events", token) ?? new List<EventViewModel>();

            bool ownsEvent = events.Any(e =>
                e.EventId == category.EventId &&
                e.OrganiserId.ToString() == userId);

            if (!ownsEvent)
                return NotFound();

            return View(category);
        }

        // Save category changes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(
            int id, CategoryViewModel model)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            string token = HttpContext.Session.GetString("Token") ?? "";

            var response = await _api.PutAsync(
                $"api/categories/{id}",
                new
                {
                    category_name = model.CategoryName,
                    category_type = model.CategoryType,
                    category_value = model.CategoryValue,
                    description = model.Description
                },
                token);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Category updated successfully!";
                return RedirectToAction("Categories",
                    new { eventId = model.EventId });
            }

            string error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "Unable to update category: " + error);

            return View(model);
        }

        // Delete a category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id, int eventId)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            string token = HttpContext.Session.GetString("Token") ?? "";

            var response = await _api.DeleteAsync(
                $"api/categories/{id}", token);

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Category deleted successfully!";
            else
                TempData["Error"] =
                    "Unable to delete this category. It may have enrolments linked to it.";

            return RedirectToAction("Categories", new { eventId });
        }

        // View participants enrolled in one of the organiser's events
        public async Task<IActionResult> Enrolments(int eventId)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            string token = HttpContext.Session.GetString("Token") ?? "";
            string userId = HttpContext.Session.GetString("UserId") ?? "";

            // Confirm that this event belongs to the logged-in organiser
            var events = await _api.GetAsync<List<EventViewModel>>(
                "api/events", token) ?? new List<EventViewModel>();

            var myEvent = events.FirstOrDefault(e =>
                e.EventId == eventId &&
                e.OrganiserId.ToString() == userId);

            if (myEvent == null)
                return NotFound();

            // Get enrolments from the API
            var enrolments = await _api.GetAsync<List<EnrollmentViewModel>>(
                $"api/enrollments/event/{eventId}", token);

            ViewBag.EventName = myEvent.Name;
            ViewBag.EventId = eventId;

            return View(enrolments ?? new List<EnrollmentViewModel>());
        }

        // Display the record result form
        [HttpGet]
        public async Task<IActionResult> RecordResult(int enrollmentId, int eventId)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            string token = HttpContext.Session.GetString("Token") ?? "";

            // Confirm that the organiser owns the event
            var events = await _api.GetAsync<List<EventViewModel>>(
                "api/events", token) ?? new List<EventViewModel>();

            var myEvent = events.FirstOrDefault(e => e.EventId == eventId);

            if (myEvent == null ||
                myEvent.OrganiserId.ToString() !=
                HttpContext.Session.GetString("UserId"))
                return NotFound();

            ViewBag.EventId = eventId;
            ViewBag.EventName = myEvent.Name;

            return View(new RecordResultViewModel
            {
                EnrollmentId = enrollmentId
            });
        }

        // Save the result
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordResult(
            int eventId, RecordResultViewModel model)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            if (!TimeSpan.TryParse(model.FinishTime, out TimeSpan finishTime)
                || finishTime < TimeSpan.Zero)
            {
                ModelState.AddModelError(
                    "FinishTime", "Enter a valid time, for example 00:35:20.");
            }

            if (!ModelState.IsValid)
            {
                string tokenForEvent = HttpContext.Session.GetString("Token") ?? "";
                var eventsForForm = await _api.GetAsync<List<EventViewModel>>(
                    "api/events", tokenForEvent) ?? new List<EventViewModel>();

                ViewBag.EventName = eventsForForm
                    .FirstOrDefault(e => e.EventId == eventId)?.Name;
                ViewBag.EventId = eventId;

                return View(model);
            }

            string token = HttpContext.Session.GetString("Token") ?? "";

            var response = await _api.PostAsync(
                "api/results",
                new
                {
                    enrollment_id = model.EnrollmentId,
                    finish_time = finishTime,
                    finishing_position = model.FinishingPosition
                },
                token);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Result recorded successfully!";
            }
            else
            {
                TempData["Error"] =
                    "Unable to record result. A result may already exist for this enrolment.";
            }

            return RedirectToAction("Enrolments", new { eventId });
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