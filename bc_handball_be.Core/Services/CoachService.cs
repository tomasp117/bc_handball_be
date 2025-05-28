using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;
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
        private readonly IUserRepository _userRepository;

        private readonly ILogger<CoachService> _logger;

        public CoachService(ICoachRepository coachRepository, ILogger<CoachService> logger, IUserRepository userRepository)
        {
            _coachRepository = coachRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Team?> GetTeamForPersonIdAsync(int personId)
        {
            var coach = await _coachRepository.GetByPersonIdAsync(personId);
            if (coach == null) return null;

            return coach.Team;
        }

        public async Task CreateCoachAsync(Coach coach)
        {
            if (coach == null) throw new ArgumentNullException(nameof(coach));
            try
            {
                if (await _userRepository.UsernameExistsAsync(coach.Person.Login.Username))
                {
                    throw new InvalidOperationException($"Username {coach.Person.Login.Username} already exists.");
                }
                await _userRepository.AddUserWithRoleAsync(coach.Person, coach.Person.Login, coach);
                _logger.LogInformation("Coach created successfully with Person ID {PersonId}", coach.Person.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating coach with person ID {PersonId}", coach.Person?.Id);
                throw;
            }
        }

    }
}
