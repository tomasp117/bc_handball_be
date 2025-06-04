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

        public async Task CreateLineupsForMatchAsync(
            int matchId,
            int homeTeamId, List<int> homePlayerIds,
            int awayTeamId, List<int> awayPlayerIds)
        {
            _logger.LogInformation("Repository: Creating lineups for matchId={matchId}, homeTeamId={homeTeamId} (count {homeCount}), awayTeamId={awayTeamId} (count {awayCount})",
    matchId, homeTeamId, homePlayerIds?.Count ?? -1, awayTeamId, awayPlayerIds?.Count ?? -1);

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Smaž existující lineupy (a hráče v nich)
                var oldLineups = await _context.Lineups
                    .Where(l => l.MatchId == matchId)
                    .Include(l => l.Players)
                    .ToListAsync();

                if (oldLineups.Any())
                {
                    foreach (var lineup in oldLineups)
                    {
                        _context.LineupPlayers.RemoveRange(lineup.Players);
                    }
                    _context.Lineups.RemoveRange(oldLineups);
                    await _context.SaveChangesAsync();
                }

                // 2. Nový lineup pro domácí tým
                var homeLineup = new Lineup
                {
                    MatchId = matchId,
                    TeamId = homeTeamId,
                };
                _context.Lineups.Add(homeLineup);
                await _context.SaveChangesAsync();

                var homeLineupPlayers = homePlayerIds.Select(pid => new LineupPlayer
                {
                    LineupId = homeLineup.Id,
                    PlayerId = pid,
                }).ToList();
                _context.LineupPlayers.AddRange(homeLineupPlayers);

                // 3. Nový lineup pro hosty
                var awayLineup = new Lineup
                {
                    MatchId = matchId,
                    TeamId = awayTeamId,
                };
                _context.Lineups.Add(awayLineup);
                await _context.SaveChangesAsync();

                var awayLineupPlayers = awayPlayerIds.Select(pid => new LineupPlayer
                {
                    LineupId = awayLineup.Id,
                    PlayerId = pid,
                }).ToList();
                _context.LineupPlayers.AddRange(awayLineupPlayers);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                _logger.LogError("Chyba při vytváření soupisky pro zápas {matchId}", matchId);
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
