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
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDay_Context _context;

        public CategoriesController(RaceDay_Context context)
        {
            _context = context;
        }

        // GET: api/categories
        // Public: view all categories.
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Select(c => new
                {
                    c.category_id,
                    c.event_id,
                    c.category_name,
                    c.category_type,
                    c.category_value,
                    c.description
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/categories/event/1
        // Public: view categories belonging to an event.
        [HttpGet("event/{eventId:int}")]
        public async Task<IActionResult> GetCategoriesByEvent(int eventId)
        {
            bool eventExists = await _context.Events
                .AnyAsync(e => e.event_id == eventId);

            if (!eventExists)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.event_id == eventId)
                .Select(c => new
                {
                    c.category_id,
                    c.event_id,
                    c.category_name,
                    c.category_type,
                    c.category_value,
                    c.description
                })
                .ToListAsync();

            return Ok(categories);
        }

        // POST: api/categories
        // Only the organiser who owns the event can add categories.
        [Authorize(Roles = "ORGANISER")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(
            [FromBody] CategoryRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            int userId = GetCurrentUserId();

            var eventItem = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.event_id == request.event_id);

            if (eventItem == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            if (eventItem.organiser_id != userId)
                return Forbid();

            string categoryType =
                request.category_type.Trim().ToUpperInvariant();

            if (categoryType != "AGE" && categoryType != "DISTANCE")
            {
                return BadRequest(new
                {
                    message = "Category type must be AGE or DISTANCE."
                });
            }

            string categoryName = request.category_name.Trim();

            bool nameExists = await _context.Categories.AnyAsync(c =>
                c.event_id == request.event_id &&
                c.category_name == categoryName);

            if (nameExists)
            {
                return Conflict(new
                {
                    message = "A category with that name already exists for this event."
                });
            }

            var category = new Category
            {
                event_id = request.event_id,
                category_name = categoryName,
                category_type = categoryType,
                category_value = request.category_value.Trim(),
                description = request.description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                message = "Category created successfully.",
                category.category_id,
                category.event_id,
                category.category_name,
                category.category_type,
                category.category_value,
                category.description
            });
        }

        // PUT: api/categories/1
        // Only the organiser who owns the category's event can update it.
        [Authorize(Roles = "ORGANISER")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(
            int id,
            [FromBody] UpdateCategoryRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            int userId = GetCurrentUserId();

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.category_id == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Category not found."
                });
            }

            var eventItem = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.event_id == category.event_id);

            if (eventItem == null)
            {
                return NotFound(new
                {
                    message = "The category's event was not found."
                });
            }

            if (eventItem.organiser_id != userId)
                return Forbid();

            string categoryType =
                request.category_type.Trim().ToUpperInvariant();

            if (categoryType != "AGE" && categoryType != "DISTANCE")
            {
                return BadRequest(new
                {
                    message = "Category type must be AGE or DISTANCE."
                });
            }

            string categoryName = request.category_name.Trim();

            bool nameExists = await _context.Categories.AnyAsync(c =>
                c.event_id == category.event_id &&
                c.category_name == categoryName &&
                c.category_id != id);

            if (nameExists)
            {
                return Conflict(new
                {
                    message = "A category with that name already exists for this event."
                });
            }

            category.category_name = categoryName;
            category.category_type = categoryType;
            category.category_value = request.category_value.Trim();
            category.description = request.description;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Category updated successfully.",
                category.category_id,
                category.event_id,
                category.category_name,
                category.category_type,
                category.category_value,
                category.description
            });
        }

        // DELETE: api/categories/1
        // Only the organiser who owns the category's event can delete it.
        [Authorize(Roles = "ORGANISER")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            int userId = GetCurrentUserId();

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.category_id == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Category not found."
                });
            }

            var eventItem = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.event_id == category.event_id);

            if (eventItem == null)
            {
                return NotFound(new
                {
                    message = "The category's event was not found."
                });
            }

            if (eventItem.organiser_id != userId)
                return Forbid();

            _context.Categories.Remove(category);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "This category cannot be deleted because it may have enrolments linked to it."
                });
            }

            return Ok(new
            {
                message = "Category deleted successfully."
            });
        }

        private int GetCurrentUserId()
        {
            string? id = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(id, out int userId) ? userId : 0;
        }
    }

    public class CategoryRequest
    {
        [Range(1, int.MaxValue)]
        public int event_id { get; set; }

        [Required]
        [StringLength(100)]
        public string category_name { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(AGE|DISTANCE)$",
            ErrorMessage = "Category type must be AGE or DISTANCE.")]
        public string category_type { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string category_value { get; set; } = string.Empty;

        [StringLength(255)]
        public string? description { get; set; }
    }

    public class UpdateCategoryRequest
    {
        [Required]
        [StringLength(100)]
        public string category_name { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(AGE|DISTANCE)$",
            ErrorMessage = "Category type must be AGE or DISTANCE.")]
        public string category_type { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string category_value { get; set; } = string.Empty;

        [StringLength(255)]
        public string? description { get; set; }
    }
}
