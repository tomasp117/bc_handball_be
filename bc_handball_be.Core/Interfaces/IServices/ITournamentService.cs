﻿using bc_handball_be.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IServices
{
    public interface ITournamentService
    {
        Task<Tournament> CreateTournamentAsync(Tournament tournament);
        Task<List<Tournament>> GetAllTournamentsAsync();
        Task<Tournament> GetTournamentByIdAsync(int id);
        Task<Tournament> UpdateTournamentAsync(Tournament tournament);
        Task<bool> DeleteTournamentAsync(int id);
    }
}
