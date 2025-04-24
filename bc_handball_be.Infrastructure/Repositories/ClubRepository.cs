using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Infrastructure.Repositories
{
    public class ClubRepository : IClubRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClubRepository> _logger;
        public ClubRepository(ApplicationDbContext context, ILogger<ClubRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddClubAsync(Club club)
        {
            _logger.LogInformation("Adding new club: {ClubName}", club.Name);
            await _context.Clubs.AddAsync(club);
            await _context.SaveChangesAsync();
        }
    }
}
