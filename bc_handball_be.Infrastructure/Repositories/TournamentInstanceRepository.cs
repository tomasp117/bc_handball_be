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
    public class TournamentInstanceRepository : ITournamentInstanceRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TournamentInstanceRepository> _logger;

        public TournamentInstanceRepository(ApplicationDbContext context, ILogger<TournamentInstanceRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(TournamentInstance tournamentInstance)
        {
            await _context.TournamentInstances.AddAsync(tournamentInstance);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TournamentInstance>> GetAllAsync()
        {
            return await _context.TournamentInstances
                .Include(ti => ti.Tournament)
                .Include(ti => ti.Categories)
                .ToListAsync();
        }

        public async Task<TournamentInstance> GetByIdAsync(int id)
        {
            return await _context.TournamentInstances
                .Include(ti => ti.Tournament)
                .FirstOrDefaultAsync(ti => ti.Id == id);
        }

        public async Task UpdateAsync(TournamentInstance tournamentInstance)
        {
            _logger.LogInformation($"Updating tournament instance with ID {tournamentInstance.Id}");
            _context.TournamentInstances.Update(tournamentInstance);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var tournamentInstance = await GetByIdAsync(id);
            if (tournamentInstance != null)
            {
                _context.TournamentInstances.Remove(tournamentInstance);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning($"Tournament instance with ID {id} not found.");
            }
        }
    }
}
