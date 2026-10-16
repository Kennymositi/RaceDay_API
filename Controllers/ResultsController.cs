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
    [Route("api/results")]
    [Authorize]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDay_Context _context;

        public ResultsController(RaceDay_Context context)
        {
            _context = context;
        }

        // GET: api/results/my
        // Participants can view only their own results.
        [Authorize(Roles = "PARTICIPANT")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyResults()
        {
            int userId = GetCurrentUserId();

            var results = await _context.Results
                .AsNoTracking()
                .Where(r => r.Enrollment != null &&
                            r.Enrollment.participant_id == userId)
                .Select(r => new
                {
                    r.result_id,
                    r.enrollment_id,
                    r.finish_time,
                    r.finishing_position
                })
                .ToListAsync();

            return Ok(results);
        }

        // POST: api/results
        // Organisers can record results for their own events.
        [Authorize(Roles = "ORGANISER")]
        [HttpPost]
        public async Task<IActionResult> CreateResult(
            [FromBody] ResultRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            int userId = GetCurrentUserId();

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e =>
                    e.enrollment_id == request.enrollment_id);

            if (enrollment == null)
            {
                return NotFound(new
                {
                    message = "Enrolment not found."
                });
            }

            var eventItem = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.event_id == enrollment.event_id);

            if (eventItem == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            if (eventItem.organiser_id != userId)
                return Forbid();

            bool resultExists = await _context.Results
                .AnyAsync(r =>
                    r.enrollment_id == request.enrollment_id);

            if (resultExists)
            {
                return Conflict(new
                {
                    message = "A result already exists for this enrolment."
                });
            }

            var result = new Result
            {
                enrollment_id = request.enrollment_id,
                finish_time = request.finish_time,
                finishing_position = request.finishing_position
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                message = "Result recorded successfully.",
                result.result_id,
                result.enrollment_id,
                result.finish_time,
                result.finishing_position
            });
        }

        // PUT: api/results/1
        // Organisers can correct results for their own events.
        [Authorize(Roles = "ORGANISER")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateResult(
            int id,
            [FromBody] UpdateResultRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            int userId = GetCurrentUserId();

            var result = await _context.Results
                .FirstOrDefaultAsync(r => r.result_id == id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Result not found."
                });
            }

            var enrollment = await _context.Enrollments
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.enrollment_id == result.enrollment_id);

            if (enrollment == null)
            {
                return NotFound(new
                {
                    message = "Enrolment not found."
                });
            }

            var eventItem = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.event_id == enrollment.event_id);

            if (eventItem == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            if (eventItem.organiser_id != userId)
                return Forbid();

            result.finish_time = request.finish_time;
            result.finishing_position = request.finishing_position;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Result updated successfully.",
                result.result_id,
                result.enrollment_id,
                result.finish_time,
                result.finishing_position
            });
        }

        private int GetCurrentUserId()
        {
            string? id = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(id, out int userId) ? userId : 0;
        }
    }

    public class ResultRequest
    {
        [Range(1, int.MaxValue)]
        public int enrollment_id { get; set; }

        [Required]
        public TimeSpan finish_time { get; set; }

        [Range(1, int.MaxValue)]
        public int finishing_position { get; set; }
    }

    public class UpdateResultRequest
    {
        [Required]
        public TimeSpan finish_time { get; set; }

        [Range(1, int.MaxValue)]
        public int finishing_position { get; set; }
    }
}