using bc_handball_be.Core.Entities;
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
    public class TournamentRepository : ITournamentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TournamentRepository> _logger;

        public TournamentRepository(ApplicationDbContext context, ILogger<TournamentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(Tournament tournament)
        {
            await _context.Tournaments.AddAsync(tournament);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Tournament>> GetAllAsync()
        {
            return await _context.Tournaments.ToListAsync();
        }

        public async Task<Tournament> GetByIdAsync(int id)
        {
            return await _context.Tournaments.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task UpdateAsync(Tournament tournament)
        {
            _logger.LogInformation($"Updating tournament with ID {tournament.Id}");
            _context.Tournaments.Update(tournament);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var tournament = await GetByIdAsync(id);
            if (tournament != null)
            {
                _context.Tournaments.Remove(tournament);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning($"Tournament with ID {id} not found.");
            }
        }
    }
}
