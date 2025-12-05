using AutoMapper;
using bc_handball_be.API.DTOs.Groups;
using bc_handball_be.API.DTOs.Groups.Brackets;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IServices;
using bc_handball_be.Core.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers;

/// <summary>
/// Handles group management operations including group stage groups, playoff brackets, standings, and placeholders.
/// </summary>
[Route("api/groups")]
[ApiController]
public class GroupController : ControllerBase
{
    private readonly IGroupService _groupService;
    private readonly ILogger<GroupController> _logger;
    private readonly IMapper _mapper;

    public GroupController(IGroupService groupService, ILogger<GroupController> logger, IMapper mapper)
    {
        _groupService = groupService;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Saves group stage groups to the database (Admin only).
    /// </summary>
    /// <param name="newGroups">List of groups with team assignments.</param>
    /// <param name="category">The category ID.</param>
    /// <returns>The saved groups.</returns>
    /// <response code="200">Groups saved successfully.</response>
    /// <response code="400">If no groups provided or all groups empty.</response>
    /// <response code="500">If an error occurred during save.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("save")]
    [ProducesResponseType(typeof(List<GroupDetailDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveGroupsToDatabase(
        [FromBody] List<GroupDetailDTO> newGroups,
        [FromQuery] int category)
    {
        if (newGroups == null || !newGroups.Any())
        {
            _logger.LogWarning("No groups provided for category {CategoryId}", category);
            return BadRequest("No groups provided.");
        }

        try
        {
            //var groups = _mapper.Map<List<Group>>(newGroups);

            var groups = new List<Group>();

            foreach (var dto in newGroups)
            {
                var group = _mapper.Map<Group>(dto);
                group.TeamGroups = dto.Teams.Select(t => new TeamGroup
                {
                    TeamId = t.Id,
                    Group = group
                }).ToList();

                groups.Add(group);
            }

            // Filtrujeme prázdné skupiny
            var validGroups = groups.Where(g => g.TeamGroups.Select(tg => tg.Team).Any()).ToList();
            if (!validGroups.Any())
            {
                _logger.LogWarning("All provided groups are empty. No data will be saved for category {CategoryId}",
                    category);
                return BadRequest("All groups are empty. Cannot save.");
            }

            _logger.LogInformation("Saving {Count} new groups for category {CategoryId}", validGroups.Count, category);
            await _groupService.SaveGroupsAsync(validGroups, category);

            var groupDTOs = _mapper.Map<List<GroupDetailDTO>>(validGroups);

            _logger.LogInformation("Groups for category {CategoryId} saved successfully", category);
            return Ok(groupDTOs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving groups for category {CategoryId}", category);
            return StatusCode(500, "An error occurred while saving groups.");
        }
    }

    /// <summary>
    /// Gets all groups for a specific category.
    /// </summary>
    /// <param name="category">The category ID.</param>
    /// <returns>List of groups.</returns>
    /// <response code="200">Returns the list of groups.</response>
    /// <response code="404">If no groups found.</response>
    /// <response code="500">If an error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<GroupDetailDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGroupsForCategory([FromQuery] int category)
    {
        try
        {
            var groups = await _groupService.GetGroupsByCategoryAsync(category);
            if (groups == null || !groups.Any())
            {
                _logger.LogWarning("No groups found for category {CategoryId}", category);
                return NotFound("No groups found.");
            }

            var groupDTOs = _mapper.Map<List<GroupDetailDTO>>(groups);
            return Ok(groupDTOs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting groups for category {CategoryId}", category);
            return StatusCode(500, "An error occurred while getting groups.");
        }
    }

    /// <summary>
    /// Gets the standings (team rankings) for a specific group.
    /// </summary>
    /// <param name="groupId">The group ID.</param>
    /// <returns>List of team standings.</returns>
    /// <response code="200">Returns the group standings.</response>
    /// <response code="500">If an error occurred.</response>
    [HttpGet("{groupId}/standings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGroupStandings(int groupId)
    {
        try
        {
            var standings = await _groupService.GetGroupStandingsAsync(groupId);

            return Ok(standings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting standings for group {GroupId}", groupId);
            return StatusCode(500, "An error occurred while getting standings.");
        }
    }

    /// <summary>
    /// Saves playoff bracket groups to the database (Admin only).
    /// </summary>
    /// <param name="groupDTOs">List of bracket groups.</param>
    /// <param name="category">The category ID.</param>
    /// <returns>Success if saved.</returns>
    /// <response code="200">Bracket groups saved successfully.</response>
    /// <response code="400">If no groups provided or all groups empty.</response>
    /// <response code="500">If an error occurred during save.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("bracket")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveBracketGroups(
        [FromBody] List<BracketGroupDTO> groupDTOs,
        [FromQuery] int category)
    {
        if (groupDTOs == null || !groupDTOs.Any())
        {
            _logger.LogWarning("No bracket groups provided for category {CategoryId}", category);
            return BadRequest("No bracket groups provided.");
        }

        try
        {
            var groups = _mapper.Map<List<Group>>(groupDTOs);

            // doplnění categoryId (není ve DTO, ale musí být v entitě)
            foreach (var group in groups) group.CategoryId = category;

            var validGroups = groups.Where(g => g.TeamGroups.Any()).ToList();
            if (!validGroups.Any())
            {
                _logger.LogWarning("All bracket groups are empty. No data saved for category {CategoryId}", category);
                return BadRequest("All bracket groups are empty.");
            }

            _logger.LogInformation("Saving {Count} bracket groups for category {CategoryId}", validGroups.Count,
                category);
            await _groupService.SaveGroupsBracketAsync(validGroups, category);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving bracket groups for category {CategoryId}", category);
            return StatusCode(500, "An error occurred while saving bracket groups.");
        }
    }

    /// <summary>
    /// Gets all playoff bracket groups for a specific category.
    /// </summary>
    /// <param name="category">The category ID.</param>
    /// <returns>List of playoff bracket groups.</returns>
    /// <response code="200">Returns the bracket groups.</response>
    /// <response code="404">If no bracket groups found.</response>
    /// <response code="500">If an error occurred.</response>
    [HttpGet("bracket")]
    [ProducesResponseType(typeof(List<BracketGroupDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBracketsByCategory([FromQuery] int category)
    {
        _logger.LogInformation("Fetching bracket groups for category {CategoryId}", category);

        try
        {
            var groups = await _groupService.GetGroupsByCategoryAsync(category);
            if (groups == null || !groups.Any())
            {
                _logger.LogWarning("No bracket groups found for category {CategoryId}", category);
                return NotFound("No bracket groups found.");
            }

            var playoffGroups = groups.Where(g => !string.IsNullOrEmpty(g.Phase)).ToList();

            if (!playoffGroups.Any())
            {
                _logger.LogWarning("No playoff groups found for category {CategoryId}", category);
                return NotFound("No playoff groups found.");
            }

            var groupDTOs = _mapper.Map<List<BracketGroupDTO>>(playoffGroups);
            return Ok(groupDTOs);
        }
        catch
        {
            _logger.LogError("Error fetching bracket groups for category {CategoryId}", category);
            return StatusCode(500, "An error occurred while fetching bracket groups.");
        }
    }

    /// <summary>
    /// Saves placeholder bracket groups for playoff structure (Admin only).
    /// </summary>
    /// <param name="groupDTOs">List of placeholder groups.</param>
    /// <param name="category">The category ID.</param>
    /// <returns>Success if saved.</returns>
    /// <response code="200">Placeholder groups saved successfully.</response>
    /// <response code="400">If no groups provided.</response>
    /// <response code="500">If an error occurred during save.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("bracket/placeholder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveBracketPlaceholders(
        [FromBody] List<PlaceholderGroupDTO> groupDTOs,
        [FromQuery] int category)
    {
        if (groupDTOs == null || !groupDTOs.Any())
        {
            _logger.LogWarning("No placeholder groups provided for category {CategoryId}", category);
            return BadRequest("No placeholder groups provided.");
        }

        try
        {
            _logger.LogInformation("Saving {Count} placeholder bracket groups for category {CategoryId}",
                groupDTOs.Count, category);

            var groups = _mapper.Map<List<PlaceholderGroup>>(groupDTOs);

            await _groupService.SavePlaceholderGroupsAsync(groups, category);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving placeholder bracket groups for category {CategoryId}", category);
            return StatusCode(500, "An error occurred while saving placeholder groups.");
        }
    }

    /// <summary>
    /// Gets groups with placeholder teams for playoff bracket setup.
    /// </summary>
    /// <param name="categoryId">The category ID.</param>
    /// <returns>List of groups with placeholders.</returns>
    /// <response code="200">Returns groups with placeholders.</response>
    /// <response code="404">If no groups found.</response>
    [HttpGet("with-placeholders")]
    [ProducesResponseType(typeof(List<GroupDetailDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGroupsWithPlaceholders([FromQuery] int categoryId)
    {
        try
        {
            var groups = await _groupService.GetGroupsWithPlaceholderTeamsAsync(categoryId);
            if (groups == null || !groups.Any()) return NotFound("Žádné skupiny s placeholdery nebyly nalezeny.");

            var result = _mapper.Map<List<GroupDetailDTO>>(groups);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching groups with placeholders for category {CategoryId}", categoryId);
            return StatusCode(500, "An error occurred while fetching groups with placeholders.");
        }
    }

    /// <summary>
    /// Gets the final standings (tournament rankings) for a category.
    /// </summary>
    /// <param name="categoryId">The category ID.</param>
    /// <returns>Final positions of all teams.</returns>
    /// <response code="200">Returns the final standings.</response>
    /// <response code="404">If no standings found.</response>
    [HttpGet("final-standings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromQuery] int categoryId)
    {
        try
        {
            var positions = await _groupService.GetFinalPositionsAsync(categoryId);
            if (positions == null || positions.Count == 0)
                return NotFound($"Žádné konečné pořadí pro kategorii {categoryId}.");

            return Ok(positions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching final standings for category {CategoryId}", categoryId);
            return StatusCode(500, "An error occurred while fetching final standings.");
        }
    }
}