using bc_handball_be.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IRepositories
{
    public interface ITournamentInstanceRepository
    {
        Task AddAsync(TournamentInstance tournamentInstance);
        Task<List<TournamentInstance>> GetAllAsync();
        Task<TournamentInstance> GetByIdAsync(int id);
        Task UpdateAsync(TournamentInstance tournamentInstance);
        Task DeleteAsync(int id);
    }
}
