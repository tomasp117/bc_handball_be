using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Services
{
    public class LineupService : ILineupService
    {
        private readonly IMatchService _matchService;
        private readonly ILineupRepository _lineupRepository;
        private readonly ITeamService _teamService;
        private readonly ILogger<LineupService> _logger;


        public LineupService(IMatchService matchService, ILineupRepository lineupRepository, ITeamService teamService, ILogger<LineupService> logger)
        {
            _matchService = matchService;
            _lineupRepository = lineupRepository;
            _teamService = teamService;
            _logger = logger;
        }


        public async Task GenerateLineupsForMatchAsync(int matchId)
        {
            _logger.LogInformation("Starting lineup generation for match {MatchId}", matchId);

            // Business Logic: Fetch match and validate
            var match = await _matchService.GetMatchByIdAsync(matchId);
            if (match == null)
                throw new Exception($"Zápas s ID {matchId} nenalezen.");

            // Business Logic: Validate team IDs
            if (!match.HomeTeamId.HasValue || !match.AwayTeamId.HasValue)
                throw new Exception("Týmové ID musí být vyplněno.");

            // Business Logic: Validate teams have players
            var homePlayers = match.HomeTeam?.Players;
            if (homePlayers == null || !homePlayers.Any())
                throw new Exception($"Domácí tým s ID {match.HomeTeamId} nemá žádné hráče.");

            var awayPlayers = match.AwayTeam?.Players;
            if (awayPlayers == null || !awayPlayers.Any())
                throw new Exception($"Hostující tým s ID {match.AwayTeamId} nemá žádné hráče.");

            _logger.LogInformation(
                "Generating lineups for match {MatchId}: homeTeam={HomeTeamId} ({HomePlayerCount} players), awayTeam={AwayTeamId} ({AwayPlayerCount} players)",
                matchId, match.HomeTeamId, homePlayers.Count(), match.AwayTeamId, awayPlayers.Count()
            );

            // Business Logic: Delete old lineups if they exist
            var existingLineups = await _lineupRepository.GetByMatchIdAsync(matchId);
            if (existingLineups.Any())
            {
                _logger.LogInformation("Deleting {Count} existing lineups for match {MatchId}", existingLineups.Count, matchId);
                await _lineupRepository.DeleteByMatchIdAsync(matchId);
            }

            // Business Logic: Create home team lineup with players
            var homeLineup = new Lineup
            {
                MatchId = matchId,
                TeamId = match.HomeTeamId.Value,
                Players = homePlayers.Select(p => new LineupPlayer
                {
                    PlayerId = p.Id
                }).ToList()
            };

            // Business Logic: Create away team lineup with players
            var awayLineup = new Lineup
            {
                MatchId = matchId,
                TeamId = match.AwayTeamId.Value,
                Players = awayPlayers.Select(p => new LineupPlayer
                {
                    PlayerId = p.Id
                }).ToList()
            };

            // Simple repository call: Add both lineups
            _logger.LogInformation("Adding lineups for match {MatchId}", matchId);
            await _lineupRepository.AddAsync(homeLineup);
            await _lineupRepository.AddAsync(awayLineup);

            _logger.LogInformation("Successfully generated lineups for match {MatchId}", matchId);
        }


    }
}
