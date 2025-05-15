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
    }
}
