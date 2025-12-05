using System.Security.Claims;
using AutoMapper;
using bc_handball_be.API.DTOs.Coaches;
using bc_handball_be.API.DTOs.Teams;
using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers;

/// <summary>
/// Handles coach management operations including account creation, team access, and deletion.
/// </summary>
[Route("api/coaches")]
[ApiController]
public class CoachController : ControllerBase
{
    private readonly ICoachService _coachService;
    private readonly ILogger<CoachController> _logger;
    private readonly IMapper _mapper;

    public CoachController(ICoachService coachService, IMapper mapper, ILogger<CoachController> logger)
    {
        _coachService = coachService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Gets the team details for the currently logged-in coach.
    /// </summary>
    /// <returns>Detailed team information including players and lineup.</returns>
    /// <response code="200">Returns the team details.</response>
    /// <response code="404">If no team found for the coach.</response>
    [Authorize(Roles = "Coach")]
    [HttpGet("my-team")]
    [ProducesResponseType(typeof(TeamDetailDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyTeam()
    {
        try
        {
            _logger.LogInformation("Getting team for coach with ID {CoachId}",
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var personId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var team = await _coachService.GetTeamForPersonIdAsync(personId);
            if (team == null) return NotFound("Tým nenalezen");

            var teamDto = _mapper.Map<TeamDetailDTO>(team);

            return Ok(teamDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching team for coach");
            return StatusCode(500, "An error occurred while fetching the team.");
        }
    }

    /// <summary>
    /// Creates a new coach account (ClubAdmin or Admin only).
    /// </summary>
    /// <param name="dto">Coach registration data.</param>
    /// <returns>Success if coach created.</returns>
    /// <response code="200">Coach created successfully.</response>
    /// <response code="400">If username already exists or data is invalid.</response>
    /// <response code="500">If an error occurred during creation.</response>
    [HttpPost]
    [Authorize(Roles = "ClubAdmin, Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCoach([FromBody] CoachCreateDTO dto)
    {
        try
        {
            var coach = _mapper.Map<Coach>(dto);

            coach.Person.Login.SetPassword(dto.Password);

            await _coachService.CreateCoachAsync(coach);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Username conflict when creating coach.");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Chyba při vytváření trenéra.");
            return StatusCode(500, "Něco se pokazilo při ukládání trenéra.");
        }
    }

    /// <summary>
    /// Deletes a coach account.
    /// </summary>
    /// <param name="id">The coach ID to delete.</param>
    /// <returns>Success if coach deleted.</returns>
    /// <response code="200">Coach deleted successfully.</response>
    /// <response code="404">If coach not found.</response>
    /// <response code="500">If an error occurred during deletion.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCoach(int id)
    {
        try
        {
            await _coachService.DeleteCoachAsync(id);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Trenér nenalezen.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Chyba při mazání trenéra.");
            return StatusCode(500, "Něco se pokazilo při mazání trenéra.");
        }
    }
}