using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces;
using bc_handball_be.Core.Interfaces.IServices;
using bc_handball_be.Core.Services;
using bc_handball_be.Core.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bc_handball_be.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly IMatchService _matchService;
        private readonly IClubService _clubService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<TeamController> _logger;
        private readonly IMapper _mapper;

        public TeamController(ITeamService teamService, IMatchService matchService, ILogger<TeamController> logger, IMapper mapper, IClubService clubService, ICategoryService categoryService)
        {
            _teamService = teamService;
            _matchService = matchService;
            _clubService = clubService;
            _categoryService = categoryService;
            _logger = logger;
            _mapper = mapper;

        }

        [HttpGet("teams/group-assign")]
        public async Task<ActionResult<List<TeamGroupAssignDTO>>> GetTeamsByCategoryGroupAssign([FromQuery] int? category)
        {
            _logger.LogInformation("Fetching teams for categoryId: {CategoryId}", category);
            if (category == null)
            {
                _logger.LogWarning("Missing categoryId in request");
                return BadRequest("CategoryId is required.");
            }
            try
            {
                var teams = await _teamService.GetTeamsByCategoryAsync(category.Value);
                if (!teams.Any())
                {
                    _logger.LogWarning("No teams found for the given category: {CategoryId}", category);
                    return NotFound("No teams found for the given category");
                }

                var teamGroupAssignDTOs = _mapper.Map<List<TeamGroupAssignDTO>>(teams);
                return Ok(teamGroupAssignDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching teams for categoryId: {CategoryId}", category);
                return StatusCode(500, "An error occurred while fetching teams");
            }
        }

        [HttpGet("teams")]
        public async Task<ActionResult<List<TeamGroupAssignDTO>>> GetTeamsByCategory([FromQuery] int? category)
        {
            _logger.LogInformation("Fetching teams for categoryId: {CategoryId}", category);
            if (category == null)
            {
                _logger.LogWarning("Missing categoryId in request");
                return BadRequest("CategoryId is required.");
            }
            try
            {
                var teams = await _teamService.GetTeamsByCategoryAsync(category.Value);
                if (!teams.Any())
                {
                    _logger.LogWarning("No teams found for the given category: {CategoryId}", category);
                    return NotFound("No teams found for the given category");
                }

                var teamDTO = _mapper.Map<List<TeamDTO>>(teams);
                return Ok(teamDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching teams for categoryId: {CategoryId}", category);
                return StatusCode(500, "An error occurred while fetching teams");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("teams/assign-groups")]
        public async Task<ActionResult> AssignTeamsToGroups(
           [FromBody] List<TeamGroupAssignDTO> teamDtos,
           [FromQuery] int category)
        {
            _logger.LogInformation("Assigning teams to groups");

            if (!teamDtos.Any())
            {
                _logger.LogWarning("No teams provided for assignment");
                return BadRequest("Teams are required for assignment");
            }

            try
            {
                var teamIds = teamDtos.Select(t => t.Id).ToList();
                var teams = await _teamService.GetTeamsByIdAsync(teamIds);

                var enrichedTeams = teams.Select(team =>
                {
                    var dto = teamDtos.FirstOrDefault(t => t.Id == team.Id);
                    return new TeamWithAttributes(
                        team,
                        dto?.Strength ?? 1,
                        dto?.IsGirls ?? false
                    );
                }).ToList();

                var variants = await _teamService.AssignTeamsToGroupsAsync(enrichedTeams, category);

                var response = variants.Select(variant => new
                {
                    GroupCount = variant.GroupCount,
                    TotalMatches = variant.TotalMatches,
                    MinMatchesPerTeam = variant.MinMatchesPerTeam,
                    Groups = variant.Groups.Select(group => new
                    {
                        Id = group.Id,
                        Name = group.Name,
                        Teams = group.TeamGroups.Select(tg => tg.Team).Select(team => new TeamGroupAssignDTO
                        {
                            Id = team.Id,
                            Name = team.Name,
                            CategoryId = team.CategoryId,
                            Strength = enrichedTeams.FirstOrDefault(t => t.Team.Id == team.Id)?.Strength ?? 1,
                            IsGirls = enrichedTeams.FirstOrDefault(t => t.Team.Id == team.Id)?.IsGirls ?? false
                        }).ToList()
                    })
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning teams to groups");
                return StatusCode(500, "An error occurred while assigning teams to groups");
            }
        }

        [HttpGet("teams/{id}")]
        public async Task<ActionResult<TeamDetailDTO>> GetTeamById(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null)
                return NotFound();

            var dto = _mapper.Map<TeamDetailDTO>(team);
            return Ok(dto);
        }

        [HttpGet("teams/{id}/matches")]
        public async Task<ActionResult<List<MatchDTO>>> GetMatchesForTeam(int id)
        {
            var matches = await _matchService.GetMatchesByTeamIdAsync(id);
            var matchDtos = _mapper.Map<List<MatchDTO>>(matches);
            return Ok(matchDtos);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("teams")]
        public async Task<ActionResult<TeamDTO>> CreateTeam([FromBody] TeamCreateDTO teamDto)
        {
            if (teamDto == null)
            {
                _logger.LogWarning("Received null team DTO");
                return BadRequest("Team data is required.");
            }
            var team = _mapper.Map<Team>(teamDto);
            await _teamService.AddTeamAsync(team);
            var createdDto = _mapper.Map<TeamDTO>(team);
            return CreatedAtAction(nameof(GetTeamById), new { id = createdDto.Id }, createdDto);
        }

        [HttpGet("{instanceId}/teams")]
        public async Task<ActionResult<List<TeamDTO>>> GetTeamsByEdition(int instanceId)
        {
            _logger.LogInformation("Fetching teams for editionId: {InstanceId}", instanceId);
            var teams = await _teamService.GetTeamsByInstanceIdAsync(instanceId);
            if (teams == null || !teams.Any())
            {
                _logger.LogWarning("No teams found for editionId: {InstanceId}", instanceId);
                return NotFound();
            }
            var teamDtos = _mapper.Map<List<TeamDTO>>(teams);
            return Ok(teamDtos);
        }

        [HttpDelete("teams/{id}")]
        public async Task<ActionResult> DeleteTeam(int id)
        {
            _logger.LogInformation("Deleting team with id: {TeamId}", id);
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null)
            {
                _logger.LogWarning("Team with id {TeamId} not found", id);
                return NotFound();
            }
            await _teamService.DeleteTeamAsync(id);
            _logger.LogInformation("Team with id {TeamId} deleted successfully", id);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("teams/import-csv")]
        public async Task<ActionResult> ImportTeamsFromCsv([FromBody] List<TeamImportCsvDTO> teams)
        {
            _logger.LogInformation("Importing teams from CSV");
            if (teams == null || !teams.Any())
                return BadRequest("Teams are required for import");

            int imported = 0, skipped = 0, failed = 0;
            foreach (var dto in teams)
            {
                try
                {
                    // 1. Najdi klub (podle slugu)
                    var club = await _clubService.GetBySlugAsync(dto.ClubName);
                    if (club == null)
                    {
                        skipped++;
                        _logger.LogWarning("Club not found for name: {ClubName}", dto.ClubName);
                        continue;
                    }

                    // 2. Najdi kategorii podle názvu
                    var category = await _categoryService.GetByNameAsync(dto.CategoryName, dto.TournamentInstanceId);
                    if (category == null)
                    {
                        skipped++;
                        _logger.LogWarning("Category not found for name: {CategoryName} in instance {TournamentInstanceId}", dto.CategoryName, dto.TournamentInstanceId);
                        continue;
                    }

                    // 3. Kontrola duplicity
                    //bool exists = await _teamService.ExistsAsync(dto.Name, club.Id, dto.TournamentInstanceId);
                    //if (exists)
                    //{
                    //    skipped++;
                    //    _logger.LogWarning("Team already exists: {TeamName} for club {ClubName} in instance {TournamentInstanceId}", dto.Name, dto.ClubName, dto.TournamentInstanceId);
                    //    continue;
                    //}

                    // 4. Sestav entitu
                    var team = new Team
                    {
                        Name = dto.Name,
                        ClubId = club.Id,
                        CategoryId = category.Id,
                        TournamentInstanceId = dto.TournamentInstanceId
                    };

                    // 5. Ulož
                    await _teamService.AddTeamAsync(team);
                    imported++;
                }
                catch
                {
                    failed++;
                }
            }
            _logger.LogInformation("Import completed: {Imported} imported, {Skipped} skipped, {Failed} failed", imported, skipped, failed);
            return Ok(new { Imported = imported, Skipped = skipped, Failed = failed });
        }
    }
}
