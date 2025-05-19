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
    public class TournamentInstanceService : ITournamentInstanceService
    {
        private readonly ITournamentInstanceRepository _tournamentInstanceRepository;
        private readonly ILogger<TournamentInstanceService> _logger;

        public TournamentInstanceService(ITournamentInstanceRepository tournamentInstanceRepository, ILogger<TournamentInstanceService> logger)
        {
            _tournamentInstanceRepository = tournamentInstanceRepository;
            _logger = logger;
        }

        public async Task<TournamentInstance> CreateTournamentInstanceAsync(TournamentInstance tournamentInstance)
        {
            await _tournamentInstanceRepository.AddAsync(tournamentInstance);
            return tournamentInstance;
        }

        public async Task<List<TournamentInstance>> GetAllTournamentInstancesAsync()
        {
            var tournamentInstances = await _tournamentInstanceRepository.GetAllAsync();
            return tournamentInstances;
        }

        public async Task<List<TournamentInstance>> GetByTournamentIdAsync(int tournamentId)
        {
            var tournamentInstances = await _tournamentInstanceRepository.GetAllAsync();
            return tournamentInstances.Where(ti => ti.TournamentId == tournamentId).ToList();
        }
    }
}
