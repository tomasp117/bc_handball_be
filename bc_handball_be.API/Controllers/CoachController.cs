using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bc_handball_be.API.Controllers
{
    [Route("api/coaches")]
    [ApiController]
    public class CoachController : ControllerBase
    {
        readonly ICoachService _coachService;
        private readonly ILogger<CoachController> _logger;
        private readonly IMapper _mapper;

        public CoachController(ICoachService coachService, IMapper mapper, ILogger<CoachController> logger)
        {
            _coachService = coachService;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize(Roles = "Coach")]
        [HttpGet("my-team")]
        public async Task<IActionResult> GetMyTeam()
        {
            _logger.LogInformation("Getting team for coach with ID {CoachId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var personId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var team = await _coachService.GetTeamForPersonIdAsync(personId);
            if (team == null) return NotFound("Tým nenalezen");

            var teamDto = _mapper.Map<TeamDetailDTO>(team);

            return Ok(teamDto);
        }

        [HttpPost]
        [Authorize(Roles = "ClubAdmin, Admin")]
        public async Task<IActionResult> CreateCoach([FromBody] CreateCoachDTO dto)
        {
            try
            {
                var coach = _mapper.Map<Coach>(dto);

                coach.Person.Login.SetPassword(dto.Password);

                await _coachService.CreateCoachAsync(coach);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Username conflict when creating coach.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při vytváření trenéra.");
                return StatusCode(500, "Něco se pokazilo při ukládání trenéra.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoach(int id)
        {
            try
            {

                await _coachService.DeleteCoachAsync(id);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Trenér nenalezen.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při mazání trenéra.");
                return StatusCode(500, "Něco se pokazilo při mazání trenéra.");
            }
        }
    }
}
