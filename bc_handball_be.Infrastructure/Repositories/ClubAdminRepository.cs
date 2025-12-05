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

        // ==================== READ OPERATIONS ====================

        public async Task<ClubAdmin?> GetByPersonIdAsync(int personId)
        {
            _logger.LogInformation("Fetching ClubAdmin with PersonId {PersonId}", personId);
            return await _context.ClubAdmins
                .Include(ca => ca.Club)
                .Include(ca => ca.Person)
                .FirstOrDefaultAsync(ca => ca.PersonId == personId);
        }

        public async Task<ClubAdmin?> GetByClubIdAsync(int clubId)
        {
            _logger.LogInformation("Fetching ClubAdmin with ClubId {ClubId}", clubId);
            return await _context.ClubAdmins
                .Include(ca => ca.Person)
                .Include(ca => ca.Club)
                .FirstOrDefaultAsync(ca => ca.ClubId == clubId);
        }
    }
}
