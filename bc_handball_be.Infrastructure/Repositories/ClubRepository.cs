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

        public async Task<bool> DeleteAsync(Club club)
        {
            _logger.LogInformation("Deleting club: {ClubName}", club.Name);
            _context.Clubs.Remove(club);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Club>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all clubs");
            return await _context.Clubs.ToListAsync();
        }

        public async Task<Club> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching club by ID: {ClubId}", id);
            return await _context.Clubs.FindAsync(id);
        }

        public async Task<Club> UpdateAsync(Club club)
        {
            _logger.LogInformation("Updating club: {ClubName}", club.Name);
            _context.Clubs.Update(club);
            await _context.SaveChangesAsync();
            return club;

        }
    }
}
