using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RaceDay_API.Database;
using RaceDay_API.Models;

namespace RaceDay_API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly RaceDay_Context _context;
        private readonly IConfiguration _configuration;

        public AuthController(
            RaceDay_Context context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/auth/register
        // Registers a user and hashes their password with BCrypt.
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            string email = request.email.Trim().ToLowerInvariant();
            string role = request.role.Trim().ToUpperInvariant();

            if (role != "ORGANISER" && role != "PARTICIPANT")
            {
                return BadRequest(new
                {
                    message = "Role must be ORGANISER or PARTICIPANT."
                });
            }

            bool emailExists = await _context.Users
                .AnyAsync(u => u.email == email);

            if (emailExists)
            {
                return Conflict(new
                {
                    message = "An account with this email already exists."
                });
            }

            var user = new User
            {
                first_name = request.first_name.Trim(),
                last_name = request.last_name.Trim(),
                email = email,
                phone = request.phone,
                role = role
            };

            // Hash the password before saving it.
            string hashed_password =
                BCrypt.Net.BCrypt.HashPassword(request.password);

            user.password_hash = hashed_password;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Never return the password or its hash.
            return StatusCode(201, new
            {
                message = "Registration successful.",
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

        // POST: api/auth/login
        // Checks the stored BCrypt hash and returns a JWT.
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            string email = request.email.Trim().ToLowerInvariant();

            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.email == email);

            if (user == null ||
                string.IsNullOrWhiteSpace(user.password_hash))
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            // Verify the entered password against the saved hash.
            bool verify_password = BCrypt.Net.BCrypt.Verify(
                request.password,
                user.password_hash
            );

            if (!verify_password)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            // Read JWT settings from appsettings.json.
            string? key = _configuration["Jwt:Key"];
            string? issuer = _configuration["Jwt:Issuer"];
            string? audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(key) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                return StatusCode(500, new
                {
                    message = "JWT configuration is incomplete."
                });
            }

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.user_id.ToString()),

                new Claim(ClaimTypes.Name, user.first_name),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.Role, user.role)
            };

            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            string tokenString =
                new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                message = "Login successful.",
                token = tokenString,
                expiresAtUtc = token.ValidTo,
                user = new
                {
                    user.user_id,
                    user.first_name,
                    user.last_name,
                    user.email,
                    user.role
                }
            });
        }
    }

    // Data required to register a user.
    public class RegisterRequest
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

        [Required]
        [MinLength(8)]
        public string password { get; set; } = string.Empty;

        [Required]
        public string role { get; set; } = string.Empty;
    }

    // Data required to log in.
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string email { get; set; } = string.Empty;

        [Required]
        public string password { get; set; } = string.Empty;
    }
}