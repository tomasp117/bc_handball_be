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
            // 1. Načti zápas včetně týmů  
            var match = await _matchService.GetMatchByIdAsync(matchId);
            if (match == null)
                throw new Exception($"Zápas s ID {matchId} nenalezen.");

            // 2. Načti aktuální hráče v týmu (můžeš upravit podle své struktury)  
            var homePlayers = match.HomeTeam?.Players;
            if (homePlayers == null || !homePlayers.Any())
                throw new Exception($"Domácí tým s ID {match.HomeTeamId} nemá žádné hráče.");
            var awayPlayers = match.AwayTeam?.Players;
            if (awayPlayers == null || !awayPlayers.Any())
                throw new Exception($"Hostující tým s ID {match.AwayTeamId} nemá žádné hráče.");

            // Fix for CS1503: Convert nullable int to int using null-coalescing operator  
            if (!match.HomeTeamId.HasValue || !match.AwayTeamId.HasValue)
                throw new Exception("Týmové ID musí být vyplněno.");

            _logger.LogInformation(
                "Calling CreateLineupsForMatchAsync with matchId={matchId}, homeTeamId={homeTeamId}, awayTeamId={awayTeamId}, homePlayers={homePlayers}, awayPlayers={awayPlayers}",
                matchId, match.HomeTeamId, match.AwayTeamId,
                homePlayers != null ? string.Join(",", homePlayers.Select(p => p.Id)) : "null",
                awayPlayers != null ? string.Join(",", awayPlayers.Select(p => p.Id)) : "null"
);

            await _lineupRepository.CreateLineupsForMatchAsync(
                matchId,
                match.HomeTeamId.Value, 
                homePlayers.Select(p => p.Id).ToList(),
                match.AwayTeamId.Value, 
                awayPlayers.Select(p => p.Id).ToList()
            );
        }


    }
}
