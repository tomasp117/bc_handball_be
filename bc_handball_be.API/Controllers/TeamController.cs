using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces;
using bc_handball_be.Core.Interfaces.IServices;
using bc_handball_be.Core.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers
{
    [Route("api/teams")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly IMatchService _matchService;
        private readonly ILogger<TeamController> _logger;
        private readonly IMapper _mapper;

        public TeamController(ITeamService teamService,IMatchService matchService, ILogger<TeamController> logger, IMapper mapper)
        {
            _teamService = teamService;
            _matchService = matchService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("group-assign")]
        public async Task<ActionResult<IEnumerable<TeamGroupAssignDTO>>> GetTeamsByCategoryGroupAssign([FromQuery] int? category)
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

                var teamGroupAssignDTOs = _mapper.Map<IEnumerable<TeamGroupAssignDTO>>(teams);
                return Ok(teamGroupAssignDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching teams for categoryId: {CategoryId}", category);
                return StatusCode(500, "An error occurred while fetching teams");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamGroupAssignDTO>>> GetTeamsByCategory([FromQuery] int? category)
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

                var teamDTO = _mapper.Map<IEnumerable<TeamDTO>>(teams);
                return Ok(teamDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching teams for categoryId: {CategoryId}", category);
                return StatusCode(500, "An error occurred while fetching teams");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("assign-groups")]
        public async Task<ActionResult> AssignTeamsToGroups(
            [FromBody] IEnumerable<TeamGroupAssignDTO> teamDtos,
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
                var teamIds = teamDtos.Select(t => t.Id);
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

                // 🟢 Namapujeme odpověď pro frontend
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

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDetailDTO>> GetTeamById(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null)
                return NotFound();

            var dto = _mapper.Map<TeamDetailDTO>(team);
            return Ok(dto);
        }

        [HttpGet("{id}/matches")]
        public async Task<ActionResult<IEnumerable<MatchDTO>>> GetMatchesForTeam(int id)
        {
            var matches = await _matchService.GetMatchesByTeamIdAsync(id);
            var matchDtos = _mapper.Map<IEnumerable<MatchDTO>>(matches);
            return Ok(matchDtos);
        }
    }
}
