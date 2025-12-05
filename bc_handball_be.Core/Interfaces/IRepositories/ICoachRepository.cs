using bc_handball_be.Core.Entities.Actors.sub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IRepositories
{
    public interface ICoachRepository
    {
        // Read operations
        Task<Coach?> GetByIdAsync(int coachId);
        Task<Coach?> GetByPersonIdAsync(int personId);

        // Write operations
        Task AddAsync(Coach coach);

        // Delete operations
        Task DeleteAsync(int coachId);
    }
}
