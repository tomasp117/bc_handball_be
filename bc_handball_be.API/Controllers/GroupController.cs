using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IServices;
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

        [HttpPost("save")]
        public async Task<IActionResult> SaveGroupsToDatabase(
            [FromBody] List<GroupDetailDTO> newGroups,
            [FromQuery] int categoryId)
        {
            if (newGroups == null || !newGroups.Any())
            {
                _logger.LogWarning("No groups provided for category {CategoryId}", categoryId);
                return BadRequest("No groups provided.");
            }

            try
            {
                var groups = _mapper.Map<List<Group>>(newGroups);

                // Filtrujeme prázdné skupiny
                var validGroups = groups.Where(g => g.Teams.Any()).ToList();
                if (!validGroups.Any())
                {
                    _logger.LogWarning("All provided groups are empty. No data will be saved for category {CategoryId}", categoryId);
                    return BadRequest("All groups are empty. Cannot save.");
                }

                _logger.LogInformation("Saving {Count} new groups for category {CategoryId}", validGroups.Count, categoryId);
                await _groupService.SaveGroupsAsync(validGroups, categoryId);

                _logger.LogInformation("Groups for category {CategoryId} saved successfully", categoryId);
                return Ok("Groups saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving groups for category {CategoryId}", categoryId);
                return StatusCode(500, "An error occurred while saving groups.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupsForCategory([FromQuery] int categoryId)
        {
            try
            {
                var groups = await _groupService.GetGroupsByCategoryAsync(categoryId);
                if (groups == null || !groups.Any())
                {
                    _logger.LogWarning("No groups found for category {CategoryId}", categoryId);
                    return NotFound("No groups found.");
                }
                var groupDTOs = _mapper.Map<List<GroupDetailDTO>>(groups);
                return Ok(groupDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting groups for category {CategoryId}", categoryId);
                return StatusCode(500, "An error occurred while getting groups.");
            }
        }
    }
}
