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
        private readonly IPersonService _personService;
        private readonly IPersonRepository _personRepository;
        private readonly ILogger<CoachService> _logger;

        public CoachService(
            ICoachRepository coachRepository,
            ILogger<CoachService> logger,
            IUserRepository userRepository,
            IPersonService personService,
            IPersonRepository personRepository)
        {
            _coachRepository = coachRepository;
            _userRepository = userRepository;
            _personService = personService;
            _personRepository = personRepository;
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

        public async Task DeleteCoachAsync(int coachId)
        {
            _logger.LogInformation("Starting deletion process for coach {CoachId}", coachId);

            // Business Logic: Fetch coach with person
            var coach = await _coachRepository.GetByIdAsync(coachId);
            if (coach == null)
            {
                _logger.LogWarning("Coach with ID {CoachId} not found", coachId);
                throw new KeyNotFoundException($"Coach with ID {coachId} not found.");
            }

            // Business Logic: Get the person to delete
            var person = await _personRepository.GetPersonByIdAsync(coach.PersonId);
            if (person == null)
            {
                _logger.LogWarning("Person with ID {PersonId} for coach {CoachId} not found", coach.PersonId, coachId);
                throw new KeyNotFoundException($"Person with ID {coach.PersonId} not found.");
            }

            // Business Logic: Delete Person
            // - Login will cascade delete automatically (configured in ApplicationDbContext line 169)
            // - Coach will cascade delete automatically (configured in ApplicationDbContext line 183)
            await _personRepository.DeletePersonAsync(person);

            _logger.LogInformation("Successfully deleted coach {CoachId} with person {PersonId}", coachId, coach.PersonId);
        }
    }
}
