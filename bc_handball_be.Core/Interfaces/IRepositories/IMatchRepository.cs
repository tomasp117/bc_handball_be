using bc_handball_be.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IRepositories
{
    public interface IMatchRepository
    {
        // Read operations
        Task<Match?> GetByIdAsync(int id);
        Task<List<Match>> GetAllAsync();
        Task<List<Match>> GetByStateAsync(MatchState state);
        Task<List<Match>> GetByCategoryIdAsync(int categoryId);
        Task<List<Match>> GetByGroupIdAsync(int groupId);
        Task<List<Match>> GetByTeamIdAsync(int teamId);

        // Write operations
        Task AddAsync(Match match);
        Task AddRangeAsync(IEnumerable<Match> matches);
        Task UpdateAsync(Match match);
        Task UpdateRangeAsync(IEnumerable<Match> matches);

        // Delete operations
        Task DeleteAsync(int matchId);
    }
}
