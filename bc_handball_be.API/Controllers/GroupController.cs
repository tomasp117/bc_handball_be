using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.API.DTOs.GroupBrackets;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IServices;
using bc_handball_be.Core.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers
{
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

        [Authorize(Roles = "Admin")]
        [HttpPost("save")]
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
                    _logger.LogWarning("All provided groups are empty. No data will be saved for category {CategoryId}", category);
                    return BadRequest("All groups are empty. Cannot save.");
                }

                _logger.LogInformation("Saving {Count} new groups for category {CategoryId}", validGroups.Count, category);
                await _groupService.SaveGroupsAsync(validGroups, category);

                _logger.LogInformation("Groups for category {CategoryId} saved successfully", category);
                return Ok(groups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving groups for category {CategoryId}", category);
                return StatusCode(500, "An error occurred while saving groups.");
            }
        }

        [HttpGet]
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

        [HttpGet("{groupId}/standings")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost("bracket")]
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
                foreach (var group in groups)
                {
                    group.CategoryId = category;
                }

                var validGroups = groups.Where(g => g.TeamGroups.Any()).ToList();
                if (!validGroups.Any())
                {
                    _logger.LogWarning("All bracket groups are empty. No data saved for category {CategoryId}", category);
                    return BadRequest("All bracket groups are empty.");
                }

                _logger.LogInformation("Saving {Count} bracket groups for category {CategoryId}", validGroups.Count, category);
                await _groupService.SaveGroupsAsync(validGroups, category);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving bracket groups for category {CategoryId}", category);
                return StatusCode(500, "An error occurred while saving bracket groups.");
            }
        }

        [HttpGet("bracket")]
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
            catch { 
                _logger.LogError("Error fetching bracket groups for category {CategoryId}", category);
                return StatusCode(500, "An error occurred while fetching bracket groups.");
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpPost("bracket/placeholder")]
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
                _logger.LogInformation("Saving {Count} placeholder bracket groups for category {CategoryId}", groupDTOs.Count, category);
                
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

        [HttpGet("with-placeholders")]
        public async Task<IActionResult> GetGroupsWithPlaceholders([FromQuery] int categoryId)
        {
            var groups = await _groupService.GetGroupsWithPlaceholderTeamsAsync(categoryId);
            if (groups == null || !groups.Any())
            {
                return NotFound("Žádné skupiny s placeholdery nebyly nalezeny.");
            }

            var result = _mapper.Map<List<GroupDetailDTO>>(groups);
            return Ok(result);
        }

    }
}
