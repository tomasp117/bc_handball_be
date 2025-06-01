using bc_handball_be.Core.Entities;
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
    public class ClubAdminRepository : IClubAdminRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClubAdminRepository> _logger;
        public ClubAdminRepository(ApplicationDbContext context, ILogger<ClubAdminRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public async Task<ClubAdmin> GetByPersonIdAsync(int personId)
        {
            try
            {
                var clubAdmin = await _context.ClubAdmins
                    .Include(ca => ca.Club)
                        .ThenInclude(club => club.Teams)
                            .ThenInclude(team => team.Players)
                                .ThenInclude(player => player.Person)
                    .Include(ca => ca.Club)
                        .ThenInclude(club => club.Teams)
                            .ThenInclude(team => team.Coach)
                                .ThenInclude(coach => coach.Person)
                    .Include(ca => ca.Club)
                        .ThenInclude(club => club.Teams)
                            .ThenInclude(team => team.Category)
                    .Include(ca => ca.Club)
                        .ThenInclude(club => club.Teams)
                            .ThenInclude(team => team.TournamentInstance)
                    .Include(ca => ca.Person)
                    .FirstOrDefaultAsync(ca => ca.PersonId == personId);

                if (clubAdmin == null)
                {
                    _logger.LogWarning("ClubAdmin with PersonId {PersonId} not found", personId);
                    return null;
                }

                return clubAdmin;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving coach with PersonId {PersonId}", personId);
                return null;
            }
        }

        public async Task<ClubAdmin> GetByClubIdAsync(int clubId)
        {
            try
            {
                var clubAdmin = await _context.ClubAdmins
                    .Include(ca => ca.Person)
                    .Include(ca => ca.Club)
                    .FirstOrDefaultAsync(ca => ca.ClubId == clubId);
                if (clubAdmin == null)
                {
                    _logger.LogWarning("ClubAdmin with ClubId {ClubId} not found", clubId);
                    return null;
                }
                return clubAdmin;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ClubAdmin with ClubId {ClubId}", clubId);
                return null;
            }
        }
    }
}
