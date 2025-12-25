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
    public class ClubRegistrationRepository : IClubRegistrationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClubRegistrationRepository> _logger;

        public ClubRegistrationRepository(ApplicationDbContext context, ILogger<ClubRegistrationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ClubRegistration> AddAsync(ClubRegistration clubRegistration)
        {
            _logger.LogInformation("Adding new club registration for ClubId: {ClubId}", clubRegistration.ClubId);
            await _context.ClubRegistrations.AddAsync(clubRegistration);
            await _context.SaveChangesAsync();
            return clubRegistration;
        }

        public async Task<ClubRegistration> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching club registration by ID: {Id}", id);
            var registration = await _context.ClubRegistrations
                .Include(cr => cr.Club)
                .Include(cr => cr.TournamentInstance)
                .Include(cr => cr.CategoryTeamCounts)
                    .ThenInclude(ctc => ctc.Category)
                .FirstOrDefaultAsync(cr => cr.Id == id);
            return registration;
        }

        public async Task<List<ClubRegistration>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all club registrations");
            return await _context.ClubRegistrations
                .Include(cr => cr.Club)
                .Include(cr => cr.TournamentInstance)
                .Include(cr => cr.CategoryTeamCounts)
                    .ThenInclude(ctc => ctc.Category)
                .ToListAsync();
        }

        public async Task<ClubRegistration> UpdateAsync(ClubRegistration clubRegistration)
        {
            _logger.LogInformation("Updating club registration ID: {Id}", clubRegistration.Id);
            _context.ClubRegistrations.Update(clubRegistration);
            await _context.SaveChangesAsync();
            return clubRegistration;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting club registration ID: {Id}", id);
            var registration = await _context.ClubRegistrations.FindAsync(id);
            if (registration == null)
            {
                _logger.LogWarning("Club registration with ID {Id} not found", id);
                return false;
            }
            _context.ClubRegistrations.Remove(registration);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ClubRegistration>> GetByTournamentInstanceIdAsync(int tournamentInstanceId)
        {
            _logger.LogInformation("Fetching club registrations for tournament instance ID: {TournamentInstanceId}", tournamentInstanceId);
            return await _context.ClubRegistrations
                .Include(cr => cr.Club)
                .Include(cr => cr.TournamentInstance)
                .Include(cr => cr.CategoryTeamCounts)
                    .ThenInclude(ctc => ctc.Category)
                .Where(cr => cr.TournamentInstanceId == tournamentInstanceId)
                .ToListAsync();
        }

        public async Task<ClubRegistration?> GetByClubIdAsync(int clubId)
        {
            _logger.LogInformation("Fetching club registration for club ID: {ClubId}", clubId);
            return await _context.ClubRegistrations
                .Include(cr => cr.Club)
                .Include(cr => cr.TournamentInstance)
                .Include(cr => cr.CategoryTeamCounts)
                    .ThenInclude(ctc => ctc.Category)
                .FirstOrDefaultAsync(cr => cr.ClubId == clubId);
        }

        public async Task<List<ClubRegistration>> GetByStatusAsync(RegistrationStatus status)
        {
            _logger.LogInformation("Fetching club registrations with status: {Status}", status);
            return await _context.ClubRegistrations
                .Include(cr => cr.Club)
                .Include(cr => cr.TournamentInstance)
                .Include(cr => cr.CategoryTeamCounts)
                    .ThenInclude(ctc => ctc.Category)
                .Where(cr => cr.Status == status)
                .ToListAsync();
        }
    }
}
