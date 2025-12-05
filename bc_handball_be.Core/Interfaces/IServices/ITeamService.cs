using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IServices
{
    public interface ITeamService
    {
        Task<List<Team>> GetTeamsAsync();
        Task<Team?> GetTeamByIdAsync(int id);
        Task<List<Team>> GetTeamsByIdAsync(List<int> ids);
        Task<List<Team>> GetTeamsByCategoryAsync(int categoryId);
        Task<List<Team>> GetTeamsByGroupAsync(int groupId);
        Task<List<Team>> GetTeamsByInstanceIdAsync(int instanceId);
        Task AddTeamAsync(Team team);
        Task DeleteTeamAsync(int id);
        Task<List<GroupAssignmentVariant>> AssignTeamsToGroupsAsync(List<TeamWithAttributes> teamsWithAttributes, int categoryId);
        Task<bool> ExistsAsync(string teamName, int clubId, int tournamentInstanceId);
    }
}
