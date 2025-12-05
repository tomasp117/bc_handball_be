using bc_handball_be.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IRepositories
{
    public interface IGroupRepository
    {
        // Read operations
        Task<IEnumerable<Group>> GetGroupsByCategoryAsync(int categoryId);
        Task<List<Group>> GetAllAsync();
        Task<Group?> GetByIdAsync(int id);
        Task<List<Group>> GetByPhaseAsync(int categoryId, string phase);
        Task<List<Group>> GetGroupsWithPlaceholderTeamsAsync(int categoryId);

        // Write operations
        Task AddAsync(Group group);
        Task AddRangeAsync(IEnumerable<Group> groups);
        Task UpdateAsync(Group group);

        // Delete operations
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(IEnumerable<Group> groups);
        Task DeleteByCategoryIdAsync(int categoryId);
    }
}
