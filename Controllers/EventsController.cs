using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_API.Database;
using RaceDay_API.Models;

namespace RaceDay_API.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDay_Context _context;

        public EventsController(RaceDay_Context context)
        {
            _context = context;
        }

        // GET: api/events
        // Public: list all events.
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .AsNoTracking()
                .Select(e => new
                {
                    e.event_id,
                    e.name,
                    e.description,
                    e.event_date,
                    e.location,
                    e.distance,
                    e.event_type_id,
                    e.organiser_id
                })
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/events/1
        // Public: view one event.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventItem = await _context.Events
                .AsNoTracking()
                .Where(e => e.event_id == id)
                .Select(e => new
                {
                    e.event_id,
                    e.name,
                    e.description,
                    e.event_date,
                    e.location,
                    e.distance,
                    e.event_type_id,
                    e.organiser_id
                })
                .FirstOrDefaultAsync();

            if (eventItem == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            return Ok(eventItem);
        }

        // POST: api/events
        // Only organisers can create events.
        [Authorize(Roles = "ORGANISER")]
        [HttpPost]
        public async Task<IActionResult> CreateEvent(
            [FromBody] EventRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            int organiserId = GetCurrentUserId();

            if (organiserId <= 0)
                return Unauthorized();

            bool typeExists = await _context.EventTypes
                .AnyAsync(t => t.event_type_id == request.event_type_id);

            if (!typeExists)
            {
                return BadRequest(new
                {
                    message = "The selected event type does not exist."
                });
            }

            var newEvent = new Event
            {
                name = request.name.Trim(),
                description = request.description,
                event_date = request.event_date,
                location = request.location.Trim(),
                distance = request.distance,
                event_type_id = request.event_type_id,
                organiser_id = organiserId
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = newEvent.event_id },
                new
                {
                    newEvent.event_id,
                    newEvent.name,
                    newEvent.description,
                    newEvent.event_date,
                    newEvent.location,
                    newEvent.distance,
                    newEvent.event_type_id,
                    newEvent.organiser_id
                });
        }

        // PUT: api/events/1
        // Only the organiser who owns the event can update it.
        [Authorize(Roles = "ORGANISER")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            [FromBody] EventRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            int organiserId = GetCurrentUserId();

            var existingEvent = await _context.Events
                .FirstOrDefaultAsync(e => e.event_id == id);

            if (existingEvent == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            if (existingEvent.organiser_id != organiserId)
            {
                return Forbid();
            }

            bool typeExists = await _context.EventTypes
                .AnyAsync(t => t.event_type_id == request.event_type_id);

            if (!typeExists)
            {
                return BadRequest(new
                {
                    message = "The selected event type does not exist."
                });
            }

            existingEvent.name = request.name.Trim();
            existingEvent.description = request.description;
            existingEvent.event_date = request.event_date;
            existingEvent.location = request.location.Trim();
            existingEvent.distance = request.distance;
            existingEvent.event_type_id = request.event_type_id;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Event updated successfully.",
                existingEvent.event_id,
                existingEvent.name,
                existingEvent.description,
                existingEvent.event_date,
                existingEvent.location,
                existingEvent.distance,
                existingEvent.event_type_id,
                existingEvent.organiser_id
            });
        }

        // DELETE: api/events/1
        // Only the organiser who owns the event can delete it.
        [Authorize(Roles = "ORGANISER")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            int organiserId = GetCurrentUserId();

            var existingEvent = await _context.Events
                .FirstOrDefaultAsync(e => e.event_id == id);

            if (existingEvent == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            if (existingEvent.organiser_id != organiserId)
            {
                return Forbid();
            }

            _context.Events.Remove(existingEvent);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "This event cannot be deleted because it has related categories, enrolments or results. Remove or handle those records first."
                });
            }

            return Ok(new { message = "Event deleted successfully." });
        }

        // Gets the logged-in user's ID from their JWT.
        private int GetCurrentUserId()
        {
            string? id = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(id, out int userId) ? userId : 0;
        }
    }

    public class EventRequest
    {
        [Required]
        [StringLength(150)]
        public string name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? description { get; set; }

        [Required]
        public DateTime event_date { get; set; }

        [Required]
        [StringLength(200)]
        public string location { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "9999.99")]
        public decimal distance { get; set; }

        [Range(1, int.MaxValue)]
        public int event_type_id { get; set; }
    }
}