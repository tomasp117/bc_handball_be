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
    public class MatchRepository : IMatchRepository
    {
        private readonly ILogger<MatchRepository> _logger;
        private readonly ApplicationDbContext _context;

        public MatchRepository(ILogger<MatchRepository> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // ==================== READ OPERATIONS ====================

        public async Task<Match?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching match with ID {MatchId}", id);
            return await _context.Matches
                .Include(m => m.HomeTeam)
                    .ThenInclude(t => t.Players)
                        .ThenInclude(p => p.Person)
                .Include(m => m.AwayTeam)
                    .ThenInclude(t => t.Players)
                        .ThenInclude(p => p.Person)
                .Include(m => m.Group)
                .Include(m => m.Events)
                .Include(m => m.Lineups)
                    .ThenInclude(l => l.Players)
                        .ThenInclude(p => p.Player)
                            .ThenInclude(p => p.Person)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<Match>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all matches");
            return await _context.Matches
                .Include(m => m.HomeTeam)
                    .ThenInclude(t => t.Players)
                        .ThenInclude(p => p.Person)
                .Include(m => m.AwayTeam)
                    .ThenInclude(t => t.Players)
                        .ThenInclude(p => p.Person)
                .Include(m => m.Group)
                    .ThenInclude(g => g.Category)
                .Include(m => m.Lineups)
                    .ThenInclude(l => l.Players)
                        .ThenInclude(p => p.Player)
                            .ThenInclude(p => p.Person)
                .ToListAsync();
        }

        public async Task<List<Match>> GetByStateAsync(MatchState state)
        {
            _logger.LogInformation("Fetching matches with state {State}", state);
            return await _context.Matches
                .Include(m => m.HomeTeam)
                    .ThenInclude(t => t.Players)
                        .ThenInclude(p => p.Person)
                .Include(m => m.AwayTeam)
                    .ThenInclude(t => t.Players)
                        .ThenInclude(p => p.Person)
                .Include(m => m.Group)
                    .ThenInclude(g => g.Category)
                .Include(m => m.Lineups)
                    .ThenInclude(l => l.Players)
                        .ThenInclude(p => p.Player)
                            .ThenInclude(p => p.Person)
                .Where(m => m.State == state)
                .ToListAsync();
        }

        public async Task<List<Match>> GetByCategoryIdAsync(int categoryId)
        {
            _logger.LogInformation("Fetching matches for category ID {CategoryId}", categoryId);
            return await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.Lineups)
                .Where(m => m.Group.Category.Id == categoryId)
                .ToListAsync();
        }

        public async Task<List<Match>> GetByGroupIdAsync(int groupId)
        {
            _logger.LogInformation("Fetching matches for group ID {GroupId}", groupId);
            return await _context.Matches
                .Include(m => m.HomeTeam)
                    .ThenInclude(t => t.Players)
                        .ThenInclude(p => p.Person)
                .Include(m => m.AwayTeam)
                    .ThenInclude(t => t.Players)
                        .ThenInclude(p => p.Person)
                .Include(m => m.Lineups)
                    .ThenInclude(l => l.Players)
                        .ThenInclude(p => p.Player)
                            .ThenInclude(p => p.Person)
                .Where(m => m.GroupId == groupId)
                .ToListAsync();
        }

        public async Task<List<Match>> GetByTeamIdAsync(int teamId)
        {
            _logger.LogInformation("Fetching matches for team ID {TeamId}", teamId);
            return await _context.Matches
                .Include(m => m.HomeTeam)
                    .ThenInclude(t => t.Players)
                        .ThenInclude(p => p.Person)
                .Include(m => m.AwayTeam)
                    .ThenInclude(t => t.Players)
                        .ThenInclude(p => p.Person)
                .Include(m => m.Lineups)
                    .ThenInclude(l => l.Players)
                        .ThenInclude(p => p.Player)
                            .ThenInclude(p => p.Person)
                .Where(m => m.HomeTeamId == teamId || m.AwayTeamId == teamId)
                .ToListAsync();
        }

        // ==================== WRITE OPERATIONS ====================

        public async Task AddAsync(Match match)
        {
            _logger.LogInformation("Adding match to the database");
            await _context.Matches.AddAsync(match);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Match> matches)
        {
            _logger.LogInformation("Adding {Count} matches to the database", matches.Count());
            await _context.Matches.AddRangeAsync(matches);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Match match)
        {
            _logger.LogInformation("Updating match {MatchId} in the database", match.Id);
            _context.Matches.Update(match);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<Match> matches)
        {
            _logger.LogInformation("Updating {Count} matches in the database", matches.Count());
            _context.Matches.UpdateRange(matches);
            await _context.SaveChangesAsync();
        }

        // ==================== DELETE OPERATIONS ====================

        public async Task DeleteAsync(int matchId)
        {
            _logger.LogInformation("Deleting match with ID {MatchId}", matchId);
            var match = await _context.Matches.FindAsync(matchId);
            if (match != null)
            {
                _context.Matches.Remove(match);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning("Match with ID {MatchId} not found for deletion", matchId);
            }
        }
    }
}
