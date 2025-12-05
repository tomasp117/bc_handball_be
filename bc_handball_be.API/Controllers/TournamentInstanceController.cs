using AutoMapper;
using bc_handball_be.API.DTOs.Tournaments;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers;

/// <summary>
/// Handles tournament instance (edition/year) management operations including CRUD.
/// </summary>
[Route("api/tournament-instances")]
[ApiController]
public class TournamentInstanceController : ControllerBase
{
    private readonly ILogger<TournamentInstanceController> _logger;
    private readonly IMapper _mapper;
    private readonly ITournamentInstanceService _tournamentInstanceService;

    public TournamentInstanceController(ILogger<TournamentInstanceController> logger, IMapper mapper,
        ITournamentInstanceService tournamentInstanceService)
    {
        _logger = logger;
        _mapper = mapper;
        _tournamentInstanceService = tournamentInstanceService;
    }

    /// <summary>
    /// Creates a new tournament instance/edition (Admin only).
    /// </summary>
    /// <param name="dto">The tournament instance data.</param>
    /// <returns>The created tournament instance.</returns>
    /// <response code="200">Tournament instance created successfully.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateInstance([FromBody] TournamentInstanceDTO dto)
    {
        try
        {
            var tournamentInstance = _mapper.Map<TournamentInstance>(dto);
            var created = await _tournamentInstanceService.CreateTournamentInstanceAsync(tournamentInstance);
            return Ok(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tournament instance");
            return StatusCode(500, "An error occurred while creating the tournament instance.");
        }
    }

    /// <summary>
    /// Gets all tournament instances.
    /// </summary>
    /// <returns>List of all tournament instances.</returns>
    /// <response code="200">Returns the list of tournament instances.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<TournamentInstanceDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllInstances()
    {
        try
        {
            var tournamentInstances = await _tournamentInstanceService.GetAllTournamentInstancesAsync();
            var tournamentInstancesDto = _mapper.Map<List<TournamentInstanceDTO>>(tournamentInstances);
            return Ok(tournamentInstancesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tournament instances");
            return StatusCode(500, "An error occurred while fetching tournament instances.");
        }
    }

    /// <summary>
    /// Gets all instances for a specific tournament.
    /// </summary>
    /// <param name="tournamentId">The tournament ID.</param>
    /// <returns>List of tournament instances.</returns>
    /// <response code="200">Returns the list of instances.</response>
    [HttpGet("by-tournament")]
    [ProducesResponseType(typeof(List<TournamentInstanceDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByTournamentId([FromQuery] int tournamentId)
    {
        try
        {
            var instances = await _tournamentInstanceService.GetByTournamentIdAsync(tournamentId);
            var instancesDto = _mapper.Map<List<TournamentInstanceDTO>>(instances);
            return Ok(instancesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching instances for tournament {TournamentId}", tournamentId);
            return StatusCode(500, "An error occurred while fetching tournament instances.");
        }
    }

    /// <summary>
    /// Gets a specific tournament instance by ID.
    /// </summary>
    /// <param name="id">The tournament instance ID.</param>
    /// <returns>The tournament instance details.</returns>
    /// <response code="200">Returns the tournament instance.</response>
    /// <response code="404">If instance not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TournamentInstanceDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var instance = await _tournamentInstanceService.GetByIdAsync(id);
            if (instance == null)
                return NotFound();

            var dto = _mapper.Map<TournamentInstanceDTO>(instance);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tournament instance with ID {InstanceId}", id);
            return StatusCode(500, "An error occurred while fetching the tournament instance.");
        }
    }

    /// <summary>
    /// Updates an existing tournament instance.
    /// </summary>
    /// <param name="id">The tournament instance ID.</param>
    /// <param name="dto">The updated tournament instance data.</param>
    /// <returns>The updated tournament instance.</returns>
    /// <response code="200">Tournament instance updated successfully.</response>
    /// <response code="400">If ID mismatch.</response>
    /// <response code="404">If instance not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] TournamentInstanceDTO dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var instance = _mapper.Map<TournamentInstance>(dto);
            var updated = await _tournamentInstanceService.UpdateTournamentInstanceAsync(instance);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tournament instance with ID {InstanceId}", id);
            return StatusCode(500, "An error occurred while updating the tournament instance.");
        }
    }

    /// <summary>
    /// Deletes a tournament instance.
    /// </summary>
    /// <param name="id">The tournament instance ID to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Tournament instance deleted successfully.</response>
    /// <response code="404">If instance not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var instance = await _tournamentInstanceService.GetByIdAsync(id);
            if (instance == null)
                return NotFound();
            await _tournamentInstanceService.DeleteTournamentInstanceAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tournament instance with ID {InstanceId}", id);
            return StatusCode(500, "An error occurred while deleting the tournament instance.");
        }
    }
}