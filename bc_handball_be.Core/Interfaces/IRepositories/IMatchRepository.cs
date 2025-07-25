﻿using bc_handball_be.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IRepositories
{
    public interface IMatchRepository
    {
        Task AddMatchAsync(Match match);
        Task UpdateMatchAsync(Match match);
        Task AddMatchesAsync(List<Match> matches);
        Task<List<Match>> GetMatchesByStateAsync(MatchState state);
        Task<List<Match>> GetMatchesAsync();
        Task<Match?> GetMatchByIdAsync(int id);
        Task UpdateMatchesAsync(List<Match> matches);
        Task SaveAsync();
        Task<List<Match>> GetMatchesByCategoryIdAsync(int categoryId);
        Task<List<Match>> GetMatchesByGroupIdAsync(int groupId);
        Task<List<Match>> GetMatchesByTeamIdAsync(int teamId);
        Task<List<Match>> GetMatchesForReportAsync();
        Task<List<Match>> GetMatchesForTimetableAsync();
        Task<List<Match>>  GetMatchesUnassignedAsync();

        Task DeleteMatchAsync(int matchId);

    }
}
