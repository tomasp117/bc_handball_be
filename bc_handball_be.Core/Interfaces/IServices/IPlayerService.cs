using bc_handball_be.Core.Entities.Actors.sub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IServices
{
    public interface IPlayerService
    {
        Task AddPlayerAsync(Player newPlayer);
        Task DeletePlayerAsync(int id);
        Task UpdatePlayerAsync(int id, Player updatedPlayer);
        Task<Player?> GetPlayerByIdAsync(int id);
        Task<IEnumerable<Player>> GetAllPlayersAsync();
        Task<List<Player>> GetFreePlayersAsync(int categoryId);
        Task RemoveFromTeamAsync(int playerId);
        Task AddPlayerToTeamAsync(int playerId, int teamId);
        Task ApplyMatchStatsAsync(int matchId);
        Task RevertMatchStatsAsync(int matchId);
    }
}
