using bc_handball_be.Core.Entities.Actors.sub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IRepositories
{
    public interface IPlayerRepository
    {
        Task AddPlayerAsync(Player player);
        Task UpdatePlayerAsync(Player player);
        Task DeletePlayerAsync(Player player);
        Task<Player?> GetPlayerByIdAsync(int id);
        Task<IEnumerable<Player>> GetAllPlayersAsync();
    }
}
