﻿using bc_handball_be.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IServices
{
    public interface ITournamentInstanceService
    {
        Task<TournamentInstance> CreateTournamentInstanceAsync(TournamentInstance tournamentInstance);
        Task<List<TournamentInstance>> GetAllTournamentInstancesAsync();
        Task<List<TournamentInstance>> GetByTournamentIdAsync(int tournamentId);
        Task<TournamentInstance> GetByIdAsync(int id);
        Task<TournamentInstance> UpdateTournamentInstanceAsync(TournamentInstance tournamentInstance);
        Task DeleteTournamentInstanceAsync(int id);
        Task<TournamentInstance> GetByCategoryIdAsync(int categoryId);

    }
}
