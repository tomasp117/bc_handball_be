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
        Task<Coach?> GetByPersonIdAsync(int personId);
        Task AddAsync(Coach coach);
        Task DeleteAsync(int coachId);
        Task<Coach> GetByIdAsync(int coachId);
        Task DeleteCoachWithPersonAsync(int coachId);
    }
}
