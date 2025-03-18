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
        Task<IEnumerable<Group>> GetGroupsByCategoryAsync(int categoryId);
        Task SaveGroupsAsync(IEnumerable<Group> newGroups, int categoryId);
    }
}
