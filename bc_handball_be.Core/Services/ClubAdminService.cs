using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Core.Interfaces.IServices;


namespace bc_handball_be.Core.Services
{
    public class ClubAdminService : IClubAdminService
    {
        private readonly IClubAdminRepository _clubRepository;

        public ClubAdminService(IClubAdminRepository clubRepository)
        {
            _clubRepository = clubRepository;
        }

        public async Task<Club> GetClubByPersonIdAsync(int personId)
        {
            var clubAdmin = await _clubRepository.GetByPersonIdAsync(personId);
            if (clubAdmin == null)
            {
                throw new Exception($"ClubAdmin with PersonId {personId} not found");
            }
            return clubAdmin.Club;
        }
    }
}
