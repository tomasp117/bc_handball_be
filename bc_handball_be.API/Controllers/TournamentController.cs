using AutoMapper;
using bc_handball_be.API.DTOs.Tournaments;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers;

/// <summary>
/// Handles tournament management operations including CRUD for tournaments.
/// </summary>
[Route("api/tournaments")]
[ApiController]
public class TournamentController : ControllerBase
{
    private readonly ILogger<TournamentController> _logger;
    private readonly IMapper _mapper;
    private readonly ITournamentService _tournamentService;

    public TournamentController(ILogger<TournamentController> logger, IMapper mapper,
        ITournamentService tournamentService)
    {
        _logger = logger;
        _mapper = mapper;
        _tournamentService = tournamentService;
    }

    /// <summary>
    /// Creates a new tournament (Admin only).
    /// </summary>
    /// <param name="dto">The tournament data.</param>
    /// <returns>The created tournament.</returns>
    /// <response code="200">Tournament created successfully.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateTournament([FromBody] TournamentDTO dto)
    {
        try
        {
            var tournament = _mapper.Map<Tournament>(dto);
            var created = await _tournamentService.CreateTournamentAsync(tournament);
            return Ok(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tournament");
            return StatusCode(500, "An error occurred while creating the tournament.");
        }
    }

    /// <summary>
    /// Gets all tournaments.
    /// </summary>
    /// <returns>List of all tournaments.</returns>
    /// <response code="200">Returns the list of tournaments.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TournamentDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTournaments()
    {
        try
        {
            var tournaments = await _tournamentService.GetAllTournamentsAsync();
            var tournamentDTOs = _mapper.Map<IEnumerable<TournamentDTO>>(tournaments);
            return Ok(tournamentDTOs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tournaments");
            return StatusCode(500, "An error occurred while fetching tournaments.");
        }
    }

    /// <summary>
    /// Gets a specific tournament by ID.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <returns>The tournament details.</returns>
    /// <response code="200">Returns the tournament.</response>
    /// <response code="404">If tournament not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TournamentDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTournamentById(int id)
    {
        try
        {
            var tournament = await _tournamentService.GetTournamentByIdAsync(id);
            if (tournament == null) return NotFound();
            var tournamentDTO = _mapper.Map<TournamentDTO>(tournament);
            return Ok(tournamentDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tournament with ID {TournamentId}", id);
            return StatusCode(500, "An error occurred while fetching the tournament.");
        }
    }

    /// <summary>
    /// Updates an existing tournament (Admin only).
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="dto">The updated tournament data.</param>
    /// <returns>The updated tournament.</returns>
    /// <response code="200">Tournament updated successfully.</response>
    /// <response code="404">If tournament not found.</response>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTournament(int id, [FromBody] TournamentDTO dto)
    {
        try
        {
            var tournament = _mapper.Map<Tournament>(dto);
            var updated = await _tournamentService.UpdateTournamentAsync(tournament);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tournament with ID {TournamentId}", id);
            return StatusCode(500, "An error occurred while updating the tournament.");
        }
    }

    /// <summary>
    /// Deletes a tournament (Admin only).
    /// </summary>
    /// <param name="id">The tournament ID to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Tournament deleted successfully.</response>
    /// <response code="404">If tournament not found.</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTournament(int id)
    {
        try
        {
            var tournament = await _tournamentService.GetTournamentByIdAsync(id);
            if (tournament == null) return NotFound();
            await _tournamentService.DeleteTournamentAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tournament with ID {TournamentId}", id);
            return StatusCode(500, "An error occurred while deleting the tournament.");
        }
    }
}