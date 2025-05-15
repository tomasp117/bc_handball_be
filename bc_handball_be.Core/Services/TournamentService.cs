using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Services
{
    public class TournamentService : ITournamentService
    {

        private readonly ITournamentRepository _tournamentRepository;

        private readonly ILogger<TournamentService> _logger;

        public TournamentService(ITournamentRepository tournamentRepository, ILogger<TournamentService> logger)
        {
            _tournamentRepository = tournamentRepository;
            _logger = logger;
        }

        public async Task<Tournament> CreateTournamentAsync(Tournament tournament)
        {
            await _tournamentRepository.AddAsync(tournament);
            
            return tournament;
        }

        public async Task<List<Tournament>> GetAllTournamentsAsync()
        {
            var tournaments = await _tournamentRepository.GetAllAsync();
            return tournaments.ToList();
        }
    }
}
