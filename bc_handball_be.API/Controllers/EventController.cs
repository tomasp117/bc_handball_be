using AutoMapper;
using bc_handball_be.API.DTOs.Events;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers;

/// <summary>
/// Handles match event operations including adding, retrieving, and deleting events during matches.
/// </summary>
[Route("api/events")]
[ApiController]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly ILogger<EventController> _logger;
    private readonly IMapper _mapper;

    public EventController(IEventService eventService, ILogger<EventController> logger, IMapper mapper)
    {
        _eventService = eventService;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Adds a new event to a match (Admin or Recorder only).
    /// </summary>
    /// <param name="dto">The event data (goal, penalty, etc.).</param>
    /// <returns>Success if event added.</returns>
    /// <response code="200">Event added successfully.</response>
    /// <response code="403">If user lacks required role.</response>
    [Authorize(Roles = "Admin, Recorder")]
    [HttpPost("add")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddEvent([FromBody] EventDTO dto)
    {
        try
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("Recorder")) return Forbid("You don't have the required role.");
            var newEvent = _mapper.Map<Event>(dto);
            await _eventService.AddEventAsync(newEvent);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding event to match");
            return StatusCode(500, "An error occurred while adding the event.");
        }
    }

    /// <summary>
    /// Gets all events for a specific match.
    /// </summary>
    /// <param name="matchId">The match ID.</param>
    /// <returns>List of events in the match.</returns>
    /// <response code="200">Returns the list of events.</response>
    /// <response code="404">If no events found for the match.</response>
    [HttpGet("match/{matchId}")]
    [ProducesResponseType(typeof(IEnumerable<EventDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<EventDTO>>> GetEventsByMatch(int matchId)
    {
        try
        {
            var events = await _eventService.GetEventsByMatchIdAsync(matchId);
            if (events == null || !events.Any()) return NotFound();
            var eventDtos = _mapper.Map<List<EventDTO>>(events);
            return Ok(eventDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching events for match {MatchId}", matchId);
            return StatusCode(500, "An error occurred while fetching events.");
        }
    }

    /// <summary>
    /// Deletes all events for a specific match (Admin or Recorder only).
    /// </summary>
    /// <param name="matchId">The match ID.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Events deleted successfully.</response>
    [Authorize(Roles = "Admin, Recorder")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteByMatchId([FromQuery] int matchId)
    {
        try
        {
            await _eventService.DeleteAllByMatchIdAsync(matchId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting events for match {MatchId}", matchId);
            return StatusCode(500, "An error occurred while deleting events.");
        }
    }

    /// <summary>
    /// Deletes the last non-info event from a match (Admin or Recorder only).
    /// </summary>
    /// <param name="matchId">The match ID.</param>
    /// <returns>The deleted event.</returns>
    /// <response code="200">Returns the deleted event.</response>
    /// <response code="404">If no event to delete.</response>
    [Authorize(Roles = "Admin, Recorder")]
    [HttpDelete("last")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLastEvent([FromQuery] int matchId)
    {
        try
        {
            var deleted = await _eventService.DeleteLastNonInfoEventAsync(matchId);
            if (deleted == null) return NotFound();
            return Ok(deleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting last event for match {MatchId}", matchId);
            return StatusCode(500, "An error occurred while deleting the last event.");
        }
    }
}