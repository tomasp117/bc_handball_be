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
    public class CoachRepository : ICoachRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CoachRepository> _logger;

        public CoachRepository(ApplicationDbContext context, ILogger<CoachRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Coach?> GetByPersonIdAsync(int personId)
        {
            try
            {
                var coach = await _context.Coaches
                    .Include(c => c.Team)
                        .ThenInclude(t => t.Players)
                            .ThenInclude(p => p.Person)
                    .Include(c => c.Team)
                        .ThenInclude(t => t.Category)
                            .ThenInclude(c => c.TournamentInstance)
                    .Include(c => c.Team)
                        .ThenInclude(t => t.Club)
                    .FirstOrDefaultAsync(c => c.PersonId == personId);

                return coach;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving coach with PersonId {PersonId}", personId);
                return null;
            }
        }

        public async Task AddAsync(Coach coach)
        {
            if (coach == null) throw new ArgumentNullException(nameof(coach));
            try
            {
                await _context.Coaches.AddAsync(coach);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Coach with ID {CoachId} added successfully.", coach.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding coach with ID {CoachId}.", coach.Id);
                throw;
            }
        }
    }
}
