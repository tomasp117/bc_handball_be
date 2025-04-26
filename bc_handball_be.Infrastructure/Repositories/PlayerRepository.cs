using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Infrastructure.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PlayerRepository> _logger;

        public PlayerRepository(ApplicationDbContext context, ILogger<PlayerRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddPlayerAsync(Player player)
        {
            _logger.LogInformation("Adding player: {Player}", player);
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePlayerAsync(Player player)
        {
            _logger.LogInformation("Updating player: {Player}", player);
            _context.Players.Update(player);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePlayerAsync(Player player)
        {
            _logger.LogInformation("Deleting player: {Player}", player);
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
        }

        public async Task<Player?> GetPlayerByIdAsync(int id)
        {
            _logger.LogInformation("Getting player by ID: {Id}", id);
            return await _context.Players
                .Include(p => p.Person)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Player>> GetAllPlayersAsync()
        {
            _logger.LogInformation("Getting all players");
            return await _context.Players
                .Include(p => p.Person)
                .ToListAsync();
        }
    }
}
