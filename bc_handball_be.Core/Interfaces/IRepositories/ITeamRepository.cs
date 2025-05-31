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
        Task<List<Team>> GetTeamsAsync();
        Task<Team?> GetTeamByIdAsync(int id);
        Task<List<Team>> GetTeamsByCategoryAsync(int categoryId);
        Task AddTeamAsync(Team team);
        Task UpdateTeamAsync(Team team);
        Task DeleteTeamAsync(int id);
        Task<List<Team>> GetTeamsByIdAsync(List<int> ids);
        Task<List<Team>> GetTeamsByGroupAsync(int groupId);
        Task<List<Team>> GetPlaceholderTeamsByCategoryAsync(int categoryId);
    }
}
