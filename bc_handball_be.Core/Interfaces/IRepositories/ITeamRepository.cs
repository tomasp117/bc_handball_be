using bc_handball_be.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IRepositories
{
    public interface ITeamRepository
    {
        // Read operations
        Task<Team?> GetTeamByIdAsync(int id);
        Task<List<Team>> GetTeamsAsync();
        Task<List<Team>> GetTeamsByCategoryAsync(int categoryId);
        Task<List<Team>> GetTeamsByGroupAsync(int groupId);
        Task<List<Team>> GetTeamsByIdAsync(List<int> ids);
        Task<List<Team>> GetPlaceholderTeamsByCategoryAsync(int categoryId);

        // Write operations
        Task AddTeamAsync(Team team);

        // Delete operations
        Task DeleteTeamAsync(int id);
    }
}
