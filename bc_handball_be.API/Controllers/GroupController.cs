using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.API.DTOs.GroupBrackets;
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
            [FromQuery] int category)
        {
            if (newGroups == null || !newGroups.Any())
            {
                _logger.LogWarning("No groups provided for category {CategoryId}", category);
                return BadRequest("No groups provided.");
            }

            try
            {
                var groups = _mapper.Map<List<Group>>(newGroups);

                // Filtrujeme prázdné skupiny
                var validGroups = groups.Where(g => g.Teams.Any()).ToList();
                if (!validGroups.Any())
                {
                    _logger.LogWarning("All provided groups are empty. No data will be saved for category {CategoryId}", category);
                    return BadRequest("All groups are empty. Cannot save.");
                }

                _logger.LogInformation("Saving {Count} new groups for category {CategoryId}", validGroups.Count, category);
                await _groupService.SaveGroupsAsync(validGroups, category);

                _logger.LogInformation("Groups for category {CategoryId} saved successfully", category);
                return Ok("Groups saved successfully.");
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

        //[HttpPost("save-bracket")]
        //public async Task<IActionResult> SaveBracket([FromBody] List<BracketRowDTO> bracketRows, [FromQuery] int categoryId)
        //{
        //    try
        //    {
        //        var groupsToSave = new List<Group>();

        //        foreach (var row in bracketRows)
        //        {
        //            foreach (var groupDto in row.Groups)
        //            {
        //                var group = _mapper.Map<Group>(groupDto);
        //                group.CategoryId = categoryId;

        //                groupsToSave.Add(group);
        //            }
        //        }

        //        await _groupService.SaveGroupsAsync(groupsToSave);

        //        return Ok(new { success = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Chyba při ukládání bracketu");
        //        return StatusCode(500, "Nastala chyba při ukládání bracketu.");
        //    }
        //}

    }
}
