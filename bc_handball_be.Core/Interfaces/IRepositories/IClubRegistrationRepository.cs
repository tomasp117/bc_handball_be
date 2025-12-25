using bc_handball_be.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IRepositories
{
    public interface IClubRegistrationRepository
    {
        Task<ClubRegistration> AddAsync(ClubRegistration clubRegistration);
        Task<ClubRegistration> GetByIdAsync(int id);
        Task<List<ClubRegistration>> GetAllAsync();
        Task<ClubRegistration> UpdateAsync(ClubRegistration clubRegistration);
        Task<bool> DeleteAsync(int id);
        Task<List<ClubRegistration>> GetByTournamentInstanceIdAsync(int tournamentInstanceId);
        Task<ClubRegistration?> GetByClubIdAsync(int clubId);
        Task<List<ClubRegistration>> GetByStatusAsync(RegistrationStatus status);
    }
}
