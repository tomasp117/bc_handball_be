using AutoMapper;
using bc_handball_be.API.DTOs.Players;
using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers;

/// <summary>
/// Handles player management operations including CRUD, team assignments, and match statistics.
/// </summary>
[ApiController]
[Route("api/players")]
public class PlayersController : ControllerBase
{
    private readonly ILogger<PlayersController> _logger;
    private readonly IMapper _mapper;
    private readonly IPlayerService _playerService;

    public PlayersController(IPlayerService playerService, ILogger<PlayersController> logger, IMapper mapper)
    {
        _playerService = playerService;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Adds a new player to a team (Admin or Coach only).
    /// </summary>
    /// <param name="newPlayer">The player data including person details.</param>
    /// <returns>Success if player added.</returns>
    /// <response code="200">Player added successfully.</response>
    [Authorize(Roles = "Admin, Coach")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddPlayerToTeam([FromBody] PlayerDetailDTO newPlayer)
    {
        try
        {
            _logger.LogInformation("Přidání nového hráče: {FirstName} {LastName}", newPlayer.Person.FirstName,
                newPlayer.Person.LastName);
            var player = _mapper.Map<Player>(newPlayer);
            await _playerService.AddPlayerAsync(player);

            _logger.LogInformation("Hráč {FirstName} {LastName} byl úspěšně přidán.", player.Person.FirstName,
                player.Person.LastName);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding player");
            return StatusCode(500, "An error occurred while adding the player.");
        }
    }

    /// <summary>
    /// Deletes a player (Admin or Coach only).
    /// </summary>
    /// <param name="id">The player ID to delete.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Player deleted successfully.</response>
    [Authorize(Roles = "Admin, Coach")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeletePlayer(int id)
    {
        try
        {
            await _playerService.DeletePlayerAsync(id);

            return Ok("Hráč byl úspěšně smazán.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting player with ID {PlayerId}", id);
            return StatusCode(500, "An error occurred while deleting the player.");
        }
    }

    /// <summary>
    /// Updates player information (Admin or Coach only).
    /// </summary>
    /// <param name="id">The player ID.</param>
    /// <param name="updatedPlayer">The updated player data.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Player updated successfully.</response>
    [Authorize(Roles = "Admin, Coach")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlayer(int id, [FromBody] PlayerDetailDTO updatedPlayer)
    {
        try
        {
            _logger.LogInformation("Aktualizace hráče s ID {Id}: {FirstName} {LastName}", id,
                updatedPlayer.Person.FirstName, updatedPlayer.Person.LastName);
            var player = _mapper.Map<Player>(updatedPlayer);
            await _playerService.UpdatePlayerAsync(id, player);
            _logger.LogInformation("Hráč s ID {Id} byl úspěšně aktualizován.", id);
            return Ok("Hráč byl úspěšně aktualizován.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating player with ID {PlayerId}", id);
            return StatusCode(500, "An error occurred while updating the player.");
        }
    }

    /// <summary>
    /// Updates a player's jersey number (Admin, Coach, or Recorder).
    /// </summary>
    /// <param name="id">The player ID.</param>
    /// <param name="newNumber">The new jersey number.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Number updated successfully.</response>
    /// <response code="404">If player not found.</response>
    [Authorize(Roles = "Admin, Coach, Recorder")]
    [HttpPatch("{id}/update-number")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePlayerNumber(int id, [FromBody] int newNumber)
    {
        try
        {
            _logger.LogInformation("Aktualizace čísla hráče s ID {Id} na {NewNumber}", id, newNumber);
            var player = await _playerService.GetPlayerByIdAsync(id);
            if (player == null)
            {
                _logger.LogWarning("Hráč s ID {Id} nebyl nalezen.", id);
                return NotFound("Hráč nebyl nalezen.");
            }

            player.Number = newNumber;
            await _playerService.UpdatePlayerAsync(id, player);
            _logger.LogInformation("Číslo hráče s ID {Id} bylo úspěšně aktualizováno na {NewNumber}.", id, newNumber);
            return Ok("Číslo hráče bylo úspěšně aktualizováno.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating player number for player {PlayerId}", id);
            return StatusCode(500, "An error occurred while updating the player number.");
        }
    }

    /// <summary>
    /// Gets all free players (not assigned to any team) for a category.
    /// </summary>
    /// <param name="categoryId">The category ID.</param>
    /// <returns>List of free players.</returns>
    /// <response code="200">Returns the list of free players.</response>
    [HttpGet("free")]
    [ProducesResponseType(typeof(List<PlayerDetailDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFreePlayers([FromQuery] int categoryId)
    {
        try
        {
            _logger.LogInformation("Získání volných hráčů pro kategorii s ID {CategoryId}", categoryId);
            var players = await _playerService.GetFreePlayersAsync(categoryId);
            var dto = _mapper.Map<List<PlayerDetailDTO>>(players);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching free players for category {CategoryId}", categoryId);
            return StatusCode(500, "An error occurred while fetching free players.");
        }
    }

    /// <summary>
    /// Removes a player from their current team (Coach or Admin only).
    /// </summary>
    /// <param name="id">The player ID.</param>
    /// <returns>Success if removed.</returns>
    /// <response code="200">Player removed from team successfully.</response>
    [Authorize(Roles = "Coach, Admin")]
    [HttpPost("{id}/remove-from-team")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveFromTeam(int id)
    {
        try
        {
            _logger.LogInformation("Odstranění hráče s ID {Id} z týmu", id);
            await _playerService.RemoveFromTeamAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing player {PlayerId} from team", id);
            return StatusCode(500, "An error occurred while removing the player from team.");
        }
    }

    /// <summary>
    /// Assigns a player to a team (Coach or Admin only).
    /// </summary>
    /// <param name="id">The player ID.</param>
    /// <param name="teamId">The team ID to assign to.</param>
    /// <returns>Success if assigned.</returns>
    /// <response code="200">Player assigned to team successfully.</response>
    [Authorize(Roles = "Coach, Admin")]
    [HttpPost("{id}/assign-to-team")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignToTeam(int id, [FromBody] int teamId)
    {
        try
        {
            _logger.LogInformation("Přiřazení hráče s ID {PlayerId} do týmu {TeamId}", id, teamId);
            await _playerService.AddPlayerToTeamAsync(id, teamId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning player {PlayerId} to team {TeamId}", id, teamId);
            return StatusCode(500, "An error occurred while assigning the player to team.");
        }
    }

    /// <summary>
    /// Applies match statistics to players (updates career stats) (Admin or Recorder only).
    /// </summary>
    /// <param name="matchId">The match ID.</param>
    /// <returns>Success if applied.</returns>
    /// <response code="200">Statistics applied successfully.</response>
    [Authorize(Roles = "Admin, Recorder")]
    [HttpPost("apply-match-stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ApplyMatchStats([FromBody] int matchId)
    {
        try
        {
            _logger.LogInformation("Aplikace statistik zápasu pro zápas s ID {MatchId}", matchId);
            await _playerService.ApplyMatchStatsAsync(matchId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying match stats for match {MatchId}", matchId);
            return StatusCode(500, "An error occurred while applying match statistics.");
        }
    }


    /// <summary>
    /// Reverts match statistics from players (removes from career stats) (Admin or Recorder only).
    /// </summary>
    /// <param name="matchId">The match ID.</param>
    /// <returns>Success if reverted.</returns>
    /// <response code="200">Statistics reverted successfully.</response>
    [Authorize(Roles = "Admin, Recorder")]
    [HttpPost("revert-match-stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RevertMatchStats([FromBody] int matchId)
    {
        try
        {
            _logger.LogInformation("Vrátit statistiky zápasu pro zápas s ID {MatchId}", matchId);
            await _playerService.RevertMatchStatsAsync(matchId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reverting match stats for match {MatchId}", matchId);
            return StatusCode(500, "An error occurred while reverting match statistics.");
        }
    }
}