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
    public class LineupRepository : ILineupRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LineupRepository> _logger;

        public LineupRepository(ApplicationDbContext context, ILogger<LineupRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ==================== READ OPERATIONS ====================

        public async Task<Lineup?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching lineup with ID {LineupId}", id);
            return await _context.Lineups
                .Include(l => l.Players)
                    .ThenInclude(lp => lp.Player)
                        .ThenInclude(p => p.Person)
                .Include(l => l.Team)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<List<Lineup>> GetByMatchIdAsync(int matchId)
        {
            _logger.LogInformation("Fetching lineups for match {MatchId}", matchId);
            return await _context.Lineups
                .Include(l => l.Players)
                    .ThenInclude(lp => lp.Player)
                        .ThenInclude(p => p.Person)
                .Include(l => l.Team)
                .Where(l => l.MatchId == matchId)
                .ToListAsync();
        }

        // ==================== WRITE OPERATIONS ====================

        public async Task AddAsync(Lineup lineup)
        {
            _logger.LogInformation("Adding lineup for match {MatchId}, team {TeamId}", lineup.MatchId, lineup.TeamId);
            await _context.Lineups.AddAsync(lineup);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Lineup> lineups)
        {
            _logger.LogInformation("Adding {Count} lineups to database", lineups.Count());
            await _context.Lineups.AddRangeAsync(lineups);
            await _context.SaveChangesAsync();
        }

        // ==================== DELETE OPERATIONS ====================

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting lineup with ID {LineupId}", id);
            var lineup = await _context.Lineups
                .Include(l => l.Players)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lineup != null)
            {
                _context.LineupPlayers.RemoveRange(lineup.Players);
                _context.Lineups.Remove(lineup);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning("Lineup with ID {LineupId} not found for deletion", id);
            }
        }

        public async Task DeleteByMatchIdAsync(int matchId)
        {
            _logger.LogInformation("Deleting lineups for match {MatchId}", matchId);
            var lineups = await _context.Lineups
                .Include(l => l.Players)
                .Where(l => l.MatchId == matchId)
                .ToListAsync();

            if (lineups.Any())
            {
                // Delete all lineup players first
                foreach (var lineup in lineups)
                {
                    _context.LineupPlayers.RemoveRange(lineup.Players);
                }
                // Then delete the lineups
                _context.Lineups.RemoveRange(lineups);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted {Count} lineups for match {MatchId}", lineups.Count, matchId);
            }
            else
            {
                _logger.LogWarning("No lineups found to delete for match {MatchId}", matchId);
            }
        }
    }
}
