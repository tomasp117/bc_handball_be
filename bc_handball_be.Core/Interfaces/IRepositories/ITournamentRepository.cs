using bc_handball_be.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IRepositories
{
    public interface ITournamentRepository
    {
        Task AddAsync(Tournament tournament);
        Task<List<Tournament>> GetAllAsync();
        Task<Tournament> GetByIdAsync(int id);
        Task UpdateAsync(Tournament tournament);
        Task DeleteAsync(int id);
    }
}
