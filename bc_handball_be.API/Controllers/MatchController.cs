using System.Security.Claims;
using AutoMapper;
using bc_handball_be.API.DTOs.Matches;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers;

/// <summary>
/// Handles match operations including scheduling, assignments, score updates, and lineup management.
/// </summary>
[Route("api")]
[ApiController]
public class MatchController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILineupService _lineupService;
    private readonly ILogger<MatchController> _logger;
    private readonly IMapper _mapper;
    private readonly IMatchService _matchService;

    public MatchController(IMapper mapper, ILogger<MatchController> logger, IMatchService matchService,
        ICategoryService categoryService, ILineupService lineupService)
    {
        _mapper = mapper;
        _logger = logger;
        _matchService = matchService;
        _categoryService = categoryService;
        _lineupService = lineupService;
    }

    /// <summary>
    /// Generates blank match slots for a tournament edition (Admin only).
    /// </summary>
    /// <param name="edition">The tournament edition number.</param>
    /// <returns>The generated blank matches.</returns>
    /// <response code="200">Matches generated successfully.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("{edition}/matches/generate-blank")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerateBlankMatches(int edition)
    {
        try
        {
            _logger.LogInformation("Generating blank matches for tournament.");
            var matches = await _matchService.GenBlankMatches(edition);
            return Ok(matches);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating blank matches for edition {Edition}", edition);
            return StatusCode(500, "An error occurred while generating blank matches.");
        }
    }

    /// <summary>
    /// Assigns group stage matches for a specific category from scratch (Admin only).
    /// </summary>
    /// <param name="categoryId">The category ID.</param>
    /// <returns>The assigned matches.</returns>
    /// <response code="200">Matches assigned successfully.</response>
    /// <response code="404">If category not found.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("matches/assign-group-matches/{categoryId}")]
    [ProducesResponseType(typeof(List<MatchDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignGroupMatches(int categoryId)
    {
        try
        {
            _logger.LogInformation("Assigning group matches for category {categoryId}", categoryId);

            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {categoryId} not found", categoryId);
                return NotFound($"Category with ID {categoryId} not found");
            }

            var edition = category.TournamentInstance.EditionNumber;

            var matches = await _matchService.AssignGroupMatchesFromScratch(categoryId, edition);
            var dtos = _mapper.Map<List<MatchDTO>>(matches);
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning group matches for category {CategoryId}", categoryId);
            return StatusCode(500, "An error occurred while assigning group matches.");
        }
    }

    /// <summary>
    /// Assigns all group stage matches for all categories in an edition (Admin only).
    /// </summary>
    /// <param name="edition">The tournament edition number.</param>
    /// <returns>All assigned matches.</returns>
    /// <response code="200">All matches assigned successfully.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("{edition}/matches/assign-all-group-matches")]
    [ProducesResponseType(typeof(List<MatchDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignAllGroupMatches(int edition)
    {
        try
        {
            _logger.LogInformation("Assigning all group matches");
            var matches = await _matchService.AssignAllGroupMatchesFromScratch(edition);
            var dtos = _mapper.Map<List<MatchDTO>>(matches);
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning all group matches for edition {Edition}", edition);
            return StatusCode(500, "An error occurred while assigning all group matches.");
        }
    }

    /// <summary>
    /// Gets all matches.
    /// </summary>
    /// <returns>List of all matches.</returns>
    /// <response code="200">Returns the list of matches.</response>
    [HttpGet("matches")]
    [ProducesResponseType(typeof(List<MatchDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMatches()
    {
        try
        {
            _logger.LogInformation("Fetching matches");
            var matches = await _matchService.GetMatchesAsync();
            var dtos = _mapper.Map<List<MatchDTO>>(matches);
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching matches");
            return StatusCode(500, "An error occurred while fetching matches.");
        }
    }

    /// <summary>
    /// Gets matches formatted for match reports.
    /// </summary>
    /// <returns>List of matches for reporting.</returns>
    /// <response code="200">Returns the list of matches.</response>
    [HttpGet("matches/match-report")]
    [ProducesResponseType(typeof(List<MatchDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMatchesSimple()
    {
        try
        {
            _logger.LogInformation("Fetching matches");
            var matches = await _matchService.GetMatchesForReportAsync();
            var dtos = _mapper.Map<List<MatchDTO>>(matches);
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching matches for report");
            return StatusCode(500, "An error occurred while fetching matches.");
        }
    }

    /// <summary>
    /// Gets matches formatted for timetable display.
    /// </summary>
    /// <returns>List of matches for timetable.</returns>
    /// <response code="200">Returns the list of matches.</response>
    [HttpGet("matches/timetable")]
    [ProducesResponseType(typeof(List<MatchDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMatchesForTimetable()
    {
        try
        {
            _logger.LogInformation("Fetching matches");
            var matches = await _matchService.GetMatchesForTimetableAsync();

            var dtos = _mapper.Map<List<MatchDTO>>(matches);
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching matches for timetable");
            return StatusCode(500, "An error occurred while fetching matches.");
        }
    }


    /// <summary>
    /// Gets unassigned group matches (matches without time/court assignments) for a category.
    /// </summary>
    /// <param name="categoryId">The category ID.</param>
    /// <returns>List of unassigned matches.</returns>
    /// <response code="200">Returns the list of unassigned matches.</response>
    [HttpGet("matches/unassigned-group-matches/{categoryId}")]
    [ProducesResponseType(typeof(List<UnassignedMatchDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UnassignedMatchDTO>>> GetUnassignedGroupMatches(int categoryId)
    {
        try
        {
            var matches = await _matchService.GetUnassignedGroupMatches(categoryId);

            var dto = _mapper.Map<List<UnassignedMatchDTO>>(matches);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching unassigned group matches for category {CategoryId}", categoryId);
            return StatusCode(500, "An error occurred while fetching unassigned matches.");
        }
    }

    /// <summary>
    /// Updates multiple match assignments (time/court) in batch (Admin only).
    /// </summary>
    /// <param name="assignments">List of match assignments.</param>
    /// <returns>Success if updated.</returns>
    /// <response code="200">Matches updated successfully.</response>
    [Authorize(Roles = "Admin")]
    [HttpPut("matches/update-batch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateBatch([FromBody] List<MatchAssignmentDTO> assignments)
    {
        try
        {
            var assignmentEntities = _mapper.Map<List<Match>>(assignments);
            await _matchService.UpdateMatchesAsync(assignmentEntities);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating match batch");
            return StatusCode(500, "An error occurred while updating matches.");
        }
    }

    /// <summary>
    /// Gets a specific match by ID (with recorder permission check).
    /// </summary>
    /// <param name="id">The match ID.</param>
    /// <returns>The match details.</returns>
    /// <response code="200">Returns the match.</response>
    /// <response code="404">If match not found.</response>
    /// <response code="403">If recorder accessing wrong playground.</response>
    [HttpGet("matches/{id}")]
    [ProducesResponseType(typeof(MatchDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMatchById(int id)
    {
        try
        {
            var match = await _matchService.GetMatchByIdAsync(id);
            if (match == null)
                return NotFound();

            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var username = User.Identity?.Name
                           ?? User.FindFirstValue(ClaimTypes.Name)
                           ?? User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;

            // Debug: zaloguj username a playground, abys viděl, co porovnáváš
            _logger.LogInformation("User {username} is requesting match {id} on playground {playground}", username, id,
                match.Playground);

            if (userRole == "Recorder")
                // Porovnej s ohledem na malá/velká písmena
                if (!string.Equals(match.Playground, username, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning(
                        "User {username} does not have permission to access match {id} on playground {playground}",
                        username, id, match.Playground);

                    return Forbid();
                }

            var dto = _mapper.Map<MatchDTO>(match);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching match with ID {MatchId}", id);
            return StatusCode(500, "An error occurred while fetching the match.");
        }
    }

    /// <summary>
    /// Updates match details (score, status, etc.) (Admin or Recorder only).
    /// </summary>
    /// <param name="id">The match ID.</param>
    /// <param name="dto">The updated match data.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Match updated successfully.</response>
    /// <response code="404">If match not found.</response>
    [Authorize(Roles = "Admin, Recorder")]
    [HttpPatch("matches/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMatch(int id, [FromBody] MatchUpdateDTO dto)
    {
        try
        {
            _logger.LogInformation("Updating match with ID {id}", id);
            var match = await _matchService.GetMatchByIdAsync(id);
            if (match == null)
                return NotFound();

            _mapper.Map(dto, match);
            await _matchService.UpdateMatchAsync(match);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating match with ID {MatchId}", id);
            return StatusCode(500, "An error occurred while updating the match.");
        }
    }

    /// <summary>
    /// Gets all matches for a specific category.
    /// </summary>
    /// <param name="category">The category ID.</param>
    /// <returns>List of matches in the category.</returns>
    /// <response code="200">Returns the list of matches.</response>
    /// <response code="404">If no matches found.</response>
    [HttpGet("matches/by-category")]
    [ProducesResponseType(typeof(List<MatchDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMatchesByCategory([FromQuery] int category)
    {
        try
        {
            var matches = await _matchService.GetMatchesByCategoryIdAsync(category);
            if (matches == null || !matches.Any())
                return NotFound();

            var matchDtos = _mapper.Map<List<MatchDTO>>(matches);
            return Ok(matchDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching matches for category {CategoryId}", category);
            return StatusCode(500, "An error occurred while fetching matches.");
        }
    }

    /// <summary>
    /// Generates lineups for a match based on team rosters (Admin or Recorder only).
    /// </summary>
    /// <param name="matchId">The match ID.</param>
    /// <returns>Success if lineups generated.</returns>
    /// <response code="200">Lineups generated successfully.</response>
    /// <response code="500">If an error occurred during generation.</response>
    [HttpPost("matches/{matchId}/generate-lineups")]
    [Authorize(Roles = "Admin, Recorder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerateLineups(int matchId)
    {
        try
        {
            _logger.LogDebug("Generating lineups for match with ID {matchId}", matchId);
            await _lineupService.GenerateLineupsForMatchAsync(matchId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Chyba při generování soupisky pro zápas {matchId}", matchId);
            return StatusCode(500, "Chyba při generování soupisky.");
        }
    }

    /// <summary>
    /// Gets all match time slots for a tournament edition (Admin only).
    /// </summary>
    /// <param name="edition">The tournament edition number.</param>
    /// <returns>List of time slots.</returns>
    /// <response code="200">Returns the list of slots.</response>
    [Authorize(Roles = "Admin")]
    [HttpGet("{edition}/matches/slots")]
    [ProducesResponseType(typeof(List<SlotDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSlots(int edition)
    {
        try
        {
            var slots = await _matchService.GetGeneratedSlotsAsync(edition);
            var dto = _mapper.Map<List<SlotDTO>>(slots);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching slots for edition {Edition}", edition);
            return StatusCode(500, "An error occurred while fetching slots.");
        }
    }

    /// <summary>
    /// Adds a new match time slot (Admin only).
    /// </summary>
    /// <param name="edition">The tournament edition number.</param>
    /// <param name="body">The slot data (time and playground).</param>
    /// <returns>The created slot.</returns>
    /// <response code="201">Slot created successfully.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("{edition}/matches/slots")]
    [ProducesResponseType(typeof(SlotDTO), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddSlot(int edition, [FromBody] SlotDTO body)
    {
        try
        {
            var slot = await _matchService.CreateSlotAsync(edition, body.Time, body.Playground);
            var dto = _mapper.Map<SlotDTO>(slot);
            return CreatedAtAction(nameof(GetSlots), new { edition }, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating slot for edition {Edition}", edition);
            return StatusCode(500, "An error occurred while creating the slot.");
        }
    }

    /// <summary>
    /// Deletes a match time slot (Admin only).
    /// </summary>
    /// <param name="slotId">The slot ID to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Slot deleted successfully.</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("matches/slots/{slotId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteSlot(int slotId)
    {
        try
        {
            await _matchService.DeleteSlotAsync(slotId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting slot {SlotId}", slotId);
            return StatusCode(500, "An error occurred while deleting the slot.");
        }
    }
}