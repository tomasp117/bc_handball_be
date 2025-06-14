using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IServices
{
    public interface IMatchService
    {
        Task<List<Match>> GenBlankMatches(int edition);
        //Task<List<Match>> GenerateMatchesForGroupStage(int category);
        Task<List<Match>> AssignGroupMatchesFromScratch(int categoryId, int edition);
        Task<List<Match>> GetMatchesAsync();
        Task<List<UnassignedMatch>> GetUnassignedGroupMatches(int categoryId);
        Task UpdateMatchesAsync(List<Match> assignments);
        Task<List<Match>> AssignAllGroupMatchesFromScratch(int edition);
        Task<Match> GetMatchByIdAsync(int id);
        Task UpdateMatchAsync(Match match);
        Task<List<Match>> GetMatchesByCategoryIdAsync(int categoryId);
        Task<List<Match>> GetMatchesByGroupIdAsync(int groupId);
        Task<List<Match>> GetMatchesByTeamIdAsync(int teamId);
        Task<List<Match>> GetMatchesForReportAsync();
        Task<List<Match>> GetMatchesForTimetableAsync();

        Task<List<Match>> GetGeneratedSlotsAsync(int edition);
        Task<Match> CreateSlotAsync(int edition, DateTime time, string playground);
        Task DeleteSlotAsync(int slotId);
    }
}
