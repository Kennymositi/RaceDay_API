using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_API.Database;
using RaceDay_API.Models;

namespace RaceDay_API.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    [Authorize]
    public class EnrollmentsController : ControllerBase
    {
        private readonly RaceDay_Context _context;

        public EnrollmentsController(RaceDay_Context context)
        {
            _context = context;
        }

        // GET: api/enrollments/my
        // Participants can view their own enrolments.
        [Authorize(Roles = "PARTICIPANT")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            int userId = GetCurrentUserId();

            var enrollments = await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.participant_id == userId)
                .Select(e => new
                {
                    e.enrollment_id,
                    e.participant_id,
                    e.event_id,
                    e.category_id,
                    e.enrollment_date,
                    e.status
                })
                .ToListAsync();

            return Ok(enrollments);
        }

        // POST: api/enrollments
        // Participants enrol themselves in an event category.
        [Authorize(Roles = "PARTICIPANT")]
        [HttpPost]
        public async Task<IActionResult> CreateEnrollment(
            [FromBody] EnrollmentRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            int userId = GetCurrentUserId();

            bool eventExists = await _context.Events
                .AnyAsync(e => e.event_id == request.event_id);

            if (!eventExists)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.category_id == request.category_id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Category not found."
                });
            }

            if (category.event_id != request.event_id)
            {
                return BadRequest(new
                {
                    message = "The selected category does not belong to this event."
                });
            }

            bool alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e =>
                    e.participant_id == userId &&
                    e.event_id == request.event_id);

            if (alreadyEnrolled)
            {
                return Conflict(new
                {
                    message = "You are already enrolled in this event."
                });
            }

            var enrollment = new Enrollment
            {
                participant_id = userId,
                event_id = request.event_id,
                category_id = request.category_id,
                enrollment_date = DateTime.Today,
                status = "ENTERED"
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                message = "Enrolment successful.",
                enrollment.enrollment_id,
                enrollment.participant_id,
                enrollment.event_id,
                enrollment.category_id,
                enrollment.enrollment_date,
                enrollment.status
            });
        }

        // GET: api/enrollments/event/1
        // Organisers can view enrolments for their own events.
        [Authorize(Roles = "ORGANISER")]
        [HttpGet("event/{eventId:int}")]
        public async Task<IActionResult> GetEventEnrollments(int eventId)
        {
            int userId = GetCurrentUserId();

            var eventItem = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.event_id == eventId);

            if (eventItem == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            if (eventItem.organiser_id != userId)
                return Forbid();

            var enrollments = await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.event_id == eventId)
                .Select(e => new
                {
                    e.enrollment_id,
                    e.participant_id,
                    e.event_id,
                    e.category_id,
                    e.enrollment_date,
                    e.status
                })
                .ToListAsync();

            return Ok(enrollments);
        }

        private int GetCurrentUserId()
        {
            string? id = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(id, out int userId) ? userId : 0;
        }
    }

    public class EnrollmentRequest
    {
        public int event_id { get; set; }

        public int category_id { get; set; }
    }
}