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

        public async Task DeleteAsync(int coachId)
        {
            try
            {
                var coach = await _context.Coaches.FindAsync(coachId);
                if (coach == null)
                {
                    _logger.LogWarning("Attempted to delete non-existing coach with ID {CoachId}", coachId);
                    return;
                }
                _context.Coaches.Remove(coach);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Coach with ID {CoachId} deleted successfully.", coachId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting coach with ID {CoachId}.", coachId);
                throw;
            }
        }

        public async Task DeleteCoachWithPersonAsync(int coachId)
        {
            var coach = await _context.Coaches
                .Include(c => c.Person)
                .ThenInclude(p => p.Login)
                .FirstOrDefaultAsync(c => c.Id == coachId);

            if (coach == null)
                throw new KeyNotFoundException($"Coach with ID {coachId} not found.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (coach.Person.Login != null)
                    _context.Logins.Remove(coach.Person.Login);

                _context.Persons.Remove(coach.Person);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<Coach> GetByIdAsync(int coachId)
        {
            try
            {
                var coach = await _context.Coaches
                    .Include(c => c.Person)
                    .FirstOrDefaultAsync(c => c.Id == coachId);
                if (coach == null)
                {
                    _logger.LogWarning("Coach with ID {CoachId} not found.", coachId);
                    throw new KeyNotFoundException($"Coach with ID {coachId} not found.");
                }
                return coach;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving coach with ID {CoachId}.", coachId);
                throw;
            }
        }
    }
}
