using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.API.DTOs.GroupBrackets;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IServices;
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

    }
}
