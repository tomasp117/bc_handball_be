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
                    .Include(c => c.Club)
                    .FirstOrDefaultAsync(c => c.PersonId == personId);

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
    }
}
