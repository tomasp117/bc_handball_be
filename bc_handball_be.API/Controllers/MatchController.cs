using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bc_handball_be.API.Controllers
{

    //[Route("api/matches")]
    [Route("api")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<MatchController> _logger;
        private readonly IMatchService _matchService;
        private readonly ICategoryService _categoryService;

        public MatchController(IMapper mapper, ILogger<MatchController> logger, IMatchService matchService, ICategoryService categoryService)
        {
            _mapper = mapper;
            _logger = logger;
            _matchService = matchService;
            _categoryService = categoryService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{edition}/matches/generate-blank")]
        public async Task<IActionResult> GenerateBlankMatches(int edition)
        {
            _logger.LogInformation("Generating blank matches for tournament.");
            var matches = await _matchService.GenBlankMatches(edition);
            return Ok(matches);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("matches/assign-group-matches/{categoryId}")]
        public async Task<IActionResult> AssignGroupMatches(int categoryId)
        {
            _logger.LogInformation("Assigning group matches for category {categoryId}", categoryId);

            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {categoryId} not found", categoryId);
                return NotFound($"Category with ID {categoryId} not found");
            }

            int edition = category.TournamentInstance.EditionNumber;

            var matches = await _matchService.AssignGroupMatchesFromScratch(categoryId, edition);
            var dtos = _mapper.Map<List<MatchDTO>>(matches);
            return Ok(dtos);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{edition}/matches/assign-all-group-matches")]
        public async Task<IActionResult> AssignAllGroupMatches(int edition)
        {
            _logger.LogInformation("Assigning all group matches");
            var matches = await _matchService.AssignAllGroupMatchesFromScratch(edition);
            var dtos = _mapper.Map<List<MatchDTO>>(matches);
            return Ok(dtos);
        }

        [HttpGet("matches")]
        public async Task<IActionResult> GetMatches()
        {
            _logger.LogInformation("Fetching matches");
            var matches = await _matchService.GetMatchesAsync();
            var dtos = _mapper.Map<List<MatchDTO>>(matches);
            return Ok(dtos);
        }

        [HttpGet("matches/unassigned-group-matches/{categoryId}")]
        public async Task<ActionResult<List<UnassignedMatchDTO>>> GetUnassignedGroupMatches(int categoryId)
        {
            var matches = await _matchService.GetUnassignedGroupMatches(categoryId);

            var dto = _mapper.Map<List<UnassignedMatchDTO>>(matches);
            return Ok(dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("matches/update-batch")]
        public async Task<IActionResult> UpdateBatch([FromBody] List<MatchAssignmentDTO> assignments)
        {
            var assignmentEntities = _mapper.Map<List<Match>>(assignments);
            await _matchService.UpdateMatchesAsync(assignmentEntities);
            return Ok();
        }

        [HttpGet("matches/{id}")]
        public async Task<IActionResult> GetMatchById(int id)
        {
            var match = await _matchService.GetMatchByIdAsync(id);
            if (match == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<MatchDTO>(match);
            return Ok(dto);
        }

        [Authorize(Roles = "Admin, Recorder")]
        [HttpPatch("matches/{id}")]
        public async Task<IActionResult> UpdateMatch(int id, [FromBody] MatchUpdateDTO dto)
        {
            _logger.LogInformation("Updating match with ID {id}", id);
            var match = await _matchService.GetMatchByIdAsync(id);
            if (match == null)
                return NotFound();

            _mapper.Map(dto, match);
            await _matchService.UpdateMatchAsync(match);
            return NoContent();
        }

        [HttpGet("matches/by-category")]
        public async Task<IActionResult> GetMatchesByCategory([FromQuery] int category)
        {
            var matches = await _matchService.GetMatchesByCategoryIdAsync(category);
            if (matches == null || !matches.Any())
                return NotFound();

            var matchDtos = _mapper.Map<List<MatchDTO>>(matches);
            return Ok(matchDtos);
        }
    }
}