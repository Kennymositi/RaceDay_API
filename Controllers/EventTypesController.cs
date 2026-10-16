using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay_API.Database;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RaceDay_API.Controllers
{
    [Route("api/event-types")]
    [ApiController]
    public class EventTypesController : ControllerBase
    {
        private readonly RaceDay_Context _context;

        public EventTypesController(RaceDay_Context context)

        {

            _context = context;

        }

        [HttpGet]

        public async Task<IActionResult> GetEventTypes()

        {

            var eventTypes = await _context.EventTypes

                .AsNoTracking()

                .Select(e => new

                {

                    e.event_type_id,

                    e.type_name,

                    e.description

                })

                .ToListAsync();

            return Ok(eventTypes);


        }
    }
}

