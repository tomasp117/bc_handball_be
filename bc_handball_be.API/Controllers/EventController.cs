using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bc_handball_be.API.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;
        private readonly IMapper _mapper;
        private readonly IEventService _eventService;

        public EventController(IEventService eventService, ILogger<EventController> logger, IMapper mapper)
        {
            _eventService = eventService;
            _logger = logger;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, Recorder")]
        [HttpPost("add")]
        public async Task<IActionResult> AddEvent([FromBody] EventDTO dto)
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("Recorder"))
            {
                return Forbid("You don't have the required role.");
            }
            var newEvent = _mapper.Map<Event>(dto);
            await _eventService.AddEventAsync(newEvent);
            return Ok();
        }

        [HttpGet("match/{matchId}")]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetEventsByMatch(int matchId)
        {
            var events = await _eventService.GetEventsByMatchIdAsync(matchId);
            if (events == null || !events.Any())
            {
                return NotFound();
            }
            var eventDtos = _mapper.Map<List<EventDTO>>(events);
            return Ok(eventDtos);
        }

        [Authorize(Roles = "Admin, Recorder")]
        [HttpDelete]
        public async Task<IActionResult> DeleteByMatchId([FromQuery] int matchId)
        {
            await _eventService.DeleteAllByMatchIdAsync(matchId);
            return NoContent();
        }

        [Authorize(Roles = "Admin, Recorder")]
        [HttpDelete("last")]
        public async Task<IActionResult> DeleteLastEvent([FromQuery] int matchId)
        {
            var deleted = await _eventService.DeleteLastNonInfoEventAsync(matchId);
            if (deleted == null) return NotFound();
            return Ok(deleted);
        }
    }
}
