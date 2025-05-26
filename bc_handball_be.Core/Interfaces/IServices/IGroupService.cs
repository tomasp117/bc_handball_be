using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IServices
{
    public interface IGroupService
    {
        Task SaveGroupsAsync(IEnumerable<Group> newGroups, int categoryId);
        Task<IEnumerable<Group>> GetGroupsByCategoryAsync(int categoryId);
        Task<List<Group>> GetGroupsAsync();
        Task<List<GroupStanding>> GetGroupStandingsAsync(int groupId);
        Task SavePlaceholderGroupsAsync(List<PlaceholderGroup> placeholderGroups, int categoryId);
        Task<List<Group>> GetGroupsWithPlaceholderTeamsAsync(int categoryId);

    }
}
