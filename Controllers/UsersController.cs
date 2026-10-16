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
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly RaceDay_Context _context;

        public UsersController(RaceDay_Context context)
        {
            _context = context;
        }

        // GET: api/users/me
        // View the currently logged-in user's profile.
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            int userId = GetCurrentUserId();

            if (userId <= 0)
                return Unauthorized();

            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.user_id == userId)
                .Select(u => new
                {
                    u.user_id,
                    u.first_name,
                    u.last_name,
                    u.email,
                    u.phone,
                    u.role
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User profile not found."
                });
            }

            return Ok(user);
        }

        // PUT: api/users/me
        // Update the currently logged-in user's profile.
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile(
            [FromBody] UpdateProfileRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            int userId = GetCurrentUserId();

            if (userId <= 0)
                return Unauthorized();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.user_id == userId);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User profile not found."
                });
            }

            string email = request.email.Trim().ToLowerInvariant();

            bool emailInUse = await _context.Users.AnyAsync(
                u => u.email == email && u.user_id != userId);

            if (emailInUse)
            {
                return Conflict(new
                {
                    message = "That email address is already in use."
                });
            }

            user.first_name = request.first_name.Trim();
            user.last_name = request.last_name.Trim();
            user.email = email;
            user.phone = request.phone;

            // Do not allow profile updates to change role or password.
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile updated successfully.",
                user = new
                {
                    user.user_id,
                    user.first_name,
                    user.last_name,
                    user.email,
                    user.phone,
                    user.role
                }
            });
        }

        private int GetCurrentUserId()
        {
            string? id = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(id, out int userId) ? userId : 0;
        }
    }

    public class UpdateProfileRequest
    {
        [Required]
        [StringLength(50)]
        public string first_name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string last_name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string email { get; set; } = string.Empty;

        [StringLength(30)]
        public string? phone { get; set; }
    }
}