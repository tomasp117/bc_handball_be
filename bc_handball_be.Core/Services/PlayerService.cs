using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Entities.Actors.super;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<PlayerService> _logger;
        private readonly IPersonService _personService;
        private readonly IEventService _eventService;

        public PlayerService(IPlayerRepository playerRepository, ILogger<PlayerService> logger, IPersonService personService, IEventService eventService)
        {
            _playerRepository = playerRepository;
            _logger = logger;
            _personService = personService;
            _eventService = eventService;
        }

        public async Task AddPlayerAsync(Player newPlayer)
        {
            if (newPlayer.Person.Id != 0)
            {
                // Person už existuje
                var existingPerson = await _personService.GetPersonByIdAsync(newPlayer.Person.Id);
                if (existingPerson == null)
                {
                    throw new Exception("Person s tímto ID neexistuje.");
                }

                newPlayer.Person = existingPerson;
            }
            else
            {
                // Person neexistuje → vytvoříme nového
                await _personService.AddPersonAsync(newPlayer.Person);
            }

            await _playerRepository.AddPlayerAsync(newPlayer);
        }


        public async Task DeletePlayerAsync(int id)
        {
            var player = await _playerRepository.GetPlayerByIdAsync(id);
            if (player == null) throw new Exception("Hráč nenalezen");

            await _playerRepository.DeletePlayerAsync(player);
        }

        public async Task<IEnumerable<Player>> GetAllPlayersAsync()
        {
            return await _playerRepository.GetAllPlayersAsync();
        }

        public async Task<Player?> GetPlayerByIdAsync(int id)
        {
            return await _playerRepository.GetPlayerByIdAsync(id);
        }

        public async Task UpdatePlayerAsync(int id, Player updatedPlayer)
        {
            var existingPlayer = await _playerRepository.GetPlayerByIdAsync(id);
            if (existingPlayer == null) throw new Exception("Hráč nenalezen");

            // Update jednotlivých polí:
            existingPlayer.Number = updatedPlayer.Number;
            existingPlayer.Person.FirstName = updatedPlayer.Person.FirstName;
            existingPlayer.Person.LastName = updatedPlayer.Person.LastName;

            await _playerRepository.UpdatePlayerAsync(existingPlayer);
        }

        public async Task<List<Player>> GetFreePlayersAsync(int categoryId)
        {
            var players = await _playerRepository.GetAllPlayersAsync();
            var freePlayers = players.Where(p => p.CategoryId == categoryId && p.TeamId == null).ToList();
            return freePlayers;
        }

        public async Task RemoveFromTeamAsync(int playerId)
        {
            var player = await _playerRepository.GetPlayerByIdAsync(playerId);
            if (player == null) throw new Exception("Hráč nenalezen");
            player.TeamId = null;
            await _playerRepository.UpdatePlayerAsync(player);
        }

        public async Task AddPlayerToTeamAsync(int playerId, int teamId)
        {
            var player = await _playerRepository.GetPlayerByIdAsync(playerId);
            if (player == null) throw new Exception("Hráč nenalezen");
            player.TeamId = teamId;
            await _playerRepository.UpdatePlayerAsync(player);
        }

        public async Task ApplyMatchStatsAsync(int matchId)
        {
            var events = await _eventService.GetEventsByMatchIdAsync(matchId);
            var groupedByPlayer = events.Where(e => e.AuthorId != null)
                .GroupBy(e => e.AuthorId);

            foreach (var group in groupedByPlayer)
            {
                var player = await _playerRepository.GetPlayerByIdAsync(group.Key.Value);
                foreach (var e in group)
                {
                    switch (e.Type)
                    {
                        case "G":
                            player.GoalCount++;
                            break;
                        case "7G":
                            player.SevenMeterGoalCount++;
                            break;
                        case "7N":
                            player.SevenMeterMissCount++;
                            break;
                        case "2":
                            player.TwoMinPenaltyCount++;
                            break;
                        case "Y":
                            player.YellowCardCount++;
                            break;
                        case "R":
                            player.RedCardCount++;
                            break;
                    }
                    await _playerRepository.UpdatePlayerAsync(player);
                }
            }
        }

        public async Task RevertMatchStatsAsync(int matchId)
        {
            var events = await _eventService.GetEventsByMatchIdAsync(matchId);
            var groupedByPlayer = events.Where(e => e.AuthorId != null)
                .GroupBy(e => e.AuthorId);
            foreach (var group in groupedByPlayer)
            {
                var player = await _playerRepository.GetPlayerByIdAsync(group.Key.Value);
                foreach (var e in group)
                {
                    switch (e.Type)
                    {
                        case "G":
                            player.GoalCount--;
                            break;
                        case "7G":
                            player.SevenMeterGoalCount--;
                            break;
                        case "7N":
                            player.SevenMeterMissCount--;
                            break;
                        case "2":
                            player.TwoMinPenaltyCount--;
                            break;
                        case "Y":
                            player.YellowCardCount--;
                            break;
                        case "R":
                            player.RedCardCount--;
                            break;
                    }
                    await _playerRepository.UpdatePlayerAsync(player);
                }
            }
        }
    }
}
