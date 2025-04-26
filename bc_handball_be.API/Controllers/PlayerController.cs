using AutoMapper;
using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Interfaces.IServices;
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


        [HttpPost("")]
        public async Task<IActionResult> AddPlayer([FromBody] PlayerDetailDTO newPlayer)
        {
            _logger.LogInformation("Přidání nového hráče: {FirstName} {LastName}", newPlayer.Person.FirstName, newPlayer.Person.LastName);
            var player = _mapper.Map<Player>(newPlayer);
            await _playerService.AddPlayerAsync(player);

            _logger.LogInformation("Hráč {FirstName} {LastName} byl úspěšně přidán.", player.Person.FirstName, player.Person.LastName);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            await _playerService.DeletePlayerAsync(id);

            return Ok("Hráč byl úspěšně smazán.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlayer(int id, [FromBody] PlayerDetailDTO updatedPlayer)
        {
            _logger.LogInformation("Aktualizace hráče s ID {Id}: {FirstName} {LastName}", id, updatedPlayer.Person.FirstName, updatedPlayer.Person.LastName);
            var player = _mapper.Map<Player>(updatedPlayer);
            await _playerService.UpdatePlayerAsync(id, player);
            _logger.LogInformation("Hráč s ID {Id} byl úspěšně aktualizován.", id);
            return Ok("Hráč byl úspěšně aktualizován.");
        }
    }
}
