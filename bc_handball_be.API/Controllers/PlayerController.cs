using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers
{
    [ApiController]
    [Route("api/players")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly ILogger<PlayersController> _logger;
        private readonly IMapper _mapper;

        public PlayersController(IPlayerService playerService, ILogger<PlayersController> logger, IMapper mapper)
        {
            _playerService = playerService;
            _logger = logger;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, Coach")]
        [HttpPost]
        public async Task<IActionResult> AddPlayerToTeam([FromBody] PlayerDetailDTO newPlayer)
        {
            _logger.LogInformation("Přidání nového hráče: {FirstName} {LastName}", newPlayer.Person.FirstName, newPlayer.Person.LastName);
            var player = _mapper.Map<Player>(newPlayer);
            await _playerService.AddPlayerAsync(player);

            _logger.LogInformation("Hráč {FirstName} {LastName} byl úspěšně přidán.", player.Person.FirstName, player.Person.LastName);

            return Ok();
        }

        [Authorize(Roles = "Admin, Coach")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            await _playerService.DeletePlayerAsync(id);

            return Ok("Hráč byl úspěšně smazán.");
        }

        [Authorize(Roles = "Admin, Coach")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlayer(int id, [FromBody] PlayerDetailDTO updatedPlayer)
        {
            _logger.LogInformation("Aktualizace hráče s ID {Id}: {FirstName} {LastName}", id, updatedPlayer.Person.FirstName, updatedPlayer.Person.LastName);
            var player = _mapper.Map<Player>(updatedPlayer);
            await _playerService.UpdatePlayerAsync(id, player);
            _logger.LogInformation("Hráč s ID {Id} byl úspěšně aktualizován.", id);
            return Ok("Hráč byl úspěšně aktualizován.");
        }

        [HttpGet("free")]
        public async Task<IActionResult> GetFreePlayers([FromQuery] int categoryId)
        {
            _logger.LogInformation("Získání volných hráčů pro kategorii s ID {CategoryId}", categoryId);
            var players = await _playerService.GetFreePlayersAsync(categoryId);
            var dto = _mapper.Map<List<PlayerDetailDTO>>(players);
            return Ok(dto);
        }

        [Authorize(Roles = "Coach, Admin")]
        [HttpPost("{id}/remove-from-team")]
        public async Task<IActionResult> RemoveFromTeam(int id)
        {
            _logger.LogInformation("Odstranění hráče s ID {Id} z týmu", id);
            await _playerService.RemoveFromTeamAsync(id);
            return Ok();
        }

        [Authorize(Roles = "Coach, Admin")]
        [HttpPost("{id}/assign-to-team")]
        public async Task<IActionResult> AssignToTeam(int id, [FromBody] int teamId)
        {
            _logger.LogInformation("Přiřazení hráče s ID {PlayerId} do týmu {TeamId}", id, teamId);
            await _playerService.AddPlayerToTeamAsync(id, teamId);
            return Ok();
        }

        [Authorize(Roles = "Admin, Recorder")]
        [HttpPost("apply-match-stats")]
        public async Task<IActionResult> ApplyMatchStats([FromBody] int matchId)
        {
            _logger.LogInformation("Aplikace statistik zápasu pro zápas s ID {MatchId}", matchId);
            await _playerService.ApplyMatchStatsAsync(matchId);
            return Ok();
        }


        [Authorize(Roles = "Admin, Recorder")]
        [HttpPost("revert-match-stats")]
        public async Task<IActionResult> RevertMatchStats([FromBody] int matchId)
        {
            _logger.LogInformation("Vrátit statistiky zápasu pro zápas s ID {MatchId}", matchId);
            await _playerService.RevertMatchStatsAsync(matchId);
            return Ok();
        }
    }
}
