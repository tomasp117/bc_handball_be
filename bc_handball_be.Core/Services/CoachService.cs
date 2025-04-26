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
    public class CoachService : ICoachService
    {
        private readonly ICoachRepository _coachRepository;
        private readonly ILogger<CoachService> _logger;

        public CoachService(ICoachRepository coachRepository, ILogger<CoachService> logger)
        {
            _coachRepository = coachRepository;
            _logger = logger;
        }

        public async Task<Team?> GetTeamForPersonIdAsync(int personId)
        {
            var coach = await _coachRepository.GetByPersonIdAsync(personId);
            if (coach == null) return null;

            return coach.Team;
        }


    }
}
