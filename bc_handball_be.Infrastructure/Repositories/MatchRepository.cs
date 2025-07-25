﻿using bc_handball_be.Core.Entities;
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


        public async Task AddMatchAsync(Match match)
        {
            _logger.LogInformation("Adding match to the database");
            _context.Add(match);
            await _context.SaveChangesAsync();
        }

        public async Task AddMatchesAsync(List<Match> matches)
        {
            _logger.LogInformation("Adding {count} matches to the database", matches.Count);

            // Vyfiltrujeme zápasy, které mají přiřazené týmy (tedy nejsou "blank")
            var teamMatches = matches
                .Where(m => m.HomeTeamId.HasValue && m.AwayTeamId.HasValue)
                .ToList();

            // Získáme všechna ID týmů z těchto zápasů
            var teamIds = teamMatches
                .SelectMany(m => new[] { m.HomeTeamId!.Value, m.AwayTeamId!.Value })
                .Distinct();

            // Zkontrolujeme, zda týmy existují
            var existingTeamIds = await _context.Teams
                .Where(t => teamIds.Contains(t.Id))
                .Select(t => t.Id)
                .ToListAsync();

            var invalidMatches = teamMatches
                .Where(m => !existingTeamIds.Contains(m.HomeTeamId!.Value) || !existingTeamIds.Contains(m.AwayTeamId!.Value))
                .ToList();

            if (invalidMatches.Any())
            {
                throw new InvalidOperationException("Some matches reference non-existent teams.");
            }

            // Uložíme všechny zápasy – i ty bez týmů
            await _context.Matches.AddRangeAsync(matches);
            await _context.SaveChangesAsync();
        }

        public async Task<Match?> GetMatchByIdAsync(int id)
        {
            _logger.LogInformation("Fetching match with ID {id}", id);
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

        public async Task<List<Match>> GetMatchesAsync()
        {
            _logger.LogInformation("Fetching matches");
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

        public async Task<List<Match>> GetMatchesForReportAsync()
        {
            _logger.LogInformation("Fetching matches");
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
                .Where(m => m.State == MatchState.None || m.State == MatchState.Pending)
                .ToListAsync();
        }

        public async Task<List<Match>> GetMatchesForTimetableAsync()
        {
            _logger.LogInformation("Fetching matches for timetable");
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
                .Where(m => m.State == MatchState.None || m.State == MatchState.Generated)
                .ToListAsync();
        }

        public async Task<List<Match>> GetMatchesUnassignedAsync()
        {
            _logger.LogInformation("Fetching unassigned matches");
            return await _context.Matches
                .Include(m => m.HomeTeam)
                    .ThenInclude(t => t.Players)
                        .ThenInclude(p => p.Person)
                .Include(m => m.AwayTeam)
                    .ThenInclude(t => t.Players)
                        .ThenInclude(p => p.Person)
                .Include(m => m.Group)
                    .ThenInclude(g => g.Category)
                .Where(m => m.HomeTeamId.HasValue && m.AwayTeamId.HasValue && m.State != MatchState.Generated)
                .ToListAsync();
        }

        public async Task<List<Match>> GetMatchesByStateAsync(MatchState state)
        {
            return await _context.Matches
                .Where(m => m.State == state)
                .ToListAsync();
        }

        public async Task UpdateMatchAsync(Match match)
        {
            _logger.LogInformation("Updating match [{id}] in the database", match.Id);
            _context.Matches.Update(match);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMatchesAsync(List<Match> matches)
        {
            _logger.LogInformation("Updating {count} matches in the database", matches.Count);
            _context.Matches.UpdateRange(matches);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            _logger.LogInformation("Saving changes to the database");
            await _context.SaveChangesAsync();
        }

        public async Task<List<Match>> GetMatchesByCategoryIdAsync(int categoryId)
        {
            _logger.LogInformation("Fetching matches for category ID {categoryId}", categoryId);
            return await _context.Matches
                .Include(m => m.HomeTeam)
                    //.ThenInclude(t => t.Players)
                    //    .ThenInclude(p => p.Person)
                .Include(m => m.AwayTeam)
                    //.ThenInclude(t => t.Players)
                    //    .ThenInclude(p => p.Person)
                .Include(m => m.Lineups)
                    //.ThenInclude(l => l.Players)
                    //    .ThenInclude(p => p.Player)
                    //        .ThenInclude(p => p.Person)
                .Where(m => m.Group.Category.Id == categoryId)
                .ToListAsync();
        }

        public async Task<List<Match>> GetMatchesByGroupIdAsync(int groupId)
        {
            _logger.LogInformation("Fetching matches for group ID {groupId}", groupId);
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

        public async Task<List<Match>> GetMatchesByTeamIdAsync(int teamId)
        {
            _logger.LogInformation("Fetching matches for team ID {teamId}", teamId);
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

        public async Task DeleteMatchAsync(int matchId)
        {
            _logger.LogInformation("Deleting match with ID {matchId}", matchId);
            var match = await _context.Matches.FindAsync(matchId);
            if (match != null)
            {
                _context.Matches.Remove(match);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning("Match with ID {matchId} not found for deletion", matchId);
            }
        }
    }
}
