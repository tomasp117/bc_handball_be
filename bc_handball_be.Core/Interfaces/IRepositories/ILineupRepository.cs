using bc_handball_be.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IRepositories
{
    public interface ILineupRepository
    {
        // Read operations
        Task<Lineup?> GetByIdAsync(int id);
        Task<List<Lineup>> GetByMatchIdAsync(int matchId);

        // Write operations
        Task AddAsync(Lineup lineup);
        Task AddRangeAsync(IEnumerable<Lineup> lineups);

        // Delete operations
        Task DeleteAsync(int id);
        Task DeleteByMatchIdAsync(int matchId);
    }
}
