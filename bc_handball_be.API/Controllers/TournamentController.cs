using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers
{
    [Route("api/tournaments")]
    public class TournamentController : ControllerBase
    {
        private readonly ILogger<TournamentController> _logger;
        private readonly IMapper _mapper;
        private readonly ITournamentService _tournamentService;

        public TournamentController(ILogger<TournamentController> logger, IMapper mapper, ITournamentService tournamentService)
        {
            _logger = logger;
            _mapper = mapper;
            _tournamentService = tournamentService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateTournament([FromBody] TournamentDTO dto)
        {
            var tournament = _mapper.Map<Tournament>(dto);
            var created = await _tournamentService.CreateTournamentAsync(tournament);
            return Ok(created);
        }

        [HttpGet]
        public async Task<IActionResult> GetTournaments()
        {
            var tournaments = await _tournamentService.GetAllTournamentsAsync();
            var tournamentDTOs = _mapper.Map<IEnumerable<TournamentDTO>>(tournaments);
            return Ok(tournamentDTOs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTournamentById(int id)
        {
            var tournament = await _tournamentService.GetTournamentByIdAsync(id);
            if (tournament == null)
            {
                return NotFound();
            }
            var tournamentDTO = _mapper.Map<TournamentDTO>(tournament);
            return Ok(tournamentDTO);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTournament(int id, [FromBody] TournamentDTO dto)
        {
            var tournament = _mapper.Map<Tournament>(dto);
            var updated = await _tournamentService.UpdateTournamentAsync(tournament);
            if (updated == null)
            {
                return NotFound();
            }
            return Ok(updated);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournament(int id)
        {
            var tournament = await _tournamentService.GetTournamentByIdAsync(id);
            if (tournament == null)
            {
                return NotFound();
            }
            await _tournamentService.DeleteTournamentAsync(id);
            return NoContent();
        }
    }
}
