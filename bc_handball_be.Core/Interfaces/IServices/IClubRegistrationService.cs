using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Entities.Actors.super;

namespace bc_handball_be.Core.Interfaces.IServices
{
    public interface IClubRegistrationService
    {
        /// <summary>
        /// Creates a complete club registration including Club, Person, Login, ClubAdmin, and ClubRegistration.
        /// Used for public registration where everything needs to be created together.
        /// </summary>
        Task<ClubRegistration> CreateFullRegistrationAsync(
            Club club,
            Person person,
            string username,
            string password,
            ClubRegistration registrationData
        );

        Task<ClubRegistration> CreateRegistrationAsync(ClubRegistration clubRegistration);
        Task<ClubRegistration> GetByIdAsync(int id);
        Task<List<ClubRegistration>> GetAllAsync();
        Task<ClubRegistration> UpdateRegistrationAsync(ClubRegistration clubRegistration);
        Task<bool> DeleteRegistrationAsync(int id);
        Task<List<ClubRegistration>> GetByTournamentInstanceIdAsync(int tournamentInstanceId);
        Task<ClubRegistration?> GetByClubIdAsync(int clubId);
        Task<List<ClubRegistration>> GetByStatusAsync(RegistrationStatus status);
        Task<ClubRegistration> ApproveRegistrationAsync(int id);
        Task<ClubRegistration> DenyRegistrationAsync(int id, string denialReason);
        Task<float> CalculateRegistrationFeeAsync(ClubRegistration clubRegistration);
    }
}
