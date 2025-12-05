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

        // ==================== READ OPERATIONS ====================

        public async Task<Coach?> GetByIdAsync(int coachId)
        {
            _logger.LogInformation("Fetching coach with ID {CoachId}", coachId);
            return await _context.Coaches
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.Id == coachId);
        }

        public async Task<Coach?> GetByPersonIdAsync(int personId)
        {
            _logger.LogInformation("Fetching coach with PersonId {PersonId}", personId);
            return await _context.Coaches
                .Include(c => c.Team)
                    .ThenInclude(t => t.Players)
                        .ThenInclude(p => p.Person)
                .Include(c => c.Team)
                    .ThenInclude(t => t.Category)
                        .ThenInclude(c => c.TournamentInstance)
                .Include(c => c.Team)
                    .ThenInclude(t => t.Club)
                .FirstOrDefaultAsync(c => c.PersonId == personId);
        }

        // ==================== WRITE OPERATIONS ====================

        public async Task AddAsync(Coach coach)
        {
            if (coach == null) throw new ArgumentNullException(nameof(coach));

            _logger.LogInformation("Adding coach to database");
            await _context.Coaches.AddAsync(coach);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Coach with ID {CoachId} added successfully", coach.Id);
        }

        // ==================== DELETE OPERATIONS ====================

        public async Task DeleteAsync(int coachId)
        {
            _logger.LogInformation("Deleting coach with ID {CoachId}", coachId);
            var coach = await _context.Coaches.FindAsync(coachId);
            if (coach == null)
            {
                _logger.LogWarning("Attempted to delete non-existing coach with ID {CoachId}", coachId);
                return;
            }
            _context.Coaches.Remove(coach);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Coach with ID {CoachId} deleted successfully", coachId);
        }
    }
}
